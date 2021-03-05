namespace BookShop
{

    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Globalization;
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
            // var result = GetGoldenBooks(db);

            //3. Books by Price
            //var result = GetBooksByPrice(db);

            //4. Not Released In
            //int year = int.Parse(Console.ReadLine());
            //var result = GetBooksNotReleasedIn(db, year);

            //5. Book Titles by Category
            //var input = Console.ReadLine();
            //var result = GetBooksByCategory(db, input);

            //6. Released Before Date
            var input = Console.ReadLine();
            var result = GetBooksReleasedBefore(db, input);

            Console.WriteLine(result);
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var targetDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var books = context.Books
                .Where(x => x.ReleaseDate.HasValue && x.ReleaseDate.Value < targetDate)
                .Select(x=>new
                {
                    x.ReleaseDate,
                    x.Title,
                    x.EditionType,
                    x.Price
                })
                .OrderByDescending(x=>x.ReleaseDate)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input
                     .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                     .Select(c => c.ToLower())
                     .ToList();

            var books = context.Books
                .Where(x => x.BookCategories.Any(c => categories.Contains(c.Category.Name.ToLower())))
                .Select(x=>x.Title)
                .OrderBy(title=>title)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(x => x.ReleaseDate.HasValue && x.ReleaseDate.Value.Year != year)
                .OrderBy(x => x.BookId)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(x => x.Price > 40)
                .Select(x => new
                {
                    x.Title,
                    x.Price
                })
                .OrderByDescending(x => x.Price)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
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

            var result = string.Join(Environment.NewLine, goldenBooks.Select(x => x.Title));

            return result;
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var ageRestriction = Enum.Parse<AgeRestriction>(command, true);

            var books = context.Books
                .Where(x => x.AgeRestriction == ageRestriction)
                .Select(x => x.Title)
                .OrderBy(title => title)
                .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }
    }
}
