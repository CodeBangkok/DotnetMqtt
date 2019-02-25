using System;
using System.Net.Mqtt;
using System.Text;
using System.IO;

namespace DotnetMqtt.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = "test.mosquitto.org";
            var port = 1883;
            var topic = "codebangkok";

            //Subscribe
            var client = MqttClient.CreateAsync(host, port).Result;
            client.Disconnected += Client_Disconnected;

            var payload = Encoding.UTF8.GetBytes("Dead");
            var lastWill = new MqttLastWill(topic, MqttQualityOfService.AtMostOnce, false, payload);

            var session = client.ConnectAsync(lastWill).Result;
            client.SubscribeAsync(topic, MqttQualityOfService.AtMostOnce);
            client.MessageStream.Subscribe(ReceivedMessage);

            //Publish
            while (true)
            {
                Console.WriteLine("Enter text to publish.");
                var text = Console.ReadLine();
                if (text == "x")
                {
                    client.DisconnectAsync();
                    break;
                }

                payload = Encoding.UTF8.GetBytes(text);
                var message = new MqttApplicationMessage(topic, payload);
                client.PublishAsync(message, MqttQualityOfService.AtMostOnce, false);

                //Send Photo
                //var filename = "bonddev.jpg";
                //if (File.Exists(filename))
                //{
                //    payload = File.ReadAllBytes(filename);
                //    var message = new MqttApplicationMessage(topic, payload);
                //    client.PublishAsync(message, MqttQualityOfService.AtMostOnce, false);
                //}
            }
        }

        static void ReceivedMessage(MqttApplicationMessage message)
        {
            var text = Encoding.UTF8.GetString(message.Payload);
            Console.WriteLine($"Received => {text}");
        }

        static void Client_Disconnected(object sender, MqttEndpointDisconnected e)
        {
            Console.WriteLine("Disconnected.");
        }

    }
}
