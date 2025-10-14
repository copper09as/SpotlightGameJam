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
        public AudioSource bgmSource;               // ��������
        public List<AudioSource> sfxSources = new(); // �����Ч������

        [Header("���뵭������")]
        public float fadeDuration = 1f; // BGM���뵭��ʱ��

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

        #region ��������
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

        #region ����BGM
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
            // ������BGM
            float startVolume = bgmSource.volume;
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
                yield return null;
            }
            bgmSource.volume = 0f;

            // ����
            bgmSource.clip = newClip;
            bgmSource.Play();

            // ������BGM
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

        #region ����SFX
        /// <summary>
        /// ����һ����Ч
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
            // ������ڲ��ţ���ռ��һ��
            sfxSources[0].PlayOneShot(clip);
        }
        #endregion

        #region ������Ƶ��Դ
        private AudioClip LoadClip(string clipName)
        {
            if (clipCache.TryGetValue(clipName, out AudioClip clip))
                return clip;

            clip = ResManager.LoadDataByAsset<AudioClip>(clipName);
            if (clip != null)
                clipCache[clipName] = clip;
            else
                Debug.LogWarning($"AudioManager: ��Դδ�ҵ� -> {clipName}");
            return clip;
        }
        #endregion
    }
