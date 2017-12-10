using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinity.Data
{
    public interface IEvent
    {
        /// <summary>
        /// Метка времени записи (по Гринвичу)
        /// </summary>
        DateTime UtcTime { get; set; }
        /// <summary>
        /// Метка времени записи (локальная)
        /// </summary>
        DateTime LocalTime { get; set; }
        /// <summary>
        /// Название источника события
        /// </summary>
        string Source { get; set; }
        /// <summary>
        /// Тип события
        /// </summary>
        EventTypeEnum EventType { get; set; }
        /// <summary>
        /// Название категории, которой принадлежит данное событие
        /// </summary>
        string Category { get; set; }
        /// <summary>
        /// Важность
        /// </summary>
        int Severity { get; set; }
        /// <summary>
        /// Условие. Имеет смысл только для событий типа "Condition"
        /// </summary>
        string Condition { get; set; }
        /// <summary>
        /// Постусловие. Имеет смысл только для событий типа "Condition"
        /// </summary>
        string SubCondition { get; set; }
        /// <summary>
        /// Текст сообщения, связанного с событием
        /// </summary>
        string Message { get; set; }
        /// <summary>
        /// Маска изменения состояния
        /// </summary>
        int ChangeMask { get; set; }
        /// <summary>
        /// Маска изменения состояния
        /// </summary>
        int NewState { get; set; }
        /// <summary>
        /// Качество
        /// </summary>
        int Quality { get; set; }
        /// <summary>
        /// Время возникновения события (по Гринвичу)
        /// </summary>
        DateTime ActiveTime { get; set; }
        /// <summary>
        /// Время возникновения события (локальная)
        /// </summary>
        DateTime LocalActiveTime { get; set; }
        /// <summary>
        /// Идентификатор события, используемый для квитирования
        /// </summary>
        int Cookie { get; set; }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        string ActorId { get; set; }
        /// <summary>
        /// Динамические аттрибуты события
        /// </summary>
        IEnumerable<IEventAttribute> Attributes { get; set; }
        /// <summary>
        /// Значение
        /// </summary>
        string Value { get; set; }
        /// <summary>
        /// Комментарий квитирования
        /// </summary>
        string Ackcomment { get; set; }
        /// <summary>
        /// Единица измерения
        /// </summary>
        string Unit { get; set; }
        /// <summary>
        /// Ответственный
        /// </summary>
        string Responsible { get; set; }

    }
}
