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

