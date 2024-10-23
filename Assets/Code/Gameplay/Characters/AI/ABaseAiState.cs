using System;
using System.Threading;
using ComixArea.Creatures;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace ComixArea.AI
{
    public abstract class ABaseAiState : IAiState
    {
        protected BotAiController _bot;
        protected Fighter _fighter;
        protected float TimeElapsed => Time.time - _stateStartTime;
        private float _stateStartTime;

        public abstract Color GizmoColor { get; }

        public virtual void Init(AAiStateController aiController)
        {
            if (aiController is BotAiController bot)
            {
                _bot = bot;
            }
            _fighter = aiController.GetComponent<Fighter>();
        }

        public virtual async UniTask<Type> Enter(CancellationToken cancellationToken = default)
        {
            _stateStartTime = Time.time;
            return null;
        }

        protected void ResetInput()
        {
            _bot.SetDpad(Vector2.zero);
            _bot.SwitchBlock(false);
        }

        protected void MoveTo(Vector3 dest, bool withJumps = false, float jumpDistanceThreshold = 1f)
        {
            Vector3 botPosition = _bot.transform.position;
            Vector3 dist = dest - botPosition;
            var horDir = Mathf.Clamp(dist.x, -1f, 1f);
            _bot.SetDpad(new Vector2(horDir, 0));
            if (withJumps && dist.y > jumpDistanceThreshold)
            {
                _bot.PushJumpButton();
            }
        }

        protected IDisposable SubscribeOnDeath(Action actionOnDeath)
        {
            return _fighter.Health.OnDamage
                .Subscribe(damageData => { if (damageData.IsFatal) { actionOnDeath?.Invoke(); } });
        }
    }
}
