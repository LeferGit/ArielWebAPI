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
    public class UserMongoDbRepository : IUserRepository
    {
        private class MongoUser
        {
            public ObjectId id;
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        readonly IMongoDatabase db;
        public UserMongoDbRepository()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            db = client.GetDatabase("arielTutorialDb");
        }
        public List<User> GetUsers()
        {
            try
            {
                var mongoUsers = db.GetCollection<MongoUser>("users").Find(Builders<MongoUser>.Filter.Empty).ToList();

                var users = new List<User>();

                mongoUsers.ForEach(mongoUser =>
                users.Add(
                    new User()
                    {
                        Id =  mongoUser.id.ToString(),
                        FirstName = mongoUser.FirstName,
                        LastName = mongoUser.LastName
                    }));

                return users;
                //return db.GetCollection<User>("users").FindSync(
                //Builders<User>.Filter.Empty,
                //new FindOptions<User>() { Projection = "{ _id : 0 }" }).ToList();
            }
            catch(Exception exc)
            {
                return null;
            }
        }

        public void Insert(User user)
        {
            db.GetCollection<MongoUser>("users").InsertOne(new MongoUser(){
                id = ObjectId.GenerateNewId(),
                FirstName = user.FirstName,
                LastName = user.LastName});
        }

        public void Remove(string id)
        {
            db.GetCollection<MongoUser>("users").DeleteOne(x=>x.id == ObjectId.Parse(id));
        }

        public User GetUser(string id)
        {
            var mongoUser = db.GetCollection<MongoUser>("users").Find(
                Builders<MongoUser>.Filter.Eq(mongoUser => mongoUser.id, ObjectId.Parse(id))).FirstOrDefault();

            if (mongoUser == null)
                return null;

            return new User() { 
                Id = mongoUser.id.ToString(), 
                FirstName = mongoUser.FirstName, 
                LastName = mongoUser.LastName };
        }
    }
}
