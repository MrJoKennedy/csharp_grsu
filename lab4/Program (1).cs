using System;
using System.IO;
using System.Text;

namespace Tokenizer
{
    class Program
    {
        static Text text;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.Write("Путь к файлу с текстом: ");
            string path = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(path)) path = "sample.txt";

            text = new TextParser().ParseFile(path);
            Console.WriteLine($"Разобрано предложений: {text.Sentences.Count}");

            bool running = true;
            while (running)
            {
                PrintMenu();
                switch (Console.ReadLine())
                {
                    case "1": SortByWordCount(); break;
                    case "2": SortByLength(); break;
                    case "3": FindWordsInQuestions(); break;
                    case "4": RemoveWordsStartingWithConsonant(); break;
                    case "5": ReplaceWords(); break;
                    case "6": RemoveStopWords(); break;
                    case "7": ExportToXml(); break;
                    case "8": BuildConcordance(); break;
                    case "0": running = false; break;
                    default: Console.WriteLine("Нет такого пункта."); break;
                }
            }
        }

        static void PrintMenu()
        {
            Console.WriteLine();
            Console.WriteLine("1 - предложения по возрастанию числа слов");
            Console.WriteLine("2 - предложения по возрастанию длины");
            Console.WriteLine("3 - слова заданной длины в вопросах");
            Console.WriteLine("4 - удалить слова заданной длины на согласную");
            Console.WriteLine("5 - заменить слова заданной длины в предложении");
            Console.WriteLine("6 - удалить стоп-слова");
            Console.WriteLine("7 - экспорт в XML");
            Console.WriteLine("8 - построить конкорданс");
            Console.WriteLine("0 - выход");
            Console.Write("> ");
        }

        static void SortByWordCount()
        {
            foreach (var s in text.SortByWordCount())
                Console.WriteLine($"[{s.WordCount}] {s}");
        }

        static void SortByLength()
        {
            foreach (var s in text.SortByLength())
                Console.WriteLine($"[{s.Length}] {s}");
        }

        static void FindWordsInQuestions()
        {
            int len = ReadInt("Длина слова: ");
            var words = text.FindWordsInQuestions(len);
            Console.WriteLine(words.Count > 0 ? string.Join(", ", words) : "Не найдено.");
        }

        static void RemoveWordsStartingWithConsonant()
        {
            int len = ReadInt("Длина слова: ");
            int n = text.RemoveWordsStartingWithConsonant(len);
            Console.WriteLine($"Удалено слов: {n}");
        }

        static void ReplaceWords()
        {
            for (int i = 0; i < text.Sentences.Count; i++)
                Console.WriteLine($"{i}: {text.Sentences[i]}");

            int index = ReadInt("Номер предложения: ");
            int len = ReadInt("Длина слова: ");
            Console.Write("Замена: ");
            string replacement = Console.ReadLine();

            bool ok = text.ReplaceWordsInSentence(index, len, replacement);
            Console.WriteLine(ok ? "Заменено." : "Подходящих слов не найдено.");
        }

        static void RemoveStopWords()
        {
            Console.Write("Язык стоп-слов (en/ru): ");
            string lang = Console.ReadLine().Trim().ToLower();
            var language = lang == "ru" ? Language.Russian : Language.English;

            var stopWords = StopWords.Load(language);
            int n = text.RemoveStopWords(stopWords);
            Console.WriteLine($"Удалено стоп-слов: {n}");
        }

        static void ExportToXml()
        {
            Console.Write("Имя файла (по умолчанию text.xml): ");
            string path = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(path)) path = "text.xml";

            text.ExportToXml(path);
            Console.WriteLine($"Сохранено в {path}");
        }

        static void BuildConcordance()
        {
            var concordance = Concordance.Build(text);
            concordance.Print();

            Console.Write("Сохранить в файл (Enter - не сохранять): ");
            string path = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(path))
            {
                concordance.ExportToFile(path);
                Console.WriteLine($"Сохранено в {path}");
            }
        }

        static int ReadInt(string prompt)
        {
            Console.Write(prompt);
            return int.Parse(Console.ReadLine());
        }
    }
}
