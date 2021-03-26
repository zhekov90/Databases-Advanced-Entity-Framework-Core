namespace VaporStore.DataProcessor
{
    using System;
    using System.Linq;
    using Data;

    public static class Bonus
    {
        public static string UpdateEmail(VaporStoreDbContext context, string username, string newEmail)
        {
            var user = context
                .Users
                .FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return $"User {username} not found";
            }

            var userWithThatEmail = context
                .Users
                .FirstOrDefault(u => u.Email == newEmail);

            if (userWithThatEmail != null)
            {
                return $"Email {newEmail} is already taken";
            }

            user.Email = newEmail;
            context.SaveChanges();

            return $"Changed {username}'s email successfully";
        }
    }
}
