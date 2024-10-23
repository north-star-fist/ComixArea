using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ComixArea.AI
{
    public class DeadState : ABaseAiState
    {
        public override Color GizmoColor => Color.black;

        public override async UniTask<Type> Enter(CancellationToken cancellationToken)
        {
            ResetInput();
            if (_bot.DeathParticles != null)
            {
                foreach (var particle in _bot.DeathParticles)
                {
                    particle.Play();
                }
            }
            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.Yield();
            }
            return null;
        }
    }
}
