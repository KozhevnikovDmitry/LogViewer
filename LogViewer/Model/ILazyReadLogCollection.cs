using System;
using System.Collections.Generic;

namespace LogViewer.Model
{
    /// <summary>
    /// Абстракция ленивой коллекции для чтения логов
    /// </summary>
    public interface ILazyReadLogCollection : IEnumerable<string>, IDisposable
    {
        bool CanMoveNext();
    }
}