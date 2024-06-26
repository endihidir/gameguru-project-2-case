using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityBase.Manager;
using UnityBase.Service;
using VContainer;
using VContainer.Unity;

namespace UnityBase.Presenter
{
    public class GameplayBootstarpper : IInitializable, IDisposable
    {
        [Inject]
        private readonly IEnumerable<IGameplayBootService> _gameplayBootServices;
        
        public GameplayBootstarpper(IObjectResolver objectResolver)
        {
            UpdateGameplayServices(objectResolver);
        }

        private void UpdateGameplayServices(IObjectResolver objectResolver)
        {
            var poolManager = objectResolver.Resolve<IPoolManager>() as PoolManager;
            poolManager?.UpdateResolver(objectResolver);
        }
        
        public void Initialize() => _gameplayBootServices.ForEach(x => x.Initialize());
        public void Dispose() => _gameplayBootServices.ForEach(x => x.Dispose());
    }
}