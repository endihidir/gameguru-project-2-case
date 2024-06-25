using System;
using UnityBase.Pool;
using UnityEngine;

public class StackController : MonoBehaviour, IStackConstructor, IStackInitEntity, IStackSliceEntity, IStackAnimationEntity
{
    [SerializeField] private Transform _stackMeshT;
    [SerializeField] private Transform _stackPiece;

    [SerializeField] private MeshRenderer _stackMeshRenderer, _pieceMeshRenderer;

    [SerializeField] private Rigidbody _stackRb, _pieceRb;

    private IStackBehaviour _stackBehaviour;
    public Component PoolableObject => this;
    public bool IsActive => isActiveAndEnabled;
    public bool IsUnique => false;

    public IStackBehaviour StackBehaviour => _stackBehaviour;
    public Transform StackTransform => transform;
    public Transform StackMeshTransform => _stackMeshT;
    public Transform PieceTransform => _stackPiece;
    public Rigidbody StackRigidBody => _stackRb;
    public Rigidbody PieceRigidBody => _pieceRb;
    public MeshRenderer StackMeshRenderer => _stackMeshRenderer;
    public MeshRenderer StackPieceMeshRenderer => _pieceMeshRenderer;

    public void Construct(IStackBehaviour stackBehaviour)
    {
        _stackBehaviour = stackBehaviour;
        _stackBehaviour.StackInitializer = new StackInitializer(this);
        _stackBehaviour.StackSliceController = new StackSliceController(this);
        _stackBehaviour.StackAnimationController = new StackAnimationController(this);
        _stackBehaviour.IsConstructed = true;
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

    private void OnDestroy()
    {
        _stackBehaviour?.Dispose();
        _stackBehaviour = null;
    }
}

public interface IStackConstructor : IPoolable
{
    public void Construct(IStackBehaviour stackBehaviour);
}