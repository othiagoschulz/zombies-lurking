using System.Collections;
using UnityEngine;

public class porta : MonoBehaviour
{
    private transicao transicao;
    public Transform tPersonagem;   //TRANSFORM DO PLAYER
    public GameObject escuridaoCaverna;
    public Transform destino;

    void Start()
    {
        transicao = FindObjectOfType(typeof(transicao)) as transicao;
    }

    public void interacao()
    {
        StartCoroutine("acionarPorta");
    }

    IEnumerator acionarPorta()
    {
        transicao.fadeIn();
        yield return new WaitWhile(() => transicao.fume.color.a < 0.9f);
        tPersonagem.position = destino.position;
        if (escuridaoCaverna != null) escuridaoCaverna.SetActive(true); // ativa a escuridão
        yield return new WaitForSeconds(0.3f);
        transicao.fadeOut();    
    }
}