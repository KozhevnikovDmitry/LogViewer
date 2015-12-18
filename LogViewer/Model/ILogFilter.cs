namespace LogViewer.Model
{
    /// <summary>
    /// Абстракция фильтра логов
    /// </summary>
    public interface ILogFilter
    {
        /// <summary>
        /// True, если запись лога подходит под фильтр
        /// </summary>
        bool IsMatch(string logEntry);

        /// <summary>
        /// True, если строка является первой(или единственной) строкой записи лога
        /// </summary>
        bool IsLogHeadLine(string logLine);
    }
}