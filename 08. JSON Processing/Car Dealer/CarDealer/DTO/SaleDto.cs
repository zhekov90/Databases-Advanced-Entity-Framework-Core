using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO
{
   public class SaleDto
    {
        public int CarId { get; set; }

        public int CustomerId { get; set; }

        public int Discount { get; set; }
    }
}