using System;
using System.Collections.Generic;
using System.IO;
using FMLib.Models;
using FMLib.Utility;

namespace FMLib.Randomizer
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
            ListDirectories(ref _fs, new[]
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
                // Choose which File to use based on the name of the Item in the loop
                string p = k.Name == "SLUS_014.11" ? Static.SlusPath : Static.WaPath;

                using (FileStream fs2 = new FileStream(p, FileMode.Open))
                {
                    // Filesize is different, abort with error
                    if (k.Size != fs2.Length)
                    {
                        return -1;
                    }

                    _fs.Position = k.Offset + 24;

                    for (int n = 0; n < fs2.Length / 2048L; n++)
                    {
                        _fs.Write(fs2.ExtractPiece(0, 2048), 0, 2048);
                        _fs.Position += 304L;
                    }
                }
            }
            _fs.Dispose();
            _fs.Close();

            string[] cueTemplate = {$"FILE \"{Static.RandomizerFileName}.bin\" BINARY", "  TRACK 01 MODE2/2352", "    INDEX 01 00:00:00" };
            
            
            File.WriteAllLines($"{Directory.GetCurrentDirectory()}\\{Static.RandomizerFileName}.cue", cueTemplate);
            return 1;
        }

        private void ListDirectories(ref FileStream fs, IEnumerable<GameFile> iso)
        {
            List<GameFile> fileList = new List<GameFile>();

            foreach (GameFile file in iso)
            {
                using (MemoryStream ms = new MemoryStream(fs.ExtractPiece(0, 2048, file.Offset)))
                {
                    ms.Position = 120L;
                    for (int j = ms.ReadByte(); j > 0; j = ms.ReadByte())
                    {
                        GameFile tmpFile = new GameFile();
                        byte[] arr = ms.ExtractPiece(0, j - 1);
                        tmpFile.Offset = arr.ExtractInt32(1) * 2352;
                        tmpFile.Size = arr.ExtractInt32(9);
                        tmpFile.IsDirectory = arr[24] == 2;
                        tmpFile.NameSize = arr[31];
                        tmpFile.Name = GetName(ref arr, tmpFile.NameSize);

                        if (tmpFile.IsDirectory)
                        {
                            fileList.Add(tmpFile);
                        }

                        if (tmpFile.NameSize == 13 && tmpFile.Name == "SLUS_014.11")
                        {
                            GameFile.Add(tmpFile);
                        }

                        if (tmpFile.NameSize == 12 && tmpFile.Name == "WA_MRG.MRG")
                        {
                            GameFile.Add(tmpFile);
                        }
                    }
                }
            }
            if (fileList.Count > 0)
            {
                ListDirectories(ref fs, fileList.ToArray());
            }
        }

        private static string GetName(ref byte[] data, int size)
        {
            string text = string.Empty;
            for (int i = 0; i < size; i++)
            {
                char c = Convert.ToChar(data[32 + i]);
                if (c == ';')
                {
                    break;
                }

                text += c.ToString();
            }
            return text;
        }
    }
}