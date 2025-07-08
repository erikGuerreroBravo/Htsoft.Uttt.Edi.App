using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Xml;


namespace Htsoft.Uttt.Edi.Infraestructure.Mongo
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> options)
        {
            var cliente = new MongoClient(options.Value.ConnectionString);
            _database = cliente.GetDatabase(options.Value.Database);
        }
        public IMongoCollection<MyEntity> MyEntities => _database.GetCollection<MyEntity>("MyEntities");
    }
}
