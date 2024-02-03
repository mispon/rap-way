using System;
using UnityEngine;

namespace UI.Base.Interfaces
{
    public interface IUIElementContainer: IDisposable
    {
        bool IsActive { get; }
    }
}
