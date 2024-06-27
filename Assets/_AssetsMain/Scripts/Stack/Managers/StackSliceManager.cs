using UnityBase.EventBus;
using UnityBase.Manager;
using UnityBase.Manager.Data;
using UnityBase.Service;
using UnityEngine;
using VContainer.Unity;

public class StackSliceManager : IStackSliceManager, IGameplayBootService, ITickable
{
    private readonly IStackContainer _stackContainer;
    
    private readonly EventBinding<GameStateData> _gameStateEventBinding;

    private bool _isInputActivated;

    private IStackBehaviour _currentStackBehaviour;
    
    public StackSliceManager(IStackContainer stackContainer)
    {
        _stackContainer = stackContainer;
        _gameStateEventBinding = new EventBinding<GameStateData>();
    }

    public void Initialize()
    {
        _stackContainer.TryGetNextStack(out _currentStackBehaviour);
        _currentStackBehaviour.StackAnimationController.StartMovement();
        _currentStackBehaviour.StackAnimationController.PauseMovement();
        
        _gameStateEventBinding.Add(OnStartGameState);
        EventBus<GameStateData>.AddListener(_gameStateEventBinding, GameStateData.GetChannel(TransitionState.Start));
    }
    
    public void Dispose()
    {
        _gameStateEventBinding.Remove(OnStartGameState);
        EventBus<GameStateData>.RemoveListener(_gameStateEventBinding, GameStateData.GetChannel(TransitionState.Start));
    }
    
    private void OnStartGameState(GameStateData gameStateData)
    {
        _isInputActivated = gameStateData is { StartState: GameState.GameLoadingState, EndState: GameState.GamePlayState };
        
        if (_isInputActivated)
        {
            _currentStackBehaviour.StackAnimationController.PlayMovement();
        }
    }

    public void Tick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SliceActiveStack();
            
            UpdateActiveStack();
        }
    }

    private void SliceActiveStack()
    {
        if(!_isInputActivated) return;

        _currentStackBehaviour?.StackAnimationController.StopMovement();
        
        var sliceCase = _currentStackBehaviour?.StackSliceController.SliceObject();

        switch (sliceCase)
        {
            case null:
                _isInputActivated = false;
                return;
            case SliceCase.None or SliceCase.OutOfBounds:
                _isInputActivated = false;
                break;
        }

        var settleData = new StackSettleData 
        { 
            stackIndex = _currentStackBehaviour.Index, 
            sliceCase = sliceCase.Value, 
            settledPos = _currentStackBehaviour.StackInitializer.GetPos() 
        };
        
        EventBus<StackSettleData>.Invoke(settleData);
    }

    private void UpdateActiveStack()
    {
        if(!_isInputActivated) return;
        
        if (_stackContainer.TryGetNextStack(out _currentStackBehaviour))
        {
            _currentStackBehaviour.StackAnimationController.StartMovement();
        }
        else
        {
            _isInputActivated = false;
        }
    }
}

public interface IStackSliceManager
{
   
}