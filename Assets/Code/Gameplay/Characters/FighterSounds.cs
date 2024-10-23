using UnityEngine;

namespace ComixArea.Creatures
{
    public class FighterSounds : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private AudioClip _punchSound;
        [SerializeField]
        private AudioClip _kickSound;
        [SerializeField]
        private AudioClip _jumpSound;

        [SerializeField]
        private AudioClip _lightHitSound;
        [SerializeField]
        private AudioClip _heavyHitSound;

        [SerializeField]
        private AudioClip _deathSound;

        public void PlayPunchSound()
        {
            Play(_punchSound);
        }
        public void PlayKickSound()
        {
            Play(_kickSound);
        }
        public void PlayJumpSound()
        {
            Play(_jumpSound);
        }
        public void PlayLightHitSound()
        {
            Play(_lightHitSound);
        }
        public void PlayHeavyHitSound()
        {
            Play(_heavyHitSound);
        }
        public void PlayDeathSound()
        {
            Play(_deathSound);
        }

        private void Play(AudioClip sound)
        {
            _audioSource.PlayOneShot(sound);
        }
    }
}
