using System;
using System.Collections.Generic;
using System.Web.Http;
using TreasureHunt.Api.Models;

namespace TreasureHunt.Api.Controllers
{
    public class GamesController : ApiController
    {
        private static List<Game> games = new List<Game>
        {
            new Game()
            {
                Id = Guid.NewGuid(),
                Username = "testUser",
                Difficulty = 1,
                Finished = false,
                StartedAt = DateTime.Now
            },
        };

        // GET api/games
        public IEnumerable<Game> Get()
        {
            return games;
        }

        // GET api/games
        public string Get(int id)
        {
            return "value";
        }

        // POST api/games
        public void Post(Game game)
        {
            games.Add(game);
        }

        // PUT api/games
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/games
        public void Delete(int id)
        {
        }
    }
}
