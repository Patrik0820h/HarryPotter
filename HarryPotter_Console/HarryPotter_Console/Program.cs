using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        }
        static void LoadSpells(string path) 
        {
            StreamReader sr = new StreamReader(path);
            if (!File.Exists(path)) { Console.WriteLine("A megadott spell file nem található!"); }
            sr.ReadLine();
            string line;
            while (!sr.EndOfStream) 
            {
                line = sr.ReadLine();
                Console.WriteLine(line);
            }
            sr.Close();
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
                Console.WriteLine(line);
            }
            sr.Close();
        }
    }
}
