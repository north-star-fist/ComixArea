using R3;
using UnityEngine;


namespace ComixArea
{
    public interface IHealth
    {
        public ReadOnlyReactiveProperty<float> CurrentHealth { get; }

        public float MaxHealth { get; }

        public Observable<DamageData> OnDamage { get; }

        public void Damage(
            float damage,
            Vector3 hitPoint,
            Vector3 impactForce,
            Vector3 hitNormal,
            Rigidbody rigidBody = null
        );

        public class DamageData
        {
            public float Damage;
            public bool IsFatal;
            public Vector3 HitPosition;
            public Vector3 HitNormal;
            public Vector3 ImpactForce;
            public Rigidbody RigidBody;

            public DamageData(float damage, bool fatal, Vector3 hitPosition, Vector3 hitNormal, Vector3 impactForce, Rigidbody rigidBody)
            {
                Damage = damage;
                IsFatal = fatal;
                HitPosition = hitPosition;
                HitNormal = hitNormal;
                ImpactForce = impactForce;
                RigidBody = rigidBody;
            }
        }
    }
}
