public class PlayerAnimationController : IPlayerAnimationController
{
    public PlayerAnimationController(IAnimationEntity animationEntity)
    {
        
    }

    public void Dispose()
    {
        
    }
}

public interface IPlayerAnimationController
{
    public void Dispose();
}

public interface IAnimationEntity
{
    
}
