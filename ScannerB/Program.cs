using Shared;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        Console.Write("Enter directory path: ");
        string? path = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(path)) return;

#pragma warning disable CA1416
        Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0x2; // CPU Core 2
#pragma warning restore CA1416

        var thread = new Thread(() => SendWordIndex(path!, "agent2"));
        thread.Start();
    }

    static void SendWordIndex(string dirPath, string pipeName)
    {
        var entries = new List<WordEntry>();

        foreach (var file in Directory.GetFiles(dirPath, "*.txt"))
        {
            string content = File.ReadAllText(file);
            var words = Regex.Matches(content.ToLower(), @"\w+");

            var wordCount = words.GroupBy(w => w.Value)
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