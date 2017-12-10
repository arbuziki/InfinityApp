using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinity.Data;

namespace Infinity.HDD
{
    public interface IInfinityFile
    {
        string FilePath { get; }
        IEnumerable<IEvent> Events { get; }
        IEnumerable<IHdaPoint> Points { get; }
        IEnumerable<IConfigEventInfo> ConfigEvents { get; }
    }
}
