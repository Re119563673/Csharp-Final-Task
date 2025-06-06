using Shared;
using System.Diagnostics;
using System.IO.Pipes;

class Program
{
    static void Main()
    {
#pragma warning disable CA1416
        Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0x4; // CPU Core 3
#pragma warning restore CA1416

        string[] pipeNames = { "agent1", "agent2" };
        var threads = new List<Thread>();

        foreach (var pipe in pipeNames)
        {
            var t = new Thread(() => Listen(pipe));
            t.Start();
            threads.Add(t);
        }

        foreach (var t in threads) t.Join();
    }

    static void Listen(string pipeName)
    {
        using var server = new NamedPipeServerStream(pipeName, PipeDirection.In);
        Console.WriteLine($"Waiting for {pipeName}...");
        server.WaitForConnection();
        using var reader = new StreamReader(server);
        var json = reader.ReadLine();
        var entries = PipeHelper.Deserialize(json ?? "[]");

        Console.WriteLine($"\nData from {pipeName}:");
        foreach (var entry in entries)
        {
            Console.WriteLine($"{entry.FileName}:{entry.Word}:{entry.Count}");
        }
    }
}