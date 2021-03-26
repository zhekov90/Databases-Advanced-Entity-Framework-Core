namespace BookShop.Data.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Author
    {
        public Author()
        {
            this.AuthorsBooks = new HashSet<AuthorBook>();
        }

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public ICollection<AuthorBook> AuthorsBooks { get; set; }
    }
}