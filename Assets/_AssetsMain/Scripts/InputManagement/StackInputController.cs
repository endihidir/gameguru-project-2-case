using System;
using UnityBase.EventBus;
using UnityBase.Manager;
using UnityBase.Manager.Data;
using UnityBase.Service;
using UnityEngine;
using VContainer.Unity;

public class StackDropController : IStackDropController, IGameplayBootService, ITickable
{
    private readonly IStackContainer _stackContainer;
    
    private readonly EventBinding<GameStateData> _gameStateEventBinding;

    private bool _isInputActivated;

    private IStackBehaviour _currentStackBehaviour;
    public event Action<Vector3> OnStackDropped;
    
    public StackDropController(IStackContainer stackContainer)
    {
        _stackContainer = stackContainer;
        _gameStateEventBinding = new EventBinding<GameStateData>();
    }

    public void Initialize()
    {
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
            _stackContainer.TryGetNextStack(out _currentStackBehaviour);
            _currentStackBehaviour.StackAnimationController.StartMovement();
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
            case CutCase.OutOfBounds:
                _isInputActivated = false;
               break;
            case CutCase.Cut or CutCase.PerfectFit:
                var droppedStackPosition = _currentStackBehaviour.StackInitializer.GetPos();
                OnStackDropped?.Invoke(droppedStackPosition);
                break;
        }
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

public interface IStackDropController
{
    public event Action<Vector3> OnStackDropped;
}
