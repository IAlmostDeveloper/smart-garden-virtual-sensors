using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using MQTTnet.Client.Subscribing;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartGardenVirtualSensor
{
    public class MqttHelper
    {
        public string MqttAddress { get; set; }
        public int MqttPort { get; set; }


        public MqttHelper(string address, int port)
        {
            MqttAddress = address;
            MqttPort = port;
        }

        public async Task<MqttClientPublishResult> Connect()
        {
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(MqttAddress, MqttPort) // Port is optional
                .Build();
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            await mqttClient.ConnectAsync(options, CancellationToken.None);
            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();

                Task.Run(() => mqttClient.PublishAsync("hello/world"));
            });

            mqttClient.UseConnectedHandler(async e =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");

                // Subscribe to a topic
                await mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder().WithTopicFilter("my/topic").Build());

                Console.WriteLine("### SUBSCRIBED ###");
            });

            var message = new MqttApplicationMessageBuilder()
                .WithTopic("MyTopic")
                .WithPayload("Hello World")
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            

            return await mqttClient.PublishAsync(message, CancellationToken.None);
        }

    }
}
