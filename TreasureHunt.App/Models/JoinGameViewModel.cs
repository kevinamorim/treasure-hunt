using System;

namespace TreasureHunt.App.Models
{
    public class JoinGameViewModel
    {
        public Guid Id
        {
            get; set;
        }

        public string GameName
        {
            get; set;
        }

        public string Difficulty
        {
            get; set;
        }
    }
}
