using System;
using System.ComponentModel.DataAnnotations;

namespace TreasureHunt.Api.Models
{
    public class User
    {
        [Required]
        public Guid Id
        {
            get; set;
        }

        [Required]
        public string Username
        {
            get; set;
        }

        [Required]
        public DateTime CreatedAt
        {
            get; set;
        }
    }
}