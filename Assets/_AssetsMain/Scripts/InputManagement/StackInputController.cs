using UnityBase.EventBus;
using UnityBase.Manager;
using UnityBase.Manager.Data;
using UnityBase.Service;
using UnityEngine;
using VContainer.Unity;

public class StackInputController : IStackInputController, IGameplayBootService, ITickable
{
    private readonly IStackContainer _stackContainer;
    private readonly IGameplayManager _gameplayManager;
    
    private readonly EventBinding<GameStateData> _gameStateEventBinding;

    private bool _isInputActivated;

    private IStackBehaviour _currentStackBehaviour;
    
    public StackInputController(IStackContainer stackContainer, IGameplayManager gameplayManager)
    {
        _stackContainer = stackContainer;

        _gameplayManager = gameplayManager;
        
        _gameStateEventBinding = new EventBinding<GameStateData>();
    }

    public void Initialize()
    {
        _gameStateEventBinding.Add(OnStartGameState);
        EventBus<GameStateData>.AddListener(_gameStateEventBinding, GameStateData.GetChannel(TransitionState.Start));
        
        _stackContainer.TryGetNextStack(out _currentStackBehaviour);
        _currentStackBehaviour.StackAnimationController.StartMovement(StartSide.Left);
    }
    
    public void Dispose()
    {
        _gameStateEventBinding.Remove(OnStartGameState);
        EventBus<GameStateData>.RemoveListener(_gameStateEventBinding, GameStateData.GetChannel(TransitionState.Start));
    }
    
    private void OnStartGameState(GameStateData gameStateData)
    {
        _isInputActivated = gameStateData is { StartState: GameState.GameLoadingState, EndState: GameState.GamePlayState };
    }

    public void Tick()
    {
        if(!_isInputActivated) return;

        if (Input.GetMouseButtonDown(0))
        {
            _currentStackBehaviour?.StackAnimationController?.StopMovement();
            var sliceCase = _currentStackBehaviour?.StackSliceController?.SliceObject();
            Debug.Log(sliceCase);
            
            if (_stackContainer.TryGetNextStack(out _currentStackBehaviour))
            {
                _currentStackBehaviour.StackAnimationController.StartMovement(StartSide.Left);
            }
            else
            {
                Debug.Log("Game Finished");
                //_gameplayManager.ChangeGameState(GameState.GameSuccessState, 1f);
            }
            
        }
    }
}

public interface IStackInputController
{
    
}
