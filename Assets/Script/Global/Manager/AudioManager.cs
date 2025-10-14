using System.Collections;
using System.Collections.Generic;
using Global.Data;
using UnityEngine;
using UnityEngine.Audio;

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [Header("Audio Mixer")]
        public AudioMixer mixer;

        [Header("Audio Sources")]
        public AudioSource bgmSource;               // 背景音乐
        public List<AudioSource> sfxSources = new(); // 五个音效播放器

        [Header("淡入淡出设置")]
        public float fadeDuration = 1f; // BGM淡入淡出时长

        private Coroutine fadeCoroutine;
        private readonly Dictionary<string, AudioClip> clipCache = new Dictionary<string, AudioClip>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            bgmSource.loop = true;
            PlayBGM(StringResource.BattleBgmPath);
        }

        #region 音量管理
        public void SetBGMVolume(float volume)
        {
            float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
            mixer.SetFloat("BgmVolume", dB);
        }

        public void SetSFXVolume(float volume)
        {
            float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
            mixer.SetFloat("SfxVolume", dB);
        }

        public void SetMasterVolume(float volume)
        {
            float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
            mixer.SetFloat("MasterVolume", dB);
        }
        #endregion

        #region 播放BGM
        public void PlayBGM(string clipName)
        {
            AudioClip clip = LoadClip(clipName);
            if (clip == null) return;

            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeInBGM(clip));
        }

        private IEnumerator FadeInBGM(AudioClip newClip)
        {
            // 淡出旧BGM
            float startVolume = bgmSource.volume;
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
                yield return null;
            }
            bgmSource.volume = 0f;

            // 换曲
            bgmSource.clip = newClip;
            bgmSource.Play();

            // 淡入新BGM
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(0f, 1f, t / fadeDuration);
                yield return null;
            }
            bgmSource.volume = 1f;
        }

        public void StopBGM()
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            StartCoroutine(FadeOutBGM());
        }

        private IEnumerator FadeOutBGM()
        {
            float startVolume = bgmSource.volume;
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
                yield return null;
            }
            bgmSource.Stop();
            bgmSource.volume = 1f;
        }
        #endregion

        #region 播放SFX
        /// <summary>
        /// 播放一个音效
        /// </summary>
        public void PlaySFX(string clipName)
        {

            AudioClip clip = LoadClip(clipName);
            if (clip == null) return;

            foreach (var source in sfxSources)
            {
                if (!source.isPlaying)
                {
                    source.PlayOneShot(clip);
                    return;
                }
            }
            // 如果都在播放，抢占第一个
            sfxSources[0].PlayOneShot(clip);
        }
        #endregion

        #region 加载音频资源
        private AudioClip LoadClip(string clipName)
        {
            if (clipCache.TryGetValue(clipName, out AudioClip clip))
                return clip;

            clip = ResManager.LoadDataByAsset<AudioClip>(clipName);
            if (clip != null)
                clipCache[clipName] = clip;
            else
                Debug.LogWarning($"AudioManager: 资源未找到 -> {clipName}");
            return clip;
        }
        #endregion
    }
