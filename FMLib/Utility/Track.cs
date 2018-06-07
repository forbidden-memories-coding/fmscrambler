using System;
using System.IO;

namespace FMLib.Utility
{
    class Track
    {
        public int TrackNumber;
        public bool Audio;
        public string Modes;
        public TrackExtension FileExtension;
        public int BlockStart;
        public int BlockSize;
        public long StartSector;
        public long StopSector;
        public long Stop;

        public static bool SwapAudioByteOrder = false;
        public static bool WavFormat = false;
        public static bool TruncatePsx = false;

        public enum TrackExtension
        {
            Iso, Cdr, Wav, Ugh
        }

        public Track(string trackNumber, string mode, string time)
        {
            TrackNumber = int.Parse(trackNumber);
            SetMode(mode);
            StartSector = Track.ToFrames(time);

            Console.WriteLine("Track {0:00}: {1} {2}", TrackNumber, mode.PadLeft(12), time);
        }

        /// <summary>
        /// Property StartPosition (long)
        /// </summary>
        public long StartPosition
        {
            get { return StartSector * BinChunk.SectorLength; }
        }

        /// <summary>
        /// Parse the mode string
        /// </summary>
        private void SetMode(string mode)
        {
            Modes = mode;
            Audio = false;
            BlockStart = 0;
            FileExtension = TrackExtension.Iso;

            switch (mode.ToUpper())
            {
                case "AUDIO":
                    BlockSize = 2352;
                    Audio = true;
                    if (WavFormat)
                        FileExtension = TrackExtension.Wav;
                    else
                        FileExtension = TrackExtension.Cdr;
                    break;
                case "MODE1/2352":
                    BlockStart = 16;
                    BlockSize = 2048;
                    break;
                case "MODE2/2336":
                    // WAS 2352 in V1.361B still work? What if MODE2/2336 single track bin, still 2352 sectors?
                    BlockStart = 16;
                    BlockSize = 2336;
                    break;
                case "MODE2/2352":
                    if (TruncatePsx)
                        // PSX: truncate from 2352 to 2336 byte tracks 
                        BlockSize = 2336;
                    else
                    {
                        // Normal MODE2/2352 
                        BlockStart = 24;
                        BlockSize = 2048;
                    }
                    break;
                default:
                    Console.WriteLine("(?) ");
                    BlockSize = 2352;
                    FileExtension = TrackExtension.Ugh;
                    break;
            }
        }

        /// <summary>
        /// Write a track
        /// </summary>
        /// <param name="bf"></param>
        /// <param name="fileName">File to write track data to.</param>
        public void Write(Stream bf, string fileName)
        {
            Console.Write(Environment.NewLine + "{0:d2}: {1} ", TrackNumber, fileName);

            try
            {
                bf.Seek(StartPosition, SeekOrigin.Begin);
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("Could not seek to track location: {0}", e.Message));
            }

            Console.Write("                                          ");
            try
            {
                using (Stream stream = File.OpenWrite(fileName))
                {
                    if (Audio && WavFormat)
                    {
                        byte[] header = MakeWavHeader(RealLength);
                        stream.Write(header, 0, header.Length);
                    }
                    long sz = StartPosition;
                    long sector = StartSector;
                    long realsz = 0;

                    byte[] buf = new byte[BinChunk.SectorLength];
                    while ((sector <= StopSector) && (bf.Read(buf, 0, BinChunk.SectorLength) > 0))
                    {
                        if (Audio && SwapAudioByteOrder)
                            DoByteSwap(buf);

                        stream.Write(buf, BlockStart, BlockSize);
                        sz += BinChunk.SectorLength;
                        realsz += BlockSize;
                        if (((sz / BinChunk.SectorLength) % 500) == 0)
                            PrintProgress(realsz, RealLength);
                        sector++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(Environment.NewLine);
                throw new ApplicationException(string.Format(" Could not write to track file {0}: {1}", fileName, e.Message));
            }

            PrintProgress(RealLength, RealLength);
        }

        void PrintProgress(long realsz, long realLength)
        {
            float fl = (float)realsz / (float)realLength;
            string msg = string.Format("{0:d4}/{1:d4} MB  [{2}] {3:d3}%",
                realsz / 1024 / 1024, realLength / 1024 / 1024, GetProgressBar(fl, 29), (int)(fl * 100));
            msg = msg.PadLeft(2 * msg.Length, '\b');
            Console.Write(msg);
        }

        /// <summary>
        /// return a progress bar
        /// </summary>
        /// <param name="value">Progress (0.0 - 1.0)</param>
        /// <param name="position">Width of the progress bar (number of characters)</param>
        /// <returns>String containing multiple '*' showing progress.</returns>
        string GetProgressBar(float position, int length)
        {
            int n = (int)(length * position);
            return "".PadLeft(n, '*').PadRight(length);
        }

        byte[] MakeWavHeader(long length)
        {
            const int wavRiffHlen = 12;
            const int wavFormatHlen = 24;
            const int wavDataHlen = 8;
            const int wavHeaderLen = wavRiffHlen + wavFormatHlen + wavDataHlen;

            MemoryStream memoryStream = new MemoryStream(wavHeaderLen);
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                // RIFF header
                writer.Write("RIFF".ToCharArray());
                uint dwordValue = (uint)length + wavDataHlen + wavFormatHlen + 4;
                writer.Write(dwordValue);  // length of file, starting from WAVE
                writer.Write("WAVE".ToCharArray());
                // FORMAT header
                writer.Write("fmt ".ToCharArray());
                dwordValue = 0x10;     // length of FORMAT header
                writer.Write(dwordValue);
                ushort wordValue = 0x01;     // constant
                writer.Write(wordValue);
                wordValue = 0x02;   // channels
                writer.Write(wordValue);
                dwordValue = 44100; // sample rate
                writer.Write(dwordValue);
                dwordValue = 44100 * 4; // bytes per second
                writer.Write(dwordValue);
                wordValue = 4;      // bytes per sample
                writer.Write(wordValue);
                wordValue = 2 * 8;  // bits per channel
                writer.Write(wordValue);
                // DATA header
                writer.Write("data".ToCharArray());
                dwordValue = (uint)length;
                writer.Write(dwordValue);
            }
            return memoryStream.ToArray();
        }

        void DoByteSwap(byte[] buf)
        {
            // swap low and high bytes 
            int p = BlockStart;
            int ep = BlockSize;
            while (p < ep)
            {
                byte c = buf[p];
                buf[p] = buf[p + 1];
                buf[p + 1] = c;
                p += 2;
            }
        }

        /// <summary>
        /// Length in bytes of track.
        /// </summary>
        public long RealLength
        {
            get { return (StopSector - StartSector + 1) * BlockSize; }
        }

        /// <summary>
        /// Convert a mins:secs:frames format to plain frames
        /// </summary>
        /// <param name="s">text containing mins:secs:frames</param>
        /// <returns>Frame number</returns>
        static long ToFrames(string time)
        {
            string[] segs = time.Split(':');

            int mins = int.Parse(segs[0]);
            int secs = int.Parse(segs[1]);
            int frames = int.Parse(segs[2]);

            return (mins * 60 + secs) * 75 + frames;
        }
    }

}

