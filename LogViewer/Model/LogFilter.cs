using System;
using System.Text.RegularExpressions;

namespace LogViewer.Model
{
    /// <summary>
    /// ������ �����
    /// </summary>
    internal sealed class LogFilter : ILogFilter
    {
        /// <summary>
        /// ������ ������ ����
        /// </summary>
        private const string LogPattern = @"^((\d{2}\.\d{2}\.\d{2} \d{2}\:\d{2}\:\d{2}\.\d{3}) ([A-Z]) ([a-zA-Z0-9]+) .+)$";

        /// <summary>
        /// ��������� ��� ������� �����
        /// </summary>
        private readonly Regex _isLogLineRegex;

        /// <summary>
        /// ������� �������� 
        /// </summary>
        private Func<string, bool> _isMatchFilter;

        /// <summary>
        /// ������ �����
        /// </summary>
        /// <param name="timeFilter">����-�����</param>
        /// <param name="typeFilter">���</param>
        /// <param name="systemFilter">�������</param>
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
        /// True, ���� ������ ���� �������� ��� ������
        /// </summary>
        public bool IsMatch(string logEntry)
        {
            return _isMatchFilter(logEntry);
        }

        /// <summary>
        /// True, ���� ������ �������� ������(��� ������������) ������� ������ ����
        /// </summary>
        public bool IsLogHeadLine(string logLine)
        {
            return logLine != null && _isLogLineRegex.IsMatch(logLine);
        }
    }
}