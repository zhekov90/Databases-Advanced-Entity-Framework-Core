
namespace AdoNetExercise
{
    using System;
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
