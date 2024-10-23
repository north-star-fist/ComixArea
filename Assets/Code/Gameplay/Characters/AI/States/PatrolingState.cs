using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ComixArea.AI
{
    public class PatrolingState : IdleState
    {
        private readonly float _jumpAfterEnemyMinHeight;

        private PatrolingFighterAiController _patrolBot;
        private float _sqrStopDistance;

        public PatrolingState(float jumpAfterEnemyMinHeight)
        {
            _jumpAfterEnemyMinHeight = jumpAfterEnemyMinHeight;
        }

        public override Color GizmoColor => Color.blue;

        public override async UniTask<Type> Enter(CancellationToken cancellationToken)
        {
            _patrolBot.SwitchToClosestWaypoint();
            return await base.Enter(cancellationToken);
        }

        public override void Init(AAiStateController aiController)
        {
            base.Init(aiController);

            if (aiController is PatrolingFighterAiController patroller)
            {
                _patrolBot = patroller;
                _sqrStopDistance = _patrolBot.StoppingDistance * _patrolBot.StoppingDistance;
            }
        }

        protected override void DoIdleActions()
        {
            base.DoIdleActions();

            // Go to the next way point
            Vector3 botPosition = _bot.transform.position;
            if ((_patrolBot.CurrentWaypoint - botPosition).sqrMagnitude <= _sqrStopDistance)
            {
                // Time to switch to the next way point (we are not standing here for simplicity)
                _patrolBot.SwitchToNextWaypoint();
            }
            var dest = _patrolBot.CurrentWaypoint;
            MoveTo(dest, true, _jumpAfterEnemyMinHeight);
        }
    }
}
