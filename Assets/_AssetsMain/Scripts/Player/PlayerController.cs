using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerConstructor, IAnimationEntity, IMovementEntity
{
    [SerializeField] private Transform _modelTransform;
    
    private IPlayerBehaviour _playerBehaviour;
    public IPlayerBehaviour PlayerBehaviour => _playerBehaviour;

    public Transform MovementTransform => transform;
    public Transform RotationTransform => _modelTransform;

    public void Construct(IPlayerBehaviour playerBehaviour)
    {
        IPlayerAnimationController playerAnimationController = new PlayerAnimationController(this);
        playerBehaviour.PlayerAnimationController = playerAnimationController;

        IPlayerMovementController playerMovementController = new PlayerMovementController(this);
        playerMovementController.SetInitialPosition(playerBehaviour.InitialPosition);
        playerBehaviour.PlayerMovementController = playerMovementController;
    }

    private void OnDestroy() => PlayerBehaviour?.Dispose();
}

public interface IPlayerConstructor
{
    public void Construct(IPlayerBehaviour playerBehaviour);
}
