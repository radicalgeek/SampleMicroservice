using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Prototype.Data.Interface;

namespace Prototype.Data
{

    public class MongoConnectionHandler<T> where T : IEntity
    {
        public IMongoCollection<T> MongoCollection { get; private set; }

        public MongoConnectionHandler()
        {
            string connectionString = ConfigurationManager.AppSettings["MongoConnection"];
            string databaseName = ConfigurationManager.AppSettings["MongoDataBase"];
            var mongoClient = new MongoClient(connectionString);

            var dataBase = mongoClient.GetDatabase(databaseName);

            MongoCollection = dataBase.GetCollection<T>(typeof (T).Name.ToLower() + "s");
        }
    }
}
