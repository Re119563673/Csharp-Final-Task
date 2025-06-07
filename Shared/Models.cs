using System.Text.Json;

namespace Shared
{
    public class WordEntry
    {
        public required string FileName { get; set; }
        public required string Word { get; set; }
        public int Count { get; set; }
    }

    public static class PipeHelper
    {
        public static string Serialize(List<WordEntry> entries)
            => JsonSerializer.Serialize(entries);

        public static List<WordEntry> Deserialize(string json)
            => JsonSerializer.Deserialize<List<WordEntry>>(json) ?? new();
    }
}