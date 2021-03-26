namespace VaporStore.DataProcessor.DTOs.Import
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class GameDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0.0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Developer { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Genre { get; set; }

        [MinLength(1)]
        public string[] Tags { get; set; }
    }
}
