using UnityBase.EventBus;
using UnityBase.Manager;
using UnityBase.Manager.Data;
using UnityBase.Service;
using UnityBase.StateMachineCore;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using IState = UnityBase.StateMachineCore.IState;

public class PlayerStateMachine : IStateMachine, ITickable, IGameplayBootService
{
    [Inject]
    private readonly IGameplayManager _gameplayManager;
    private readonly ICinemachineManager _cinemachineManager;
    
    private readonly IState _playerIdleState, _playerMoveState, _playerFailState, _playerSuccessState;
    private readonly EventBinding<GameStateData> _gameStateBinding = new();
    
    private IState _currentPlayerState;
    public IState CurrentPlayerState => _currentPlayerState;
    
    public PlayerStateMachine(IPlayerInitializer playerInitializer, ICinemachineManager cinemachineManager, ILevelManager levelManager, IStackDropController stackDropController)
    {
        _cinemachineManager = cinemachineManager;
        _cinemachineManager.ResetGameplayTarget();
        var playerMovementSpeed = levelManager.GetCurrentLevelData().playerMovementSpeed;

        _playerIdleState = new PlayerIdleState();
        _playerMoveState = new PlayerMoveState(playerInitializer, _cinemachineManager, stackDropController, playerMovementSpeed);
        _playerFailState = new PlayerFailState();
        _playerSuccessState = new PlayerSuccessState();

        ChangePlayerState(_playerIdleState);
    }

    public void Initialize()
    {
        _gameStateBinding.Add(OnGameplayStateChanged);
        EventBus<GameStateData>.AddListener(_gameStateBinding, GameStateData.GetChannel(TransitionState.Start));
        
        CurrentPlayerState.OnStateComplete += OnCurrentPlayerStateComplete;
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
    
    private void OnCurrentPlayerStateComplete()
    {
        switch (CurrentPlayerState)
        {
            case PlayerMoveState playerMoveState:
            {
                var nextPlayerState = playerMoveState.isLevelFinished ? _playerSuccessState : _playerFailState;
                ChangePlayerState(nextPlayerState);
                break;
            }
            case PlayerFailState playerFailState:
                _gameplayManager.ChangeGameState(GameState.GameFailState, 1f);
                break;
            case PlayerSuccessState playerSuccessState:
                _gameplayManager.ChangeGameState(GameState.GameSuccessState, 1f);
                break;
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