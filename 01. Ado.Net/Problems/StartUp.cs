
namespace AdoNetExercise
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Data.SqlClient;
    public class StartUp
    {
        //when creating db use Database=master to create it, after that change it to MinionsDB to create and insert into tables
        const string SqlConnectionString = "Server=.;Database=MinionsDB;Integrated Security=true";

        public static void Main(string[] args)
        {
            using var connection = new SqlConnection(SqlConnectionString);

            connection.Open();

            // InitialSetup(connection);

            // GetVillainNames(connection);

            // AddMinion(connection);

            // ChangeTownNamesCasing(connection);

            // RemoveVillain(connection);

            // PrintAllMinionsNames(connection);


        }

        private static void PrintAllMinionsNames(SqlConnection connection)
        {
            var minionsQuery = @"SELECT Name FROM Minions";
           using var selectCommand = new SqlCommand(minionsQuery, connection);
           using var reader = selectCommand.ExecuteReader();
            var minions = new List<string>();
            while (reader.Read())
            {
                minions.Add((string)reader[0]);
            }

            //prints the minions

            int counter = 0;

            for (int i = 0; i < minions.Count / 2; i++)
            {
                Console.WriteLine(minions[0 + counter]);
                Console.WriteLine(minions[minions.Count - 1 - counter]);
                counter++;
            }

            if (minions.Count % 2 == 1)
            {
                Console.WriteLine(minions[minions.Count / 2]);
            }
        }

        private static void RemoveVillain(SqlConnection connection)
        {
            int villainId = int.Parse(Console.ReadLine());

            string villainToRemoveQuery = @"SELECT Name FROM Villains WHERE Id = @villainId";
            var sqlCommand = new SqlCommand(villainToRemoveQuery, connection);
            sqlCommand.Parameters.AddWithValue("@villainId", villainId);
            var name = (string)sqlCommand.ExecuteScalar();

            if (name == null)
            {
                Console.WriteLine("No such villain was found.");
                return;
            }

            var deleteMinionsVillainsQuery = @"DELETE FROM MinionsVillains 
      WHERE VillainId = @villainId";

            using var sqlDeleteMinVilCommand = new SqlCommand(deleteMinionsVillainsQuery, connection);
            sqlDeleteMinVilCommand.Parameters.AddWithValue("@villainId", villainId);
            var affectedRows = sqlDeleteMinVilCommand.ExecuteNonQuery();


            var deleteVillainsQuery = @"DELETE FROM Villains
      WHERE Id = @villainId";
            using var sqlDeleteVillainCommand = new SqlCommand(deleteVillainsQuery, connection);
            sqlDeleteVillainCommand.Parameters.AddWithValue("@villainId", villainId);
            sqlDeleteVillainCommand.ExecuteNonQuery();


            Console.WriteLine($"{name} was deleted.");
            Console.WriteLine($"{affectedRows} minions were released.");
        }

        private static void ChangeTownNamesCasing(SqlConnection connection)
        {
            string countryName = Console.ReadLine();

            string updateTownNamesQuery = @"UPDATE Towns
   SET Name = UPPER(Name)
 WHERE CountryCode = (SELECT c.Id FROM Countries AS c WHERE c.Name = @countryName)";

            string selectTownNamesQuery = @"SELECT t.Name 
   FROM Towns as t
   JOIN Countries AS c ON c.Id = t.CountryCode
  WHERE c.Name = @countryName";
            var updateCommand = new SqlCommand(updateTownNamesQuery, connection);
            updateCommand.Parameters.AddWithValue("@countryName", countryName);
            var affectedRows = updateCommand.ExecuteNonQuery();

            if (affectedRows == 0)
            {
                Console.WriteLine("No town names were affected.");
            }
            else
            {
                Console.WriteLine($"{affectedRows} town names were affected.");

                using var selectCommand = new SqlCommand(selectTownNamesQuery, connection);
                selectCommand.Parameters.AddWithValue("@countryName", countryName);

                using var reader = selectCommand.ExecuteReader();

                var towns = new List<string>();
                while (reader.Read())
                {
                    towns.Add((string)reader[0]);
                }
                Console.WriteLine($"[{string.Join(", ", towns)}]");
            }
        }

        private static void AddMinion(SqlConnection connection)
        {
            string[] minionInfo = Console.ReadLine().Split(" ");
            string[] villainInfo = Console.ReadLine().Split(" ");
            string minionName = minionInfo[1];
            int age = int.Parse(minionInfo[2]);
            string town = minionInfo[3];

            int? townId = GetTownId(connection, town);

            if (townId == null)
            {
                string createTownQuery = "INSERT INTO Towns(Id, Name) VALUES (10, @name)";

                using var createTownCommand = new SqlCommand(createTownQuery, connection);
                createTownCommand.Parameters.AddWithValue("@name", town);
                createTownCommand.ExecuteNonQuery();
                townId = GetTownId(connection, town);

                Console.WriteLine($"Town {town} was added to the database.");
            }


            string villainName = villainInfo[1];

            int? villainId = GetVillainId(connection, town);

            if (villainId == null)
            {
                string createVillain = "INSERT INTO Villains (Name, EvilnessFactorId)  VALUES (@villainName, 4)";
                using var createMinionCommand = new SqlCommand(createVillain, connection);
                createMinionCommand.Parameters.AddWithValue("@villainName", villainName);
                createMinionCommand.ExecuteNonQuery();
                villainId = GetVillainId(connection, town);
                Console.WriteLine($"Villain {villainName} was added to the database.");
            }

            CreateMinion(connection, minionName, age, townId);

            var minionId = GetMinionId(connection, minionName);

            InsertMinionVillain(connection, villainId, minionId);

            Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
        }

        private static void InsertMinionVillain(SqlConnection connection, int? villainId, int? minionId)
        {
            var insertIntoMinionVillain = "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@villainId, @minionId)";
            var minVilCommand = new SqlCommand(insertIntoMinionVillain, connection);
            minVilCommand.Parameters.AddWithValue("@villainId", villainId);
            minVilCommand.Parameters.AddWithValue("@minionId", minionId);
            minVilCommand.ExecuteNonQuery();
        }

        private static int? GetMinionId(SqlConnection connection, string minionName)
        {
            var minionIdQuery = "SELECT Id FROM Minions WHERE Name = @Name";
            var minionIdCommand = new SqlCommand(minionIdQuery, connection);
            minionIdCommand.Parameters.AddWithValue("@Name", minionName);
            var minionId = minionIdCommand.ExecuteScalar();
            return (int?)minionId;
        }

        private static void CreateMinion(SqlConnection connection, string minionName, int age, int? townId)
        {
            string createMinion = "INSERT INTO Minions (Name, Age, TownId) VALUES (@name, @age, @townId)";
            var sqlCommand = new SqlCommand(createMinion, connection);
            sqlCommand.Parameters.AddWithValue("@name", minionName);
            sqlCommand.Parameters.AddWithValue("@age", age);
            sqlCommand.Parameters.AddWithValue("@townId", townId);
            sqlCommand.ExecuteNonQuery();
        }

        private static int? GetVillainId(SqlConnection connection, string villainName)
        {
            string query = "SELECT Id FROM Villains WHERE Name = @Name";
            using var sqlCommand = new SqlCommand(query, connection);
            sqlCommand.Parameters.AddWithValue("@Name", villainName);
            var villainId = sqlCommand.ExecuteScalar();
            return (int?)villainId;
        }

        private static int? GetTownId(SqlConnection connection, string town)
        {
            string townIdQuery = "SELECT Id FROM Towns WHERE Name = @townName";
            using var sqlCommand = new SqlCommand(townIdQuery, connection);
            sqlCommand.Parameters.AddWithValue("@townName", town);
            var townId = sqlCommand.ExecuteScalar();
            return (int?)townId;
        }

        private static void GetVillainNames(SqlConnection connection)
        {
            string query = @"SELECT Name, COUNT(mv.MinionId) FROM Villains AS v
JOIN MinionsVillains AS mv ON mv.VillainId = v.Id
GROUP BY v.Id,v.Name";

            using (var command = new SqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader[0];
                        var count = reader[1];
                        Console.WriteLine($"{name} - {count}");
                    }
                }
            }
        }

        private static void InitialSetup(SqlConnection connection)
        {
            //create db

            string createDatabase = "CREATE DATABASE MinionsDB";

            ExecuteNonQuery(connection, createDatabase);

            //create tables

            var createTableStatements = GetCreateTableStatements();

            foreach (var query in createTableStatements)
            {
                ExecuteNonQuery(connection, query);
            }


            //insert into tables

            var insertStatements = GetInsertDataStatements();

            foreach (var query in insertStatements)
            {
                ExecuteNonQuery(connection, query);
            }
        }

        private static void ExecuteNonQuery(SqlConnection connection, string query)
        {
            using var command = new SqlCommand(query, connection);
            command.ExecuteNonQuery();
        }

        private static string[] GetInsertDataStatements()
        {
            var result = new string[]
            {
                "INSERT INTO Countries(Id, Name) VALUES (1, 'Bulgaria'), (2, 'Norway'), (3, 'Cyprus'), (4, 'Greece'), (5, 'UK')",
                "INSERT INTO Towns(Id, Name, CountryCode) VALUES (1, 'Plovdiv', 1), (2, 'Oslo', 2), (3, 'Larnaka', 3), (4, 'Athens', 4), (5, 'London', 5)",
                "INSERT INTO Minions(Id, Name, Age, TownId) VALUES (1, 'Stoyan', 12, 1), (2, 'George', 22, 2), (3, 'Ivan', 25, 3), (4, 'Kiro', 35, 4), (5, 'Niki', 25, 5)",
                "INSERT INTO EvilnessFactors(Id, Name) VALUES (1, 'super good'), (2, 'good'), (3, 'bad'), (4, 'evil'),(5, 'super evil')",
                "INSERT INTO Villains(Id, Name, EvilnessFactorId) VALUES (1, 'Gru', 1), (2, 'Ivo', 2), (3, 'Teo', 3), (4, 'Sto', 4), (5, 'Pro', 5)",
                "INSERT INTO MinionsVillains VALUES (1,1), (2,2), (3,3), (4,4), (5,5)"
            };

            return result;
        }

        private static string[] GetCreateTableStatements()
        {
            var result = new string[]
            {
                "CREATE TABLE Countries(Id INT PRIMARY KEY, Name VARCHAR(50))",
                "CREATE TABLE Towns(Id INT PRIMARY KEY, Name VARCHAR(50), CountryCode INT REFERENCES Countries(Id))",
                "CREATE TABLE Minions(Id INT PRIMARY KEY, Name VARCHAR(50), Age INT, TownId INT REFERENCES Towns(Id))",
                "CREATE TABLE EvilnessFactors(Id INT PRIMARY KEY, Name VARCHAR(50))",
                "CREATE TABLE Villains(Id INT PRIMARY KEY, Name VARCHAR(50), EvilnessFactorId INT REFERENCES EvilnessFactors(Id))",
"CREATE TABLE MinionsVillains(MinionId INT REFERENCES Minions(Id), VillainId INT REFERENCES Villains(Id), PRIMARY KEY(MinionId, VillainId))"
            };

            return result;
        }
    }
}
