using System;
using System.Collections.Generic;
using UnityBase.Extensions;
using UnityEngine;
using VContainer;

namespace UnityBase.UI.ViewCore
{
    public class ViewBehaviourFactory : IViewBehaviourFactory
    {
        private readonly IDictionary<Type, IViewBehaviourGroup> _viewBehaviourGroup = new Dictionary<Type, IViewBehaviourGroup>();

        public TModel CreateViewModel<TModel>(Component component, IObjectResolver resolver) where TModel : class, IViewModel
        {
            var key = component.GetType();
            
            if (!_viewBehaviourGroup.TryGetValue(key, out var viewAnimationGroup))
            {
                viewAnimationGroup = new ViewBehaviourGroup(resolver);
                
                _viewBehaviourGroup[key] = viewAnimationGroup;
            }
           
            return viewAnimationGroup.CreateModel<TModel>();
        }

        public TAnim CreateViewAnimation<TAnim>(Component component, IObjectResolver resolver) where TAnim : class, IViewAnimation
        {
            var key = component.GetType();
            
            if (!_viewBehaviourGroup.TryGetValue(key, out var viewAnimationGroup))
            {
                viewAnimationGroup = new ViewBehaviourGroup(resolver);

                _viewBehaviourGroup[key] = viewAnimationGroup;
            }
            
            return viewAnimationGroup.CreateAnimation<TAnim>();
        }

        public TModel CreateViewLocalModel<TModel>(IObjectResolver resolver) where TModel : class, IViewModel => ClassExtensions.CreateInstance<TModel>(resolver);
        public TAnim CreateViewLocalAnimation<TAnim>(IObjectResolver resolver) where TAnim : class, IViewAnimation => ClassExtensions.CreateInstance<TAnim>(resolver);

        public bool TryGetModel<TViewUI, TViewModel>(out TViewModel viewModel) where TViewUI : Component where TViewModel : class, IViewModel
        {
            var viewKey = typeof(TViewUI);
            
            if (_viewBehaviourGroup.TryGetValue(viewKey, out var viewBehaviourGroup))
            {
                return viewBehaviourGroup.TryGetModel(out viewModel);
            }

            viewModel = null;
            return false;
        }

        public bool TryGetViewAnimation<TViewUI, TViewAnim>(out TViewAnim viewAnimation) where TViewUI : Component where TViewAnim : class, IViewAnimation
        {
            var viewKey = typeof(TViewUI);
            
            if (_viewBehaviourGroup.TryGetValue(viewKey, out var viewBehaviourGroup))
            {
                return viewBehaviourGroup.TryGetAnimation(out viewAnimation);
            }

            viewAnimation = null;
            return false;
        }

    }
}