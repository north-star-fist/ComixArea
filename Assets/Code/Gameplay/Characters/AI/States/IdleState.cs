using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace ComixArea.AI
{
    public class IdleState : ABaseAiState
    {
        private Type _nextState = null;

        public override Color GizmoColor => Color.gray;

        public override async UniTask<Type> Enter(CancellationToken cancellationToken)
        {
            DisposableBag disposableBag = new DisposableBag();
            initSubscriptions();
            ResetInput();
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_nextState != null)
                {
                    break;
                }
                else
                {
                    DoIdleActions();
                    await UniTask.Yield();
                }
            }

            disposableBag.Dispose();
            var result = cancellationToken.IsCancellationRequested ? null : _nextState;
            _nextState = null;
            return result;

            void initSubscriptions()
            {
                _bot.OnEnemyIsInScope.Subscribe(e =>
                {
                    if (e.isInScope)
                    {
                        _bot.SetCurrentValue(e.enemy);
                        _nextState = typeof(AggressiveState);
                    }
                }).AddTo(ref disposableBag);

                SubscribeOnDeath(() => _nextState = typeof(DeadState));
            }
        }

        protected virtual void DoIdleActions()
        {
        }
    }
}
