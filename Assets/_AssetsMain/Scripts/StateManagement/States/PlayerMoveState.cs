using System;
using System.Collections.Generic;
using UnityBase.Service;
using UnityBase.StateMachineCore;
using UnityEngine;

public class PlayerMoveState : IState
{
    public event Action OnStateComplete;

    private readonly IPlayerMovementController _playerMovementController;
    private readonly ICinemachineManager _cinemachineManager;
    private readonly IStackDropController _stackDropController;
    
    private List<Vector3> _stacksPositions;
    public bool isLevelFinished;
    private float _movementSpeed;

    public PlayerMoveState(IPlayerInitializer playerInitializer, ICinemachineManager cinemachineManager, IStackDropController stackDropController, float movementSpeed)
    {
        var playerBehaviour = playerInitializer.InitializePlayer();
        _playerMovementController = playerBehaviour.PlayerMovementController;
        _cinemachineManager = cinemachineManager;
        _cinemachineManager.SetGameplayTargetParent(_playerMovementController.PlayerTransform);
        _stackDropController = stackDropController;
        _movementSpeed = movementSpeed;
    }

    public void OnEnter()
    {
        _stackDropController.OnStackDropped += OnStackDropped;
        _playerMovementController.SetMovementSpeed(_movementSpeed);
    }

    public void OnUpdate(float deltaTime)
    {
        _playerMovementController.MoveForward();
    }

    public void OnExit()
    {
        _playerMovementController.SetMovementSpeed(0f);
        _stackDropController.OnStackDropped -= OnStackDropped;
    }

    public void Dispose()
    {
        OnStateComplete = null;
        _stacksPositions?.Clear();
    }
    private void OnStackDropped(Vector3 stackTransform) => _stacksPositions.Add(stackTransform);
}