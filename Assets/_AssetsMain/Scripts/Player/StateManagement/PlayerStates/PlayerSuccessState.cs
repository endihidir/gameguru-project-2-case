using System;
using UnityBase.Manager;
using UnityBase.Service;
using UnityBase.StateMachineCore;

public class PlayerSuccessState : IState
{
    public event Action OnStateComplete;
    
    private readonly IGameplayManager _gameplayManager;
    private readonly ICinemachineManager _cinemachineManager;

    public PlayerSuccessState(IGameplayManager gameplayManager, ICinemachineManager cinemachineManager)
    {
        _gameplayManager = gameplayManager;
        _cinemachineManager = cinemachineManager;
    }
    
    public void OnEnter()
    {
        _gameplayManager.ChangeGameState(GameState.GameSuccessState);
        
        _cinemachineManager.ResetGameplayTarget(false);
    }

    public void OnUpdate(float deltaTime)
    {
        _cinemachineManager.RotateGameplayTarget(30f, deltaTime);
    }

    public void OnExit()
    {
        
    }

    public void Dispose()
    {
        OnStateComplete = null;
    }
}