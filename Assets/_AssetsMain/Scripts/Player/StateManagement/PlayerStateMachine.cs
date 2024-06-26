using UnityBase.EventBus;
using UnityBase.Manager;
using UnityBase.Manager.Data;
using UnityBase.Service;
using UnityBase.StateMachineCore;
using UnityEngine;
using VContainer.Unity;
using IState = UnityBase.StateMachineCore.IState;

public class PlayerStateMachine : IStateMachine, ITickable, IGameplayBootService
{
    private readonly IGameplayManager _gameplayManager;
    private readonly ICinemachineManager _cinemachineManager;
    
    private readonly IState _playerIdleState, _playerMoveState, _playerFailState, _playerSuccessState;
    private readonly EventBinding<GameStateData> _gameStateBinding = new EventBinding<GameStateData>(Priority.Normal);
    
    private IState _currentPlayerState;
    public IState CurrentPlayerState => _currentPlayerState;
    
    public PlayerStateMachine(IGameplayManager gameplayManager, IPlayerInitializer playerInitializer, ICinemachineManager cinemachineManager, ILevelManager levelManager)
    {
        _cinemachineManager = cinemachineManager;
        _cinemachineManager.ResetGameplayTarget(true);

        var currentLevelData = levelManager.GetCurrentLevelData();
        
        _playerIdleState = new PlayerIdleState();
        _playerMoveState = new PlayerMoveState(playerInitializer, _cinemachineManager, currentLevelData);
        _playerFailState = new PlayerFailState(gameplayManager);
        _playerSuccessState = new PlayerSuccessState(gameplayManager, cinemachineManager);

        ChangePlayerState(_playerIdleState);
    }

    public void Initialize()
    {
        _gameStateBinding.Add(OnGameplayStateChanged);
        EventBus<GameStateData>.AddListener(_gameStateBinding, GameStateData.GetChannel(TransitionState.Start));
        
        _playerMoveState.OnStateComplete += OnStateComplete;
    }
    
    public void Dispose()
    {
        _gameStateBinding.Remove(OnGameplayStateChanged);
        EventBus<GameStateData>.RemoveListener(_gameStateBinding, GameStateData.GetChannel(TransitionState.Start));
        
        _playerIdleState.Dispose();
        _playerMoveState.Dispose();
        _playerFailState.Dispose();
        _playerSuccessState.Dispose();
    }
    
    public void Tick() => CurrentPlayerState?.OnUpdate(Time.deltaTime);
    
    private void OnGameplayStateChanged(GameStateData gameStateData)
    {
        if (gameStateData is { StartState: GameState.GameLoadingState, EndState: GameState.GamePlayState })
        {
            ChangePlayerState(_playerMoveState);
        }
    }
    
    private void OnStateComplete()
    {
        switch (CurrentPlayerState)
        {
            case PlayerMoveState playerMoveState:
            {
                var nextPlayerState = playerMoveState.isLevelFinished ? _playerSuccessState : _playerFailState;
                ChangePlayerState(nextPlayerState);
                break;
            }
        }
    }
    
    public void ChangePlayerState(IState state)
    {
        if (_currentPlayerState != state)
        {
            _currentPlayerState?.OnExit();
            _currentPlayerState = state;
            _currentPlayerState.OnEnter();
        }
        else
        {
            Debug.LogError("You can not set same state!");
        }
    }
}