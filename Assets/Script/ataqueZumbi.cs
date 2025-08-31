using UnityEngine;

public class AtaqueZumbi : MonoBehaviour
{
    private _GameController _GameController;
    public int dano = 1;
    public bool podeCausarDano = false; // controlado por animation event

    void Start()
    {
        _GameController = FindObjectOfType<_GameController>();
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (podeCausarDano && col.CompareTag("Player"))
        {
            scriptPersonagem player = col.GetComponent<scriptPersonagem>();
            if (player != null && player.vidaAtual > 0)
            {                
                player.vidaAtual -= dano;
                podeCausarDano = false; // impede múltiplos danos no mesmo ataque
            }
        }
    }

    public void AtivarDano()  => podeCausarDano = true;
    public void DesativarDano() => podeCausarDano = false;
}