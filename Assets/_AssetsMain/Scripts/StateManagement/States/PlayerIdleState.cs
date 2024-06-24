using System;
using UnityBase.StateMachineCore;

public class PlayerIdleState : IState
{
    public event Action OnStateComplete;

    public PlayerIdleState()
    {
        
    }
    public void OnEnter()
    {
        
    }

    public void OnUpdate(float deltaTime) { }

    public void OnExit()
    {
        
    }

    public void Dispose()
    {
        OnStateComplete = null;
    }
}