using System;
using System.Collections;
using System.Collections.Generic;

namespace Utils
{
    public sealed class CompositeDisposableImmediate : ICollection<IDisposable>, IDisposable
    {
        private readonly IList<IDisposable> _disposables;

        public int Count => _disposables.Count;

        public bool IsReadOnly => false;

        public CompositeDisposableImmediate()
        {
            _disposables = new List<IDisposable>();
        }

        public CompositeDisposableImmediate(IDisposable[] disposables)
        {
            _disposables = disposables;
        }

        public CompositeDisposableImmediate(IList<IDisposable> disposables)
        {
            _disposables = disposables;
        }

        public void Add(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        public void Remove(IDisposable disposable)
        {
            _disposables.Remove(disposable);
        }

        public void Dispose()
        {
            for (int i = 0; i < _disposables.Count; i++)
                _disposables[i]?.Dispose();
            Clear();
        }

        public void Clear()
        {
            _disposables.Clear();
        }

        public bool Contains(IDisposable item)
        {
            for (int i = 0; i < _disposables.Count; i++)
                if (_disposables == item)
                    return true;
            return false;
        }

        public void CopyTo(IDisposable[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        bool ICollection<IDisposable>.Remove(IDisposable item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IDisposable> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}