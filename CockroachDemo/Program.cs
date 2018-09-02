using Npgsql;
using System;
using System.Text;

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
                {
                    CreatePerson(conn);
                    Console.WriteLine("{0:HH:mm:ss}: Person Inserted!", DateTime.Now);
                }

                //// Retrieve all rows
                //using (var cmd = new NpgsqlCommand("SELECT some_field FROM data", conn))
                //using (var reader = cmd.ExecuteReader())
                //    while (reader.Read())
                //        Console.WriteLine(reader.GetString(0));
            }
        }

        private static void CreatePerson(NpgsqlConnection conn)
        {
            var sql = new StringBuilder(1024);

            var id = Guid.NewGuid();

            sql.AppendLine($"UPSERT INTO Person (Id, Name) VALUES ('{id}', 'name-{id}');");

            sql.Append("UPSERT INTO Address (Id, PersonId, Name, Street, Number) VALUES ");

            for (var i = 0; i < 100; ++i)
            {
                CreateAddress(sql, id);
            }

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = sql.ToString().TrimEnd(',') + ";";
                cmd.ExecuteNonQuery();
            }
        }

        private static void CreateAddress(StringBuilder sql, Guid personId)
        {
            var id = Guid.NewGuid();

            sql.Append($"('{id}', '{personId}', 'Name-{id}', 'Street-{id}', 'Number-{id}'),");
        }
    }
}
