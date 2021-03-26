namespace VaporStore.DataProcessor.DTOs.Export
{
    using System.Xml.Serialization;

    [XmlType("User")]
    public class UserDTO
    {
        [XmlAttribute("username")]
        public string Username { get; set; }

        public PurchaseDTO[] Purchases { get; set; }

        public decimal TotalSpent { get; set; }
    }
}
