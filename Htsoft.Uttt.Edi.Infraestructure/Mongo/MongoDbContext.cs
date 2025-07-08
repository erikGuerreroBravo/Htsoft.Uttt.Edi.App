using Htsoft.Uttt.Edi.Domain;
using Microsoft.Extensions.Options;
using MongoDB.Driver;


namespace Htsoft.Uttt.Edi.Infraestructure.Mongo
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<Htsoft.Uttt.Edi.Infraestructure.Mongo.MongoDbSettings> options)
        {
            var cliente = new MongoClient(options.Value.ConnectionString);
            _database = cliente.GetDatabase(options.Value.Database);
        }
        public IMongoCollection<EdiModel> EdiModels => _database.GetCollection<EdiModel>("MyEntities");
    }
}
