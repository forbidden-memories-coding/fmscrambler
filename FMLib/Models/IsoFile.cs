namespace FMLib.Models
{
    /// <summary>
    /// GameFile Model Class
    /// </summary>
    public class GameFile
    {
        /// <summary>
        /// Offset in the file as Integer
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Size of the file as Integer
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Name of the file as String
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Size of the Name as Integer
        /// </summary>
        public int NameSize { get; set; }

        /// <summary>
        /// Is file a directory as Boolean
        /// </summary>
        public bool isDirectory { get; set; }
    }
}