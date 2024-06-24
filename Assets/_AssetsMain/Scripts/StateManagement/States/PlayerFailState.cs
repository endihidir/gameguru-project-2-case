using System;
using UnityBase.StateMachineCore;

public class PlayerFailState : IState
{
    public event Action OnStateComplete;

    public PlayerFailState()
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