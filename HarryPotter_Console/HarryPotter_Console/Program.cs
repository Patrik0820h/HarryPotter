using MySql.Data.MySqlClient;
using Mysqlx.Datatypes;
using Org.BouncyCastle.Pqc.Crypto.Frodo;
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
            //List<Spell> spells = LoadSpells("spells.csv");
            LoadCharacter("characters.csv");

            //string conn = "Server=localhost;Database=harrypotter;UId=root;Password=;";
            //MySQL(conn);
        }
        static List<Spell> LoadSpells(string path) 
        {
            List<Spell> spells = new List<Spell>();

            if (!File.Exists(path)) 
            { 
                Console.WriteLine("A megadott spell file nem található!"); 
                return spells;
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
            return spells;
        }
        static List<Character> LoadCharacter(string path) 
        {
            List<Character> characters = new List<Character>();

            if (!File.Exists(path))
            {
                Console.WriteLine("A megadott character file nem található!");
                return characters;
            }

            using (StreamReader sr = new StreamReader(path))
            {
                sr.ReadLine(); // header átlépése
                string line;

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] fields = line.Split(new char[] { ',' }, System.StringSplitOptions.None);

                    if (fields.Length < 9)
                    {
                        Console.WriteLine($"Hibás sor: {line}");
                        continue;
                    }

                    var character = new Character();
                    character.FullName = fields[0].Trim('"');
                    character.Nickname = fields[1].Trim('"');
                    character.HogwartsHouse = fields[2].Trim('"');
                    character.InterpretedBy = fields[3].Trim('"');
                    character.Image = fields[5].Trim('"');
                    
                    if (DateTime.TryParse(fields[6], out DateTime birthdate))
                    {
                        character.Birthdate = birthdate;
                    }
                    
                    if (int.TryParse(fields[7], out int index))
                    {
                        character.Index = index;
                    }

                    // Gyerekek feldolgozása
                    character.Children = new List<Child>();
                    if (!string.IsNullOrEmpty(fields[4]))
                    {
                        string[] childrenNames = fields[4].Split(';');
                        foreach (var childName in childrenNames)
                        {
                            character.Children.Add(new Child { FullName = childName.Trim() });
                        }
                    }

                    // Varázslatok feldolgozása
                    character.KnownSpells = new List<Spell>();
                    if (!string.IsNullOrEmpty(fields[8]))
                    {
                        string[] spellNames = fields[8].Split(';');
                        foreach (var spellName in spellNames)
                        {
                            character.KnownSpells.Add(new Spell { Name = spellName.Trim() });
                        }
                    }

                    characters.Add(character);
                }
            }

            foreach (var item in characters)
            {
                Console.WriteLine($"Név: {item.FullName} | Becenév: {item.Nickname} | Mágusház: {item.HogwartsHouse} | Megformálta: {item.InterpretedBy} | Kép: {item.Image} | Születésnap: {item.Birthdate}");
                
                if (item.Children.Count > 0)
                {
                    Console.WriteLine($"Gyerek neve: {string.Join(", ", item.Children.Select(c => c.FullName))}");
                }
                
                if (item.KnownSpells.Count > 0)
                {
                    Console.WriteLine($"Ismert varázslatok neve: {string.Join(", ", item.KnownSpells.Select(s => s.Name))}");
                }
            }

            return characters;
        }
        static void MySQL(string connection) 
        {
            string connectionString = connection;
            List<Spell> spells = LoadSpells("spells.csv");

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                //----------------------------------------------------------------
                string createSpellsTable = "CREATE TABLE IF NOT EXISTS Spells";
                MySqlCommand cmdCreateSpells = new MySqlCommand(createSpellsTable, conn);
                cmdCreateSpells.ExecuteNonQuery();

                string insertSpellQuery = "INSERT INTO Spells (Name, Use, Index) VALUES (@Name, @Use, @Index)";
                MySqlCommand cmdInsertSpell = new MySqlCommand(insertSpellQuery, conn);
                for (int i = 0; i < spells.Count; i++)
                {
                    cmdInsertSpell.Parameters.AddWithValue("@Name", spells[i].Name);
                    cmdInsertSpell.Parameters.AddWithValue("@Use", spells[i].Use);
                    cmdInsertSpell.Parameters.AddWithValue("@Index", spells[i].Index);
                }
                cmdInsertSpell.ExecuteNonQuery();
                //----------------------------------------------------------------


                //string createCharactersTable = "CREATE TABLE IF NOT EXISTS Employees ()";
                //string createChildrenTable = "CREATE TABLE IF NOT EXISTS Employees ()";
                //string createCharacterSpellsTable = "CREATE TABLE IF NOT EXISTS Employees ()";




                string query = "INSERT INTO Employees (FirstName, LastName, Position, Salary) VALUES (@FirstName, @LastName, @Position, @Salary)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                // Add parameters to prevent SQL injection
                //cmd.Parameters.AddWithValue("@FirstName", firstName);
                //cmd.Parameters.AddWithValue("@LastName", lastName);
                //cmd.Parameters.AddWithValue("@Position", position);
                //cmd.Parameters.AddWithValue("@Salary", salary);
                cmd.ExecuteNonQuery(); // Execute the insert command
            }
        }
    }
}
