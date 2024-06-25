using DG.Tweening;
using UnityBase.Extensions;
using UnityEngine;

public class StackSliceController : IStackSliceController
{
    private readonly IStackSliceEntity _stackSliceEntity;
    
    private IStackSliceEntity _previousStackSliceEntity;
    
    private Tween _resetTween;

    private Transform StackTransform => _stackSliceEntity.StackTransform;
    private Transform MeshTransform => _stackSliceEntity.StackMeshTransform;
    private Transform PieceTransform => _stackSliceEntity.PieceTransform;

    private Rigidbody StackRb => _stackSliceEntity.StackRigidBody;
    private Rigidbody PieceRb => _stackSliceEntity.PieceRigidBody;

    public IStackSliceEntity StackSliceEntity => _stackSliceEntity;

    public StackSliceController(IStackSliceEntity stackSliceEntity)
    {
        _stackSliceEntity = stackSliceEntity;
        
        ResetPiece();
    }

    public void SetPreviousSliceEntity(IStackSliceEntity previousStackSliceEntity)
    {
        _previousStackSliceEntity = previousStackSliceEntity;
    }

    public CutCase SliceObject(float fitThreshold = 0.1f)
    {
        if (_previousStackSliceEntity is null) return CutCase.None;
        
        var xDist = _previousStackSliceEntity.StackTransform.position.x - StackTransform.position.x;

        if (Mathf.Abs(xDist) >= _previousStackSliceEntity.StackMeshTransform.localScale.x)
        {
            StackRb.isKinematic = false;
            _resetTween = DOVirtual.DelayedCall(3f, ResetStack);
            return CutCase.OutOfBounds;
        }

        if (Mathf.Abs(xDist) <= _previousStackSliceEntity.StackMeshTransform.localScale.x * fitThreshold)
        {
            StackTransform.position = StackTransform.position.With(x: _previousStackSliceEntity.StackTransform.position.x);
            return CutCase.PerfectFit;
        }

        var side = xDist > 0 ? 1f : -1f;
        var cutSize = Mathf.Abs(xDist);
        var remainingSize = _previousStackSliceEntity.StackMeshTransform.localScale.x - cutSize;

        var stackXPos = StackTransform.position.x + side * (cutSize * 0.5f);
        StackTransform.position = StackTransform.position.With(x: stackXPos);
        MeshTransform.localScale = MeshTransform.localScale.With(x: remainingSize);
        
        var pieceXPos = StackTransform.position.x - side * (remainingSize + cutSize) * 0.5f;
        PieceTransform.position = StackTransform.position.With(x: pieceXPos, y: MeshTransform.position.y);
        PieceTransform.localScale = MeshTransform.localScale.With(x: cutSize);
        
        PieceTransform.gameObject.SetActive(true);
        PieceRb.isKinematic = false;

        _resetTween = DOVirtual.DelayedCall(3f, ResetPiece);
        
        return CutCase.Cut;
    }

    private void ResetStack()
    {
        StackRb.isKinematic = true;
        StackRb.gameObject.SetActive(false);
        StackTransform.localPosition = Vector3.zero;
    }

    private void ResetPiece()
    {
        PieceRb.isKinematic = true;
        PieceTransform.gameObject.SetActive(false);
        PieceTransform.localPosition = Vector3.zero;
    }

    public void Dispose() => _resetTween?.Kill();
}

public interface IStackSliceController
{
    public IStackSliceEntity StackSliceEntity { get; }
    public void SetPreviousSliceEntity(IStackSliceEntity previousStackSliceEntity);
    public CutCase SliceObject(float fitThreshold = 0.1f);
    public void Dispose();
}

public interface IStackSliceEntity
{
    public Transform StackTransform { get; }
    public Transform StackMeshTransform { get; }
    public Transform PieceTransform { get; }
    public Rigidbody StackRigidBody { get; }
    public Rigidbody PieceRigidBody { get; }
}

public enum CutCase
{
    None,
    OutOfBounds,
    PerfectFit,
    Cut
}