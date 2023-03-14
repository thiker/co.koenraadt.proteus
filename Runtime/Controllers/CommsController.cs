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

namespace Packages.co.koenraadt.proteus.Runtime.Controllers
{
    public class CommsController
    {
        private static CommsController _instance = null;
        private static MqttFactory _mqttFactory = new();
        private static ConcurrentQueue<MqttApplicationMessage> _mqttMessageQueue = new();
        private IMqttClient _mqttClient;

        public static CommsController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ();
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
            _mqttClient = _mqttFactory.CreateMqttClient();
            await ConnectClient();
            await SubscribeTopics();
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
            Debug.Log($"Processing message: {message.ConvertPayloadToString()} of topic {message.Topic}");
        }

        /// <summary>
        /// Subscribes to the desired communication topics.
        /// </summary>
        /// <returns></returns>
        private async Task SubscribeTopics()
        {
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("proteus/debug").Build());
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
