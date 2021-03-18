using System.Xml.Serialization;

namespace CarDealer.DTOs.Input
{
    [XmlType("Car")]
   public class CarInputModel
    {
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("TraveledDistance")]
        public long TraveledDistance { get; set; }

        [XmlArray("parts")]
        public CarPartsInputModel[] Parts { get; set; }
    }
}
