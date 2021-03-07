namespace BookShop
{
    using BookShop.Models;
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
            // var input = Console.ReadLine();
            // var result = GetBooksReleasedBefore(db, input);

            //7. Author Search
            // var input = Console.ReadLine();
            // var result = GetAuthorNamesEndingIn(db, input);

            //8. Book Search
            // var input = Console.ReadLine();
            // var result = GetBookTitlesContaining(db, input);

            //9. Book Search by Author
            // var input = Console.ReadLine();
            // var result = GetBooksByAuthor(db, input);

            //10. Count Books
            //var lengthCheck = int.Parse(Console.ReadLine());
            //var result = CountBooks(db, lengthCheck);

            //11.Total Book Copies
            //var result = CountCopiesByAuthor(db);

            //12. Profit by Category
            //var result = GetTotalProfitByCategory(db);

            //13. Most Recent Books
            //var result = GetMostRecentBooks(db);

            //14. Increase Prices
            //IncreasePrices(db);

            //15.Remove Books
           var result = RemoveBooks(db);
            Console.WriteLine(result);

        }

        public static int RemoveBooks(BookShopContext context)
        {
            var booksCategories = context.BooksCategories
               .Where(x => x.Book.Copies < 4200);

            var books = context.Books
                .Where(x => x.Copies < 4200);

            var count = books.Count();


            context.BooksCategories.RemoveRange(booksCategories);

            context.Books.RemoveRange(books);

            context.SaveChanges();
           
            return count;
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var booksBefore2010 = context.Books
                .Where(x => x.ReleaseDate.HasValue && x.ReleaseDate.Value.Year < 2010)
                .ToList();

            foreach (var b in booksBefore2010)
            {
                b.Price += 5;
            }
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories
                .Select(c => new
                {
                    c.Name,
                    CategoryBooks = c.CategoryBooks.Select(cb => new
                    {
                        BookTitle = cb.Book.Title,
                        ReleaseDate = cb.Book.ReleaseDate
                    })
                   .OrderByDescending(cb => cb.ReleaseDate)
                   .Take(3)
                })
                .OrderBy(c => c.Name)
                .ToList();

            var sb = new StringBuilder();

            foreach (var category in categories)
            {
                sb.AppendLine($"--{category.Name}");
                foreach (var book in category.CategoryBooks)
                {
                    sb.AppendLine($"{book.BookTitle} ({book.ReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories
                .Select(c => new
                {
                    Name = c.Name,
                    TotalProfit = c.CategoryBooks.Sum(bc => bc.Book.Price * bc.Book.Copies)
                })
                .OrderByDescending(c => c.TotalProfit)
                .ThenBy(c => c.Name)
                .ToList();

            var sb = new StringBuilder();

            foreach (var c in categories)
            {
                sb.AppendLine($"{c.Name} ${c.TotalProfit:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authorBooks = context.Authors
                .Select(x => new
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    CountOfBooks = x.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(x => x.CountOfBooks)
                .ToList();

            var sb = new StringBuilder();

            foreach (var a in authorBooks)
            {
                sb.AppendLine($"{a.FirstName} {a.LastName} - {a.CountOfBooks}");
            }

            return sb.ToString().TrimEnd();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var countOfBooks = context.Books
                .Where(x => x.Title.Length > lengthCheck)
                .Select(x => x.Title)
                .ToList();

            return countOfBooks.Count();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(x => x.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(x => new
                {
                    BookId = x.BookId,
                    Title = x.Title,
                    AuthorFirstName = x.Author.FirstName,
                    AuthorLastName = x.Author.LastName
                })
                .OrderBy(x => x.BookId)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} ({book.AuthorFirstName} {book.AuthorLastName})");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(x => x.Title.ToLower().Contains(input.ToLower()))
                .OrderBy(x => x.Title)
                .Select(x => x.Title)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(x => x.FirstName.EndsWith(input))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var a in authors)
            {
                sb.AppendLine($"{a.FirstName} {a.LastName}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var targetDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var books = context.Books
                .Where(x => x.ReleaseDate.HasValue && x.ReleaseDate.Value < targetDate)
                .Select(x => new
                {
                    x.ReleaseDate,
                    x.Title,
                    x.EditionType,
                    x.Price
                })
                .OrderByDescending(x => x.ReleaseDate)
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
                .Select(x => x.Title)
                .OrderBy(title => title)
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
