using ComixArea.Util;
using UnityEngine;

namespace ComixArea
{

    [RequireComponent(typeof(Collider))]
    public class DamagingSpace : MonoBehaviour
    {
        public int Damage = 10;

        [SerializeField]
        private LayerMask _hitLayerMask = -1;
        [SerializeField]
        private bool _hitTriggerColliders = false;
        [SerializeField]
        private Collider _trigCollider;

        void OnTriggerEnter(Collider other)
        {
            if (!enabled)
            {
                return;
            }
            if (ShouldBeDamaged(other))
            {
                var h = other.GetComponent<IHealth>();
                if (h != null)
                {
                    h.Damage(Damage, other.transform.position, Vector3.zero, Vector3.zero);
                }
            }
        }


        virtual protected bool ShouldBeDamaged(Collider other)
        {
            if (other.isTrigger && !_hitTriggerColliders
                || !_hitLayerMask.IsLayerIncluded(other.gameObject.layer)
                || !_trigCollider.bounds.Intersects(other.bounds))
            {
                return false;
            }

            return true;
        }
    }
}
