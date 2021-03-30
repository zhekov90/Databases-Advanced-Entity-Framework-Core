namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
    {
        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var games = JsonConvert.DeserializeObject<GameJsonImportModel[]>(jsonString);

            foreach (var jsonGame in games)
            {
                if (!IsValid(jsonGame) || !jsonGame.Tags.Any())
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                //either finds the genre or creates it
                var genre = context.Genres.FirstOrDefault(x => x.Name == jsonGame.Genre) ?? new Genre { Name = jsonGame.Genre };

                var developer = context.Developers.FirstOrDefault(x => x.Name == jsonGame.Developer) ?? new Developer { Name = jsonGame.Developer };

                
                var game = new Game
                {
                    Name = jsonGame.Name,
                    Price = jsonGame.Price,
                    ReleaseDate = jsonGame.ReleaseDate.Value,
                    Developer = developer, 
                    Genre = genre,
                };

                foreach (var jsonTag in jsonGame.Tags)
                {
                    var tag = context.Tags.FirstOrDefault(x => x.Name == jsonTag) ?? new Tag { Name = jsonTag };

                    game.GameTags.Add(new GameTag { Tag = tag });
                }

                context.Games.Add(game);

                context.SaveChanges();

                sb.AppendLine($"Added {jsonGame.Name} ({jsonGame.Genre}) with {jsonGame.Tags.Count()} tags");
            }

           

            return sb.ToString().TrimEnd();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var users = JsonConvert.DeserializeObject<UserJsonImportModel[]>(jsonString);

            foreach (var jsonUser in users)
            {
                if (!IsValid(jsonUser))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var user = new User
                {
                    FullName = jsonUser.FullName,
                    Username = jsonUser.Username,
                    Email = jsonUser.Email,
                    Age = jsonUser.Age,
                    Cards = jsonUser.Cards.Select(c => new Card
                    {
                        Number = c.Number,
                        Cvc = c.CVC,
                        Type = c.Type.Value
                    }).ToList()

                };

                context.Users.Add(user);

                sb.AppendLine($"Imported {jsonUser.Username} with {jsonUser.Cards.Count()} cards");
                context.SaveChanges();
            }

            return sb.ToString().TrimEnd();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            return "TODO";
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
