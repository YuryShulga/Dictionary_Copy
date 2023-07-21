using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyDictionary
{
    internal class MyDictionary
    {
        /// <summary>
        /// Название словаря
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// язык с которого производится перевод
        /// </summary>
        public string OriginalLanguage { get; set; }
        /// <summary>
        /// язык на который производится перевод
        /// </summary>
        public string TargetLanguage { get; set; }
        /// <summary>
        /// путь по которому хранится файл словаря, если еще ни разу не сохранялся равен ""
        /// </summary>
        public string DictionaryFilePath { get; set; }
        /// <summary>
        /// массив слов словаря
        /// </summary>
        public List<DictionaryWord> Dictionary { get; set; }
        /// <summary>
        /// Отображает состояние словаря в памяти по отношению с сохраненной версией
        /// true - есть изменения, false - словарь не изменялся
        /// </summary>
        public bool Changed {get; set; }
        /// <summary>
        /// Конструктор словаря
        /// </summary>
        /// <param name="name">Название словаря</param>
        /// <param name="originalLanguage">язык с которого производится перевод</param>
        /// <param name="targetLanguage">язык на который производится перевод</param>        
        public MyDictionary(string name, string originalLanguage, string targetLanguage)
        {
            Name = name;
            OriginalLanguage = originalLanguage;
            TargetLanguage = targetLanguage;
            Dictionary= new List<DictionaryWord>();
            Changed = true;
        }
        /// <summary>
        /// Определяет есть ли в словаре такое слово
        /// </summary>
        /// <param name="word">Слово которое ищем</param>
        /// <returns>true - если слово найдено, false - если не найдено</returns>
        public bool Contains(DictionaryWord word)
        {
            if (word == null)
            {
                return false;
            }
            DictionaryWord findResult = Dictionary.Find((DictionaryWord tWord) => tWord.OriginalWord==word.OriginalWord);
            if (findResult == null)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// добавляет слово + перевод в словарь 
        /// </summary>
        /// <param name="originalWord">слово в оригинальном виде</param>
        /// <param name="wordTranslation">список вариантов перевода для слова</param>
        /// <returns>true - слово добавлено в словарь, false -слово не добавлено, оно уже есть в словаре</returns>
        public bool AddWord(DictionaryWord word)
        {
            if (!this.Contains(word)) {
                Dictionary.Add(word);
                Changed = true;
                return true;
            }   
            return false;
        }
        /// <summary>
        /// Замена слова(и его перевод) в словаре на другое слово;
        /// </summary>
        /// <param name="oldWord">слово в словаре</param>
        /// <param name="newWord">слово на которое меняем</param>
        /// <returns>0 - замена успено прошла,
        /// 1 - слова с таким индексом в словаре нет,
        /// 2 - слово на которое меняем == null,
        /// 3 - слово на которое меняем уже есть в словаре
        /// </returns>
        public int EditWord(int oldWordIndex, DictionaryWord newWord) 
        {
            
            //если новое слово == Null
            if (newWord == null)
            {
                return 2;
            }
            //проверка есть ли заменяемое слово в словаре
            if (oldWordIndex < 0 || oldWordIndex > Dictionary.Count-1)
            {
                return 1;
            }
            //проверяем, не образуется ли при замене еще одно такое же OriginalWord
            if (Dictionary[oldWordIndex].OriginalWord != newWord.OriginalWord)
            {
                if (this.Contains(newWord))
                {
                    return 3;
                }
            }
            //меняем слово
            Dictionary[oldWordIndex]= newWord;
            Changed = true;
            return 0; 
        }

        /// <summary>
        /// Поиск слова в словаре 
        /// </summary>
        /// <param name="word">искомое слово</param>
        /// <returns>Индекс слова в словаре либо -1 если слово не найдено</returns>
        public int FindWord(string word)
        {
            for (int i = 0; i < Dictionary.Count; i++)
            {
                if (word == Dictionary[i].OriginalWord)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Удаление слова
        /// </summary>
        /// <param name="word"></param>
        /// <returns>true - удалил false - не удалил(нет слова или слово null)</returns>
        public bool RemoveWord(DictionaryWord word) 
        {
            if (!this.Contains(word) || word==null)
            {
                return false;
            }
            for (int i = 0; i < Dictionary.Count; i++)
            {
                if (Dictionary[i].OriginalWord == word.OriginalWord)
                {
                    Dictionary.RemoveAt(i);
                    Changed = true;
                    break;
                }
            }
            return true;
        }
        /// <summary>
        /// Удаление слова
        /// </summary>
        /// <param name="wordIndex">индекс удаляемого элемента</param>
        /// <returns>true - удалил false - не удалил(нет слова или слово null)</returns>
        public bool RemoveWord(int wordIndex)
        {
            if (Dictionary.Count < wordIndex)
            {
                return false;
            }
            Dictionary.RemoveAt(wordIndex);
            return true;
        }

        /// <summary>
        /// Метод ищет слово(string) в словаре и возвращает его перевод(через out параметр translation)
        /// </summary>
        /// <param name="originalWord"></param>
        /// <param name="translation"></param>
        /// <returns>true - слово найдено (через translations возврат  - варианты перевода),
        /// false - слово не найдено(через translations возврат - пустой список)</returns>
        public bool FindTranslationOriginalWord(string originalWord, out List<WordTranslation> translation)
        {
            translation = new List<WordTranslation>();
            foreach (DictionaryWord word in Dictionary) 
            {
                if(word.OriginalWord == originalWord) 
                {
                    foreach (WordTranslation item in word.Translation)
                    {
                        translation.Add(item); 
                    }
                    return true;
                }
            }
            return false; 
        }

        /// <summary>
        /// сохранение словаря в файл DictionaryFilePath
        /// </summary>
        /// <returns>true - словарь успешно сохранен, false - отказано в доступе к файлу</returns>  
        public bool SaveToFile()
        {
            if (DictionaryFilePath == "")
            {
                return false;
            }
           try
            {
                using (FileStream fs = new FileStream(DictionaryFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    string stringDictionary;
                    stringDictionary = JsonSerializer.Serialize(this);
                    byte[] byteArrayDictionary = Encoding.Default.GetBytes(stringDictionary);
                    fs.Write(byteArrayDictionary, 0, byteArrayDictionary.Length);
                    Changed = false;
                    return true;
                }
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
        /// <summary>
        /// сохранение словаря в файл
        /// </summary>
        /// <param name="path">путь+имя файла по которому сохраняется словарь</param>
        /// <returns>true - словарь успешно сохранен, false - отказано в доступе к файлу</returns>  
        public bool SaveAsToFile(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    string stringDictionary;
                    stringDictionary = JsonSerializer.Serialize(this);
                    byte[] byteArrayDictionary = Encoding.Default.GetBytes(stringDictionary);
                    fs.Write(byteArrayDictionary, 0, byteArrayDictionary.Length);
                    Changed = false;
                    DictionaryFilePath = path;
                    return true;
                }
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// загрузка словаря из файла
        /// </summary>
        /// <param name="path">путь+имя файла по которому загружается словарь</param>
        /// <returns>true - словарь успешно загружен, false - файл не найден/неправильный формат файла</returns>
        public bool LoadFromFile(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    byte[] byteArrayDictionary = new byte[(int)fs.Length];
                    fs.Read(byteArrayDictionary, 0, byteArrayDictionary.Length);
                    string stringDictionary = Encoding.Default.GetString(byteArrayDictionary).ToString();
                    MyDictionary temp = JsonSerializer.Deserialize<MyDictionary>(stringDictionary);
                    this.Name = temp.Name;
                    this.OriginalLanguage = temp.OriginalLanguage;
                    this.TargetLanguage = temp.TargetLanguage;
                    this.Dictionary = temp.Dictionary;
                    DictionaryFilePath = path;
                    Changed = false;
                    return true;
                }
            }
            catch (JsonException)
            {
                return false;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }
        /// <summary>
        /// сохранение слова в файл
        /// </summary>
        /// <param name="word">слово из словаря которое нужно сохранить</param>
        /// <param name="path">путь к файлу в который будем сохранять слово</param>
        /// <returns>true - сохранилось, false - ну получается сохранить(нет доступа к файлу)</returns>
        public bool ExportWordToFile(DictionaryWord word, string path)
        {
            try
            {
                using (StreamWriter fs = new StreamWriter(path))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append($"Слово: \"{word.OriginalWord}\"\n");
                    stringBuilder.Append("Варианты перевода:\n");
                    foreach (WordTranslation item in word.Translation)
                    {
                        stringBuilder.Append($"{item}\n");
                    }
                    fs.WriteLine(stringBuilder.ToString());
                    return true;
                }
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch 
            {
                return false;
            }
        }
    }
}
