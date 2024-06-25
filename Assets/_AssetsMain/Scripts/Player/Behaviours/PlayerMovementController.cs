using DG.Tweening;
using UnityEngine;

public class PlayerMovementController : IPlayerMovementController
{
    private readonly IMovementEntity _movementEntity;

    private float _movementSpeed;
    
    private Tween _moveSidewayTween;

    public Transform PlayerTransform => _movementEntity.MovementTransform;
    public PlayerMovementController(IMovementEntity movementEntity) => _movementEntity = movementEntity;
    public void SetInitialPosition(Vector3 position) => _movementEntity.MovementTransform.position = position;

    public void MoveForward()
    {
        var moveTr = _movementEntity.MovementTransform;
        
        moveTr.position += moveTr.forward * (_movementSpeed * Time.deltaTime);
    }

    public void MoveSideways(float targetXPos, float duration)
    {
        if(_moveSidewayTween.IsActive()) return;
        
        var moveTr = _movementEntity.MovementTransform;
        
        _moveSidewayTween = moveTr.DOMoveX(targetXPos, duration).SetEase(Ease.InOutQuad);
    }
    
    public void SetMovementSpeed(float speed) => _movementSpeed = speed;
    public void Dispose() => _moveSidewayTween?.Kill();
}

public interface IPlayerMovementController
{
    public Transform PlayerTransform { get; }
    public void SetInitialPosition(Vector3 position);
    public void MoveForward();
    public void MoveSideways(float targetXPos, float duration);
    public void SetMovementSpeed(float speed);
    public void Dispose();
}

public interface IMovementEntity
{
    public Transform MovementTransform { get; }
    public Transform RotationTransform { get; }
}
