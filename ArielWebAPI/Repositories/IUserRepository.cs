using ArielWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArielWebAPI.DBs
{
    public interface IUserRepository
    {
        public List<User> GetUsers();
        public void Insert(User user);
        public void Remove(string id);
        public User GetUser(string id);
    }
}
