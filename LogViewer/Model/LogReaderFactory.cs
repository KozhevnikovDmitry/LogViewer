using System;

namespace LogViewer.Model
{
    /// <summary>
    /// Фабрика источников чтения логов
    /// </summary>
    internal sealed class LogReaderFactory : ILogReaderFactory
    {
        /// <summary>
        /// Настройки чтения
        /// </summary>
        private readonly ReadLogOptions _readLogOptions;

        /// <summary>
        ///  Фабрика источников чтения логов
        /// </summary>
        public LogReaderFactory(string logPath, int readAmount)
        {
            if (logPath == null) throw new ArgumentNullException(nameof(logPath));
            _readLogOptions = new ReadLogOptions(logPath, readAmount);
        }

        /// <summary>
        /// Возвращает новый источник чтения логов
        /// </summary>
        /// <param name="timeFilter">Фильтр по дате-времение</param>
        /// <param name="typeFilter">Фильтр по типу</param>
        /// <param name="systemFilter">Фильтр по системе</param>
        /// <param name="pageSize">Размер страницы поиска</param>
        public ILogSource InitLogSource(string timeFilter, 
                                        string typeFilter, 
                                        string systemFilter,
                                        int pageSize)
        {
            var filter = new LogFilter(timeFilter, typeFilter, systemFilter);
            var reader = new LazyReadLogCollection(_readLogOptions, filter).Start();
            return new LogSource(reader, pageSize);
        }
    }
}