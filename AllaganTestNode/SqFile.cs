using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace AllaganTestNode
{
    class SqFile
    {
        public uint FileKey { get; set; }
        public string FileName { get; set; }
        public uint DirectoryKey { get; set; }
        public string DirectoryName { get; set; }
        public uint WrappedOffset { get; set; }
        public byte DatNum
        {
            get
            {
                return (byte)((WrappedOffset & 0x7) >> 1);
            }

            set
            {
                WrappedOffset = (WrappedOffset & 0xfffffff8) | (uint)((value & 0x3) << 1);
            }
        }
        public int Offset
        {
            get
            {
                return (int)(WrappedOffset & 0xfffffff8) << 3;
            }

            set
            {
                WrappedOffset = (WrappedOffset & 0x7) | (uint)((value >> 3) & 0xfffffff8);
            }
        }
        public byte[] Data;

        public void LoadData(string datBasePath)
        {
            try
            {
                string datPath = datBasePath + DatNum.ToString();

                if (!File.Exists(datPath)) throw new Exception("Data file not found at " + datPath);

                using (FileStream fs = File.OpenRead(datPath))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    br.BaseStream.Position = 0x0;
                    if (Encoding.ASCII.GetString(br.ReadBytes(6)) != "SqPack") throw new Exception("Invalid dat file.");

                    br.BaseStream.Position = Offset;
                    int endOfHeader = br.ReadInt32();

                    byte[] header = new byte[endOfHeader];
                    br.BaseStream.Position = Offset;
                    br.Read(header, 0, endOfHeader);

                    if (BitConverter.ToInt32(header, 0x4) != 2) throw new Exception("This tool only supports binary type.");

                    long length = BitConverter.ToInt32(header, 0x10) * 0x80;
                    short blockCount = BitConverter.ToInt16(header, 0x14);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        for (int i = 0; i < blockCount; i++)
                        {
                            int blockOffset = BitConverter.ToInt32(header, 0x18 + i * 0x8);

                            byte[] blockHeader = new byte[0x10];
                            br.BaseStream.Position = Offset + endOfHeader + blockOffset;
                            br.Read(blockHeader, 0, 0x10);

                            int sourceSize = BitConverter.ToInt32(blockHeader, 0x8);
                            int rawSize = BitConverter.ToInt32(blockHeader, 0xc);

                            bool isCompressed = sourceSize < 0x7d00;
                            int actualSize = isCompressed ? sourceSize : rawSize;

                            int paddingLeftover = (actualSize + 0x10) % 0x80;
                            if (isCompressed && paddingLeftover != 0)
                            {
                                actualSize += 0x80 - paddingLeftover;
                            }

                            byte[] blockBuffer = new byte[actualSize];
                            br.Read(blockBuffer, 0, actualSize);

                            if (isCompressed)
                            {
                                using (MemoryStream blockBufferStream = new MemoryStream(blockBuffer))
                                using (DeflateStream ds = new DeflateStream(blockBufferStream, CompressionMode.Decompress))
                                {
                                    ds.CopyTo(ms);
                                }
                            }
                            else
                            {
                                ms.Write(blockBuffer, 0, blockBuffer.Length);
                            }
                        }

                        Data = ms.ToArray();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("SqFile on ReadData -> " + e.Message);
            }
        }

        protected void CheckEndian(ref byte[] data, bool isBigEndian)
        {
            if (isBigEndian == BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }
        }

        protected short ToInt16(byte[] buffer, int offset, bool isBigEndian)
        {
            byte[] tmp = new byte[2];
            Array.Copy(buffer, offset, tmp, 0, 2);
            CheckEndian(ref tmp, isBigEndian);
            return BitConverter.ToInt16(tmp, 0);
        }

        protected ushort ToUInt16(byte[] buffer, int offset, bool isBigEndian)
        {
            byte[] tmp = new byte[2];
            Array.Copy(buffer, offset, tmp, 0, 2);
            CheckEndian(ref tmp, isBigEndian);
            return BitConverter.ToUInt16(tmp, 0);
        }

        protected int ToInt32(byte[] buffer, int offset, bool isBigEndian)
        {
            byte[] tmp = new byte[4];
            Array.Copy(buffer, offset, tmp, 0, 4);
            CheckEndian(ref tmp, isBigEndian);
            return BitConverter.ToInt32(tmp, 0);
        }

        protected uint ToUInt32(byte[] buffer, int offset, bool isBigEndian)
        {
            byte[] tmp = new byte[4];
            Array.Copy(buffer, offset, tmp, 0, 4);
            CheckEndian(ref tmp, isBigEndian);
            return BitConverter.ToUInt32(tmp, 0);
        }
    }
}