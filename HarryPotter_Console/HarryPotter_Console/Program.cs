using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
            string conn = "Server=localhost;Database=harrypotter;UId=root;Password=;";
            MySQL(conn);
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
                sr.ReadLine();
                string line;

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] fields = line.Split(',');

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

                    string datePart = fields[6] + "," + fields[7];
                    if (DateTime.TryParse(datePart.Trim('"'), out DateTime birthdate))
                    {
                        character.Birthdate = birthdate;
                    }

                    if (int.TryParse(fields[8], out int index))
                    {
                        character.Index = index;
                    }

                    character.Children = new List<Child>();
                    if (!string.IsNullOrEmpty(fields[4]))
                    {
                        character.Children.Add(new Child { FullName = fields[4].Trim('"') });
                    }

                    character.KnownSpells = new List<Spell>();
                    if (fields.Length > 9 && !string.IsNullOrEmpty(fields[9]))
                    {
                        string[] spellNames = fields[9].Split(';');
                        foreach (var spellName in spellNames)
                        {
                            character.KnownSpells.Add(new Spell { Name = spellName.Trim() });
                        }
                    }

                    characters.Add(character);
                }
            }

            var sortedCharacters = characters.OrderByDescending(c => c.Birthdate).ToList();
            Console.WriteLine("Rendezve születési dátum szerint csökkenő sorrendben:");
            Console.WriteLine();
            foreach (var item in sortedCharacters)
            {
                Console.WriteLine($"Név: {item.FullName} | Születés: {item.Birthdate:yyyy.MM.dd} | Becenév: {item.Nickname}");

                if (item.KnownSpells.Count > 0)
                {
                    Console.WriteLine($"Varázslatok: {string.Join(", ", item.KnownSpells.Select(s => s.Name))}");
                }

                if (item.Children.Count > 0)
                {
                    Console.WriteLine($"Gyermekek: {string.Join(", ", item.Children.Select(c => c.FullName))}");
                }

                Console.WriteLine();
            }

            var oldest = characters.OrderBy(c => c.Birthdate).First();
            var youngest = characters.OrderByDescending(c => c.Birthdate).First();

            Console.WriteLine($"Legidősebb: {oldest.FullName}");
            Console.WriteLine($"Legatalabb: {youngest.FullName}");
            Console.WriteLine();

            Console.Write("Adj meg egy becenevet: ");
            string input = Console.ReadLine();

            var finder = characters.FirstOrDefault(c => c.Nickname.ToLower().Equals(input.ToLower()));

            if (string.IsNullOrWhiteSpace(input) || finder == null)
            {
                Console.WriteLine("Nincs ilyen karakter.");
            }
            else
            {
                var spells = string.Join(", ", finder.KnownSpells.Select(s => s.Name));

                var children = string.Join(", ", finder.Children.Select(c => c.FullName));

                Console.WriteLine($"Varázslatai: {spells}, Gyermekei: {children}");
            }

            return characters;
        }
        static void MySQL(string connection) 
        {
            string connectionString = connection;
            List<Spell> spells = LoadSpells("spells.csv");
            List<Character> characters = LoadCharacter("characters.csv");

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();


                string createSpellsTable = "CREATE TABLE IF NOT EXISTS Spells (SpellID INT AUTO_INCREMENT PRIMARY KEY, Name VARCHAR(255), `Use` VARCHAR(255))";
                MySqlCommand cmdCreateSpells = new MySqlCommand(createSpellsTable, conn);
                cmdCreateSpells.ExecuteNonQuery();

                string insertSpellQuery = "INSERT INTO Spells (Name, `Use`) VALUES (@Name, @Use)";
                MySqlCommand cmdInsertSpell = new MySqlCommand(insertSpellQuery, conn);
                for (int i = 0; i < spells.Count; i++)
                {
                    cmdInsertSpell.Parameters.Clear();
                    cmdInsertSpell.Parameters.AddWithValue("@Name", spells[i].Name);
                    cmdInsertSpell.Parameters.AddWithValue("@Use", spells[i].Use);
                    cmdInsertSpell.ExecuteNonQuery();
                }



                string createCharactersTable = "CREATE TABLE IF NOT EXISTS Characters (CharacterID INT AUTO_INCREMENT PRIMARY KEY, FullName VARCHAR(255), NickName VARCHAR(255), HogWartsHouse VARCHAR(255), InterPretedBy VARCHAR(255), Image VARCHAR(255), BirthDate DATE)";
                MySqlCommand cmdCreateCharacters = new MySqlCommand(createCharactersTable, conn);
                cmdCreateCharacters.ExecuteNonQuery();

                string insertCharacterQuery = "INSERT INTO Characters (FullName, NickName, HogWartsHouse, InterPretedBy, Image, BirthDate) VALUES (@FullName, @NickName, @HogWartsHouse, @InterPretedBy, @Image, @BirthDate)";
                MySqlCommand cmdInsertCharacter = new MySqlCommand(insertCharacterQuery, conn);
                for (int i = 0; i < characters.Count; i++)
                {
                    cmdInsertCharacter.Parameters.Clear();
                    cmdInsertCharacter.Parameters.AddWithValue("@FullName", characters[i].FullName);
                    cmdInsertCharacter.Parameters.AddWithValue("@NickName", characters[i].Nickname);
                    cmdInsertCharacter.Parameters.AddWithValue("@HogWartsHouse", characters[i].HogwartsHouse);
                    cmdInsertCharacter.Parameters.AddWithValue("@InterPretedBy", characters[i].InterpretedBy);
                    cmdInsertCharacter.Parameters.AddWithValue("@Image", characters[i].Image);
                    cmdInsertCharacter.Parameters.AddWithValue("@BirthDate", characters[i].Birthdate);
                    cmdInsertCharacter.ExecuteNonQuery();
                }



                string createChildrenTable = "CREATE TABLE IF NOT EXISTS Children (ChildID INT AUTO_INCREMENT PRIMARY KEY, CharacterID INT, FOREIGN KEY (CharacterID) REFERENCES Characters(CharacterID), FullName VARCHAR(255))";
                MySqlCommand cmdCreateChildren = new MySqlCommand(createChildrenTable, conn);
                cmdCreateChildren.ExecuteNonQuery();

                string insertChildQuery = "INSERT INTO Children (FullName) VALUES (@FullName)";
                MySqlCommand cmdInsertChild = new MySqlCommand(insertChildQuery, conn);
                for (int i = 0; i < characters.Count; i++)
                {
                    foreach (var child in characters[i].Children)
                    {
                        cmdInsertChild.Parameters.Clear();
                        cmdInsertChild.Parameters.AddWithValue("@FullName", child.FullName);
                        cmdInsertChild.ExecuteNonQuery();
                    }
                }



                string createCharacterSpellsTable = "CREATE TABLE CharacterSpells (CharacterID INT, SpellID INT, PRIMARY KEY(CharacterID, SpellID), FOREIGN KEY(CharacterID) REFERENCES Characters(CharacterID), FOREIGN KEY(SpellID) REFERENCES Spells(SpellID));";
                MySqlCommand cmdCreateCharacterSpells = new MySqlCommand(createCharacterSpellsTable, conn);
                cmdCreateCharacterSpells.ExecuteNonQuery();

            }
        }
    }
}
