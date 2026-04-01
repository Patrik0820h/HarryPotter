using MySql.Data.MySqlClient;
using Mysqlx.Datatypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HarryPotter_Console
{
    internal class Program
    {
        class Spell
        {
            public int Index { get; set; }
            public string Name { get; set; }
            public string Use { get; set; }
        }

        class Child
        {
            public string FullName { get; set; }
        }

        class Character
        {
            public int Index { get; set; }
            public string FullName { get; set; }
            public string Nickname { get; set; }
            public string HogwartsHouse { get; set; }
            public string InterpretedBy { get; set; }
            public string Image { get; set; }
            public DateTime Birthdate { get; set; }

            public List<Spell> KnownSpells { get; set; }
            public List<Child> Children { get; set; }
        }
        static void Main(string[] args)
        {
            LoadSpells("spells.csv");
            LoadCharacter("characters.csv");

            string conn = "Server=localhost;Database=harrypotter;UId=root;Password=;";
            MySQL(conn);
        }
        static void LoadSpells(string path) 
        {
            List<Spell> spells = new List<Spell>();

            if (!File.Exists(path)) 
            { 
                Console.WriteLine("A megadott spell file nem található!"); 
                return;
            }

            var regex = new Regex(@"^([^,]+),(""[^""]*""|[^,]+),(\d+)$");

            using (StreamReader sr = new StreamReader(path))
            {
                sr.ReadLine();

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var match = regex.Match(line);

                    if (!match.Success)
                    {
                        Console.WriteLine($"Hibás sor: {line}");
                        continue;
                    }

                    var spell = new Spell();
                    spell.Name = match.Groups[1].Value;
                    spell.Use = match.Groups[2].Value.Trim('"');
                    spell.Index = int.Parse(match.Groups[3].Value);

                    spells.Add(spell);
                }
            }

            foreach (var item in spells)
            {
                Console.WriteLine($"Név: {item.Name} | Használat: {item.Use} | Index: {item.Index}");
            }
        }
        static void LoadCharacter(string path) 
        {
            StreamReader sr = new StreamReader(path);
            if (!File.Exists(path)) { Console.WriteLine("A megadott character file nem található!"); }
            sr.ReadLine();
            string line;
            while (!sr.EndOfStream) 
            {
                line = sr.ReadLine();
            }
            sr.Close();
        }
        static void MySQL(string connection) 
        {
            string connectionString = connection;

            using (var conn = new MySqlConnection(connectionString))
            {
                string createSpellsTable = "CREATE TABLE IF NOT EXISTS Spells";
                string createCharactersTable = "CREATE TABLE IF NOT EXISTS Employees ()";
                string createChildrenTable = "CREATE TABLE IF NOT EXISTS Employees ()";
                string createCharacterSpellsTable = "CREATE TABLE IF NOT EXISTS Employees ()";




                string query = "INSERT INTO Employees (FirstName, LastName, Position, Salary) VALUES (@FirstName, @LastName, @Position, @Salary)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                // Add parameters to prevent SQL injection
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@Position", position);
                cmd.Parameters.AddWithValue("@Salary", salary);
                connection.Open();
                cmd.ExecuteNonQuery(); // Execute the insert command
            }
        }
    }
}
