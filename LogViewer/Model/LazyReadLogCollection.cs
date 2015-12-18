using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LogViewer.Model
{
    /// <summary>
    /// Ленивой коллекции для чтения логов из файла по заданным фильтрам
    /// </summary>
    internal sealed class LazyReadLogCollection : ILazyReadLogCollection
    {
        /// <summary>
        /// Настройки чтения
        /// </summary>
        private readonly ReadLogOptions _logOptions;

        /// <summary>
        /// Фильтр логов
        /// </summary>
        private readonly ILogFilter _logFilter;

        /// <summary>
        /// Сырые логи построчно
        /// </summary>
        private readonly BlockingCollection<string> _rawLogs;

        /// <summary>
        /// Склееные многострочные логи
        /// </summary>
        private readonly BlockingCollection<string> _stuckLogs;

        /// <summary>
        /// Фильтрованные логи
        /// </summary>
        private readonly BlockingCollection<string> _filteredLogs;

        /// <summary>
        /// Ридер файла логов
        /// </summary>
        private StreamReader _reader;

        private readonly CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Ленивой коллекции для чтения логов из файла по заданным фильтрам
        /// </summary>
        /// <param name="logOptions">Настройки чтения</param>
        /// <param name="logFilter">Фильтр логов</param>
        public LazyReadLogCollection(ReadLogOptions logOptions, ILogFilter logFilter)
        {
            if (logOptions == null) throw new ArgumentNullException(nameof(logOptions));
            if (logFilter == null) throw new ArgumentNullException(nameof(logFilter));
            _logOptions = logOptions;
            _logFilter = logFilter;
            _rawLogs = new BlockingCollection<string>(new ConcurrentQueue<string>());
            _stuckLogs = new BlockingCollection<string>(new ConcurrentQueue<string>());
            _filteredLogs = new BlockingCollection<string>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Запускает чтение файла
        /// </summary>
        public LazyReadLogCollection Start()
        {
            // файл должен быть
            if (!File.Exists(_logOptions.LogPath))
            {
                throw new FileNotFoundException($"Log file {_logOptions.LogPath} is not found", _logOptions.LogPath);
            }

            // инициализация ридера
            _reader = new StreamReader(new FileStream(_logOptions.LogPath, FileMode.Open, FileAccess.Read, FileShare.Read));

            // запуск циклов чтения и обработки
            ReadLoop(_cancellationTokenSource.Token);
            StickLoop(_cancellationTokenSource.Token);
            FilterLoop(_cancellationTokenSource.Token);
            return this;
        }

        /// <summary>
        /// True, если можно читать логи дальше
        /// </summary>
        /// <returns></returns>
        public bool CanMoveNext()
        {
            return !(_rawLogs.Count == 0 && _filteredLogs.Count == 0);
        }

        /// <summary>
        /// Цикл построчного чтения логов
        /// </summary>
        private void ReadLoop(CancellationToken token)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    while (!_reader.EndOfStream)
                    {
                        if (token.IsCancellationRequested)
                        {
                            return;
                        }

                        if (_rawLogs.Count >= _logOptions.ReadAmount)
                        {
                            continue;
                        }
                        _rawLogs.Add(_reader.ReadLine(), token);
                    }
                }
                catch (ObjectDisposedException)
                {

                }
            }, token);
        }

        /// <summary>
        /// Цикл склеивания записей логов
        /// </summary>
        private void StickLoop(CancellationToken token)
        {
            Task.Factory.StartNew(() =>
            {
                string logEntry = string.Empty;
                while (true)
                {
                    try
                    {
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        if (_stuckLogs.Count >= _logOptions.ReadAmount)
                        {
                            continue;
                        }
                        
                        string logLine;
                        if (!_rawLogs.TryTake(out logLine)) continue;

                        if (_logFilter.IsLogHeadLine(logLine))
                        {
                            _stuckLogs.Add(logEntry, token);
                            logEntry = logLine;
                        }
                        else
                        {
                            logEntry += $"{Environment.NewLine}{logLine}";
                        }
                    }
                    catch (ObjectDisposedException)
                    {

                    }
                }
            }, token);
        }

        /// <summary>
        /// Цикл фильтрации записей логов
        /// </summary>
        private void FilterLoop(CancellationToken token)
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        if (_filteredLogs.Count >= _logOptions.ReadAmount)
                        {
                            continue;
                        }

                        string logEntry;
                        if (!_stuckLogs.TryTake(out logEntry)) continue;

                        if (_logFilter.IsMatch(logEntry))
                        {
                            _filteredLogs.Add(logEntry, token);
                        }
                    }
                    catch (ObjectDisposedException)
                    {

                    }
                }
            }, token);
        }


        #region IEnumerable<string> Implementation

        public IEnumerator<string> GetEnumerator()
        {
            while (true)
            {
                if (_rawLogs.Count == 0)
                {
                    break;
                }

                string result;
                if (_filteredLogs.TryTake(out result))
                {
                    yield return result;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion


        #region IDisposable Implementation

        private bool _isDisposed;

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _cancellationTokenSource.Cancel();
            _rawLogs.Dispose();
            _stuckLogs.Dispose();
            _filteredLogs.Dispose();
            _reader.Dispose();
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }

        #endregion

    }
}