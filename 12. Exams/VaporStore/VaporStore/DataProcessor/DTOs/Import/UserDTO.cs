namespace VaporStore.DataProcessor.DTOs.Import
{
    using Data.Models;
    using System.ComponentModel.DataAnnotations;

    public class UserDTO
    {
        [Required]
        [StringLength(maximumLength: 20, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [RegularExpression("([A-Z]{1}[a-z]+) (([A-Z]{1}[a-z]+))")]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [Range(3, 103)]
        public int Age { get; set; }

        public Card[] Cards { get; set; }
    }
}
