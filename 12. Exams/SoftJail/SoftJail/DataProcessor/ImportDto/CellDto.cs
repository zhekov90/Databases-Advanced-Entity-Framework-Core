using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    public class CellDto
    {
        [JsonPropertyName("CellNumber")]
        [Required]
        [Range(1, 1000)]
        public int CellNumber { get; set; }

        [JsonPropertyName("HasWindow")]
        public bool HasWindow { get; set; }
    }
}
