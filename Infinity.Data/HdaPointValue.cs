using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinity.Data
{
    [Serializable]
    public class HdaPointValue
    {
        /// <summary>
        /// Метка времени записи (по Гринвичу)
        /// </summary>
        public DateTime UtcTime { get; set; }

        /// <summary>
        /// Метка времени записи (локальная)
        /// </summary>
        public DateTime LocalTime { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Качество
        /// </summary>
        public int Quality { get; set; }
    }
}
