using System;
using System.Collections.Generic;
using System.Linq;

namespace LogViewer.Model
{
    /// <summary>
    /// Источник чтения логов по страницам
    /// </summary>
    /// <remarks>
    /// Использует объкект <see cref="LazyReadLogCollection"/> для чтения логов.
    /// Кэширует результаты. Позволяет преключаться между страницами, используя кэш.
    /// </remarks>
    internal sealed class LogSource : ILogSource
    {
        /// <summary>
        /// Кэш страниц поиска логов
        /// </summary>
        private readonly Dictionary<int, IEnumerable<string>> _cache; 

        /// <summary>
        /// Ленивая коллекция логов для чтения
        /// </summary>
        private readonly ILazyReadLogCollection _logCollection;

        /// <summary>
        /// Размер страницы
        /// </summary>
        private readonly int _pageSize;

        /// <summary>
        /// Номер текущей страницы
        /// </summary>
        private int _currentPage;

        /// <summary>
        /// Источник чтения логов по страницам
        /// </summary>
        /// <param name="logCollection">Ленивая коллекция логов</param>
        /// <param name="pageSize">Размер страницы</param>
        public LogSource(ILazyReadLogCollection logCollection, int pageSize)
        {
            if (logCollection == null) throw new ArgumentNullException(nameof(logCollection));
            _logCollection = logCollection;
            _pageSize = pageSize;
            _cache = new Dictionary<int, IEnumerable<string>>();
            _currentPage = 0;
        }

        /// <summary>
        /// True, если есть следующая страница поиска логов
        /// </summary>
        public bool CanMoveNext()
        {
            return _logCollection.CanMoveNext();
        }

        /// <summary>
        /// True, если есть предыдущая страница поиска логов
        /// </summary>
        public bool CanMovePrev()
        {
            return _currentPage > 1;
        }

        /// <summary>
        /// Возвращает следующую страницу поиска логов
        /// </summary>
        /// <remarks>
        /// Если в кэше есть искомая страница, то страницы восстанавливается из кэша.
        /// В ином случае произойдёт обращение к текущему источнику логов. Страница кжшируется.
        /// </remarks>
        public IEnumerable<string> NextPage()
        {
            // больше страниц нет
            if (!CanMoveNext())
            {
                return new string[] { };
            }

            IEnumerable<string> page;
            // страница была закэширована
            if (_cache.ContainsKey(_currentPage + 1))
            {
                page = _cache[_currentPage + 1];
            }
            else
            {
                // страница вычитывается из коллекции и кэшируется
                page = _logCollection.Take(_pageSize).ToList();
                _cache[_currentPage + 1] = page;
            }
            
            _currentPage++;
            return page;
        }

        /// <summary>
        /// Возвращает предыдущую страницу поиска логов
        /// </summary>
        /// <remarks>
        /// Восстанавливает предыдущую страницу относительно текущей из кэша
        /// </remarks>
        public IEnumerable<string> PreviousPage()
        {
            if (_cache.ContainsKey(_currentPage - 1))
            {
                var page = _cache[_currentPage - 1];
                _currentPage--;
                return page;
            }

            return new string[] {};
        }



        #region IDisposable Implementation

        /// <summary>
        /// True, если ресурсы уже освобождены
        /// </summary>
        private bool _isDisposed;

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _cache.Clear();
            _logCollection.Dispose();
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }

        #endregion

    }
}
