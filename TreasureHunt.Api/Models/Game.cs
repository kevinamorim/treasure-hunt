using System;

namespace TreasureHunt.Api.Models
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