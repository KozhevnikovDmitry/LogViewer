using System;

namespace LogViewer.Model
{
    /// <summary>
    /// ������� ���������� ������ �����
    /// </summary>
    internal sealed class LogReaderFactory : ILogReaderFactory
    {
        /// <summary>
        /// ��������� ������
        /// </summary>
        private readonly ReadLogOptions _readLogOptions;

        /// <summary>
        ///  ������� ���������� ������ �����
        /// </summary>
        public LogReaderFactory(string logPath, int readAmount)
        {
            if (logPath == null) throw new ArgumentNullException(nameof(logPath));
            _readLogOptions = new ReadLogOptions(logPath, readAmount);
        }

        /// <summary>
        /// ���������� ����� �������� ������ �����
        /// </summary>
        /// <param name="timeFilter">������ �� ����-��������</param>
        /// <param name="typeFilter">������ �� ����</param>
        /// <param name="systemFilter">������ �� �������</param>
        /// <param name="pageSize">������ �������� ������</param>
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