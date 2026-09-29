using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tokenizer
{
    public enum Language { English, Russian }

    public static class StopWords
    {
        public static HashSet<string> Load(Language language)
        {
            string file = language == Language.English ? "stopwords_en.txt" : "stopwords_ru.txt";
            return new HashSet<string>(
                File.ReadAllLines(file).Select(w => w.Trim().ToLower()).Where(w => w.Length > 0));
        }
    }
}
