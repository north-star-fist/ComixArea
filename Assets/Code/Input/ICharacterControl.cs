using R3;
using UnityEngine;

namespace ComixArea.Input
{
    public interface ICharacterControl
    {
        public Observable<Vector2> OnDPad { get; }

        public Observable<Unit> OnAction { get; }

        public Observable<Unit> OnJump { get; }

        public Observable<bool> OnBlock { get; }
    }
}
