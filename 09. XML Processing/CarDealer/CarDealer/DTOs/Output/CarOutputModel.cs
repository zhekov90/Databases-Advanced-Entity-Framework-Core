﻿using System.Xml.Serialization;

namespace CarDealer.DTOs.Output
{
    [XmlType("car")]
   public class CarOutputModel
    {
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("travelled-distance")]
        public long TravelledDistance { get; set; }
    }
}
