using System.Text.Json;

namespace Shared
{
    public class WordEntry
    {
        public required string FileName { get; set; }
        public required string Word { get; set; }
        public int Count { get; set; }
    }

