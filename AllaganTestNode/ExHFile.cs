using System.Collections.Generic;
using System.Linq;

namespace AllaganTestNode
{
    class ExHColumn
    {
        public ushort Type { get; set; }
        public ushort Offset { get; set; }
    }

    class ExHRange
    {
        public uint Start { get; set; }
        public uint Length { get; set; }
    }

    enum ExHLanguage
    {
        Null = 0,
        Ja = 1,
        En = 2,
        De = 3,
        Fr = 4,
        Chs = 5,
        Cht = 6,
        Ko = 7
    }

    class ExHFile : SqFile
    {
        public string Name { get; set; }
        public ushort Variant { get; set; }
        public ushort FixedSizeDataLength { get; set; }
        public ExHColumn[] Columns { get; set; }
        public ExHRange[] Ranges { get; set; }
        public ExHLanguage[] Languages { get; set; }
        public List<ExDFile> ExDs { get; set; }

        public ExHFile()
        {
            ExDs = new List<ExDFile>();
        }

        public void ProcessSheetName(string line)
        {
            DirectoryName = string.Empty;
            Name = line;

            if (Name.Contains("/"))
            {
                DirectoryName = "/" + Name.Substring(0, Name.LastIndexOf("/"));
                Name = Name.Substring(Name.LastIndexOf("/") + 1);
            }

            DirectoryName = "exd" + DirectoryName;

            FileName = Name + ".exh";
        }

        public void LoadExH()
        {
            if (Data == null || Data.Length == 0) return;

            FixedSizeDataLength = ToUInt16(Data, 0x6, true);
            ushort columnCount = ToUInt16(Data, 0x8, true);
            Variant = ToUInt16(Data, 0x10, true);
            ushort rangeCount = ToUInt16(Data, 0xa, true);
            ushort langCount = ToUInt16(Data, 0xc, true);

            if (Variant != 1) return;

            Columns = new ExHColumn[columnCount];
            for (int i = 0; i < columnCount; i++)
            {
                int columnOffset = 0x20 + i * 0x4;

                Columns[i] = new ExHColumn();
                Columns[i].Type = ToUInt16(Data, columnOffset, true);
                Columns[i].Offset = ToUInt16(Data, columnOffset + 0x2, true);
            }
            Columns = Columns.Where(c => c.Type == 0x0).ToArray();

            Ranges = new ExHRange[rangeCount];
            for (int i = 0; i < rangeCount; i++)
            {
                int rangeOffset = (0x20 + columnCount * 0x4) + i * 0x8;

                Ranges[i] = new ExHRange();
                Ranges[i].Start = ToUInt32(Data, rangeOffset, true);
                Ranges[i].Length = ToUInt32(Data, rangeOffset + 0x4, true);
            }

            Languages = new ExHLanguage[langCount];
            for (int i = 0; i < langCount; i++)
            {
                int langOffset = ((0x20 + columnCount * 0x4) + rangeCount * 0x8) + i * 0x2;

                Languages[i] = (ExHLanguage)Data[langOffset];
            }
        }
    }
}