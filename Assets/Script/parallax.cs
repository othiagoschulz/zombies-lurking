using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax : MonoBehaviour
{
    public Transform fundo; //FUNDO QUE SOFRERÁ A ANIMAÇÃO
    public float qtdParallax; //QUANTIDADE DE PARALLAX APLICADO
    public float velocidade; //EFEITO DE SUAVIZAÇÃO
    public Transform camera; //O EFEITO É BASEADO NA CAMERA
    private Vector3 cameraPosicao; //VETOR RESPONSAVEL POR PEGAR A POSIÇÃO ANTERIOR DA CAMERA

    void Start()
    {
        camera = Camera.main.transform; //SETA A CAMERA AUTOMATICAMENTE
        cameraPosicao = camera.position;
    }

    void LateUpdate()
    {
        float efeitoParallax = (cameraPosicao.x - camera.position.x) * qtdParallax; //PEGA O VALOR DA DISTANCIA MOVIMENTADA E APLICA O EFEITO
        float bgDestino = fundo.position.x + efeitoParallax;

        Vector3 posicaoBg = new Vector3(bgDestino, fundo.position.y, fundo.position.z);
        fundo.position = Vector3.Lerp(fundo.position, posicaoBg, velocidade * Time.deltaTime);    //Time.deltaTime = tempo que passou de um frame para outro

        cameraPosicao = camera.position;
    }
}
