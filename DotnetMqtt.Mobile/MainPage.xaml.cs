using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Mqtt;
using System.IO;

namespace DotnetMqtt.Mobile
{
    public partial class MainPage : ContentPage
    {
        IMqttClient client;
        string host = "test.mosquitto.org";
        int port = 1883;
        string topic = "codebangkok";

        //string home = "home";
        //string floor1 = "home/floor1";
        //string room11 = "home/floor1/room1";
        //string room12 = "home/floor1/room2";

        //string floor1 = "home/floor2";
        //string room21 = "home/floor2/room1";
        //string room22 = "home/floor2/room2";

        //string sub = "home/#";

        public MainPage()
        {
            InitializeComponent();

            publishButton.Clicked += PublishButton_Clicked;
        }

        protected async override void OnAppearing()
        {
            client = await MqttClient.CreateAsync(host, port);
            var payload = Encoding.UTF8.GetBytes("Dead");
            var lastWill = new MqttLastWill(topic, MqttQualityOfService.AtMostOnce, false, payload);
            await client.ConnectAsync(lastWill);
            await client.SubscribeAsync(topic, MqttQualityOfService.AtMostOnce);
            client.MessageStream.Subscribe(ReceivedMessage);
        }

        void ReceivedMessage(MqttApplicationMessage message)
        {
            Device.BeginInvokeOnMainThread(() => {
                var text = Encoding.UTF8.GetString(message.Payload);
                messageLabel.Text = text;

                //ReceivePhoto
                //var stream = new MemoryStream(message.Payload);
                //photoImage.Source = ImageSource.FromStream(() => { return stream; });
            });
        }

        async void PublishButton_Clicked(object sender, EventArgs e)
        {
            var payload = Encoding.UTF8.GetBytes(textEntry.Text);
            var message = new MqttApplicationMessage(topic, payload);
            await client.PublishAsync(message, MqttQualityOfService.AtMostOnce, false);
        }

    }
}
