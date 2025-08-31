using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class transicao : MonoBehaviour
{
    public GameObject painelFume;    //VARIAVEL PARA HABILITAR E DESABILITAR A TRANSICAO
    public Image fume;
    public Color[] corTransicao;
    public float step;

    public void fadeIn()
    {
        painelFume.SetActive(true);
        StartCoroutine("fadeI");
    }

    public void fadeOut()
    {
        StartCoroutine("fadeO");
    }

    IEnumerator fadeI()
    {
        for (float i = 0; i <= 1; i += step)
        {
            fume.color = Color.Lerp(corTransicao[0], corTransicao[1], i);
            yield return new WaitForEndOfFrame();
        }
        fume.color = corTransicao[1];
    }

    IEnumerator fadeO()
    {
        for (float i = 0; i <= 1; i += step)
        {
            fume.color = Color.Lerp(corTransicao[1], corTransicao[0], i);
            yield return new WaitForEndOfFrame();
        }
        painelFume.SetActive(false);
    }
}
