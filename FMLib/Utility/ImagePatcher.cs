using System;
using System.Collections.Generic;
using System.IO;
using FMLib.Models;

namespace FMLib.Helper
{
    /// <summary>
    /// Patching of the Game Image File (BIN/ISO)
    /// </summary>
    public class ImagePatcher
    {
        /// <summary>
        /// List of Files in the Game Image
        /// </summary>
        public List<GameFile> GameFile = new List<GameFile>();

        /// <summary>
        /// Filestream to handle the data
        /// </summary>
        private FileStream _fs;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file">Filename</param>        
        public ImagePatcher(string file)
        {
            _fs = new FileStream(file, FileMode.Open);
        }

        /// <summary>
        /// Method to patch the Game Image File
        /// </summary>
        /// <returns>1 for success, -1 for failure</returns>
        public int PatchImage()
        {
            ListDirectories(ref _fs, new GameFile[]
            {
                new GameFile
                {
                    Offset = 51744,
                    Name = "",
                    Size = 2048
                }
            });

            foreach (GameFile k in GameFile)
            {
                #if DEBUG
                Console.WriteLine(k.Name);
                Console.WriteLine(k.Offset.ToString("X"));
                Console.WriteLine(k.Size.ToString("X"));
                Console.WriteLine("p = "+ ((k.Name == "SLUS_014.11") ? Static.SLUSPath : Static.WAPath));
                #endif
    
                // Choose which File to use based on the name of the Item in the loop
                string p = (k.Name == "SLUS_014.11") ? Static.SLUSPath : Static.WAPath;


                using (FileStream fs2 = new FileStream(p, FileMode.Open))
                {
                    // Filesize is different, abort with error
                    if (k.Size != fs2.Length)
                        return -1;

                    _fs.Position = (k.Offset + 24);

                    for (int n = 0; n < fs2.Length / 2048L; n++)
                    {
                        _fs.Write(fs2.extractPiece(0, 2048, -1), 0, 2048);
                        _fs.Position += 304L;
                    }
                }
            }
            _fs.Dispose();
            _fs.Close();

            return 1;
        }

        private void ListDirectories(ref FileStream fs, GameFile[] iso)
        {
            List<GameFile> fileList = new List<GameFile>();

            for (int i = 0; i < iso.Length; i++)
            {
                GameFile file = iso[i];
                using (MemoryStream ms = new MemoryStream(fs.extractPiece(0, 2048, file.Offset)))
                {
                    ms.Position = 120L;
                    for (int j = ms.ReadByte(); j > 0; j = ms.ReadByte())
                    {
                        GameFile tmpFile = new GameFile();
                        byte[] arr = ms.extractPiece(0, j - 1, -1);
                        tmpFile.Offset = arr.extractInt32(1) * 2352;
                        tmpFile.Size = arr.extractInt32(9);
                        tmpFile.isDirectory = (arr[24] == 2);
                        tmpFile.NameSize = (int)arr[31];
                        tmpFile.Name = GetName(ref arr, tmpFile.NameSize);

                        if (tmpFile.isDirectory) fileList.Add(tmpFile);
                        if (tmpFile.NameSize == 13)
                            if (tmpFile.Name == "SLUS_014.11")
                                GameFile.Add(tmpFile);
                        if (tmpFile.NameSize == 12 && tmpFile.Name == "WA_MRG.MRG")
                            GameFile.Add(tmpFile);
                    }
                }
            }
            if (fileList.Count > 0)
                ListDirectories(ref fs, fileList.ToArray());
        }

        private static string GetName(ref byte[] data, int size)
        {
            string text = string.Empty;
            for (int i = 0; i < size; i++)
            {
                char c = Convert.ToChar(data[32 + i]);
                if (c == ';') break;
                
                text += c.ToString();
            }
            return text;
        }
    }
}