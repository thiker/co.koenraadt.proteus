using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Packages.co.koenraadt.proteus.Runtime.Repositories;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using UnityEngine;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using Unity.Plastic.Newtonsoft.Json;
using System.Linq;

namespace Packages.co.koenraadt.proteus.Runtime.Controllers
{
    public class CommsController
    {
        private static CommsController _instance = null;
        private static MqttFactory _mqttFactory = new();
        private static ConcurrentQueue<MqttApplicationMessage> _mqttMessageQueue = new();
        private IMqttClient _mqttClient;
        private MqttServer _mqttServer;
        public static CommsController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ();
                    _instance.Init();
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
            Debug.Log("<color=lightblue>PROTEUS</color> Initializing Server...");
            await InitServer();
            Debug.Log("<color=lightblue>PROTEUS</color> Initializing Client...");
            await InitClient();
            Debug.Log("<color=lightblue>PROTEUS</color> CommsController Init Completed.");
            
        }

        private async Task InitClient()
        {
            _mqttClient = _mqttFactory.CreateMqttClient();
            await ConnectClient();
            await SubscribeTopics();
        }

        private async Task InitServer()
        {
            Debug.Log("<color=lightblue>PROTEUS</color> starting server...");
            MqttServerOptions optionsBuilder = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint().WithDefaultEndpointPort(1883).Build();
            _mqttServer = _mqttFactory.CreateMqttServer(optionsBuilder);
            await _mqttServer.StartAsync();
            Debug.Log("<color=lightblue>PROTEUS</color> Server Started.");
        }

        /// <summary>
        /// Connect the communication client to the server.
        /// </summary>
        /// <returns></returns>
        private async Task ConnectClient()
        {
            await DisconnectClient();

            // Setup the message handling so that queued messages are not lost.
            _mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                _mqttMessageQueue.Enqueue(e.ApplicationMessage);
                return Task.CompletedTask;
            };

            // Connect the client
            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("127.0.0.1").Build();
            await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
        }

        /// <summary>
        /// Process a received message.
        /// </summary>
        /// <param name="message"></param>
        private void ProcessMessage(MqttApplicationMessage message)
        {
            string t = message.Topic;

            // Node Updates
            if (t.StartsWith("proteus/data/update/3dml/nodes"))
            {
                PTNode nodeUpdate = JsonConvert.DeserializeObject<PTNode>(message.ConvertPayloadToString());
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
                PTModelElement modelElementUpdate = JsonConvert.DeserializeObject<PTModelElement>(message.ConvertPayloadToString());
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
                string id = message.Topic.Split("/").Last();

                Debug.Log($"<color=lightblue>PROTEUS</color> Node Update received for node {id}");
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(message.Payload);

                PTNode nodeUpdate = new()
                {
                    Id = id,
                    ImageTexture = tex,
                };
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
        /// Called on unity update to ensure the messages are processed in the main thread.
        /// </summary>
        public void Update()
        {
            while (_mqttMessageQueue.TryDequeue(out MqttApplicationMessage message))
            {
                ProcessMessage(message);
            }
            
        }
    }
}
