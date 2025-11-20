using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider sliderGeral;
    public Slider sliderMusica;
    public Slider sliderEfeitosSonoros;

    private bool jaInicializado = false;

    void OnEnable()
    {
        if (!jaInicializado)
        {
            InicializarSliders();
            jaInicializado = true;
        }
        
        CarregarVolumes();
    }

    void InicializarSliders()
    {
        sliderGeral.onValueChanged.AddListener(SetGeralVolume);
        sliderMusica.onValueChanged.AddListener(SetMusicaVolume);
        sliderEfeitosSonoros.onValueChanged.AddListener(SetEfeitosSonorosVolume);
    }

    void CarregarVolumes()
    {
        // ✅ VALOR PADRÃO AGORA É 1f (100%) SE NÃO HOUVER VALOR SALVO
        sliderGeral.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        sliderMusica.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sliderEfeitosSonoros.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    public void SetGeralVolume(float volume)
    {
        float dB = volume > 0 ? Mathf.Log10(volume) * 20 : -80f;
        audioMixer.SetFloat("MasterVolume", dB);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicaVolume(float volume)
    {
        float dB = volume > 0 ? Mathf.Log10(volume) * 20 : -80f;
        audioMixer.SetFloat("MusicVolume", dB);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetEfeitosSonorosVolume(float volume)
    {
        float dB = volume > 0 ? Mathf.Log10(volume) * 20 : -80f;
        audioMixer.SetFloat("SFXVolume", dB);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
}
