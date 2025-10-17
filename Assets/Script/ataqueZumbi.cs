using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueZumbi : MonoBehaviour
{
    private _GameController _GameController;
    public int dano = 1;
    private bool podeCausarDano = true;
    public float tempoEntreDanos = 1f; // 1 segundo entre danos


    void Start()
    {
        _GameController = FindObjectOfType<_GameController>();
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && podeCausarDano)
        {
            scriptPersonagem player = col.GetComponent<scriptPersonagem>();
            if (player != null && !player.invulneravel && !player.emDano && player.vidaAtual > 0)
            {
                StartCoroutine(DelayDano(player));
            }
        }
    }

    IEnumerator DelayDano(scriptPersonagem player)
    {
        podeCausarDano = false;
        player.emDano = true;

        player.animacaoPersonagem.Play("Dano");
        player.velocidade = 0;
        player.personagemRb.linearVelocity = new Vector2(0, player.personagemRb.linearVelocity.y);

        player.vidaAtual -= 1;
        _GameController.vidaAtual -= 1;

        yield return player.StartCoroutine(player.Invulneravel());
        yield return new WaitForSeconds(tempoEntreDanos);

        player.emDano = false;
        podeCausarDano = true;
    }

    IEnumerator ResetarAtaque()
    {
        yield return new WaitForSeconds(1f);
        podeCausarDano = true;
    }

    public void AtivarDano()  => podeCausarDano = true;
    public void DesativarDano() => podeCausarDano = false;
}