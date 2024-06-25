using DG.Tweening;
using UnityBase.Extensions;
using UnityEngine;

public class StackAnimationController : IStackAnimationController
{
    private IStackAnimationEntity _stackAnimationEntity;
    private float StackWidth => _stackAnimationEntity.StackMeshTransform.localScale.x;

    private Tween _moveTween;

    private float _movementDuration;

    public StackAnimationController(IStackAnimationEntity stackAnimationEntity)
    {
        _stackAnimationEntity = stackAnimationEntity;
    }

    public void SetMovementDuration(float duration) => _movementDuration = duration;

    public void StartMovement(StartSide startSide, Ease ease = Ease.InOutQuad, float offset = 0.5f)
    {
        var transform = _stackAnimationEntity.StackTransform;
        
        var xDir = startSide == StartSide.Left ? -1 : 1;

        var startPos = transform.position.x;
        var startOffset = xDir * (StackWidth + offset);
        var xPos = startPos + startOffset;
        
        transform.position = transform.position.With(x: xPos);
        
        _moveTween = transform.DOMoveX(startPos - startOffset, _movementDuration)
                              .SetEase(ease)
                              .SetLoops(-1, LoopType.Yoyo);
    }

    public void StopMovement()
    {
        _moveTween?.Kill();
    }

    public void Dispose() => StopMovement();
}

public interface IStackAnimationController
{
    public void SetMovementDuration(float duration);
    public void StartMovement(StartSide startSide, Ease ease = Ease.InOutQuad, float offset = 0.5f);
    public void StopMovement();
    public void Dispose();
}

public interface IStackAnimationEntity
{
    public Transform StackTransform { get; }
    public Transform StackMeshTransform { get; }
}

public enum StartSide
{
    Left,
    Right
}