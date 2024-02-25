using System;

namespace UI.Base.Interfaces
{
    public interface IUIElement : IDisposable
    {
        void Show(object ctx);
        void Hide();
    }
}