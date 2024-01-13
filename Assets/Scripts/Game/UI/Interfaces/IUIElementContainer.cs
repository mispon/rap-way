using System;
using UnityEngine;

namespace Game.UI.Interfaces
{
    public interface IUIElementContainer: IDisposable
    {
        Canvas Canvas { get; }
        bool IsActive { get; }
    }
}
