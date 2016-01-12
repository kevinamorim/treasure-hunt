using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using TreasureHunt.Api.Models;

namespace TreasureHunt.Api.Controllers
{
    public class GamesController : ApiController
    {
        private static List<Game> games = new List<Game>();

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

        // GET api/games/finished
        [Route("api/games/{finished:alpha}")]
        public IEnumerable<Game> Get(bool finished)
        {
            return games.FindAll(m => m.Finished == finished);
        }

        // POST api/games
        public void Post(Game game)
        {
            games.Add(game);
        }

        // PUT api/games
        public void Put(Game game)
        {
            Game originalGame = games.Find(m => m.Id == game.Id);
            if (originalGame != null)
            {
                originalGame.Finished = true;
                originalGame.FinishedAt = game.FinishedAt;
            }

        }

        [HttpPut]
        [Route("api/games/JoinGame/{id?}")]
        public string JoinGame(Guid id, User user)
        {
            Game game = games.Find(m => m.Id == id);

            if (game != null)
            {
                user.Id = Guid.NewGuid();
                user.CreatedAt = DateTime.Now;

                game.Users = new List<User>();
                game.Users.Add(user);
            }

            return id.ToString();
        }

        // DELETE api/games
        public void Delete(int id)
        {
        }

    }
}
