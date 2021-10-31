using ArielWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArielWebAPI.DBs
{
    public interface IUserRepo
    {
        public IList<User> GetCollection();
        public void Insert(User user);

        public void Remove(User user);
    }
}
