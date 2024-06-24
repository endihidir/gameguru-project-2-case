using UnityBase.Service;
using VContainer.Unity;

public class StackContainer : IStackContainer, ITickable, IGameplayBootService
{
    private readonly IPoolManager _poolManager;
    
    public StackContainer(IPoolManager poolManager)
    {
        _poolManager = poolManager;
    }
    
    public void Initialize()
    {
        
    }

    public void Dispose()
    {
        
    }
    
    public void Tick()
    {
        
    }
}

public interface IStackContainer
{
    
}
