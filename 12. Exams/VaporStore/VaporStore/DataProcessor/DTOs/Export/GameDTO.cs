namespace VaporStore.DataProcessor.DTOs.Export
{
    using System.Xml.Serialization;

    [XmlType("Game")]
    public class GameDTO
    {
        [XmlAttribute("title")]
        public string Title { get; set; }

        public string Genre { get; set; }

        public decimal Price { get; set; }
    }
}
