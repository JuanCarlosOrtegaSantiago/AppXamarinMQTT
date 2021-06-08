using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMQTT.Server
{
    public static class DAL
    {
        public static void Insertar(BD registro)
        {
            try
            {
                registro.Id = Guid.NewGuid().ToString();
                registro.FechaHora = DateTime.Now;
                using (var db = new LiteDatabase(new ConnectionString() { Filename = "Registros.db" }))
                {
                    db.GetCollection<BD>(typeof(BD).Name).Insert(registro);
                    Console.WriteLine("Insertado");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
