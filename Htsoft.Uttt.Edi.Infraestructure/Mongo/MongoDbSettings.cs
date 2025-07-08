using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Htsoft.Uttt.Edi.Infraestructure.Mongo
{
    public class MongoDbSettings
    {
        /// <summary>
        /// Cadena de conexion para MongoDB.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;
        /// <summary>
        /// Nombre de la base de datos MongoDB.
        /// </summary>
        public string Database { get; set; } = string.Empty;
    }
}
