using System;

namespace TreasureHunt.App.Models
{
    public class Game
    {
        public Guid Id
        {
            get; set;
        }

        public string Username
        {
            get; set;
        }

        public int Difficulty
        {
            get; set;
        }

        public bool Finished
        {
            get; set;
        }

        public DateTime StartedAt
        {
            get; set;
        }
    }
}
