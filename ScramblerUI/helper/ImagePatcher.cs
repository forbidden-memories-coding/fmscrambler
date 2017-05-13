using System;
using System.Collections.Generic;
using System.IO;
using FMScrambler.Models;

namespace FMScrambler.helper
{
    public class ImagePatcher
    {
        public List<IsoFile> IsoFile = new List<IsoFile>();
        private FileStream _fs;

        public ImagePatcher(string file)
        {
            _fs = new FileStream(file, FileMode.Open);
        }

        public int PatchImage()
        {

            ListDirectories(ref _fs, new IsoFile[]
            {
                new IsoFile
                {
                    Offset = 51744,
                    Name = "",
                    Size = 2048
                }
            });

            foreach (IsoFile k in IsoFile)
            {
                Console.WriteLine(k.Name);
                Console.WriteLine(k.Offset.ToString("X"));
                Console.WriteLine(k.Size.ToString("X"));
                Console.WriteLine("p = "+ ((k.Name == "SLUS_014.11") ? Static.SLUSPath : Static.WAPath));
                var p = (k.Name == "SLUS_014.11") ? Static.SLUSPath : Static.WAPath;
                using (FileStream fs2 = new FileStream(p, FileMode.Open))
                {
                    if ((long) k.Size != fs2.Length)
                        return -1;
                    _fs.Position = (long) (k.Offset + 24);

                    int n = 0;
                    while ((long) n < fs2.Length / 2048L)
                    {
                        _fs.Write(fs2.extractPiece(0, 2048, -1), 0, 2048);
                        _fs.Position += 304L;
                        n++;
                    }
                }

            }
            _fs.Dispose();
            _fs.Close();

            return 1;
        }

        private void ListDirectories(ref FileStream fs, IsoFile[] iso)
        {
            List<IsoFile> fileList = new List<IsoFile>();

            for (int i = 0; i < iso.Length; i++)
            {
                IsoFile file = iso[i];
                using (MemoryStream ms = new MemoryStream(fs.extractPiece(0, 2048, file.Offset)))
                {
                    ms.Position = 120L;
                    for (int j = ms.ReadByte(); j > 0; j = ms.ReadByte())
                    {
                        IsoFile tmpFile = new IsoFile();
                        byte[] arr = ms.extractPiece(0, j - 1, -1);
                        tmpFile.Offset = arr.extractInt32(1) * 2352;
                        tmpFile.Size = arr.extractInt32(9);
                        tmpFile.isDirectory = (arr[24] == 2);
                        tmpFile.NameSize = (int)arr[31];
                        tmpFile.Name = GetName(ref arr, tmpFile.NameSize);

                        if (tmpFile.isDirectory) fileList.Add(tmpFile);
                        if (tmpFile.NameSize == 13)
                            if (tmpFile.Name == "SLUS_014.11")
                                IsoFile.Add(tmpFile);
                        if (tmpFile.NameSize == 12 && tmpFile.Name == "WA_MRG.MRG")
                            IsoFile.Add(tmpFile);
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