using System;
using UnityEngine;

namespace ComixArea.AI
{
    [CreateAssetMenu(fileName = "Idle State Factory", menuName = "Comix Area/AI/States/Idle State Factory")]
    public class IdleStateFactory : AAiStateFactory
    {
        public override Type StateType => typeof(IdleState);

        public override IAiState CreateState()
        {
            return new IdleState();
        }
    }
}
