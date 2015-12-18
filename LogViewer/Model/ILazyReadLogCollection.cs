using System;
using System.Collections.Generic;

namespace LogViewer.Model
{
    /// <summary>
    /// ���������� ������� ��������� ��� ������ �����
    /// </summary>
    public interface ILazyReadLogCollection : IEnumerable<string>, IDisposable
    {
        bool CanMoveNext();
    }
}