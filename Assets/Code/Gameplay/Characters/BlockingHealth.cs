using System;
using R3;
using UnityEngine;

namespace ComixArea.Creatures
{
    public class BlockingHealth : Health
    {
        [SerializeField, Tooltip("Damage = SrcDamage * BlockFactor")]
        private float _blockFactor = 0.2f;

        private Observable<bool> _blockRx;
        private bool _blocked;

        private IDisposable _blockSub = null;

        public void SetBlockRx(Observable<bool> blockRx)
        {
            Unsubscribe();

            _blockRx = blockRx;
            _blockSub = _blockRx.Subscribe(b => _blocked = b);
        }

        public override void Damage(
            float damage,
            Vector3 hitPoint,
            Vector3 impactForce,
            Vector3 hitNormal,
            Rigidbody rigidBody = null
        )
        {
            float factor = 1f;
            if (_blocked)
            {
                factor = _blockFactor;
            }
            base.Damage(damage * factor, hitPoint, impactForce, hitNormal, rigidBody);
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            if (_blockSub != null)
            {
                _blockSub.Dispose();
                _blockSub = null;
            }
        }
    }
}
