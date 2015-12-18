namespace LogViewer.Model
{
    /// <summary>
    /// ��������� ������ �����
    /// </summary>
    public sealed class ReadLogOptions
    {
        public ReadLogOptions(string logPath, int readAmount)
        {
            LogPath = logPath;
            ReadAmount = readAmount;
        }

        /// <summary>
        /// ���� � ����� � ������
        /// </summary>
        public string LogPath { get; }
        
        /// <summary>
        /// ����� ������ ����� �� �����
        /// </summary>
        public int ReadAmount { get; }
    }
}