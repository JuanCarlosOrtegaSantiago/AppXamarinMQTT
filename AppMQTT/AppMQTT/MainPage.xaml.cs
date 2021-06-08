using Microcharts;
using OpenNETCF.MQTT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppMQTT
{
    public partial class MainPage : ContentPage
    {
        MQTTClient mqtt;
        List<string> mensajes;
        List<ChartEntry> puntos;
        int m = 0;
        Random r = new Random();
        public MainPage()
        {
            InitializeComponent();
            mensajes = new List<string>();
            puntos = new List<ChartEntry>();
            mqtt = new MQTTClient("broker.hivemq.com", 1883);
            mqtt.MessageReceived += Mqtt_MessageReceived;
            mqtt.Connect("AppMovilESP8266");
            mqtt.Subscriptions.Add(new Subscription("ServerProfeCarlos"));
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (mensajes.Count > m)
                {
                    lstMensajes.ItemsSource = null;
                    lstMensajes.ItemsSource = mensajes;
                    m = mensajes.Count;
                    if (mensajes.Last().Contains("Fotoresistencia"))
                       // (mensajes.Last().Contains("TEMPERATURA"))
                    {
                        float v = v = float.Parse(mensajes.Last().Substring(16));
                        puntos.Add(new Microcharts.ChartEntry(v)
                        {
                            Label = DateTime.Now.ToShortTimeString(),
                            Color = new SkiaSharp.SKColor(byte.Parse(r.Next(0,255).ToString()),byte.Parse(r.Next(0,255).ToString()), byte.Parse(r.Next(0,255).ToString())),
                            ValueLabel= v.ToString()
                        });
                        charView.Chart = null;
                        charView.Chart = new LineChart()
                        {
                            Entries = puntos
                        };

                    }

                }
                return true;

            });

        }

        private void Mqtt_MessageReceived(string topic, QoS qos, byte[] payload)
        {
            mensajes.Add(Encoding.UTF8.GetString(payload));
        }

        private void btnEncenderLed_Clicked(object sender, EventArgs e)
        {
            if (!mqtt.IsConnected)
                return;
                mqtt.Publish("ESP8266ProfeCarlos", "L1", QoS.FireAndForget, false);
        }

        private void btnApagarLed_Clicked(object sender, EventArgs e)
        {
            if (!mqtt.IsConnected)
                return;
            mqtt.Publish("ESP8266ProfeCarlos", "L0", QoS.FireAndForget, false);
        }

        private void btnEstatusLed_Clicked(object sender, EventArgs e)
        {
            if (!mqtt.IsConnected)
                return;
                mqtt.Publish("ESP8266ProfeCarlos", "?L", QoS.FireAndForget, false);
        }

        private void btnMoverServo_Clicked(object sender, EventArgs e)
        {
            if (!mqtt.IsConnected || string.IsNullOrWhiteSpace(entAngulo.Text))
                return;

            string Txt = string.Format("S{0}", entAngulo.Text);

            mqtt.Publish("ESP8266ProfeCarlos", Txt, QoS.FireAndForget, false);
        }

        private void btnConsultarLuminosidad_Clicked(object sender, EventArgs e)
        {
            if (!mqtt.IsConnected)
                return;

                mqtt.Publish("ESP8266ProfeCarlos", "?F", QoS.FireAndForget, false);
        }

        private void btnColsultarTemperatura_Clicked(object sender, EventArgs e)
        {
            mqtt.Publish("ESP8266ProfeCarlos", "?T", QoS.FireAndForget, false);
        }

        private void btnColsultarHumedad_Clicked_1(object sender, EventArgs e)
        {
            mqtt.Publish("ESP8266ProfeCarlos", "?H", QoS.FireAndForget, false);
        }
    }
}
