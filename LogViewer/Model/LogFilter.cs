using System;
using System.Text.RegularExpressions;

namespace LogViewer.Model
{
    /// <summary>
    /// Фильтр логов
    /// </summary>
    internal sealed class LogFilter : ILogFilter
    {
        /// <summary>
        /// Шаблон записи лога
        /// </summary>
        private const string LogPattern = @"^((\d{2}\.\d{2}\.\d{2} \d{2}\:\d{2}\:\d{2}\.\d{3}) ([A-Z]) ([a-zA-Z0-9]+) .+)$";

        /// <summary>
        /// Выражение для разбора логов
        /// </summary>
        private readonly Regex _isLogLineRegex;

        /// <summary>
        /// Функция проверки 
        /// </summary>
        private Func<string, bool> _isMatchFilter;

        /// <summary>
        /// Фильтр логов
        /// </summary>
        /// <param name="timeFilter">Дата-время</param>
        /// <param name="typeFilter">Тип</param>
        /// <param name="systemFilter">Система</param>
        public LogFilter(string timeFilter, string typeFilter, string systemFilter)
        {
            _isLogLineRegex = new Regex(LogPattern);

            if (timeFilter == null && typeFilter == null && systemFilter == null)
            {
                _isMatchFilter = s => true;
                return;
            }

            if (timeFilter != null && typeFilter == null && systemFilter == null)
            {
                _isMatchFilter = s => s.StartsWith(timeFilter);
                return;
            }

            if (!string.IsNullOrEmpty(typeFilter) && typeFilter.Length > 1)
            {
                _isMatchFilter = s => false;
                return;
            }

            _isMatchFilter = s =>
            {
                var groups = _isLogLineRegex.Match(s).Groups;
                
                return groups[2].Value.StartsWith(timeFilter ?? string.Empty) &&
                       (string.IsNullOrEmpty(typeFilter) || groups[3].Value == typeFilter) &&
                       groups[4].Value.StartsWith(systemFilter ?? string.Empty);
            };
        }

        /// <summary>
        /// True, если запись лога подходит под фильтр
        /// </summary>
        public bool IsMatch(string logEntry)
        {
            return _isMatchFilter(logEntry);
        }

        /// <summary>
        /// True, если строка является первой(или единственной) строкой записи лога
        /// </summary>
        public bool IsLogHeadLine(string logLine)
        {
            return logLine != null && _isLogLineRegex.IsMatch(logLine);
        }
    }
}