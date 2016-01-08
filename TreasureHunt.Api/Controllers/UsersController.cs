using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TreasureHunt.Api.Models;

namespace TreasureHunt.Api.Controllers
{
    public class UsersController : ApiController
    {

        private static List<User> users = new List<User>
        {
            new User()
            {
                Id = Guid.NewGuid(),
                Email = "test@test.com",
                Latitude = 0,
                Longitude = 0
            },

            new User()
            {
                Id = Guid.NewGuid(),
                Email = "kevinamorim4@gmail.com",
                Latitude = 1,
                Longitude = 2
            }
        };

        // GET api/users
        public IEnumerable<User> Get()
        {
           return users;
        }

        // GET api/users/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/users
        public void Post(User user)
        {
            users.Add(user);
        }

        // PUT api/users/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/users/5
        public void Delete(int id)
        {
        }
    }
}
