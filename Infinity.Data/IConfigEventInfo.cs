using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinity.Data
{
    public interface IConfigEventInfo
    {
        /// <summary>
        /// Время возникновения события
        /// </summary>
        DateTime TimeStamp { get; set; }
        /// <summary>
        /// Значение
        /// </summary>
        string Value { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// Тип события
        /// </summary>
        ConfigEventEnum Type { get; set; }
    }
}
