using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VaporStore.Data.Models;

namespace VaporStore.DataProcessor.Dto.Import
{
   public class ImportGameDto
    {
        [Required]
        [JsonProperty("Name")]
        public string Name { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        [JsonProperty("Price")]
        public decimal Price { get; set; }

        [Required]
        [JsonProperty("ReleaseDate")]
        public DateTime ReleaseDate { get; set; }

        [Required]
        [JsonProperty("Developer")]
        public string Developer { get; set; }

        [JsonProperty("Genre")]
        public string Genre { get; set; }

        [JsonProperty("Tags")]
        public string[] Tags { get; set; }

    }
}
