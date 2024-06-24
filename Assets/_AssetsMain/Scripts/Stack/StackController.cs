using System;
using UnityBase.Extensions;
using UnityBase.Pool;
using UnityEngine;

public class StackController : MonoBehaviour, IPoolable
{
    [SerializeField] private Transform _meshT;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;
    public float Width => _meshT.localScale.x;
    public Component PoolableObject => this;
    public bool IsActive => isActiveAndEnabled;
    public bool IsUnique => false;
    
    public void Initialize(Vector3 defaultScale)
    {
        _meshT.localScale = defaultScale;
    }
    public void Show(float duration, float delay, Action onComplete)
    {
        gameObject.SetActive(true);
        onComplete?.Invoke();
    }

    public void Hide(float duration, float delay, Action onComplete)
    {
        gameObject.SetActive(false);
        onComplete?.Invoke();
    }
    
    public CutCase CutObject(StackController previousStack, float fitThreshold = 0.2f)
    {
        var xDist = previousStack.transform.position.x - transform.position.x;
        
        if (Mathf.Abs(xDist) >= previousStack.Width) return CutCase.OutOfBounds;

        if (Mathf.Abs(xDist) <= fitThreshold)
        {
            transform.position = transform.position.With(x: previousStack.transform.position.x);
            return CutCase.PerfectFit;
        }

        var side = xDist > 0 ? 1f : -1f;
        var cutSize = Mathf.Abs(xDist);
        var remainingSize = previousStack.Width - cutSize;
        
        transform.position = transform.position.With(x: transform.position.x + side * (cutSize * 0.5f));
        _meshT.localScale = _meshT.localScale.With(x: remainingSize);
        
        /*GameObject cutPiece = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cutPiece.transform.localScale = _meshT.localScale.With(x: cutSize);
        cutPiece.transform.position = transform.position.With(x: transform.position.x - side * (remainingSize + cutSize) * 0.5f, y: _meshT.position.y);*/
        
        return CutCase.Cut;
    }
}


public enum CutCase
{
    OutOfBounds,
    PerfectFit,
    Cut
}