using System;
using UnityEngine;

namespace ComixArea.AI
{
    public abstract class AAiStateFactory : ScriptableObject
    {
        public abstract Type StateType { get; }
        public abstract IAiState CreateState();
    }
}
