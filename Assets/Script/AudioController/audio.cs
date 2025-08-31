using UnityEngine;
using UnityEngine.UI;

public class audio : MonoBehaviour
{
    public Slider EfeitoSlider;
    public Slider MusicaSlider;
    void Start()
    {
        if(PlayerPrefs.HasKey("Geral"))
        {
            CarregarAudio();
        }
        else
        {
            EfeitoSlider.value = 1;
            MusicaSlider.value = 1;
        }
    }

    public void SetAudio()
    {              
        AudioListener.volume = EfeitoSlider.value;
        AudioListener.volume = MusicaSlider.value;
        SalvarAudio();
    }

    public void SalvarAudio()
    {        
        PlayerPrefs.SetFloat("Efeito", EfeitoSlider.value);
        PlayerPrefs.SetFloat("Musica", MusicaSlider.value);
    }

    public void CarregarAudio()
    {
        if (PlayerPrefs.HasKey("Geral"))
        {            
            EfeitoSlider.value = PlayerPrefs.GetFloat("Efeito");
            MusicaSlider.value = PlayerPrefs.GetFloat("Musica");
        }
        else
        {            
            EfeitoSlider.value = 1;
            MusicaSlider.value = 1;
        }
    }
}
