using System.Collections.Generic;

namespace Utils
{
    public class MementoHistory<TElement> where TElement : class
    {
        private readonly List<TElement> _elementHistory = new List<TElement>();

        public TElement CurrentElement {get; private set;}

        public void AddNewElement(TElement newElement)
        {
            if (_elementHistory.Count > 0 && PeekLastElement() == newElement)
            {
                PopLastElement();
                CurrentElement = newElement;
                return;
            }
            
            if(CurrentElement is not null)
                _elementHistory.Add(CurrentElement);
            CurrentElement = newElement;
        }

        public TElement GetPreviousElement()
        {
            if (_elementHistory.Count == 0)
                return null;

            return PeekLastElement();
        }

        public bool RemoveFromHistory(TElement element)
        {
            if (_elementHistory.Contains(element) == false)
                return false;

            if (CurrentElement == element)
                CurrentElement = PopLastElement();

            return _elementHistory.Remove(element);
        }

        public void ClearHistory()
        {
            _elementHistory.Clear();
            CurrentElement = null;
        }

        private TElement PeekLastElement()
        {
            int count = _elementHistory.Count;
            if(count == 0)
                return null;
            return _elementHistory[count - 1];
        }

        private TElement PopLastElement()
        {
            int count = _elementHistory.Count;
            if (count == 0)
                return null;
            TElement element = _elementHistory[count - 1];
            _elementHistory.RemoveAt(count - 1);
            return element;
        }
    }
}