using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Infinity.HDD.Utils
{
    public static class CompressedBlocksUtils
    {
        /// <summary>
        /// Сериализует переданные элементы в отдельные файлы со сжатием.
        /// </summary>
        public static string[] SaveToFileByParts(IList array, string fileName, string filePath, int itemsCountByPart)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            var result = new List<string>();

            if (array.Count == 0)
                return result.ToArray();

            int iterations = (int)Math.Ceiling(array.Count / (1.0 * itemsCountByPart));

            for (int i = 0; i < iterations; i++)
            {
                using (
                    var memStream = new MemoryStream())
                {
                    var serializer = new BinaryFormatter();

                    int start = itemsCountByPart * i;
                    int end = start + itemsCountByPart;

                    if (end > array.Count)
                        end = array.Count;

                    for (int p = start; p < end; p++)
                    {
                        serializer.Serialize(memStream, array[p]);
                    }

                    memStream.Seek(0, SeekOrigin.Begin);

                    var file = string.Format("{0}{1}_{2}.part", filePath, fileName, i);

                    result.Add(file);

                    using (var stream = File.Open(file,
                                                  FileMode.OpenOrCreate,
                                                  FileAccess.Write))
                    {
                        using (var compressStream = new GZipStream(stream, CompressionMode.Compress))
                        {
                            memStream.CopyTo(compressStream);
                        }
                    }
                }
            }

            return result.ToArray();
        }

        public static IList LoadFromParts(string fileName, string filePath, int totalItemsCount, int itemsCountByPart)
        {
            List<object> result = new List<object>();

            int iterations = (int)Math.Ceiling(totalItemsCount / (1.0 * itemsCountByPart));

            for (int i = 0; i < iterations; i++)
            {
                var file = string.Format("{0}{1}_{2}.part", filePath, fileName, i);

                using (var stream = File.Open(file, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    int start = itemsCountByPart * i;
                    int end = itemsCountByPart * i + itemsCountByPart;

                    if (end > itemsCountByPart)
                        end = itemsCountByPart;

                    var formatter = new BinaryFormatter();

                    for (int p = start; p < end; p++)
                    {
                        result.Add(formatter.Deserialize(stream));
                    }
                }
            }

            return result;
        }
    }
}
