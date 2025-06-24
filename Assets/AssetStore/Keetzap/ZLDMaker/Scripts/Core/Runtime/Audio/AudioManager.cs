using UnityEngine;

namespace Keetzap.Core
{
    public enum Channel
    {
        Music = 0,
        Effects = 1
    }

    public enum TypeOfEvent
    {
        OnEnable,
        OnDisable,
        OnCooldown
    }

    [RequireComponent(typeof(AudioData))]
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public bool initFade;
        public float initFadeDuration;
        [Space]
        public AudioSource[] audioSource;
        public static AudioManager Instance;

        private bool isFadeDown, isFadeUp;
        private float counter = 0;
        private float fadeDuration;
        private float finalVolume;
        private AudioClip newClip;
        private AudioData audiosList;

        void Awake()
        {
            Instance = this;
            audiosList = GetComponent<AudioData>();
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            InitFadeUp();
            FadeClip();
        }

        #region MUSIC
        public void PlayMusic()
        {
            audioSource[(int)Channel.Music].Play();
        }

        public void PlayMusic(string clip)
        {
            audioSource[(int)Channel.Music].clip = GetAudioClipFromDatabase(clip);
            audioSource[(int)Channel.Music].Play();
        }

        public void PlayMusic(AudioClip clip)
        {
            audioSource[(int)Channel.Music].clip = clip;
            audioSource[(int)Channel.Music].Play();
        }

        public void PlayMusic(AudioClip clip, Channel source)
        {
            audioSource[(int)source].clip = clip;
            audioSource[(int)source].Play();
        }

        public void PlayMusic(AudioClip clip, float volume)
        {
            audioSource[(int)Channel.Music].volume = volume;
            audioSource[(int)Channel.Music].clip = clip;
            audioSource[(int)Channel.Music].Play();
        }

        public void PlayMusic(AudioClip clip, Channel source, float volume)
        {
            audioSource[(int)source].volume = volume;
            audioSource[(int)source].clip = clip;
            audioSource[(int)source].Play();
        }

        public void PlayMusic(string clip, Channel source, float volume, float pitch, bool loop)
        {
            audioSource[(int)source].loop = loop;
            audioSource[(int)source].volume = volume;
            audioSource[(int)source].pitch = pitch;
            audioSource[(int)source].clip = GetAudioClipFromDatabase(clip);
            audioSource[(int)source].Play();
        }

        public void PlayMusic(AudioClip clip, Channel source, float volume, float pitch, bool loop)
        {
            audioSource[(int)source].loop = loop;
            audioSource[(int)source].volume = volume;
            audioSource[(int)source].pitch = pitch;
            audioSource[(int)source].clip = clip;
            audioSource[(int)source].Play();
        }

        public void PlayMusic(Channel source)
        {
            audioSource[(int)source].Play();
        }

        public void PlayMusic(float volume)
        {
            audioSource[(int)Channel.Music].volume = volume;
            audioSource[(int)Channel.Music].Play();
        }

        public void PlayMusic(Channel source, float volume)
        {
            audioSource[(int)source].volume = volume;
            audioSource[(int)source].Play();
        }

        public void ChangeMusicWithFade(string newClip, float fadeDuration, float finalVolume = 1)
        {
            counter = audioSource[(int)Channel.Music].volume;
            this.fadeDuration = fadeDuration;
            this.newClip = GetAudioClipFromDatabase(newClip);
            this.finalVolume = finalVolume;

            isFadeDown = true;
        }

        private void ChangeClip(AudioClip clip)
        {
            audioSource[(int)Channel.Music].clip = clip;
            audioSource[(int)Channel.Music].Play();
        }

        private void FadeClip()
        {
            if (isFadeDown)
            {
                counter -= (Time.deltaTime / fadeDuration);
                audioSource[(int)Channel.Music].volume = counter;

                if (counter < 0)
                {
                    isFadeDown = false;
                    isFadeUp = true;
                    ChangeClip(newClip);
                }
            }

            if (isFadeUp)
            {
                counter += (Time.deltaTime / fadeDuration);
                audioSource[(int)Channel.Music].volume = counter;

                if (counter > finalVolume) isFadeUp = false;
            }
        }

        private void InitFadeUp(float initVolume = 1.0f, float fadeDuration = 1.0f)
        {
            if (!initFade) return;

            counter += (Time.deltaTime / initFadeDuration);
            audioSource[(int)Channel.Music].volume = counter;
            if (counter > initVolume) initFade = false;
        }

        public void StopMusic()
        {
            audioSource[(int)Channel.Music].Stop();
        }

        public void StopMusic(Channel source)
        {
            audioSource[(int)source].Stop();
        }

        public void Mute(bool state)
        {
            audioSource[(int)Channel.Music].mute = state;
        }

        public void Mute(Channel source, bool state)
        {
            audioSource[(int)source].mute = state;
        }

        #endregion

        #region EFFECT

        public void PlayEffect(AudioClip clip)
        {
            audioSource[(int)Channel.Effects].clip = clip;
            audioSource[(int)Channel.Effects].PlayOneShot(clip);
        }

        public void PlayEffect(AudioSource source, AudioClip clip)
        {
            source.clip = clip;
            source.PlayOneShot(clip);
        }

        public void PlayEffect(AudioSource source, AudioClip clip, bool loop)
        {
            source.loop = loop;
            source.clip = clip;
            source.Play();
        }

        public void PlayEffect(AudioClip clip, Channel source)
        {
            audioSource[(int)source].clip = clip;
            audioSource[(int)source].PlayOneShot(clip);
        }

        public void PlayEffect(AudioClip clip, float volume)
        {
            audioSource[(int)Channel.Effects].volume = volume;
            audioSource[(int)Channel.Effects].clip = clip;
            audioSource[(int)Channel.Effects].PlayOneShot(clip);
        }

        public void PlayEffect(AudioClip clip, Channel source, float volume)
        {
            audioSource[(int)source].volume = volume;
            audioSource[(int)source].clip = clip;
            audioSource[(int)source].PlayOneShot(clip);
        }

        public void PlayEffect(AudioClip clip, float volume, float pitch)
        {
            audioSource[(int)Channel.Effects].volume = volume;
            audioSource[(int)Channel.Effects].pitch = pitch;
            audioSource[(int)Channel.Effects].clip = clip;
            audioSource[(int)Channel.Effects].PlayOneShot(clip);
        }

        public void PlayEffect(AudioClip clip, Channel source, float volume, float pitch)
        {
            audioSource[(int)source].volume = volume;
            audioSource[(int)source].pitch = pitch;
            audioSource[(int)source].clip = clip;
            audioSource[(int)source].PlayOneShot(clip);
        }

        public void PlayEffect(string clip)
        {
            audioSource[(int)Channel.Effects].volume = 1;
            audioSource[(int)Channel.Effects].pitch = 1;
            audioSource[(int)Channel.Effects].clip = GetAudioClipFromDatabase(clip);
            audioSource[(int)Channel.Effects].PlayOneShot(audioSource[(int)Channel.Effects].clip);
        }

        public void PlayEffect(string clip, Channel source)
        {
            audioSource[(int)source].clip = GetAudioClipFromDatabase(clip);
            audioSource[(int)source].PlayOneShot(audioSource[(int)source].clip);
        }

        public void PlayEffect(string clip, float volume)
        {
            audioSource[(int)Channel.Effects].volume = volume;
            audioSource[(int)Channel.Effects].clip = GetAudioClipFromDatabase(clip);
            audioSource[(int)Channel.Effects].PlayOneShot(audioSource[(int)Channel.Effects].clip);
        }

        public void PlayEffect(string clip, Channel source, float volume)
        {
            audioSource[(int)source].volume = volume;
            audioSource[(int)source].clip = GetAudioClipFromDatabase(clip);
            audioSource[(int)source].PlayOneShot(audioSource[(int)source].clip);
        }

        public void PlayEffect(string clip, float volume, float pitch)
        {
            audioSource[(int)Channel.Effects].volume = volume;
            audioSource[(int)Channel.Effects].pitch = pitch;
            audioSource[(int)Channel.Effects].clip = GetAudioClipFromDatabase(clip);
            audioSource[(int)Channel.Effects].PlayOneShot(audioSource[(int)Channel.Effects].clip);
        }

        public void PlayEffect(string clip, Channel source, float volume, float pitch)
        {
            audioSource[(int)source].volume = volume;
            audioSource[(int)source].pitch = pitch;
            audioSource[(int)source].clip = GetAudioClipFromDatabase(clip);
            audioSource[(int)source].PlayOneShot(audioSource[(int)source].clip);
        }

        public void StopEffect(AudioSource source, AudioClip clip)
        {
            source.clip = clip;
            source.Stop();
        }

        private AudioClip GetAudioClipFromDatabase(string clip)
        {
            return audiosList.audio.Find(a => a.name == clip);
        }

        #endregion
    }
}