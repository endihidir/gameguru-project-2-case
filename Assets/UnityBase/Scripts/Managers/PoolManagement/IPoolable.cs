using System;
using UnityEngine;

namespace UnityBase.Pool
{
    public interface IPoolable
    {
        public Component PoolableObject { get; }
        public bool IsActive { get; }
        public bool IsUnique { get; }
        public void Show(float duration = 0f, float delay = 0f, Action onComplete = default);
        public void Hide(float duration = 0f, float delay = 0f, Action onComplete = default);
    }
}