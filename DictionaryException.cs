using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDictionary
{
    internal class DictionaryException: Exception
    {
        public DictionaryException(string message):base(message) { }
    }
}
