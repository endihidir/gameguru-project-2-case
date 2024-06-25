using System;
using UnityBase.Service;
using UnityBase.StateMachineCore;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerMoveState : IState
{
    public event Action OnStateComplete;

    private readonly IPlayerMovementController _playerMovementController;

    private readonly ICinemachineManager _cinemachineManager;

    public bool isLevelFinished;

    public PlayerMoveState(IPlayerInitializer playerInitializer, ICinemachineManager cinemachineManager)
    {
        var playerBehaviour = playerInitializer.InitializePlayer();
        
        _playerMovementController = playerBehaviour.PlayerMovementController;
        
        _cinemachineManager = cinemachineManager;
        
        _cinemachineManager.SetGameplayTargetParent(_playerMovementController.PlayerTransform);
    }
    
    public void OnEnter()
    {
        _playerMovementController.SetMovementSpeed(0f);
    }

    public void OnUpdate(float deltaTime)
    {
        _playerMovementController.MoveForward();

        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            var xPos = Random.Range(-3f, 3f);
            _playerMovementController.MoveSideways(xPos, 0.1f);
        }*/
    }

    public void OnExit()
    {
        _playerMovementController.SetMovementSpeed(0f);
    }

    public void Dispose()
    {
        OnStateComplete = null;
    }
}