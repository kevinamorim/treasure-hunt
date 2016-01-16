using System;
using System.Collections.Generic;

namespace TreasureHunt.Api.Models
{
    public class Game
    {
        public Guid Id
        {
            get; set;
        }

        public List<User> Users
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