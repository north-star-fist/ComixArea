using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ComixArea.Configuration
{
    [CreateAssetMenu(fileName = "level", menuName = "Comix Area/Levels/Level")]
    public class LevelSO : ScriptableObject
    {
        [field: SerializeField, AssetReferenceUILabelRestriction(AddressablesTags.GameLevelScene)]
        public AssetReference LevelScene { get; private set; }

        [field: SerializeField]
        public CharacterListSO Characters { get; private set; }
    }
}
