using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerConstructor, IAnimationEntity, IMovementEntity
{
    [SerializeField] private Transform _modelTransform;
    [SerializeField] private Animator _animator;
    
    private IPlayerBehaviour _playerBehaviour;
    public IPlayerBehaviour PlayerBehaviour => _playerBehaviour;

    public Animator Animator => _animator;
    public Transform MovementTransform => transform;
    public Transform RotationTransform => _modelTransform;

    public void Construct(IPlayerBehaviour playerBehaviour)
    {
        IPlayerAnimationController playerAnimationController = new PlayerAnimationController(this);
        playerBehaviour.PlayerAnimationController = playerAnimationController;

        IPlayerMovementController playerMovementController = new PlayerMovementController(this);
        playerBehaviour.PlayerMovementController = playerMovementController;
    }

    private void OnDestroy() => PlayerBehaviour?.Dispose();
}

public interface IPlayerConstructor
{
    public void Construct(IPlayerBehaviour playerBehaviour);
}
