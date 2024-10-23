using System;
using UnityEngine;

namespace ComixArea.AI
{
    [CreateAssetMenu(fileName = "Dead State Factory", menuName = "Comix Area/AI/States/Dead State Factory")]
    public class DeadStateFactory : AAiStateFactory
    {
        public override Type StateType => typeof(DeadState);

        public override IAiState CreateState()
        {
            return new DeadState();
        }
    }
}
