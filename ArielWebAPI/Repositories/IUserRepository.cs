using ArielWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArielWebAPI.DBs
{
    public interface IUserRepository
    {
        public Task<List<User>> GetUsersAsync();
        public Task InsertUserAsync(User user);
        public Task RemoveUserAsync(string id);
        public Task<User> GetUserAsync(string id);
        public Task UpdateUserAsync(User user);
    }
}
