using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDictionary
{
    internal class WordTranslation
    {
        public string Name { get; set; }
        public WordTranslation(string name) 
        {
            Name = name;
        }
        public WordTranslation(): this (null) { }
        public override string ToString()
        {
            return Name;
        }
    }
}
