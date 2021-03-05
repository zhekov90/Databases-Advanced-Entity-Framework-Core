namespace BookShop
{
    
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
             var db = new BookShopContext();
            // DbInitializer.ResetDatabase(db);


            // 1. Age Restriction
            //  var result = GetBooksByAgeRestriction(db, "miNor");

            //2. Golden Books
            var result = GetGoldenBooks(db);

            Console.WriteLine(result);
        }

        public static string GetGoldenBooks(BookShopContext context)
        {

            var goldenBooks = context.Books
                .Where(x => x.EditionType == EditionType.Gold && x.Copies < 5000)
                .Select(x => new
                { 
                    x.BookId,
                    x.Title 
                })
                .OrderBy(x => x.BookId)
                .ToList();

            var result = string.Join(Environment.NewLine, goldenBooks.Select(x=>x.Title));

            return result;
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var ageRestriction = Enum.Parse<AgeRestriction>(command, true);

            var books = context.Books
                .Where(x => x.AgeRestriction == ageRestriction)
                .Select(x=>x.Title)
                .OrderBy(title => title)
                .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }
    }
}
