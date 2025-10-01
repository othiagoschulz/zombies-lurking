using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class scriptHUD : MonoBehaviour
{
    private _GameController _GameController;
    private scriptPersonagem scriptPersonagem;   //VARIAVEL PARA INSTANCIAR O SCRIPT DO PERSONAGEM
    public Image[] barraVida;      //BARRA VIDA
    public Sprite metade, inteiro;
    public GameObject painelFlecha;
    public TMP_Text qtdFlechas;
    public Image iconFlecha;
    public TMP_Text qtdBandagemHud;
    void Start()
    {
        _GameController = FindObjectOfType(typeof(_GameController)) as _GameController;
        scriptPersonagem = FindObjectOfType(typeof(scriptPersonagem)) as scriptPersonagem;   //INSTANCIANDO O CRIPT DO PERSONAGEM
        
    }

    // Update is called once per frame
    void Update()
    {
        controleVida();        
        qtdBandagemHud.text = "x " +_GameController.qtdBandagens.ToString();
    }

    void controleVida()
    {
        float percVida = (float)_GameController.vidaAtual / (float)_GameController.vidaMaxima;   //CALCULA O PERCENTUAL DA VIDA QUE VAI DE 0 - 1

        if (Input.GetButtonDown("itemA")&& percVida < 1)
        {
            _GameController.usarBandagem(); //UTILIZAÇÃO DA BANDAGEM
        }

        //100% VIDA
        foreach (Image img in barraVida)
        {
            img.enabled = true;
            img.sprite = inteiro;
        }

        if (percVida == 1)
        {

        }
        else if (percVida >= 0.66f)
        {
            barraVida[2].enabled = false;
        }
        else if (percVida >= 0.33f)
        {
            barraVida[2].enabled = false;
            barraVida[1].enabled = false;
        }
        else if (percVida <= 0)
        {
            barraVida[2].enabled = false;
            barraVida[1].enabled = false;
            barraVida[0].enabled = false;
        }
    }
}