using System;
using System.Collections.Generic;
using System.Text;

namespace AppMQTT.Server
{
    public class BD
    {
        public string Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string Dispositivo { get; set; }
        public int Valor { get; set; }
    }
}
