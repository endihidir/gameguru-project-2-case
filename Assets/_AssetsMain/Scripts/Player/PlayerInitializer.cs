using UnityBase.Service;
using UnityEngine;

public class PlayerInitializer : IPlayerInitializer, IGameplayBootService
{
    private readonly IPlayerConstructor _playerConstructor;
    public PlayerInitializer(IPlayerConstructor playerConstructor) => _playerConstructor = playerConstructor;

    public void Initialize() { }
    public void Dispose() { }

    public IPlayerBehaviour InitializePlayer()
    {
        IPlayerBehaviour playerBehaviour = new PlayerBehaviour();
        
        _playerConstructor.Construct(playerBehaviour);

        playerBehaviour.Initialize(Vector3.zero);
        
        return playerBehaviour;
    }
}

public interface IPlayerInitializer
{
    public IPlayerBehaviour InitializePlayer();
}