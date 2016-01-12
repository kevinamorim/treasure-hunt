using System;

namespace TreasureHunt.App.Models
{
    public class GameView
    {
        public Guid Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string Difficulty
        {
            get; set;
        }

        public string Finished
        {
            get; set;
        }
    }
}
