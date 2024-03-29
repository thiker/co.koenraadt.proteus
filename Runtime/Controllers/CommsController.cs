﻿using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using co.koenraadt.proteus.Runtime.Repositories;
using co.koenraadt.proteus.Runtime.ViewModels;
using UnityEngine;
using System.Collections.Concurrent;
using System.Linq;
using Newtonsoft.Json;
 using System.Net;
 using System.Net.NetworkInformation;
 using System.Net.Sockets;
using co.koenraadt.proteus.Runtime.Other;
namespace co.koenraadt.proteus.Runtime.Controllers
{
    /// <summary>
    /// Controls the communication of Proteus and implements an MQTT client and server. 
    /// </summary>
    public class CommsController
    {
        /// <summary>
        /// The IP address of the broker that Proteus should connect to. If left empty, Proteus will create its own broker server.
        /// </summary>
        public static string BROKER_IP = "";

        private static CommsController _instance = null;
        private static MqttFactory _mqttFactory = new();
        private static ConcurrentQueue<MqttApplicationMessage> _mqttMessageQueue = new();
        private IMqttClient _mqttClient;
        private MqttServer _mqttServer;

        /// <summary>
        /// The singleton instance of the communication controller
        /// </summary>
        public static CommsController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ();
                    _instance.Init(); // auto start comms controller
                }
                return _instance;
            }
        }

        /// <summary>
        /// Initialize the communication controller.
        /// </summary>
        /// <returns></returns>
        public async Task Init()
        {
            Debug.Log("PROTEUS: Initializing Server...");
            await InitServer();
            Debug.Log("PROTEUS: Initializing Client...");
            await InitClient();
            Debug.Log("PROTEUS: CommsController Init Completed.");
        }


        /// <summary>
        /// Initializes the MQTT client for the communication controller.
        /// </summary>
        /// <returns></returns>
        private async Task InitClient()
        {
            _mqttClient = _mqttFactory.CreateMqttClient();
            await ConnectClient();
            await SubscribeTopics();
        }

        /// <summary>
        /// Initialize the MQTT broker server used by Proteus.
        /// </summary>
        /// <returns></returns>
        private async Task InitServer()
        {
            Debug.Log("PROTEUS: creating mqtt server...");
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            Debug.Log($"PROTEUS: Local ip found is {ipAddress.ToString()}");

            MqttServerOptions optionsBuilder = new MqttServerOptionsBuilder()
            .WithDefaultEndpointPort(1883).Build();
            _mqttServer = _mqttFactory.CreateMqttServer(optionsBuilder);


            Debug.Log("PROTEUS: starting mqtt server...");
            await _mqttServer.StartAsync();
            Debug.Log("PROTEUS: Server Started.");
        }

        /// <summary>
        /// Connect the communication client to the server.
        /// </summary>
        /// <returns></returns>
        private async Task ConnectClient()
        {
            Debug.Log("PROTEUS: Try disconnecting client...");
            await DisconnectClient();

            // Setup the message handling so that queued messages are not lost.
            Debug.Log("PROTEUS: Setup application message received async.");
            _mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                _mqttMessageQueue.Enqueue(e.ApplicationMessage);
                return Task.CompletedTask;
            };

            // Connect the client
            Debug.Log("PROTEUS: Connecting client....");
            string tcpServerIp = "127.0.0.1"; // the tcp server ip defaults to localhost

            // If the broker IP is set override the tcpServerIp
            if (BROKER_IP != null && BROKER_IP != "" && !BROKER_IP.Equals("")) {
                Debug.Log($"PROTEUS: Overriding client to connnect to external broker ip {BROKER_IP}");
                tcpServerIp = BROKER_IP;
            }

            // Setup the mqtt client
            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(tcpServerIp).Build();
            await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
            Debug.Log($"PROTEUS: Client connected to {tcpServerIp}.");
            Instance.SendMessage("proteus/debug/hello-world", "Hello World from Proteus!");
        }

        /// <summary>
        /// Process a received message.
        /// </summary>
        /// <param name="message"></param>
        private void ProcessMessage(MqttApplicationMessage message)
        {
            string t = message.Topic;

            // State value updates
            if (t.StartsWith("proteus/data/update/3dml/states/"))
            {
                string key = message.Topic.Split("/").Last(); //TODO: if implementing whole state object updates, need to check if it is not the id of the state instead of key.
                Dictionary<string, object> payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(message.ConvertPayloadToString()); ;

                payload.TryGetValue("StateId", out object stateId);
                payload.TryGetValue("value", out object value);

                Repository.Instance.States.UpdateStateValue(stateId.ToString(), key, value);
            }

            // Viewer updates
            if (t.StartsWith("proteus/data/create/viewer")) 
            {
                Debug.Log("Creating viewer from mqtt");
                PTViewer viewer  = JsonConvert.DeserializeObject<PTViewer>(message.ConvertPayloadToString()); ;
                viewer.Id = Helpers.GenerateUniqueId();
                Repository.Instance.Viewers.CreateViewer(viewer);
            }

            // Node Updates
            if (t.StartsWith("proteus/data/update/3dml/nodes"))
            {
                PTNode nodeUpdate = JsonConvert.DeserializeObject<PTNode>(message.ConvertPayloadToString());
                Debug.Log($"PROTEUS: Node Update received for node {nodeUpdate.Id}");
                Repository.Instance.Models.UpdateNode(nodeUpdate);
                return;
            }

            // Edge Updates
            if (t.StartsWith("proteus/data/update/3dml/edges"))
            {
                PTEdge edgeUpdate = JsonConvert.DeserializeObject<PTEdge>(message.ConvertPayloadToString());
                Repository.Instance.Models.UpdateEdge(edgeUpdate);
                return;
            }

            // Model Elements Updates
            if (t.StartsWith("proteus/data/update/3dml/model-elements"))
            {
                PTModelElement modelElementUpdate =JsonConvert.DeserializeObject<PTModelElement>(message.ConvertPayloadToString());
                Repository.Instance.Models.UpdateModelElement(modelElementUpdate);
                return;
            }

            // Node Deletion
            if (t.StartsWith("proteus/data/delete/3dml/nodes"))
            {
                string id = message.ConvertPayloadToString();
                Repository.Instance.Models.DeleteNodeById(id);
                return;
            }

            // Edge Deletion
            if (t.StartsWith("proteus/data/delete/3dml/edges"))
            {
                string id = message.ConvertPayloadToString();
                Repository.Instance.Models.DeleteEdgeById(id);
                return;
            }

            // Model Element Deletion
            if (t.StartsWith("proteus/data/delete/3dml/model-elements"))
            {
                string id = message.ConvertPayloadToString();
                Repository.Instance.Models.DeleteModelElementById(id);
                return;
            }

            // Node Image update
            if (t.StartsWith("proteus/data/update/3dml/images/"))
            {
                float aspectRatio;
                string id = message.Topic.Split("/").Last();

                Debug.Log($"PROTEUS: Processing image update for node id {id}");
                
                Texture2D tex = new(2, 2);
                tex.LoadImage(message.Payload);
                aspectRatio = (float)tex.width / (float)tex.height;

                PTNode nodeUpdate = new()
                {
                    Id = id,
                    UnitHeight= Repository.Instance.Proteus.GetGlobals().DefaultNodeUnitHeight,
                    UnitWidth=aspectRatio * Repository.Instance.Proteus.GetGlobals().DefaultNodeUnitHeight
                };

                Repository.Instance.Models.UpdateNodeTexture(id, tex);
                Repository.Instance.Models.UpdateNode(nodeUpdate);
            }
        }

        /// <summary>
        /// Subscribes to the desired communication topics.
        /// </summary>
        /// <returns></returns>
        private async Task SubscribeTopics()
        {
            if (Debug.isDebugBuild)
            {
                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("proteus/debug/#").Build());
            }

            Debug.Log("PROTEUS: Subscribe to topics...");
            // States Data
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("proteus/data/update/3dml/states/#").Build());

            // Viewer actions
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("proteus/data/create/viewers").Build());
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("proteus/data/create/viewers/#").Build());

            // Nodes Data
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("proteus/data/update/3dml/nodes/#").Build());
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("proteus/data/delete/3dml/nodes/#").Build());
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("proteus/data/update/3dml/images/#").Build());

            // Edges data
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("proteus/data/update/3dml/edges/#").Build());
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("proteus/data/delete/3dml/edges/#").Build());

            // Model Elements data
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("proteus/data/update/3dml/model-elements/#").Build());
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("proteus/data/delete/3dml/model-elements/#").Build());
            Debug.Log("PROTEUS: Subscribed to topics.");
        }

        /// <summary>
        /// Disconnect the client from the server.
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectClient()
        {
            await _mqttClient?.DisconnectAsync(new MqttClientDisconnectOptionsBuilder().WithReason(MqttClientDisconnectReason.NormalDisconnection).Build());
        }

        /// <summary>
        /// Send a message to the MQTT broker for a specific topic. 
        /// </summary>
        /// <param name="topic">The topic to send the message to.</param>
        /// <param name="payload">The string payload of the message.</param>
        public async void SendMessage(string topic, string payload) {
            Debug.Log($"PROTEUS: Sending MQTT message to {topic}");
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .Build();

            await _mqttClient?.PublishAsync(applicationMessage, CancellationToken.None);
        }

        /// <summary>
        /// Update function which should be called by the CommsController gameobject on Unity Update,to ensure the messages are processed in the main thread.
        /// The CommsController does not inherit from monobehavior and therefore has no Update override itself.
        /// </summary>
        public void Update()
        {
            while (_mqttMessageQueue.TryDequeue(out MqttApplicationMessage message))
            {
                ProcessMessage(message);
            }
        }

        /// <summary>
        /// Destroys and cleans up the client / server created by the communication controller.
        /// </summary>
        public void Destroy()
        {
            _mqttClient.Dispose();
            _mqttServer.Dispose();
        }

    }
}
