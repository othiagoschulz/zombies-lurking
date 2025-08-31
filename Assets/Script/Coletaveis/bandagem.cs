using UnityEngine;

public class bandagem : MonoBehaviour
{
    private _GameController _GameController;
    public int quantidade = 1;

    private bool coletada;  // indica se a bandagem já foi coletada
    private Collider2D meuCol;  // referência ao collider para desativar colisões após coleta
    private Rigidbody2D rb;

    void Start()
    {
        _GameController = FindObjectOfType<_GameController>();

        meuCol = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        // Evita que a bandagem deslize
        rb.linearDamping = 5f;          
        rb.angularDamping = 5f;

        float forcaX = Random.Range(-15f, 15f);
        float forcaY = 35f;
        rb.AddForce(new Vector2(forcaX, forcaY));
    }

    public void coletar()
    {
        if (coletada == true)
        {
            return;
        }

        coletada = true;

        // Desativa o colisor, garantindo que não haja novas colisões
        if (meuCol != null)
        {
            meuCol.enabled = false;
        }

        _GameController.qtdBandagens += quantidade;
        Destroy(gameObject);
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
        {
            coletar();
        }
    }
}