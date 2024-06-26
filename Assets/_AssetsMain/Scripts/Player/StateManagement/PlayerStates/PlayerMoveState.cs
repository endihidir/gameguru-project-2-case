using System;
using System.Collections.Generic;
using System.Linq;
using UnityBase.EventBus;
using UnityBase.Managers.SO;
using UnityBase.Service;
using UnityBase.StateMachineCore;

public class PlayerMoveState : IState
{
    private const float INDEX_UPDATE_THRESHOLD = 0.3f;
    private const float LEVEL_FINISH_THRESHOLD = 0.65f;
    private const float FALL_DISTANCE_THRESHOLD = 0.65f;
    public event Action OnStateComplete;

    private readonly IPlayerMovementController _playerMovementController;
    private readonly IPlayerAnimationController _playerAnimationController;
    private readonly ICinemachineManager _cinemachineManager;
    private readonly EventBinding<StackSettleData> _stackSettleEventBinding = new();
    private List<StackSettleData> _stackSettleDatas = new();

    private StackSettleData _currentStackSettleData, _lastStackSettleData;
    private float _movementSpeed, _stackDepth;
    private int _indexCounter = 0, _totalStackCount;
    private bool _isIndexUpdated;
    
    public bool isLevelFinished;

    public PlayerMoveState(IPlayerInitializer playerInitializer, ICinemachineManager cinemachineManager, LevelSO currentLevelData)
    {
        var playerBehaviour = playerInitializer.InitializePlayer();
        _playerMovementController = playerBehaviour.PlayerMovementController;
        _playerAnimationController = playerBehaviour.PlayerAnimationController;
        
        _cinemachineManager = cinemachineManager;
        _cinemachineManager.SetGameplayTargetParent(_playerMovementController.PlayerTransform);
        
        _movementSpeed = currentLevelData.playerMovementSpeed;
        _stackDepth = currentLevelData.stackConfigSo.stackSize.z;
        _totalStackCount = currentLevelData.stackConfigSo.stackCount;
        
        _stackSettleEventBinding.Add(OnNewStackSettled);
        EventBus<StackSettleData>.AddListener(_stackSettleEventBinding);
    }

    public void OnEnter()
    {
        _indexCounter = 0;
        _currentStackSettleData = _stackSettleDatas.FirstOrDefault(x => x.stackIndex == _indexCounter);
        _lastStackSettleData = _stackSettleDatas.FirstOrDefault(x => x.stackIndex == (_totalStackCount - 1));
        _playerMovementController?.SetMovementSpeed(_movementSpeed);
        _playerAnimationController?.PlayRunAnim(_movementSpeed);
    }
    private void OnNewStackSettled(StackSettleData stackSettleData) => _stackSettleDatas.Add(stackSettleData);

    public void OnUpdate(float deltaTime)
    {
        _playerMovementController.MoveForward(deltaTime);

        var dist = CalculateDistanceToCurrentStack();

        if (CheckLevelFinished(dist))
        {
            _playerAnimationController.PlayDanceAnim();
            OnStateComplete?.Invoke();
            return;
        }

        if (ShouldUpdateIndex(dist))
        {
            if (TryUpdateIndex())
            {
                HandleLastStack(dist);
            }
            else
            {
                HandleNextStack(dist);
            }
        }
    }
    
    private bool TryUpdateIndex()
    {
        if (_isIndexUpdated)
        {
            return _indexCounter == _lastStackSettleData.stackIndex;
        }
        
        _isIndexUpdated = true;
        
        _indexCounter++;
        
        return _indexCounter == _lastStackSettleData.stackIndex;
    }

    private void HandleLastStack(float dist)
    {
        if (!isLevelFinished && dist > _stackDepth * LEVEL_FINISH_THRESHOLD)
        {
            isLevelFinished = true;
            _currentStackSettleData = _lastStackSettleData;
            _playerMovementController.MoveSideways(_currentStackSettleData.settledPos.x, 0.5f);
        }
    }

    private void HandleNextStack(float dist)
    {
        StackSettleData nextSettleData = _stackSettleDatas.FirstOrDefault(x => x.stackIndex == _indexCounter);

        if (nextSettleData.stackIndex == _indexCounter && nextSettleData.sliceCase != SliceCase.OutOfBounds)
        {
            _isIndexUpdated = false;
            _currentStackSettleData = nextSettleData;
            _playerMovementController.MoveSideways(_currentStackSettleData.settledPos.x, 0.5f);
        }
        else if (dist > _stackDepth * FALL_DISTANCE_THRESHOLD)
        {
            isLevelFinished = false;
            _cinemachineManager.ResetGameplayTarget(false);
            _playerMovementController.Fall();
            OnStateComplete?.Invoke();
        }
    }
    private float CalculateDistanceToCurrentStack() => _playerMovementController.PlayerTransform.position.z - _currentStackSettleData.settledPos.z;
    private bool CheckLevelFinished(float dist) => isLevelFinished && dist >= 0f;
    private bool ShouldUpdateIndex(float dist) => dist > _stackDepth * INDEX_UPDATE_THRESHOLD;
    public void OnExit() => _playerMovementController?.SetMovementSpeed(0f);

    public void Dispose()
    {
        _stackSettleEventBinding.Remove(OnNewStackSettled);
        EventBus<StackSettleData>.RemoveListener(_stackSettleEventBinding);
        OnStateComplete = null;
        _stackSettleDatas?.Clear();
    }
}