using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
public class menuScript : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject menuInicial, menuOpcoes, rawImage;
    public AudioSource efeitoSonoro;
    void Start()
    {
        rawImage.SetActive(false);
        menuOpcoes.SetActive(false);
        menuInicial.SetActive(false);
    }

    void Update()
    {
        if (!videoPlayer.isPlaying && Input.anyKeyDown)
        {
            efeitoSonoro.Play();
            videoPlayer.Play();
            rawImage.SetActive(true);
            menuInicial.SetActive(true);
        }
    }    

    public void Opcoes()
    {
        menuInicial.SetActive(false);
        menuOpcoes.SetActive(true);
    }

    public void RetornarMenuInicial()
    {
        menuOpcoes.SetActive(false);
        menuInicial.SetActive(true);
    }

    public void NovoJogo()
    {
        SceneManager.LoadScene("Cena 1");
    }

    public void SairJogo()
    {
        Application.Quit();
    }
}
