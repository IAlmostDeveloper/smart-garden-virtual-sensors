using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;

namespace SmartGardenVirtualSensor
{
    class MainClass
    {
        private static ManagedMqttClientOptions TcpMqttClientOptions(string url)
        {
            return new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(
                    new MqttClientOptionsBuilder()
                        .WithClientId("EMQX_" + Guid.NewGuid().ToString())
                        .WithTcpServer(url)
                        //.WithCredentials("user", "pass")
                        .WithCleanSession()
                        .Build()
                )
                .Build();
        }

        public static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Console.InputEncoding = System.Text.Encoding.GetEncoding("UTF-8");

            var humidityHelper = new HumidityHelper(50.0f, 0.2f);
            string topic = string.Format("/class/stand{0}/humidity", args[0]);
            string commandsTopic = string.Format("/class/stand{0}/commands", args[0]);
            ManagedMqttClientOptions options = TcpMqttClientOptions(args[1]);
            var mqttClient = new MqttFactory().CreateManagedMqttClient();

            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(commandsTopic).Build());
            mqttClient.UseApplicationMessageReceivedHandler(
                msg => {
                    Console.Write(
                        "Получено сообщение: '{0}' from '{1}'\n",
                        System.Text.Encoding.UTF8.GetString(msg.ApplicationMessage.Payload),
                        msg.ApplicationMessage.Topic
                    );
                    Console.WriteLine("Начинаю полив");
                    humidityHelper.IncreaseHumidity(10.0f);
                    }
            );

            mqttClient.UseConnectedHandler(
                (arg) => Console.WriteLine("Establish connection " + arg.ConnectResult.ResultCode)
            );

            await mqttClient.StartAsync(options);
            Console.WriteLine("Модуль полива запущен");
            Console.Write("Пишем в топик '{0}'\n", topic);
            string payload;
            //while ((payload = Console.ReadLine()) != "quit")
            while(true)
            {
                payload = Math.Round(humidityHelper.CurrentValue, 2).ToString();

                await mqttClient.PublishAsync(
                    new MqttApplicationMessageBuilder()
                        .WithTopic(topic)
                        .WithPayload(payload)
                        .WithExactlyOnceQoS()
                        .Build()
                );
                await Task.Delay(5000);
                humidityHelper.Tick();
            }

            await mqttClient.StopAsync();
        }
    }
}