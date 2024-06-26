
using VContainer;

namespace UnityBase.UI.ButtonCore
{
    public interface IButtonBehaviourFactory
    {
        public TAct CreateButtonAction<TAct>(IButtonUI buttonUI, IObjectResolver resolver) where TAct : class, IButtonAction;
        public TAnim CreateButtonAnimation<TAnim>(IButtonUI buttonUI, IObjectResolver resolver) where TAnim : class, IButtonAnimation;
    }
}