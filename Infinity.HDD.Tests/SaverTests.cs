using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Infinity.Data;
using Infinity.HDD.Implementation;
using NUnit.Framework;

namespace Infinity.HDD.Tests
{
    [TestFixture]
    public class SaverTests
    {
        [Test]
        public void SaveOpenFile()
        {
            string filePath = "C:\\Users\\Evgeny\\Desktop\\test.bin";

            var saver = new HDDDataManager();

            var savedFile = saver.CreateFile(filePath);

            int moqPointsCount = 2; //Количество точек-заглушек
            int moqPointValuesCount = 1000000; //Количество значений, которые вернет точка-заглушка

            for (int i = 0; i < moqPointsCount; i++)
            {
                var moqPoint = new Moq.Mock<IHdaPoint>();

                moqPoint.SetupProperty(T => T.Item, $"TestPoint_{i}");
                moqPoint.SetupProperty(T => T.Description, $"TestDescription_{i}");
                moqPoint.SetupProperty(T => T.EngUnit, $"TestEngUnit_{i}");

                //
                moqPoint.Setup(T => T.GetValues()).Returns(() => GetTestValues(moqPointValuesCount));
                savedFile.AddPoint(moqPoint.Object);
            }

            savedFile.Save();

            
            var loader = new HDDDataManager();

            var loadedFile = loader.LoadFile(filePath);

            var firstPoint = loadedFile.Points.First();

            var values = firstPoint.GetValues();

            Assert.AreEqual(loadedFile.Points.Count(), moqPointsCount);
            Assert.AreEqual(values.Count(), moqPointValuesCount);
        }

        private IEnumerable<HdaPointValue> GetTestValues(int valuesCount)
        {
            var rnd = new Random();
            var result = new List<HdaPointValue>();

            for (int i = 0; i < valuesCount; i++)
            {
                result.Add(new HdaPointValue
                {
                    LocalTime = new DateTime(2017, 12, 10, 16, 10, 0),
                    UtcTime = new DateTime(2017, 12, 14, 16, 10, 0),
                    Value = 1.0 / rnd.Next(10000),
                    Quality = 0
                });
            }

            return result.ToArray();
        }
    }
}
