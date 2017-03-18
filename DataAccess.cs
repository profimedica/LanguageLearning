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
                words.Add(new Word(line.Split('=')));
            }
        }
    }

    public class Word
    {
        public string Native { get; set; }
        public string Foreign { get; set; }

        public int Difficulty { get; set; }

        public Word(string[] fragments)
        {
            this.Native = fragments[2].Trim();
            this.Foreign = fragments[1].Trim();
        }
    }
}
