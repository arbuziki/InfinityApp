using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinity.HDD
{
    public interface IDataSaver
    {
        IWriteableInfinityFile CreateFile(string path);
    }
}
