using System;
using UnityEngine;

namespace ComixArea.AI
{
    [CreateAssetMenu(fileName = "Patroling State Factory", menuName = "Comix Area/AI/States/Patroling State Factory")]
    public class PatrolingStateFactory : AAiStateFactory
    {
        [SerializeField, Tooltip("If a way point is higher than this value bot jumps to reach it")]
        private float _jumpAfterWaypointMinHeight = 1f;
        public override Type StateType => typeof(PatrolingState);

        public override IAiState CreateState()
        {
            return new PatrolingState(_jumpAfterWaypointMinHeight);
        }
    }
}
