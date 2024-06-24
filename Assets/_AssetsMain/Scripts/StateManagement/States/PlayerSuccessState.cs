using System;
using UnityBase.StateMachineCore;

public class PlayerSuccessState : IState
{
    public event Action OnStateComplete;

    public PlayerSuccessState()
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