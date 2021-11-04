using ArielWebAPI.DBs;
using ArielWebAPI.Models;
using Microsoft.Extensions.Logging;
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

        private readonly IMongoDatabase db;
        private readonly ILogger _logger;

        public UserMongoDbRepository(ILogger<UserMongoDbRepository> logger)
        {
            _logger = logger;

            var client = new MongoClient("mongodb://localhost:27017");
            db = client.GetDatabase("arielTutorialDb");
        }
        public async Task<List<User>> GetUsersAsync()
        {
            try
            {
                var mongoUsers =  (await db.GetCollection<MongoUser>("users").FindAsync(Builders<MongoUser>.Filter.Empty)).ToListAsync();

                var users = new List<User>();

                mongoUsers.Result.ForEach(mongoUser =>
                users.Add(
                    new User()
                    {
                        Id =  mongoUser.id.ToString(),
                        FirstName = mongoUser.FirstName,
                        LastName = mongoUser.LastName
                    }));

                return users;
            }
            catch(Exception exc)
            {
                _logger.LogError(exc.ToString());
                return null;
            }
        }

        public async Task InsertUserAsync(User user)
        {
            await db.GetCollection<MongoUser>("users").InsertOneAsync(new MongoUser(){
                id = ObjectId.GenerateNewId(),
                FirstName = user.FirstName,
                LastName = user.LastName});
        }

        public async Task RemoveUserAsync(string id)
        {
            await db.GetCollection<MongoUser>("users").DeleteOneAsync(x=>x.id == ObjectId.Parse(id));
        }

        public async Task<User> GetUserAsync(string id)
        {
            var mongoUser = await db.GetCollection<MongoUser>("users").FindAsync(
                Builders<MongoUser>.Filter.Eq(mongoUser => mongoUser.id, ObjectId.Parse(id))).Result.FirstOrDefaultAsync();

            if (mongoUser == null)
                return null;

            return new User() { 
                Id = mongoUser.id.ToString(), 
                FirstName = mongoUser.FirstName, 
                LastName = mongoUser.LastName };
        }

        public async Task UpdateUserAsync(User user)
        {
            var MongoUser = new MongoUser()
            {
                id = ObjectId.Parse(user.Id),
                FirstName = user.FirstName,
                LastName = user.LastName
            };
            try
            {
                await db.GetCollection<MongoUser>("users").ReplaceOneAsync(
                Builders<MongoUser>.Filter.Eq(mongoUser => mongoUser.id, ObjectId.Parse(user.Id)),
                MongoUser);
            }
            catch(Exception exc)
            {
                _logger.LogError(exc.ToString());
            }
        }
    }
}
