namespace LogViewer.Model
{
    /// <summary>
    /// Абстракция фабрики для источников чтения логов
    /// </summary>
    public interface ILogReaderFactory
    {
        /// <summary>
        /// Возвращает новый источник чтения логов
        /// </summary>
        /// <param name="timeFilter">Фильтр по дате-времение</param>
        /// <param name="typeFilter">Фильтр по типу</param>
        /// <param name="systemFilter">Фильтр по системе</param>
        /// <param name="pageSize">Размер страницы поиска</param>
        ILogSource InitLogSource(string timeFilter, 
            string typeFilter, 
            string systemFilter,
            int pageSize);
    }
}