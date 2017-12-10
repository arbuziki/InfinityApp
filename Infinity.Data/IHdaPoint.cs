using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinity.Data
{
    public interface IHdaPoint
    {
        /// <summary>
        /// Полное имя тега, которому соответствует запись
        /// </summary>
        string Item { get; set; }
        /// <summary>
        /// Описание точки
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// Единица измерения
        /// </summary>
        string EngUnit { get; set; }
        /// <summary>
        /// Загрузить все значения из файла
        /// </summary>
        IEnumerable<HdaPointValue> GetValues();
    }
}
