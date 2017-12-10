using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using Infinity.Data;
using Infinity.HDD.Utils;

namespace Infinity.HDD.Implementation
{
    public class HDDDataManager : IDataLoader, IDataSaver
    {
        public IInfinityFile LoadFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");

            if (!File.Exists(path))
                throw new Exception(string.Format("Файла {0} не существует.", path));

            List<string> blockFiles = new List<string>();

            var result = new HDDInfinityFileImpl(this, path);

            var formatter = new BinaryFormatter();

            var rootPath = Path.GetTempPath() + Assembly.GetExecutingAssembly().GetName().Name + "\\";

            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var header = LoadHeader(reader);

                    var points = new List<HDDHdaPoint>();
                    
                    for (int i = 0; i < header.PointsCount; i++)
                    {
                        var pointInfo = (PointInfo)formatter.Deserialize(reader.BaseStream);

                        var blocks = (int)Math.Ceiling(pointInfo.ValuesCount / (1.0 * BLOCK_SIZE));

                        pointInfo.Folder = rootPath + Guid.NewGuid() + "\\";
                        pointInfo.BlockFiles = new string[blocks];

                        for (int j = 0; j < blocks; j++)
                        {
                            var file = string.Format("{0}{1}_{2}.part", pointInfo.Folder, "points", j);

                            pointInfo.BlockFiles[j] = file;
                            
                            int blockSize = reader.ReadInt32();

                            var block = reader.ReadBytes(blockSize);

                            if (File.Exists(file))
                                File.Delete(file);

                            new FileInfo(file).Directory.Create();

                            using (var blockStream = File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                            {
                                using (var memStream = new MemoryStream())
                                {
                                    memStream.Write(block, 0, blockSize);

                                    memStream.Seek(0, SeekOrigin.Begin);

                                    using (var zipStream = new GZipStream(memStream, CompressionMode.Decompress))
                                    {
                                        zipStream.CopyTo(blockStream);
                                    }
                                }
                            }
                        }
                        points.Add(new HDDHdaPoint(pointInfo));
                    }

                    result.Points = points;
                }
            }

            foreach (var blockFile in blockFiles)
            {
                File.Delete(blockFile);
            }

            return result;
        }

        internal struct FileHeader
        {
            public int EventsCount { get; set; }
            public int PointsCount { get; set; }
            public int ConfigEventsCount { get; set; }
        }

        private FileHeader LoadHeader(BinaryReader reader)
        {
            if (!reader.ReadBytes(16).SequenceEqual(Guid.Parse(Header).ToByteArray()))
                throw new Exception("Файл не содержит заголовок.");

            if (reader.ReadInt32() != Version)
                throw new Exception("Версии файлов не совпадают.");

            byte[] hash = reader.ReadBytes(16);

#if DEBUG
            Debug.WriteLine("MD5: " + hash.Select(T => T.ToString("X")).Aggregate((T1, T2) => T1 + " " + T2));
#endif

            var position = reader.BaseStream.Position;

            using (var md5 = MD5.Create())
            {
                if (!md5.ComputeHash(reader.BaseStream).SequenceEqual(hash))
                    throw new Exception("Контрольные суммы не совпадают.");
            }

            reader.BaseStream.Seek(position, SeekOrigin.Begin);

            return new FileHeader
            {
                PointsCount = reader.ReadInt32(),
                EventsCount = reader.ReadInt32(),
                ConfigEventsCount = reader.ReadInt32()
            };
        }

        public IWriteableInfinityFile CreateFile(string path)
        {
            return new HDDInfinityFileImpl(this, path);
        }

        #region Internal

        private const string Header = "{F3DEBDFA-7EBA-4D11-A9D3-8DB8232A8CEC}";
        private const int Version = 1;
        /// <summary>
        /// Количество записываемых элементов.
        /// </summary>
        public const int BLOCK_SIZE = 100000;

        internal void Save(HDDInfinityFileImpl file)
        {
            if (string.IsNullOrWhiteSpace(file.FilePath))
                throw new Exception("FileName is empty.");

            List<string> blockFiles = new List<string>();

            var rootPath = Path.GetTempPath() + Assembly.GetExecutingAssembly().GetName().Name + "\\";

            blockFiles.AddRange(CompressedBlocksUtils.SaveToFileByParts(file.AddedEvents, "events", rootPath, BLOCK_SIZE));
            blockFiles.AddRange(CompressedBlocksUtils.SaveToFileByParts(file.AddedConfigEvents,
                                                                        "configEvents",
                                                                        rootPath,
                                                                        BLOCK_SIZE));

            if (File.Exists(file.FilePath))
                File.Delete(file.FilePath);

            new FileInfo(file.FilePath).Directory.Create();

            var serializer = new BinaryFormatter();

            using (var stream = File.Open(file.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(Guid.Parse(Header).ToByteArray());

                    writer.Write(Version);

                    long hashPosition = stream.Position;

                    writer.Write(new byte[16]);

                    writer.Write(file.AddedPoints.Count);
                    writer.Write(file.AddedEvents.Length);
                    writer.Write(file.AddedConfigEvents.Length);

                    foreach (var addedPoint in file.AddedPoints)
                    {
                        serializer.Serialize(writer.BaseStream, addedPoint);
                        foreach (var blockFile in addedPoint.BlockFiles)
                        {
                            using (var blockStream = File.Open(blockFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                            {
                                writer.Write((int)blockStream.Length);
                                blockStream.CopyTo(stream);
                            }
                        }
                    }

                    //Запись для событий и конфигурационных событий
                    foreach (var blockFile in blockFiles)
                    {
                        using (var blockStream = File.Open(blockFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            writer.Write((int)blockStream.Length);
                            blockStream.CopyTo(stream);
                        }
                    }

                    stream.Seek(hashPosition + 16, SeekOrigin.Begin);

                    using (var md5 = MD5.Create())
                    {
                        var hash = md5.ComputeHash(stream);

                        stream.Seek(hashPosition, SeekOrigin.Begin);

                        writer.Write(hash);
                    }
                }
            }

            foreach (var blockFile in blockFiles)
            {
                File.Delete(blockFile);
            }
        }

        #endregion
    }
}
