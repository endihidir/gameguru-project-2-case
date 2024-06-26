using System.Linq;
using UnityBase.EventBus;
using UnityBase.Extensions;
using UnityBase.Service;
using UnityEngine;
using Object = UnityEngine.Object;

public class StackManager : IStackContainer, IGameplayBootService
{
    private readonly IPoolManager _poolManager;
    private readonly ILevelManager _levelManager;

    private IStackBehaviour[] _stackBehaviours;
    private StackConfigSO _stackConfigSo;
    private Tag_StacksParent _stacksParent;
    
    public StackManager(IPoolManager poolManager, ILevelManager levelManager)
    {
        _poolManager = poolManager;
        _levelManager = levelManager;
        _stacksParent = Object.FindObjectOfType<Tag_StacksParent>();
    }
    
    public void Initialize()
    {
        _stackConfigSo = _levelManager.GetCurrentLevelData()?.stackConfigSo;

        if (!_stackConfigSo) return;

        _stackBehaviours = new IStackBehaviour[_stackConfigSo.stackCount];
        
        for (int i = 0; i < _stackConfigSo.stackCount; i++) 
            _stackBehaviours[i] = new StackBehaviour(i);
        
        ConstructStack(_stackBehaviours[0], Vector3.zero, _stackConfigSo.stackSize);
        InvokeStackSettleData(_stackBehaviours[0]);

        var lastStackZPos = (_stackConfigSo.stackCount - 1) * _stackConfigSo.stackSize.z;
        ConstructStack(_stackBehaviours[^1], Vector3.zero.With(z: lastStackZPos), _stackConfigSo.stackSize);
        InvokeStackSettleData(_stackBehaviours[^1]);
    }

    private void InvokeStackSettleData(IStackBehaviour stackBehaviour)
    {
        var firstStackSettleData = new StackSettleData 
        { 
            stackIndex = stackBehaviour.Index, 
            sliceCase = SliceCase.NotSliceable, 
            settledPos = stackBehaviour.StackInitializer.GetPos() 
        };

        EventBus<StackSettleData>.Invoke(firstStackSettleData);
    }

    public void Dispose() => _poolManager?.HideAllObjectsOfType<StackEntityController>();

    public bool TryGetNextStack(out IStackBehaviour stackBehaviour)
    {
        stackBehaviour = GetNextUnConstructedStack();

        if (stackBehaviour == null) return false;

        var stackIndex = stackBehaviour.Index;
        var previousStackBehaviour = _stackBehaviours[stackIndex - 1];
        var position = CalculateNextStackPosition(stackIndex, previousStackBehaviour.StackInitializer.GetPos().x);
        var scale = previousStackBehaviour.StackInitializer.GetScale();
        ConstructStack(stackBehaviour, position, scale);
        SetPreviousStackSliceEntity(stackBehaviour, previousStackBehaviour);
        
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
        var stackIndex = stackBehaviour.Index;
        stackBehaviour.StackInitializer.SetPos(pos);
        stackBehaviour.StackInitializer.SetScale(size);
        stackBehaviour.StackInitializer.SetColor(_stackConfigSo.stacks[stackIndex].colorSo.color);
        stackBehaviour.StackAnimationController.SetMovementDuration(_stackConfigSo.movementDuration);
        stackBehaviour.StackAnimationController.SetMovementStartSide(_stackConfigSo.stacks[stackIndex].movementStartSide);
        stackBehaviour.StackSliceController.ResetStack(true);
        stackBehaviour.StackSliceController.ResetPiece();
    }
    private Vector3 CalculateNextStackPosition(int index, float xPos)
    {
        var zPos = index * _stackConfigSo.stackSize.z;
        return Vector3.zero.With(x: xPos, z: zPos);
    }

    private IStackConstructor InitializeStackConstructor()
    {
        var stackController = _poolManager.GetObject<StackEntityController>();
        stackController.transform.SetParent(_stacksParent.transform);
        return stackController;
    }
    
    private void SetPreviousStackSliceEntity(IStackBehaviour stackBehaviour, IStackBehaviour previousStackBehaviour)
    {
        var previousStackSliceEntity = previousStackBehaviour.StackSliceController.StackSliceEntity;
        stackBehaviour.StackSliceController.SetPreviousSliceEntity(previousStackSliceEntity);
    }

    private void ShowStack(IStackConstructor stackConstructor) => stackConstructor.Show();
    private IStackBehaviour GetNextUnConstructedStack() => _stackBehaviours.FirstOrDefault(x => !x.IsConstructed);
}

public interface IStackContainer
{
    public bool TryGetNextStack(out IStackBehaviour stackBehaviour);
}