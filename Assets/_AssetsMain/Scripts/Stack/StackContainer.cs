using System.Collections.Generic;
using System.Linq;
using UnityBase.Extensions;
using UnityBase.Service;
using UnityEngine;

public class StackContainer : IStackContainer, IGameplayBootService
{
    private readonly IPoolManager _poolManager;
    private readonly ILevelManager _levelManager;
    private readonly List<StackController> _stackControllers = new();

    private IStackBehaviour[] _stackBehaviours;
    private StackConfigSO _stackConfigSo;
    public IStackBehaviour[] StackBehaviours => _stackBehaviours;
    
    public StackContainer(IPoolManager poolManager, ILevelManager levelManager)
    {
        _poolManager = poolManager;
        _levelManager = levelManager;
    }
    
    public void Initialize()
    {
        _stackConfigSo = _levelManager.GetCurrentLevelData()?.stackConfigSo;

        if (!_stackConfigSo) return;

        _stackBehaviours = new IStackBehaviour[_stackConfigSo.stackCount];
        
        for (int i = 0; i < _stackConfigSo.stackCount; i++) 
            _stackBehaviours[i] = new StackBehaviour(i);
        
        ConstructStack(_stackBehaviours[0], Vector3.zero, _stackConfigSo.stackSize);

        var lastStackZPos = (_stackConfigSo.stackCount - 1) * _stackConfigSo.stackSize.z;
        ConstructStack(_stackBehaviours[^1], Vector3.zero.With(z: lastStackZPos), _stackConfigSo.stackSize);
    }

    public void Dispose()
    {
        foreach (var stackController in _stackControllers)
        {
            _poolManager.HideObject(stackController);
        }
    }

    public bool TryGetNextStack(out IStackBehaviour stackBehaviour)
    {
        stackBehaviour = GetNextUnconstructedStack();

        if (stackBehaviour == null) return false;

        var previousStackBehaviour = _stackBehaviours[stackBehaviour.Index - 1];
        var position = CalculateNextStackPosition(stackBehaviour.Index, previousStackBehaviour.StackInitializer.GetPos().x);
        var scale = previousStackBehaviour.StackInitializer.GetScale();
        
        ConstructStack(stackBehaviour, position, scale);
        
        var previousStackSliceEntity = _stackBehaviours[stackBehaviour.Index - 1].StackSliceController.StackSliceEntity;
        stackBehaviour.StackSliceController.SetPreviousSliceEntity(previousStackSliceEntity);
        stackBehaviour.StackAnimationController.SetMovementDuration(_stackConfigSo.movementDuration);
        return true;
    }

    private void ConstructStack(IStackBehaviour stackBehaviour, Vector3 pos, Vector3 size)
    {
        var stackConstructor = InitializeStackConstructor();
        ConfigureStack(stackConstructor, stackBehaviour, pos, size);
        ShowStack(stackConstructor);
    }

    private void ConfigureStack(IStackConstructor stackConstructor, IStackBehaviour stackBehaviour, Vector3 pos, Vector3 size)
    {
        stackConstructor.Construct(stackBehaviour);
        stackBehaviour.StackInitializer.SetPos(pos);
        stackBehaviour.StackInitializer.SetScale(size);
        stackBehaviour.StackInitializer.SetColor(_stackConfigSo.stacks[stackBehaviour.Index].color);
    }
    private Vector3 CalculateNextStackPosition(int index, float xPos)
    {
        var zPos = index * _stackConfigSo.stackSize.z;
        return Vector3.zero.With(x: xPos, z: zPos);
    }

    private IStackConstructor InitializeStackConstructor()
    {
        var stackController = _poolManager.GetObject<StackController>();
        _stackControllers.Add(stackController);
        return stackController;
    }

    private void ShowStack(IStackConstructor stackConstructor) => stackConstructor.Show();
    private IStackBehaviour GetNextUnconstructedStack() => _stackBehaviours.FirstOrDefault(x => !x.IsConstructed);
}

public interface IStackContainer
{
    public IStackBehaviour[] StackBehaviours { get; }
    public bool TryGetNextStack(out IStackBehaviour stackBehaviour);
}