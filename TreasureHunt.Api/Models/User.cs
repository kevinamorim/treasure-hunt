﻿using System;

namespace TreasureHunt.Api.Models
{
    public class User
    {
        public Guid Id
        {
            get; set;
        }

        public string Email
        {
            get; set;
        }

        public double Longitude
        {
            get; set;
        }

        public double Latitude
        {
            get; set;
        }
    }
}