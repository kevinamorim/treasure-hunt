using Newtonsoft.Json;
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
        [Route("api/games/{id}")]
        public Game Get(Guid id)
        {
            return games.Find(m => m.Id == id);
        }

        // GET api/games/finished
        [Route("api/games/{finished:alpha}")]
        public IEnumerable<Game> Get(bool finished)
        {
            return games.FindAll(m => m.Finished == finished);
        }

        [HttpGet]
        [Route("api/games/GetPlayers/{id}")]
        public IEnumerable<User> GetPlayers(Guid id)
        {
            Game game = games.Find(m => m.Id == id);
            if (game != null)
            {
                return game.Users;
            }

            return new List<User>();
        }

        // POST api/games
        public Guid Post(Game game)
        {
            game.Id = Guid.NewGuid();
            game.StartedAt = DateTime.Now;
            game.Users = new List<User>();
            games.Add(game);
            return game.Id;
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
        [Route("api/games/JoinGame/")]
        public string JoinGame([FromBody] JoinGame joinGame)
        {

            Game game = games.Find(m => m.Id == joinGame.GameId);

            if (game != null)
            {
                User user = UsersController.users.Find(m => m.Id == joinGame.UserId);

                if (user != null)
                {
                    if (!game.Users.Contains(user))
                    {
                        game.Users.Add(user);
                    }
                }
            }

            return joinGame.GameId.ToString();
        }

        // DELETE api/games
        public void Delete(int id)
        {
        }

    }
}
