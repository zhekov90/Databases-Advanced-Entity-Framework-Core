namespace CarDealer.DTO
{
    using System.Collections.Generic;
    using Models;

    public class CarPartsDTO
    {
        public int Id { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public long TravelledDistance { get; set; }

        public List<int> PartsId { get; set; }
    }
}
