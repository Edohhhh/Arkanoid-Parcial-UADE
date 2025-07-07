using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance { get; private set; }

    [Header("Audio")]
    public AudioMixer audioMixer;

    [Header("Canvas References")]
    public GameObject gameplayCanvas;
    public GameObject settingsCanvas;

    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
    }

    public void ShowSettings()
    {
        gameplayCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    public void HideSettings()
    {
        gameplayCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
        Time.timeScale = 1f;
    }
}