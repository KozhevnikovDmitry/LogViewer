namespace LogViewer.Model
{
    /// <summary>
    /// ���������� ������� ��� ���������� ������ �����
    /// </summary>
    public interface ILogReaderFactory
    {
        /// <summary>
        /// ���������� ����� �������� ������ �����
        /// </summary>
        /// <param name="timeFilter">������ �� ����-��������</param>
        /// <param name="typeFilter">������ �� ����</param>
        /// <param name="systemFilter">������ �� �������</param>
        /// <param name="pageSize">������ �������� ������</param>
        ILogSource InitLogSource(string timeFilter, 
            string typeFilter, 
            string systemFilter,
            int pageSize);
    }
}