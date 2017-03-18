using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordLex
{
    class DataAccess
    {
        public List<Word> words = new List<Word>();
        public DataAccess()
        {
            string[] lines = File.ReadAllLines("DEEN.txt");
            foreach (string line in lines)
            {
                words.Add(new Word(line.Split(':')));
            }
        }

        public void Save()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Word word in words)
            {
                sb.AppendLine(word.ToString());
            }
            File.WriteAllText("DEEN.txt", sb.ToString());
        }
    }

    public class Word
    {
        public int Id { get; set; }
        public string Native { get; set; }
        public string Foreign { get; set; }

        public int ForeignDifficulty { get; set; }

        public int NativeDifficulty { get; set; }

        public Word(string[] fragments)
        {
            int givenId = 0;
            int.TryParse(fragments[0].Trim(), out givenId);

            int foreignDifficulty = 0;
            int.TryParse(fragments[5].Trim(), out foreignDifficulty);

            int nativeDifficulty = 0;
            int.TryParse(fragments[8].Trim(), out nativeDifficulty);

            this.Id = givenId;
            this.Native = fragments[2].Trim();
            this.Foreign = fragments[1].Trim();
            this.ForeignDifficulty = foreignDifficulty;
            this.NativeDifficulty = nativeDifficulty;
        }
        public string ToString()
        {
            return Id + " : " + Foreign + " : " + Native + " : " + 0 + " : " + 0 + " : " + ForeignDifficulty + " : " + 0 + " : " + 0 + " : " + NativeDifficulty;
        }
    }
}
