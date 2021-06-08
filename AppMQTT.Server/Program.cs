using OpenNETCF.MQTT;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMQTT.Server
{
    class Program
    {
        static MQTTClient mqtt;
        static void Main(string[] args)
        {
            Console.WriteLine("Conectando a MQTT...");
            mqtt = new MQTTClient("broker.hivemq.com", 1883);
            mqtt.MessageReceived += Mqtt_MessageReceived;
            mqtt.Connect("AppMqttServer");
            mqtt.Subscriptions.Add(new Subscription("ServerProfeCarlos"));
            Console.WriteLine("Preciona una tecla para terminar...");
            Console.ReadLine();
        }

        private static void Mqtt_MessageReceived(string topic, QoS qos, byte[] payload)
        {
            string mensaje = Encoding.UTF8.GetString(payload);
            Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: {mensaje}");
            string d="";
            int v=0;
            if (mensaje.Contains("LED"))
            {
                d = "LED";
                v = mensaje.Contains("ENCENDIO") ? 1 : 0;
            }
            if (mensaje.Contains("Servo"))
            {
                d = "Servo";
                v = int.Parse(mensaje.Substring(6));
            }
            if (mensaje.Contains("Fotoresistencia"))
            {
                d = "Fotoresistencia";
                v = int.Parse(mensaje.Substring(16));
            }
            if (mensaje.Contains("TEMPERATURA"))
            {
                d = "TEMPERATURA";
                v = int.Parse(mensaje.Substring(6));
            }
            if (mensaje.Contains("HUMEDAD"))
            {
                d = "HUMEDAD";
                v = int.Parse(mensaje.Substring(6));
            }
            DAL.Insertar(new BD()
            {
                Dispositivo = d,
                Valor = v
            });
        }
    }
}
