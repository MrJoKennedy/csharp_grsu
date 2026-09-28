using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GeneticSearch
{
    class Program
    {
        struct GeneticData
        {
            public string protein;
            public string organism;
            public string amino_acids;
        }

        const string Line = "--------------------------------------------------------------------------";

        static string RLEncoding(string amino_acids)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < amino_acids.Length; i++)
            {
                char ch = amino_acids[i];
                int count = 1;

                while (i + 1 < amino_acids.Length && amino_acids[i + 1] == ch)
                {
                    count++;
                    i++;
                }

                if (count >= 3)
                    result.Append(count);

                result.Append(ch, count);
            }

            return result.ToString();
        }

        static string RLDecoding(string amino_acids)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < amino_acids.Length; i++)
            {
                if (char.IsDigit(amino_acids[i]))
                {
                    int count = amino_acids[i] - '0';
                    i++;
                    if (i < amino_acids.Length)
                        result.Append(amino_acids[i], count);
                }
                else
                {
                    result.Append(amino_acids[i]);
                }
            }

            return result.ToString();
        }

        static List<GeneticData> ReadData(string filename)
        {
            List<GeneticData> data = new List<GeneticData>();

            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    if (String.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split('\t');
                    if (parts.Length < 3) continue;

                    GeneticData item;
                    item.protein = parts[0];
                    item.organism = parts[1];
                    item.amino_acids = RLDecoding(parts[2]);
                    data.Add(item);
                }
            }

            return data;
        }

        static GeneticData? FindProtein(List<GeneticData> data, string proteinName)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].protein == proteinName)
                    return data[i];
            }
            return null;
        }

        static void Search(List<GeneticData> data, string sequence, StreamWriter writer)
        {
            sequence = RLDecoding(sequence);

            writer.WriteLine("organism\t\t\t\tprotein");
            bool found = false;

            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].amino_acids.Contains(sequence))
                {
                    writer.WriteLine(data[i].organism + "\t\t" + data[i].protein);
                    found = true;
                }
            }

            if (!found)
                writer.WriteLine("NOT FOUND");
        }

        static void Diff(List<GeneticData> data, string protein1, string protein2, StreamWriter writer)
        {
            GeneticData? p1 = FindProtein(data, protein1);
            GeneticData? p2 = FindProtein(data, protein2);

            writer.WriteLine("amino-acids difference:");

            if (!p1.HasValue || !p2.HasValue)
            {
                StringBuilder missing = new StringBuilder("MISSING:");
                if (!p1.HasValue) missing.Append(" ").Append(protein1);
                if (!p2.HasValue) missing.Append(" ").Append(protein2);
                writer.WriteLine(missing.ToString());
                return;
            }

            string a = p1.Value.amino_acids;
            string b = p2.Value.amino_acids;
            int minLength = Math.Min(a.Length, b.Length);
            int difference = Math.Abs(a.Length - b.Length);

            for (int i = 0; i < minLength; i++)
            {
                if (a[i] != b[i])
                    difference++;
            }

            writer.WriteLine(difference);
        }

        static void Mode(List<GeneticData> data, string proteinName, StreamWriter writer)
        {
            GeneticData? protein = FindProtein(data, proteinName);

            writer.WriteLine("amino-acid occurs:");

            if (!protein.HasValue)
            {
                writer.WriteLine("MISSING: " + proteinName);
                return;
            }

            int[] count = new int[26];
            string sequence = protein.Value.amino_acids;

            for (int i = 0; i < sequence.Length; i++)
            {
                if (sequence[i] >= 'A' && sequence[i] <= 'Z')
                    count[sequence[i] - 'A']++;
            }

            int max = 0;
            char result = 'A';

            for (int i = 0; i < 26; i++)
            {
                if (count[i] > max)
                {
                    max = count[i];
                    result = (char)('A' + i);
                }
            }

            writer.WriteLine(result + "\t\t" + max);
        }

        static void ProcessCommands(List<GeneticData> data, string commandsFile, StreamWriter writer)
        {
            using (StreamReader reader = new StreamReader(commandsFile))
            {
                int number = 1;

                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    if (String.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split('\t');
                    string command = parts[0];

                    writer.WriteLine(Line);

                    if (command == "search" && parts.Length >= 2)
                    {
                        string sequence = RLDecoding(parts[1]);
                        writer.WriteLine(number.ToString("D3") + "   search   " + sequence);
                        Search(data, sequence, writer);
                    }
                    else if (command == "diff" && parts.Length >= 3)
                    {
                        writer.WriteLine(number.ToString("D3") + "   diff   " + parts[1] + "   " + parts[2]);
                        Diff(data, parts[1], parts[2], writer);
                    }
                    else if (command == "mode" && parts.Length >= 2)
                    {
                        writer.WriteLine(number.ToString("D3") + "   mode   " + parts[1]);
                        Mode(data, parts[1], writer);
                    }

                    number++;
                }
            }

            writer.WriteLine(Line);
        }

        static string? ChooseFile(string title, string pattern)
        {
            string dir = Directory.GetCurrentDirectory();
            List<string> files = new List<string>(Directory.GetFiles(dir, pattern));

            if (files.Count == 0)
                files = new List<string>(Directory.GetFiles(dir, "*.txt"));

            files.Sort(StringComparer.OrdinalIgnoreCase);

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine(title);

                for (int i = 0; i < files.Count; i++)
                    Console.WriteLine("  " + (i + 1) + ". " + Path.GetFileName(files[i]));

                Console.WriteLine("  0. Enter path manually");
                Console.Write("Your choice: ");

                string? input = Console.ReadLine();
                if (input == null) return null;
                input = input.Trim();

                if (input == "0")
                {
                    Console.Write("Path: ");
                    string? path = Console.ReadLine();
                    if (path == null) return null;
                    path = path.Trim().Trim('"');

                    if (File.Exists(path)) return path;
                    Console.WriteLine("File not found.");
                }
                else if (int.TryParse(input, out int number) && number >= 1 && number <= files.Count)
                {
                    return files[number - 1];
                }
                else
                {
                    Console.WriteLine("Invalid choice, try again.");
                }
            }
        }

        static void Main(string[] args)
        {
            string? sequencesFile;
            string? commandsFile;
            string outputFile;

            if (args.Length >= 2)
            {
                sequencesFile = args[0];
                commandsFile = args[1];
                outputFile = args.Length > 2 ? args[2] : "genedata.txt";
            }
            else
            {
                sequencesFile = ChooseFile("Select sequences file:", "sequences*.txt");
                if (sequencesFile == null) return;

                commandsFile = ChooseFile("Select commands file:", "commands*.txt");
                if (commandsFile == null) return;

                Console.Write("\nOutput file [genedata.txt]: ");
                string? answer = Console.ReadLine();
                outputFile = String.IsNullOrWhiteSpace(answer) ? "genedata.txt" : answer.Trim();
            }

            if (!File.Exists(sequencesFile) || !File.Exists(commandsFile))
            {
                Console.WriteLine("Input file not found.");
                return;
            }

            List<GeneticData> data = ReadData(sequencesFile);

            using (StreamWriter writer = new StreamWriter(outputFile, false, Encoding.UTF8))
            {
                writer.WriteLine("Mr.Kennedy");
                writer.WriteLine("Genetic Searching");
                ProcessCommands(data, commandsFile, writer);
            }

            Console.WriteLine("Done. Results written to " + outputFile);
        }
    }
}
