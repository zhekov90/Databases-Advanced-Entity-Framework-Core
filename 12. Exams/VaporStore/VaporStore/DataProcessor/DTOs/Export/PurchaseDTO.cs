namespace VaporStore.DataProcessor.DTOs.Export
{
    using System.Xml.Serialization;

    [XmlType("Purchase")]
    public class PurchaseDTO
    {
        public string Card { get; set; }

        public string Cvc { get; set; }

        public string Date { get; set; }

        public GameDTO Game { get; set; }
    }
}
