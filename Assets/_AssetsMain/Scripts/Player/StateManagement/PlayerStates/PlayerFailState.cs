using System;
using UnityBase.Manager;
using UnityBase.Service;
using UnityBase.StateMachineCore;

public class PlayerFailState : IState
{
    public event Action OnStateComplete;
    
    private readonly IGameplayManager _gameplayManager;

    public PlayerFailState(IGameplayManager gameplayManager) => _gameplayManager = gameplayManager;
    public void OnEnter() => _gameplayManager.ChangeGameState(GameState.GameFailState, 1f);

    public void OnUpdate(float deltaTime) { }

    public void OnExit()
    {
        
    }

    public void Dispose()
    {
        OnStateComplete = null;
    }
}