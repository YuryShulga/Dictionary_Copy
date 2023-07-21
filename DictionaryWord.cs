using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDictionary
{
    internal class DictionaryWord
    {
        public string OriginalWord { get; set; }
        public List<WordTranslation> Translation { get; set; }
        /// <summary>
        /// используется только для десериализации
        /// </summary>
        public DictionaryWord() { }
        private DictionaryWord(string originalWord, List<WordTranslation> translation) 
        {
            if (originalWord == null || originalWord == "") 
            {
                throw new DictionaryException("OriginalWord равно null или пустое");
            }
            if (translation == null || translation.Count == 0)
            {
                throw new DictionaryException("Translation равно null или пустое");
            }
            OriginalWord = originalWord;
            Translation = translation;
        }
        /// <summary>
        /// Метод возвращает экземпляр DictionaryWord, либо null (если оригинальное слово не указано, 
        /// или не указан перевод)
        /// </summary>
        /// <param name="originalWord">Слово которое будем переводить</param>
        /// <param name="translation">Перевод слова, допускается несколько вариантов перевода</param>
        /// <returns></returns>
        public static DictionaryWord NewWord(string originalWord, List<WordTranslation> translation)
        {
            DictionaryWord temp=null;
            try
            {
                temp = new DictionaryWord(originalWord, translation);
            }
            catch (Exception)
            {
                temp = null;
            }
            return temp;
        }
        public override string ToString()
        {
            return OriginalWord;
        }
    }
}
