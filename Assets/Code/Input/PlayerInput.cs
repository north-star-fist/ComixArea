using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using static ComixArea.Input.GameControl;

namespace ComixArea.Input
{
    public class PlayerInput : MonoBehaviour, IGameplayActions, ICharacterControlProvider, ICharacterControl
    {
        private GameControl _inputMonitor;

        public Observable<Vector2> OnDPad => _onDpad;
        private readonly ReactiveProperty<Vector2> _onDpad = new ReactiveProperty<Vector2>(Vector2.zero);

        public Observable<Unit> OnAction => _onAction;
        private readonly Subject<Unit> _onAction = new Subject<Unit>();

        public Observable<Unit> OnJump => _onJump;
        private readonly Subject<Unit> _onJump = new Subject<Unit>();

        public Observable<bool> OnBlock => _onBlock;
        private readonly ReactiveProperty<bool> _onBlock = new ReactiveProperty<bool>(false);

        private void Awake()
        {
            _inputMonitor = new GameControl();
            _inputMonitor.Gameplay.AddCallbacks(this);
            _inputMonitor.Gameplay.Enable();
        }

        private void OnDestroy()
        {
            _inputMonitor.Gameplay.Disable();
            _inputMonitor?.Dispose();
        }

        public ICharacterControl GetCharacterControl()
        {
            return this;
        }

        void IGameplayActions.OnDPad(InputAction.CallbackContext context)
        {
            _onDpad.OnNext(context.ReadValue<Vector2>());
        }

        void IGameplayActions.OnAction(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }
            _onAction.OnNext(Unit.Default);
        }

        void IGameplayActions.OnJump(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }
            _onJump.OnNext(Unit.Default);
        }

        void IGameplayActions.OnBlock(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _onBlock.OnNext(true);
            }
            else if (context.canceled)
            {
                _onBlock.OnNext(false);
            }
        }
    }
}
