using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinity.Data
{
    public interface IEventAttribute
    {
        /// <summary>
        /// Название атрибута
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Название категории, которой принадлежит атрибут
        /// </summary>
        string Category { get; set; }
        /// <summary>
        /// Значение
        /// </summary>
        object Value { get; set; }
    }
}
