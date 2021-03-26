namespace VaporStore.DataProcessor
{
    using Data;
    using Data.Models;
    using DTOs.Import;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Microsoft.EntityFrameworkCore;

    public static class Deserializer
    {
        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            GameDTO[] deserializedGameDtos = JsonConvert.DeserializeObject<GameDTO[]>(jsonString);

            List<Developer> developers = new List<Developer>();
            List<Genre> genres = new List<Genre>();
            List<Tag> tags = new List<Tag>();
            List<Game> games = new List<Game>();

            foreach (var gameDto in deserializedGameDtos)
            {
                if (!IsValid(gameDto) || !gameDto.Tags.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                Developer developer = developers.FirstOrDefault(d => d.Name == gameDto.Developer);
                if (developer == null)
                {
                    developer = new Developer()
                    {
                        Name = gameDto.Developer
                    };

                    developers.Add(developer);
                }

                Genre genre = genres.FirstOrDefault(g => g.Name == gameDto.Genre);
                if (genre == null)
                {
                    genre = new Genre()
                    {
                        Name = gameDto.Genre
                    };

                    genres.Add(genre);
                }

                List<Tag> currentGameTags = new List<Tag>();
                foreach (var tagName in gameDto.Tags)
                {
                    Tag tag = tags.FirstOrDefault(t => t.Name == tagName);
                    if (tag == null)
                    {
                        tag = new Tag()
                        {
                            Name = tagName
                        };

                        tags.Add(tag);
                    }

                    currentGameTags.Add(tag);
                }

                Game game = new Game()
                {
                    Name = gameDto.Name,
                    Price = gameDto.Price,
                    Developer = developer,
                    Genre = genre,
                    ReleaseDate = gameDto.ReleaseDate,
                    GameTags = currentGameTags.Select(gt => new GameTag()
                    {
                        Tag = gt
                    })
                        .ToArray()
                };

                games.Add(game);

                sb.AppendLine($"Added {gameDto.Name} ({gameDto.Genre}) with {gameDto.Tags.Length} tags");
            }

            context.Games.AddRange(games);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            UserDTO[] deserializedUserDtos = JsonConvert.DeserializeObject<UserDTO[]>(jsonString);

            List<User> users = new List<User>();

            foreach (var dto in deserializedUserDtos)
            {
                if (!IsValid(dto) || !dto.Cards.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                Card[] currentCards = dto
                    .Cards
                    .Select(c => new Card()
                    {
                        Number = c.Number,
                        Cvc = c.Cvc,
                        Type = c.Type
                    })
                    .ToArray();

                User currentUser = new User()
                {
                    FullName = dto.FullName,
                    Age = dto.Age,
                    Email = dto.Email,
                    Username = dto.Username,
                    Cards = currentCards
                };

                users.Add(currentUser);

                sb.AppendLine($"Imported {currentUser.Username} with {currentUser.Cards.Count} cards");
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlSerializer serializer = new XmlSerializer(typeof(PurchaseDTO[]), new XmlRootAttribute("Purchases"));

            PurchaseDTO[] deserializedPurchaseDtos = (PurchaseDTO[])serializer.Deserialize(new StringReader(xmlString));

            List<Purchase> purchases = new List<Purchase>();

            foreach (var dto in deserializedPurchaseDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                Game game = context.Games.FirstOrDefault(g => g.Name == dto.Title);
                Card card = context.Cards.Include(x => x.User).FirstOrDefault(c => c.Number == dto.Card);
                DateTime date = DateTime.ParseExact(dto.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                Purchase purchase = new Purchase()
                {
                    Game = game,
                    Card = card,
                    Date = date,
                    Type = dto.Type,
                    ProductKey = dto.Key
                };

                purchases.Add(purchase);
                sb.AppendLine($"Imported {purchase.Game.Name} for {purchase.Card.User.Username}");
            }

            context.Purchases.AddRange(purchases);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);

            return isValid;
        }
    }
}