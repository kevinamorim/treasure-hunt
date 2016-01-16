using System;

namespace TreasureHunt.Api.Models
{
    public class JoinGame
    {
        public Guid GameId
        {
            get; set;
        }

        public Guid UserId
        {
            get; set;
        }
    }
}