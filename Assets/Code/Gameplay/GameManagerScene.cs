using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ComixArea
{
    public class GameManagerScene : LifetimeScope
    {
        [SerializeField]
        private GameManager _gameManager;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterInstance<IGameManager>(_gameManager);
        }
    }
}
