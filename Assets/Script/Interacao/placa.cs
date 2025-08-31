using System.Collections;
using UnityEngine;

public class placa : MonoBehaviour
{
    private transicao transicao;
    public Transform tPersonagem;   //TRANSFORM DO PLAYER
    public GameObject placaA;   //PLACA MUNDO NORMAL
    public GameObject placaB;   //PLACA FLORESTA(destino)
    public Transform destino;
    void Start()
    {
        transicao = FindObjectOfType(typeof(transicao)) as transicao;
    }

    public void interacao()
    {
        StartCoroutine("acionarPlaca");
    }

    IEnumerator acionarPlaca()
    {
        transicao.fadeIn();
        yield return new WaitWhile(() => transicao.fume.color.a < 0.9f);
        tPersonagem.position = destino.position;
        yield return new WaitForSeconds(0.3f);
        transicao.fadeOut();

        placaA.SetActive(false);
        placaB.SetActive(false);                
    }
}
