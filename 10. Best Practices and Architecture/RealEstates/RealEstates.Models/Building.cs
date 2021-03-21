using System.Collections.Generic;

namespace RealEstates.Models
{
   public class Building
    {
        public Building()
        {
            this.Properties = new HashSet<Property>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Property> Properties { get; set; }
    }
}
