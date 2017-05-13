using System;
using System.Collections.Generic;
using System.IO;

namespace FMScrambler.helper
{
    public static class NumberHandler
    {
        public static int extractInt32(this byte[] bytes, int index = 0)
        {
            return (int)bytes[index + 3] << 24 | (int)bytes[index + 2] << 16 | (int)bytes[index + 1] << 8 | (int)bytes[index + 0];
        }

        public static byte[] extractPiece(this FileStream ms, int offset, int length, int changeOffset = -1)
        {
            if (changeOffset > -1)
                ms.Position = (long)changeOffset;
            byte[] buffer = new byte[length];
            ms.Read(buffer, 0, length);
            return buffer;
        }

        public static byte[] extractPiece(this MemoryStream ms, int offset, int length, int changeOffset = -1)
        {
            if (changeOffset > -1)
                ms.Position = (long)changeOffset;
            byte[] buffer = new byte[length];
            ms.Read(buffer, 0, length);
            return buffer;
        }

        public static void Save(this byte[] data, string path, int offset = -1, int length = -1)
        {
            int offset1 = offset > -1 ? offset : 0;
            int count = length > -1 ? length : data.Length;
            using (FileStream fileStream = File.Create(path))
                fileStream.Write(data, offset1, count);
        }

        public static byte[] int32ToByteArray(this int value)
        {
            byte[] numArray = new byte[4];
            for (int index = 0; index < 4; ++index)
                numArray[index] = (byte)(value >> index * 8 & (int)byte.MaxValue);
            return numArray;
        }

        public static ushort extractUInt16(this byte[] bytes, int index = 0)
        {
            return (ushort)((uint)bytes[index + 1] << 8 | (uint)bytes[index + 0]);
        }

        public static byte[] int16ToByteArray(this ushort value)
        {
            return ((short)value).int16ToByteArray();
        }

        public static byte[] int16ToByteArray(this short value)
        {
            byte[] numArray = new byte[2];
            for (int index = 0; index < 2; ++index)
                numArray[index] = (byte)((int)value >> index * 8 & (int)byte.MaxValue);
            return numArray;
        }

        public static byte[] copyFrom(this byte[] self, byte[] data, int copyOffset, int length, int destinyOffset = 0)
        {
            for (int index = copyOffset; index < length; ++index)
                self[destinyOffset + (index - copyOffset)] = data[index];
            return self;

        }

      

        public static String GetText(this MemoryStream ms, Dictionary<byte, char> dic)
        {
            string text = string.Empty;

            while (true)
            {
                byte b = ms.extractPiece(0, 1, -1)[0];

                if (dic.ContainsKey(b)) text += dic[b].ToString();
                else if (b == 254) text += "\r\n";
                else
                {
                    if (b == 255) break;
                    text = text + "[" + b.ToString("X2") + "]";
                }
            }
            return text;
        }

       
    }
}