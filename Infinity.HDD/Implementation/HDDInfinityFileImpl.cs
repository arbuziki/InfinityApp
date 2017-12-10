using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using Infinity.Data;
using Infinity.HDD.Utils;

namespace Infinity.HDD.Implementation
{
    [Serializable]
    public struct PointInfo
    {
        public string Item;
        public string Description;
        public string EngUnit;
        public int ValuesCount;

        [NonSerialized] public string[] BlockFiles;
        [NonSerialized] public string Folder;
    }

    /* ================================== ФОРМАТ ФАЙЛА ==================================
     * 
     * 16 байт - заголовок 
     * 4 байта - версия
     * 16 байт - MD5
     * 4 байта - количество HdaPoint
     * 4 байта - количество Events
     * 4 байта - количество ConfigEvents
     * -----------------------------------
     * Сериализованный заголовок точки
     * 4 байта - размер сжатого блока
     * СЖАТЫЙ БЛОК
     * -----------------------------------
     * 4 байта - размер сжатого блока
     * СЖАТЫЙ БЛОК
     * 
     * =================================================================================*/
    public class HDDInfinityFileImpl : IWriteableInfinityFile
    {


        private readonly HDDDataManager _owner;

        public string FilePath { get; private set; }
        public IEnumerable<IEvent> Events { get; internal set; }
        public IEnumerable<IHdaPoint> Points { get; internal set; }
        public IEnumerable<IConfigEventInfo> ConfigEvents { get; internal set; }

        internal HDDInfinityFileImpl(HDDDataManager owner, string filePath)
        {
            _owner = owner ?? throw new ArgumentNullException("owner");
            FilePath = filePath ?? throw new ArgumentNullException("filePath");

            AddedPoints = new List<PointInfo>();
            AddedEvents = new IEvent[0];
            AddedConfigEvents = new IConfigEventInfo[0];
        }

        #region Internal

        internal List<PointInfo> AddedPoints { get; private set; }
        internal IEvent[] AddedEvents { get; private set; }
        internal IConfigEventInfo[] AddedConfigEvents { get; private set; }

        #endregion



        public void AddPoint(IHdaPoint point)
        {
            if (point == null)
                throw new ArgumentNullException("point");

            var values = point.GetValues().ToList();

            if (values == null || !values.Any())
                throw new ArgumentException("Точка не содержит значений.");

            var fileGroupId = Guid.NewGuid();

            var rootPath = Path.GetTempPath() + Assembly.GetExecutingAssembly().GetName().Name + "\\" + fileGroupId.ToString() + "\\";

            new FileInfo(rootPath).Directory.Create();

            var blockFiles = CompressedBlocksUtils.SaveToFileByParts(values, "points", rootPath, HDDDataManager.BLOCK_SIZE);

            //Сохраняем заголовок (ссылку на саму точку не храним, ибо она очень много весит)

            AddedPoints.Add(new PointInfo
            {
                Folder = rootPath,
                Item = point.Item,
                Description = point.Description,
                EngUnit = point.EngUnit,
                BlockFiles = blockFiles,
                ValuesCount = values.Count
            });
        }

        public void AddEvents(IEvent[] events)
        {
            AddedEvents = events.ToArray();
        }

        public void AddConfigEvents(IConfigEventInfo[] configEvents)
        {
            AddedConfigEvents = configEvents.ToArray();
        }

        public void Save()
        {
            _owner.Save(this);
        }
    }
}
