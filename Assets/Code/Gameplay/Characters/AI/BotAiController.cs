using ComixArea.Creatures;
using ComixArea.Input;
using ComixArea.Util;
using R3;
using R3.Triggers;
using UnityEngine;


namespace ComixArea.AI
{
    [RequireComponent(typeof(SphereCollider))]
    public class BotAiController : AAiStateController, ICharacterControlProvider, ICharacterControl
    {
        [SerializeField]
        private SphereCollider _visionSphere;
        [SerializeField]
        private string[] _enemyTags = new[] { "Player" };

        public ReadOnlyReactiveProperty<GameObject> CurrentEnemyRx => _currentEnemy;
        private readonly ReactiveProperty<GameObject> _currentEnemy = new ReactiveProperty<GameObject>(null);

        public Observable<Vector2> OnDPad => _onDpad;
        private readonly ReactiveProperty<Vector2> _onDpad = new ReactiveProperty<Vector2>(Vector2.zero);

        public Observable<Unit> OnAction => _onAction;
        private readonly Subject<Unit> _onAction = new Subject<Unit>();

        public Observable<Unit> OnJump => _onJump;
        private readonly Subject<Unit> _onJump = new Subject<Unit>();

        public Observable<bool> OnBlock => _onBlock;
        private readonly ReactiveProperty<bool> _onBlock = new ReactiveProperty<bool>(false);

        public Observable<(bool isInScope, GameObject enemy)> OnEnemyIsInScope;

        public ICharacterControl GetCharacterControl() => this;

        private IHealth _health;

        protected override void Awake()
        {
            _visionSphere.isTrigger = true;
            _health = GetComponent<Fighter>().Health;

            var enemyIsInScope = this.OnTriggerEnterAsObservable()
                .Where(other => _health != null && _health.CurrentHealth.CurrentValue > 0 && IsEnemy(other))
                .Select(enemyCol => (true, enemyCol.gameObject));
            var enemyIsOutOfScope = this.OnTriggerExitAsObservable()
                .Where(other => _health != null && _health.CurrentHealth.CurrentValue > 0 && IsEnemy(other))
                .Select(enemyCol => (false, enemyCol.gameObject));
            OnEnemyIsInScope = enemyIsInScope.Merge(enemyIsOutOfScope);
        }


        public void SetDpad(Vector2 dpad) => _onDpad.OnNext(ClampDpad(dpad));

        public void SetDpad(float dpadX, float dpadY) => SetDpad(new(dpadX, dpadY));

        public void PushActionButton() => _onAction.OnNext(R3.Unit.Default);

        public void PushJumpButton() => _onJump.OnNext(Unit.Default);

        public void SwitchBlock(bool on) => _onBlock.OnNext(on);

        public void ForgetEnemy()
        {
            _currentEnemy.Value = null;
        }

        public void SetCurrentValue(GameObject enemy)
        {
            _currentEnemy.Value = enemy;
        }

        private bool IsEnemy(Collider other)
        {
            return other.gameObject.CompareTags(_enemyTags, true);
        }

        private Vector2 ClampDpad(Vector2 dpad)
        {
            return new Vector2(Mathf.Clamp(dpad.x, -1f, 1f), Mathf.Clamp(dpad.y, -1f, 1f));
        }
    }
}
