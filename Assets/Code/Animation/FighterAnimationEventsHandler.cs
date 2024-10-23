using System;
using UnityEngine;

namespace ComixArea.Animation
{
    public class FighterAnimationEventsHandler : MonoBehaviour
    {
        private Action _onEnteredIdle;
        private Action _onMotionAllowed;
        private Action _onMotionProhibited;
        private Action _onActionsAllowed;
        private Action _onActionsProhibited;
        private Action _onTurningFinished;
        private Action _onExitingGettingHit;
        private Action _onRightHandAttack;
        private Action _onLeftHandAttack;
        private Action _onRightLegAttack;
        private Action _onLeftLegAttack;

        public void Init(
            Action onEnteredIdle,
            Action onMotionAllowed,
            Action onMotionProhibited,
            Action onActionsAllowed,
            Action onActionsProhibited,
            Action onTurningFinished,
            Action onExitingGettingHit,

            Action onRightHandAttack,
            Action onLeftHandAttack,
            Action onRightLegAttack,
            Action onLeftLegAttack
        )
        {
            _onEnteredIdle = onEnteredIdle;
            _onMotionAllowed = onMotionAllowed;
            _onMotionProhibited = onMotionProhibited;
            _onActionsAllowed = onActionsAllowed;
            _onActionsProhibited = onActionsProhibited;
            _onTurningFinished = onTurningFinished;
            _onExitingGettingHit = onExitingGettingHit;

            _onRightHandAttack = onRightHandAttack;
            _onLeftHandAttack = onLeftHandAttack;
            _onRightLegAttack = onRightLegAttack;
            _onLeftLegAttack = onLeftLegAttack;
        }

        public void EnteredIdle()
        {
            _onEnteredIdle?.Invoke();
        }

        public void ExitedGettingHit()
        {
            _onExitingGettingHit?.Invoke();
        }

        public void AllowMotion()
        {
            _onMotionAllowed?.Invoke();
        }

        public void ProhibitMotion()
        {
            _onMotionProhibited?.Invoke();
        }

        public void AllowActions()
        {
            _onActionsAllowed?.Invoke();
        }

        public void ProhibitActions()
        {
            _onActionsProhibited?.Invoke();
        }

        public void FinishTurning()
        {
            _onTurningFinished?.Invoke();
        }


        public void AttackLeftHand()
        {
            _onLeftHandAttack?.Invoke();
        }

        public void AttackRightHand()
        {
            _onRightHandAttack?.Invoke();
        }

        public void AttackLeftLeg()
        {
            _onLeftLegAttack?.Invoke();
        }

        public void AttackRightLeg()
        {
            _onRightLegAttack?.Invoke();
        }
    }
}
