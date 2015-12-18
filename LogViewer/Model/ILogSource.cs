using System;
using System.Collections.Generic;

namespace LogViewer.Model
{
    /// <summary>
    /// Абстракция источника чтения логов по страницам
    /// </summary>
    public interface ILogSource : IDisposable
    {
        /// <summary>
        /// True, если есть следующая страница поиска логов
        /// </summary>
        bool CanMoveNext();

        /// <summary>
        /// True, если есть предыдущая страница поиска логов
        /// </summary>
        bool CanMovePrev();

        /// <summary>
        /// Возвращает следующую страницу поиска логов
        /// </summary>
        IEnumerable<string> NextPage();

        /// <summary>
        /// Возвращает предыдущую страницу поиска логов
        /// </summary>
        IEnumerable<string> PreviousPage();
    }
}