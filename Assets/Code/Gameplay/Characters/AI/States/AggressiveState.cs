using System;
using System.Threading;
using ComixArea.Navigation;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace ComixArea.AI
{
    public class AggressiveState : ABaseAiState
    {
        public override Color GizmoColor => Color.red;

        private readonly float _cautiousHealthPortion;
        private readonly float _blockingDistanceSqr;
        private readonly float _jumpAfterEnemyMinHeight;
        private readonly float _maxAttackDistanceSqr;

        private Type _nextState = null;

        public AggressiveState(
            float cautiousHealthPortion,
            float blockingDistance,
            float jumpAfterEnemyMinHeight,
            float maxAttackDistance
        )
        {
            _cautiousHealthPortion = cautiousHealthPortion;
            _blockingDistanceSqr = blockingDistance * blockingDistance;
            _jumpAfterEnemyMinHeight = jumpAfterEnemyMinHeight;
            _maxAttackDistanceSqr = maxAttackDistance * maxAttackDistance;
        }

        public override async UniTask<Type> Enter(CancellationToken cancellationToken)
        {
            ResetInput();
            DisposableBag subscriptions = makeSubscriptions();
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_nextState != null)
                {
                    break;
                }
                else
                {
                    DoAggressiveActions();
                    await UniTask.Yield();
                }
            }

            subscriptions.Dispose();
            var result = cancellationToken.IsCancellationRequested ? null : _nextState;
            _nextState = null;
            return result;

            DisposableBag makeSubscriptions()
            {
                DisposableBag subs = new DisposableBag();
                _bot.OnEnemyIsInScope.Subscribe(e =>
                {
                    if (!e.isInScope && (_bot.CurrentEnemyRx.CurrentValue == e.enemy))
                    {
                        // for simplicity we assume that there is only one enemy in the game and it's player 1
                        _bot.ForgetEnemy();
                        _nextState = _bot.IsStateRegistered(typeof(PatrolingState))
                        ? typeof(PatrolingState)
                        : typeof(IdleState);
                    }
                }).AddTo(ref subs);
                SubscribeOnDeath(() => _nextState = typeof(DeadState)).AddTo(ref subs);
                return subs;
            }
        }

        private void DoAggressiveActions()
        {
            Vector3 dist = _bot.CurrentEnemyRx.CurrentValue.transform.position - _bot.transform.position;
            float sqrDistance = dist.sqrMagnitude;

            float randMove = UnityEngine.Random.value;
            if (randMove >= 0.5f)
            {
                _bot.SetDpad(Vector2.zero);
            }
            else
            {
                MoveTo(_bot.CurrentEnemyRx.CurrentValue.transform.position);
            }
            MoveTo(_bot.CurrentEnemyRx.CurrentValue.transform.position);
            _bot.SwitchBlock(shouldBeBlocked());
            if ((dist.y > _jumpAfterEnemyMinHeight && UnityEngine.Random.value > 0.3f) || UnityEngine.Random.value > 0.9f)
            {
                _bot.PushJumpButton();
            }
            bool lookDirRight = _fighter.LookDirection is LookDirection.Right or LookDirection.SwitchingToRight;
            bool lookAtEnemy = !(lookDirRight ^ (dist.x > 0));
            if (lookAtEnemy && sqrDistance <= _maxAttackDistanceSqr && UnityEngine.Random.value > 0.2f)
            {
                if (UnityEngine.Random.value > 0.7f)
                {
                    _bot.SetDpad(new(0f, 1f));
                }
                _bot.PushActionButton();
            }

            bool shouldBeBlocked()
            {
                Vector3 distance = _bot.CurrentEnemyRx.CurrentValue.transform.position - _bot.transform.position;
                bool enemyToTheRight = distance.x >= 0;
                var botLookToTheRight = _fighter.LookDirection is Navigation.LookDirection.Right
                    or Navigation.LookDirection.SwitchingToRight;
                if (enemyToTheRight ^ botLookToTheRight)
                {
                    return false;
                }

                if (sqrDistance >= _blockingDistanceSqr)
                {
                    return false;
                }
                if (_fighter.Health.CurrentHealth.CurrentValue < _fighter.Health.MaxHealth * _cautiousHealthPortion)
                {
                    return true;
                }
                return UnityEngine.Random.value > .5f;
            }
        }
    }
}
