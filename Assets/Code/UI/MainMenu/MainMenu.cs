using System.Linq;
using ComixArea.Configuration;
using ComixArea.Util;
using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace ComixArea.UI
{
    public class MainMenu : MonoBehaviour, IMainMenu
    {
        [SerializeField] Button _startButton;
        [SerializeField] Button _exitButton;
        [SerializeField] TMP_Dropdown _levelDropdown;
        [SerializeField] ToggleGroup _portraitsParent;
        [SerializeField] CharacterPortraitToggle _portraitPrefab;

        [Inject]
        private LevelsSO _levelsSO;
        [Inject]
        private CharacterListSO _charactersSO;

        public Observable<Unit> OnStart => _startButton.OnClickAsObservable();
        public Observable<Unit> OnExit => _exitButton.OnClickAsObservable();
        public Observable<int> OnLevelChosen => _levelDropdown.OnValueChangedAsObservable();
        public Observable<int> OnHeroChosen => _heroChooser;

        private readonly ReactiveProperty<int> _heroChooser = new(0);

        DisposableBag _disposables = new DisposableBag();

        private void Start()
        {
            InitPortraitGallery();
            int num = 1;
            var options = _levelsSO.Levels.Select(l => new TMP_Dropdown.OptionData($"Level {num++}")).ToList();
            _levelDropdown.options = options;

            void InitPortraitGallery()
            {
                Toggle first = null;
                for (int i = 0; i < _charactersSO.Characters.Length; i++)
                {
                    int chInd = i;
                    var character = _charactersSO.Characters[i];
                    var portrait = Instantiate(_portraitPrefab, _portraitsParent.transform);
                    if (first == null)
                    {
                        first = portrait.Toggle;
                    }
                    portrait.Toggle.group = _portraitsParent;
                    portrait.Toggle.onValueChanged.AsObservable()
                        .Subscribe(selected =>
                        {
                            if (selected) { _heroChooser.OnNext(chInd); }
                        })
                        .AddTo(ref _disposables);
                    portrait.Setup(character.Portrait, character.HighlightColor, character.Name);
                }
                if (first != null)
                {
                    first.isOn = true;
                }
            }
        }
    }
}
