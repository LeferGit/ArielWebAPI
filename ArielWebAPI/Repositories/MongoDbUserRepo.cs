using ArielWebAPI.DBs;
using ArielWebAPI.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArielWebAPI.Repositories
{
    public class MongoDbUserRepo : IUserRepo
    {
        readonly IMongoDatabase db;
        public MongoDbUserRepo()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            db = client.GetDatabase("arielTutorialDb");
        }
        public IList<User> GetCollection()
        {
            try
            {
                return db.GetCollection<User>("users").FindSync(
                Builders<User>.Filter.Empty,
                new FindOptions<User>() { Projection = "{ _id : 0 }" }).ToList();
            }
            catch(Exception exc)
            {
                return null;
            }
        }

        public void Insert(User user)
        {
            db.GetCollection<User>("users").InsertOne(user);
        }

        public void Remove(User user)
        {
            db.GetCollection<User>("users").DeleteOne(x=>x.LastName.Equals(user.LastName));
        }
    }
}
