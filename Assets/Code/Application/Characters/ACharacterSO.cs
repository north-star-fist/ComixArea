using UnityEngine;

namespace ComixArea.Configuration
{
    /// <summary>
    /// Character configuration.
    /// </summary>
    public abstract class ACharacterSO : ScriptableObject
    {
        [field: SerializeField]
        public Sprite Portrait { get; private set; }
        [field: SerializeField]
        public Color HighlightColor { get; private set; }
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField, Tooltip("Character model prefab")]
        public GameObject Model { get; private set; }
    }
}
