using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;
using VaporStore.Data.Models;

namespace VaporStore.DataProcessor.Dto.Import
{
   public class ImportUserDto
    {
        [Required]
        [RegularExpression("([A-Z]{1}[a-z]+) (([A-Z]{1}[a-z]+))")]
        public string FullName { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [Range(3, 103)]
        public int Age { get; set; }

        [JsonPropertyName("Cards")]
        public Card[] Cards { get; set; }


    }

}
