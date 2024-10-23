using UnityEngine;

namespace ComixArea.Configuration
{
    [CreateAssetMenu(fileName = "Character Database", menuName = "Comix Area/Characters/Character List")]
    public class CharacterListSO : ScriptableObject
    {
        public ACharacterSO[] Characters;
    }
}
