using UnityBase.Extensions;
using VContainer;

namespace UnityBase.UI.ButtonCore
{
    public class ButtonBehaviourFactory : IButtonBehaviourFactory
    {
        public TAct CreateButtonAction<TAct>(IButtonUI buttonUI, IObjectResolver resolver) where TAct : class, IButtonAction
        {
            return ClassExtensions.CreateInstance<TAct>(resolver, buttonUI);
        }

        public TAnim CreateButtonAnimation<TAnim>(IButtonUI buttonUI, IObjectResolver resolver) where TAnim : class, IButtonAnimation
        {
            return ClassExtensions.CreateInstance<TAnim>(resolver, buttonUI);
        }
    }
}