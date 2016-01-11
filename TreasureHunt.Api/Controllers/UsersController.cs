using System;
using System.Collections.Generic;
using System.Web.Http;
using TreasureHunt.Api.Models;

namespace TreasureHunt.Api.Controllers
{
    public class UsersController : ApiController
    {
        private static List<User> users = new List<User>();

        public IEnumerable<User> Get()
        {
            return users;
        }

        public Guid Post(User user)
        {

            User originalUser = users.Find(m => m.Username == user.Username);

            if (originalUser != null)
            {
                return originalUser.Id;
            }

            user.Id = Guid.NewGuid();
            user.CreatedAt = DateTime.Now;

            users.Add(user);

            return user.Id;

        }
    }
}
