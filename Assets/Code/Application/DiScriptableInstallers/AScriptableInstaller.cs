using UnityEngine;
using VContainer;

namespace ComixArea.Configuration
{
    public abstract class AScriptableInstaller : ScriptableObject
    {
        public abstract void Install(IContainerBuilder builder);
    }
}
