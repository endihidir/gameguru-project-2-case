
namespace UnityBase.StateMachineCore
{
    public interface IStateMachine
    {
        public IState CurrentPlayerState { get; }
        public void ChangePlayerState(IState state);
    }
}
