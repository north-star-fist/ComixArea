using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ComixArea.Configuration
{
    [CreateAssetMenu(fileName = "levels", menuName = "Comix Area/Levels/Level Set")]
    public class LevelsSO : ScriptableObject
    {
        public List<LevelSO> Levels => _levels != null ? _levels.Where(l => l != null).ToList() : new List<LevelSO>();

        [SerializeField]
        private List<LevelSO> _levels;
    }
}
