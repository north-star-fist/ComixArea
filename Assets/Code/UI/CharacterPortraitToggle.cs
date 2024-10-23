using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ComixArea.UI
{

    public class CharacterPortraitToggle : MonoBehaviour
    {
        [field: SerializeField]
        public Toggle Toggle { get; private set; }

        [SerializeField]
        private Image _portraitImage;
        [SerializeField]
        private TMP_Text _characterNameText;

        [SerializeField]
        private Image _checkMarkImage;

        public void Setup(Sprite portrait, Color highlightColor, string name)
        {
            _portraitImage.sprite = portrait;
            _checkMarkImage.color = highlightColor;
            _characterNameText.text = name;
        }
    }
}
