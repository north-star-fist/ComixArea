using System;
using UnityEngine;

namespace ComixArea.AI
{
    [CreateAssetMenu(fileName = "Aggressive State Factory", menuName = "Comix Area/AI/States/Aggressive State Factory")]
    public class AggressiveStateFactory : AAiStateFactory
    {
        [SerializeField, Tooltip("Bot becomes more cautious when it's health = maxHealth * CautiousHealthPortion")]
        private float _cautiousHealthPortion = 0.33f;
        [SerializeField, Tooltip("Within this range bot blocks itself more")]
        private float _blockingDistance = 3f;
        [SerializeField, Tooltip("If enemy is higher than this value bot jumps to reach them")]
        private float _jumpAfterEnemyMinHeight = 1f;
        [SerializeField, Tooltip("No reason to attack if enemy is farer than this value")]
        private float _maxAttackDistance = 1.5f;

        public override Type StateType => typeof(AggressiveState);

        public override IAiState CreateState()
        {
            return new AggressiveState(
                _cautiousHealthPortion,
                _blockingDistance,
                _jumpAfterEnemyMinHeight,
                _maxAttackDistance
            );
        }
    }
}
