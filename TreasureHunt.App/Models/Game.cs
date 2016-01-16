using System;

namespace TreasureHunt.App.Models
{
    public class Game
    {
        public Guid Id
        {
            get; set;
        }

        public string Name
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

        public DateTime FinishedAt
        {
            get; set;
        }

        public double OriginalLatitude
        {
            get; set;
        }

        public double OriginalLongitude
        {
            get; set;
        }

        public double TargetLatitude
        {
            get; set;
        }

        public double TargetLongitude
        {
            get; set;
        }
    }
}
