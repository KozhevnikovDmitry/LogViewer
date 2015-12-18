namespace LogViewer.Model
{
    /// <summary>
    /// ���������� ������� �����
    /// </summary>
    public interface ILogFilter
    {
        /// <summary>
        /// True, ���� ������ ���� �������� ��� ������
        /// </summary>
        bool IsMatch(string logEntry);

        /// <summary>
        /// True, ���� ������ �������� ������(��� ������������) ������� ������ ����
        /// </summary>
        bool IsLogHeadLine(string logLine);
    }
}