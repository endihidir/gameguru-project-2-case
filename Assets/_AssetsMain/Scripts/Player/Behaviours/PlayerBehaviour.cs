using UnityEngine;

public class PlayerBehaviour : IPlayerBehaviour
{
    public IPlayerAnimationController PlayerAnimationController { get; set; }
    public IPlayerMovementController PlayerMovementController { get; set; }
    public void Initialize(Vector3 startPos)
    {
        PlayerMovementController.SetInitialPosition(startPos);
    }

    public void Dispose()
    {
        PlayerAnimationController?.Dispose();
        PlayerMovementController?.Dispose();
    }
}

public interface IPlayerBehaviour
{
    public void Initialize(Vector3 startPos);
    public IPlayerAnimationController PlayerAnimationController { get; set; }
    public IPlayerMovementController PlayerMovementController { get; set;}
    public void Dispose();
}