using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.Data.Models
{
   public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [RegularExpression(@"^[A-Z][a-z]* [A-Z][a-z]*$")]
        public string FullName { get; set; }

        public string Email { get; set; }

        [Required]
        [Range(3, 103)]
        public int Age { get; set; }

        public ICollection<Card> Cards { get; set; }

    }
}
