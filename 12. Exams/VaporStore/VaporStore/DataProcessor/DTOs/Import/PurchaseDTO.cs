namespace VaporStore.DataProcessor.DTOs.Import
{
    using Data.Models.Enums;
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Purchase")]
    public class PurchaseDTO
    {
        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlElement]
        [Required]
        public PurchaseType Type { get; set; }

        [XmlElement]
        [RegularExpression("^([A-Z0-9]{4}\\-[A-Z0-9]{4}\\-[A-Z0-9]{4})$")]
        public string Key { get; set; }

        [XmlElement]
        public string Card { get; set; }

        [XmlElement]
        public string Date { get; set; }
    }
}
