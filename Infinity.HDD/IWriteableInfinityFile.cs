using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinity.Data;

namespace Infinity.HDD
{
    public interface IWriteableInfinityFile : IInfinityFile
    {
        /// <param name="point"></param>
        void AddPoint(IHdaPoint point);

        void AddEvents(IEvent[] events);

        void AddConfigEvents(IConfigEventInfo[] configEvents);

        void Save();
    }
}
