using Npgsql;
using System;

namespace CockroachDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //var connString = "Host=localhost:26257,localhost:26258,localhost:26259,localhost:26257,localhost:26260,localhost:26261;Database=test";
            var connString = "Host=localhost;Port=26257;Database=test;User ID=root";

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                while(true)
                    CreatePerson(conn);

                //// Retrieve all rows
                //using (var cmd = new NpgsqlCommand("SELECT some_field FROM data", conn))
                //using (var reader = cmd.ExecuteReader())
                //    while (reader.Read())
                //        Console.WriteLine(reader.GetString(0));
            }
        }

        private static void CreatePerson(NpgsqlConnection conn)
        {
            var id = Guid.NewGuid();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO Person (Id, Name) VALUES (@id, @name)";
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("name", "name-" + id);
                cmd.ExecuteNonQuery();
            }

            for (var i = 0; i < 20; ++i)
                CreateAddress(conn, id);
        }

        private static void CreateAddress(NpgsqlConnection conn, Guid personId)
        {
            using (var cmd = new NpgsqlCommand())
            {
                var id = Guid.NewGuid();

                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO Address (Id, PersonId, Name, Street, Number) VALUES (@Id, @PersonId, @Name, @Street, @Number)";
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("PersonId", personId);
                cmd.Parameters.AddWithValue("Name", "Name-" + id);
                cmd.Parameters.AddWithValue("Street", "Street-" + id);
                cmd.Parameters.AddWithValue("Number", "Number-" + id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
