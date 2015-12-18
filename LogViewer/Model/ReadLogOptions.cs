namespace LogViewer.Model
{
    /// <summary>
    /// Настройки чтения логов
    /// </summary>
    public sealed class ReadLogOptions
    {
        public ReadLogOptions(string logPath, int readAmount)
        {
            LogPath = logPath;
            ReadAmount = readAmount;
        }

        /// <summary>
        /// Путь к файлу с логами
        /// </summary>
        public string LogPath { get; }
        
        /// <summary>
        /// Объём чтения логов из файла
        /// </summary>
        public int ReadAmount { get; }
    }
}