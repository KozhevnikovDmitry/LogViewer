using System;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LogViewer.Model;

namespace LogViewer.UI.ViewModel
{
    /// <summary>
    /// VM главного окна
    /// </summary>
    public sealed class MainVm : ViewModelBase
    {
        private readonly ILogReaderFactory _logReaderFactory;

        /// <summary>
        /// VM главного окна
        /// </summary>
        /// <param name="logReaderFactory">Фаюрика </param>
        public MainVm(ILogReaderFactory logReaderFactory)
        {
            if (logReaderFactory == null) throw new ArgumentNullException(nameof(logReaderFactory));
            _logReaderFactory = logReaderFactory;
            Logs = new ObservableCollection<string>();
        }


        #region Data Properties

        private string _timeFilter;

        /// <summary>
        /// Значение фильтра по дате-времени
        /// </summary>
        public string TimeFilter
        {
            get { return _timeFilter; }
            set
            {
                if (_timeFilter != value)
                {
                    _timeFilter = value;
                    RaisePropertyChanged(() => TimeFilter);
                }
            }
        }

        private string _typeFilter;

        /// <summary>
        /// Значение фильтра по типу сообщения
        /// </summary>
        public string TypeFilter
        {
            get { return _typeFilter; }
            set
            {
                if (_typeFilter != value)
                {
                    _typeFilter = value;
                    RaisePropertyChanged(() => TypeFilter);
                }
            }
        }

        private string _systemFilter;

        /// <summary>
        /// Значение фильтра по системе, к которой относитя сообщение
        /// </summary>
        public string SystemFilter
        {
            get { return _systemFilter; }
            set
            {
                if (_systemFilter != value)
                {
                    _systemFilter = value;
                    RaisePropertyChanged(() => SystemFilter);
                }
            }
        }

        private ObservableCollection<string> _logs;

        /// <summary>
        /// Отображаемая страница логов
        /// </summary>
        public ObservableCollection<string> Logs
        {
            get { return _logs; }
            set
            {
                if (_logs != value)
                {
                    _logs = value;
                    RaisePropertyChanged(() => Logs);
                }
            }
        }

        #endregion


        #region Commands

        /// <summary>
        /// Текущий источник чтения логов
        /// </summary>
        private ILogSource _currentLogSource;

        private RelayCommand _applyFiltersCommand;

        /// <summary>
        /// Команда применения набранных фильтров к логам
        /// </summary>
        public RelayCommand ApplyFiltersCommand
        {
            get
            {
                if (_applyFiltersCommand == null)
                    _applyFiltersCommand = new RelayCommand(ApplyFilters);
                return _applyFiltersCommand;
            }
        }

        /// <summary>
        /// Применяет набранные фильтры к логам и выводит первую страницу поиска.
        /// </summary>
        /// <remarks>
        /// Создаёт новый источник чтения логов по набранным фильтрам. Берёт первую страниу.
        /// </remarks>
        private void ApplyFilters()
        {
            try
            {
                _currentLogSource?.Dispose();
                _currentLogSource = _logReaderFactory.InitLogSource(TimeFilter, TypeFilter, SystemFilter, 10);
                NextPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private RelayCommand _nextPageCommand;

        /// <summary>
        /// Команда вывода следующей страницы поиска.
        /// </summary>
        public RelayCommand NextPageCommand
        {
            get
            {
                if (_nextPageCommand == null)
                    _nextPageCommand = new RelayCommand(NextPage, CanNextPage);
                return _nextPageCommand;
            }
        }

        /// <summary>
        /// Выводит следующую страницу поиска относительно текущей.
        /// </summary>
        private void NextPage()
        {
            try
            {
                Logs.Clear();
                var page = _currentLogSource.NextPage();
                Logs = new ObservableCollection<string>(page);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            RaiseCommandsExecuteChanged();
        }

        /// <summary>
        /// True, если источника позволяет перейти к следующей странице
        /// </summary>
        private bool CanNextPage()
        {
            try
            {
                return _currentLogSource != null && _currentLogSource.CanMoveNext();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }


        private RelayCommand _previousPageCommand;

        /// <summary>
        /// Команда вывода предыдущей страницы логов
        /// </summary>
        public RelayCommand PreviousPageCommand
        {
            get
            {
                if (_previousPageCommand == null)
                    _previousPageCommand = new RelayCommand(PreviousPage, CanPreviousPage);
                return _previousPageCommand;
            }
        }

        /// <summary>
        /// Выводит предыдущую страницу поиска
        /// </summary>
        private void PreviousPage()
        {
            try
            {
                Logs.Clear();
                Logs = new ObservableCollection<string>(_currentLogSource.PreviousPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            RaiseCommandsExecuteChanged();
        }

        /// <summary>
        /// True, если источника позволяет перейти к предыдущей странице
        /// </summary>
        private bool CanPreviousPage()
        {
            try
            {
                return _currentLogSource != null && _currentLogSource.CanMovePrev();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Оповещает View об изменении состояния команд погинации.
        /// </summary>
        private void RaiseCommandsExecuteChanged()
        {
            NextPageCommand.RaiseCanExecuteChanged();
            PreviousPageCommand.RaiseCanExecuteChanged();
        }

        #endregion
    }
}
