using R3;
using UnityEngine;
using static ComixArea.IHealth;

namespace ComixArea
{
    public class Health : MonoBehaviour, IHealth
    {
        public ReadOnlyReactiveProperty<float> CurrentHealth => _health;
        private ReactiveProperty<float> _health;

        public float MaxHealth => _maxHealth;
        [SerializeField]
        private float _maxHealth = 100;

        public Observable<DamageData> OnDamage => _onDamage;
        private readonly Subject<DamageData> _onDamage = new Subject<DamageData>();

        private void Awake()
        {
            _health = new ReactiveProperty<float>(_maxHealth);
        }

        public virtual void Damage(
            float damage,
            Vector3 hitPoint,
            Vector3 impactForce,
            Vector3 hitNormal,
            Rigidbody rigidBody = null
        )
        {
            if (_health.Value <= 0)
            {
                return;
            }
            _health.Value -= damage;
            _onDamage.OnNext(new DamageData(damage, _health.Value <= 0, hitPoint, hitNormal, impactForce, rigidBody));
        }
    }
}
