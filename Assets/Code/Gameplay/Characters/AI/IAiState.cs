using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ComixArea.AI
{
    public interface IAiState
    {
        Color GizmoColor { get; }

        public void Init(AAiStateController aiController);

        public UniTask<Type> Enter(CancellationToken cancellationToken = default);
    }
}
