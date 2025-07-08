using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;



public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance { get; private set; }

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Clips de música y efectos")]
    public AudioClip backgroundMusic;
    public AudioClip ballHitClip;
    public AudioClip lifeLostClip;
    public AudioClip winClip;
    public AudioClip gameOverClip;
    public AudioClip brickHitClip;

    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 🎵 Fuente de música
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;

        // 🔊 Fuente de efectos
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f;
    }

    // 🎵 Música de fondo
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic == null || musicSource == null) return;
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    // 🔊 Volumen desde sliders
    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
    }

    // 🔉 Efectos
    public void PlayBrickHit() => PlaySFX(brickHitClip);
    public void PlayBallHit() => PlaySFX(ballHitClip);
    public void PlayLifeLost() => PlaySFX(lifeLostClip);
    public void PlayWin() => PlaySFX(winClip);
    public void PlayGameOver() => PlaySFX(gameOverClip);

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);
    }
}