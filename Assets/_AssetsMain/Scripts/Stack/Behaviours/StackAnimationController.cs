using DG.Tweening;
using UnityBase.Extensions;
using UnityEngine;

public class StackAnimationController : IStackAnimationController
{
    private IStackAnimationEntity _stackAnimationEntity;
    private float StackWidth => _stackAnimationEntity.StackMeshTransform.localScale.x;
    
    private float _movementDuration;

    private StartSide _movementStartSide;
    
    private Tween _moveTween;

    public StackAnimationController(IStackAnimationEntity stackAnimationEntity)
    {
        _stackAnimationEntity = stackAnimationEntity;
    }

    public void SetMovementDuration(float duration) => _movementDuration = duration;
    public void SetMovementStartSide(StartSide startSide) => _movementStartSide = startSide;

    public void StartMovement(Ease ease = Ease.InOutQuad, float offset = 0.5f)
    {
        var transform = _stackAnimationEntity.StackTransform;
        
        var xDir = _movementStartSide == StartSide.Left ? -1 : 1;

        var startPos = transform.position.x;
        var startOffset = xDir * (StackWidth + offset);
        var xPos = startPos + startOffset;
        
        transform.position = transform.position.With(x: xPos);
        
        _moveTween = transform.DOMoveX(startPos - startOffset, _movementDuration)
                              .SetEase(ease)
                              .SetLoops(-1, LoopType.Yoyo);
    }
    public void PlayMovement() => _moveTween?.Play();
    public void PauseMovement() => _moveTween?.Pause();
    public void StopMovement() => _moveTween?.Kill();
    public void Reset() => StopMovement();
    public void Dispose() => StopMovement();
}

public interface IStackAnimationController
{
    public void SetMovementDuration(float duration);
    public void SetMovementStartSide(StartSide startSide);
    public void StartMovement(Ease ease = Ease.InOutQuad, float offset = 0.5f);
    public void PauseMovement();
    public void PlayMovement();
    public void StopMovement();
    public void Reset();
    public void Dispose();
}

public interface IStackAnimationEntity
{
    public Transform StackTransform { get; }
    public Transform StackMeshTransform { get; }
}

public enum StartSide
{
    Idle,
    Left,
    Right
}