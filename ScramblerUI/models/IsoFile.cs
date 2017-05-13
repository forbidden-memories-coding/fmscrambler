namespace FMScrambler.Models
{
    public class IsoFile
    {
        public int Offset { get; set; }
        public int Size { get; set; }
        public string Name { get; set; }
        public int NameSize { get; set; }
        public bool isDirectory { get; set; }
    }
}