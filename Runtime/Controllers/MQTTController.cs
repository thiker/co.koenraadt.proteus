using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Packages.co.koenraadt.proteus.Runtime.Repository;

namespace Packages.co.koenraadt.proteus.Runtime.Controllers
{
    internal class MQTTController
    {
        private static MQTTController _instance = null;
        private static MqttFactory _mqttFactory = new();
        private IMqttClient _mqttClient;


        public static MQTTController Instance
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
        public async Task Init()
        {
            _mqttClient = _mqttFactory.CreateMqttClient();
            await ConnectClient();
            await SendMessage();
        }

        private async Task ConnectClient()
        {
            await DisconnectClient();

            // Setup the message handling so that queued messages are not lost.
            _mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                Debug.Log("received message");
                // TOD: Implement handler that is run on every update so the events are handled on the main thread.
                return Task.CompletedTask;
            };

            // Connect the client
            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("127.0.0.1").Build();
            await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            //TODO: remove test subscribe
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("proteus/debug").Build());
        }

        public async Task DisconnectClient()
        {
            await _mqttClient?.DisconnectAsync(new MqttClientDisconnectOptionsBuilder().WithReason(MqttClientDisconnectReason.NormalDisconnection).Build());
        }

        private async Task SendMessage()
        {
            var message = new MqttApplicationMessageBuilder()
            .WithTopic("MyTopic")
            .WithPayload("Hello World")
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

            await _mqttClient.PublishAsync(message, CancellationToken.None);
        }
    }
}
