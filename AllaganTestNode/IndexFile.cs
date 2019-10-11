using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AllaganTestNode
{
    class IndexFile
    {
        public string IndexPath { get; set; }
        public string DatBasePath { get; set; }
        public Dictionary<string, Dictionary<string, ExHFile>> ExHs { get; set; }

        public Dictionary<ExHLanguage, bool> Load(string indexPath)
        {
            try
            {
                if (string.IsNullOrEmpty(indexPath) || !File.Exists(indexPath)) throw new Exception("Invalid path.");

                Dictionary<uint, Dictionary<uint, SqFile>> sqFiles = new Dictionary<uint, Dictionary<uint, SqFile>>();

                using (FileStream fs = File.OpenRead(indexPath))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    br.BaseStream.Position = 0x0;
                    if (Encoding.ASCII.GetString(br.ReadBytes(6)) != "SqPack") throw new Exception("Invalid index file.");

                    br.BaseStream.Position = 0xc;
                    int headerOffset = br.ReadInt32();

                    br.BaseStream.Position = headerOffset + 0x50;
                    byte numDat = br.ReadByte();
                    string datBasePath = string.Format("{0}\\{1}.dat", Path.GetDirectoryName(indexPath), Path.GetFileNameWithoutExtension(indexPath));

                    for (int i = 0; i < numDat; i++)
                    {
                        if (!File.Exists(datBasePath + i.ToString())) throw new Exception(string.Format(".dat{0} file is not found.", i.ToString()));
                    }

                    IndexPath = indexPath;
                    DatBasePath = datBasePath;

                    br.BaseStream.Position = headerOffset + 0x8;
                    int fileOffset = br.ReadInt32();
                    int fileCount = br.ReadInt32() / 0x10;

                    br.BaseStream.Position = fileOffset;
                    for (int i = 0; i < fileCount; i++)
                    {
                        SqFile sqFile = new SqFile();
                        sqFile.FileKey = br.ReadUInt32();
                        sqFile.DirectoryKey = br.ReadUInt32();
                        sqFile.WrappedOffset = br.ReadUInt32();
                        br.ReadInt32();

                        if (!sqFiles.ContainsKey(sqFile.DirectoryKey)) sqFiles.Add(sqFile.DirectoryKey, new Dictionary<uint, SqFile>());
                        sqFiles[sqFile.DirectoryKey].Add(sqFile.FileKey, sqFile);

                        Program.Report(string.Format("Processing index file: {0} / {1} - {2}", i, fileCount, sqFile.FileKey));
                    }
                }

                SqFile rootFile = sqFiles[Hash.Compute("exd")][Hash.Compute("root.exl")];
                rootFile.LoadData(DatBasePath);

                ExHs = new Dictionary<string, Dictionary<string, ExHFile>>();
                Dictionary<ExHLanguage, bool> availableLanguages = new Dictionary<ExHLanguage, bool>();

                using (MemoryStream ms = new MemoryStream(rootFile.Data))
                using (StreamReader sr = new StreamReader(ms, Encoding.ASCII))
                {
                    sr.ReadLine();

                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        string[] split = line.Split(',');
                        if (split.Length != 2) continue;

                        Program.Report(line);

                        ExHFile exHFile = new ExHFile();
                        exHFile.ProcessSheetName(split[0].ToLower());

                        SqFile exHSqFile = sqFiles[Hash.Compute(exHFile.DirectoryName)][Hash.Compute(exHFile.FileName)];
                        exHFile.FileKey = exHSqFile.FileKey;
                        exHFile.DirectoryKey = exHSqFile.DirectoryKey;
                        exHFile.WrappedOffset = exHSqFile.WrappedOffset;

                        exHFile.LoadData(DatBasePath);
                        exHFile.LoadExH();

                        if (exHFile.Variant == 1 && exHFile.Columns != null && exHFile.Columns.Length > 0)
                        {
                            foreach (ExHLanguage lang in exHFile.Languages)
                            {
                                foreach (ExHRange range in exHFile.Ranges)
                                {
                                    string exDName = string.Format("{0}_{1}_{2}.exd", exHFile.Name, range.Start.ToString(), lang.ToString()).ToLower();
                                    uint exDKey = Hash.Compute(exDName);

                                    if (!sqFiles[exHFile.DirectoryKey].ContainsKey(exDKey)) continue;

                                    if (!availableLanguages.ContainsKey(lang)) availableLanguages.Add(lang, true);

                                    ExDFile exDFile = new ExDFile();
                                    exDFile.Name = exHFile.Name;
                                    exDFile.DirectoryName = exHFile.DirectoryName;
                                    exDFile.FileName = exDName;

                                    SqFile exDSqFile = sqFiles[exHFile.DirectoryKey][exDKey];
                                    exDFile.FileKey = exDSqFile.FileKey;
                                    exDFile.DirectoryKey = exDSqFile.DirectoryKey;
                                    exDFile.WrappedOffset = exDSqFile.WrappedOffset;

                                    exHFile.ExDs.Add(exDFile);
                                }
                            }

                            if (exHFile.ExDs.Count > 0)
                            {
                                if (!ExHs.ContainsKey(exHFile.DirectoryName)) ExHs.Add(exHFile.DirectoryName, new Dictionary<string, ExHFile>());
                                ExHs[exHFile.DirectoryName].Add(exHFile.Name, exHFile);
                            }
                        }
                    }
                }

                if (availableLanguages.Keys.Count == 0) throw new Exception("No available languages found!");

                return availableLanguages;
            }
            catch (Exception e)
            {
                throw new Exception("IndexFile on Load -> " + e.Message);
            }
        }
    }
}