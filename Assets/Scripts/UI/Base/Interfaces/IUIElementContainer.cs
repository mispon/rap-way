using System;
using UnityEngine;

namespace UI.Base.Interfaces
{
    public interface IUIElementContainer: IDisposable
    {
        Canvas Canvas { get; }
        bool IsActive { get; }
    }
}
