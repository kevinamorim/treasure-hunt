using System.Collections.Generic;
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

        // DELETE api/games
        public void Delete(int id)
        {
        }
    }
}
