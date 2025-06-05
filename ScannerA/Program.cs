using Shared;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Enter directory path: ");
        string path = Console.ReadLine();
        string pipeName = "agent1"; // Update AgentB to use agent2 instead.

        Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0x1; // Core 1

        var readThread = new Thread(() => SendWordIndex(path, pipeName));
        readThread.Start();
    }

    static void SendWordIndex(string dirPath, string pipeName)
    {
        var entries = new List<WordEntry>();

        foreach (var file in Directory.GetFiles(dirPath, "*.txt"))
        {
            string content = File.ReadAllText(file);
            var words = Regex.Matches(content.ToLower(), @"\w+");

            var wordCount = words
                .GroupBy(w => w.Value)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var pair in wordCount)
            {
                entries.Add(new WordEntry
                {
                    FileName = Path.GetFileName(file),
                    Word = pair.Key,
                    Count = pair.Value
                });
            }
        }

        using var client = new NamedPipeClientStream(".", pipeName, PipeDirection.Out);
        client.Connect();
        using var writer = new StreamWriter(client) { AutoFlush = true };
        writer.WriteLine(PipeHelper.Serialize(entries));
    }
}