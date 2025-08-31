using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum estadoInimigo
{
    PARADO,
    ALERTA,
    PATRULHA,
    ATACANDO,
    RECUAR
}
public class IAzumbi : MonoBehaviour
{
    private scriptPersonagem scriptPersonagem;
    private _GameController _GameController;
    public Rigidbody2D rBody;
    private Animator animator;
    public estadoInimigo estadoInimigoAtual;
    public estadoInimigo estadoInimigoInicial;

    public float velocidadeBase;
    public float velocidade;

    public float tempoParado;
    public float tempoRecuo;

    private Vector3 dir = Vector3.right;
    public float distanciaMudarRota;
    public LayerMask layerObstaculo;

    public float distanciaVerPersonagem;
    public float distanciaAtaque;
    public float distanciaSairAlerta;
    public LayerMask layerPersonagem;

    public GameObject balaoAlerta;

    public bool olhandoEsquerda;
    private bool atacando;
    public bool levandoDano = false;
    public GameObject[] armas;
    void Start()
    {
        _GameController = FindObjectOfType(typeof(_GameController)) as _GameController;
        scriptPersonagem = FindObjectOfType(typeof(scriptPersonagem)) as scriptPersonagem;

        rBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        controleDanoInimigo cdi = GetComponent<controleDanoInimigo>();
        if (cdi != null)
        {
            cdi._GameController = _GameController;
        }


        if (olhandoEsquerda == true)
            {
                flip();
            }
        mudarEstado(estadoInimigoInicial);

        velocidade = velocidadeBase;
    }

    void Update()
    {
        if (levandoDano || GetComponent<controleDanoInimigo>().morto)
        {
            rBody.linearVelocity = Vector2.zero; // zera movimento
            animator.SetInteger("idAnimacao", 0); // garante animação de parado
            return;
        }

        if (estadoInimigoAtual != estadoInimigo.ATACANDO && estadoInimigoAtual != estadoInimigo.RECUAR)
        {
            Debug.DrawRay(transform.position, dir * distanciaVerPersonagem, Color.blue);
            RaycastHit2D hitPersonagem = Physics2D.Raycast(transform.position, dir, distanciaVerPersonagem, layerPersonagem);
            if (hitPersonagem == true)
            {
                mudarEstado(estadoInimigo.ALERTA);
            }
        }


        if (estadoInimigoAtual == estadoInimigo.PATRULHA)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, distanciaMudarRota, layerObstaculo);

            if (hit == true)
            {
                mudarEstado(estadoInimigo.PARADO);
            }
        }

        if (estadoInimigoAtual == estadoInimigo.RECUAR)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, distanciaMudarRota, layerObstaculo);

            if (hit == true)
            {
                flip();
            }
        }

        rBody.linearVelocity = new Vector2(velocidade, rBody.linearVelocity.y);

        if (velocidade == 0)
        {
            animator.SetInteger("idAnimacao", 0);
        }
        else if (velocidade != 0)
        {
            animator.SetInteger("idAnimacao", 1);
        }

        if (estadoInimigoAtual == estadoInimigo.ALERTA)
        {
            float dist = Vector3.Distance(transform.position, scriptPersonagem.transform.position);
            if (dist <= distanciaAtaque)
            {
                mudarEstado(estadoInimigo.ATACANDO);
            }
            else if (dist >= distanciaSairAlerta)
            {
                mudarEstado(estadoInimigo.PARADO);
            }

        }
        if (estadoInimigoAtual != estadoInimigo.ALERTA)
        {
            balaoAlerta.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (estadoInimigoAtual == estadoInimigo.ATACANDO && col.CompareTag("Player"))
        {
            _GameController.vidaAtual -= 1;
        }
    }

    void flip()
    {
        olhandoEsquerda = !olhandoEsquerda; //INVERTE O VALOR DA VARIAVEL BOLEANA
        float x = transform.localScale.x;
        x *= -1; //INVERTE O SINAL DO SCALE X
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
        dir.x = x;
        velocidadeBase *= -1;
        float vAtual = velocidade * -1;
        velocidade = vAtual;
    }

    IEnumerator parado()
    {
        yield return new WaitForSeconds(tempoParado);
        flip();
        mudarEstado(estadoInimigo.PATRULHA);
    }

    IEnumerator recuar()
    {
        yield return new WaitForSeconds(tempoRecuo);
        flip();
        mudarEstado(estadoInimigo.ALERTA);
    }

    public void mudarEstado(estadoInimigo novoEstado)
    {
        if (levandoDano && novoEstado != estadoInimigo.RECUAR)
        {
            // Se estiver levando dano, só permite mudar para RECUAR
            return;
        }

        StopCoroutine("parado");
        StopCoroutine("recuar");

        estadoInimigoAtual = novoEstado;
        switch (novoEstado)
        {
            case estadoInimigo.PARADO:
                velocidade = 0;
                StartCoroutine("parado");
                break;

            case estadoInimigo.PATRULHA:
                velocidade = velocidadeBase;
                break;

            case estadoInimigo.ALERTA:
                velocidade = 0;
                balaoAlerta.SetActive(true);
                break;

            case estadoInimigo.ATACANDO:
                animator.SetTrigger("ataque");
                break;

            case estadoInimigo.RECUAR:
                atacando = false;
                foreach (GameObject o in armas)
                    o.SetActive(false);

                StopAllCoroutines();
                flip();
                velocidade = velocidadeBase * 2;
                StartCoroutine("recuar");
                break;
        }
    }

    void atack(int atk)
    {
        switch (atk)
        {
            case 0:
                atacando = false;
                foreach (GameObject o in armas)
                    o.SetActive(false);
                mudarEstado(estadoInimigo.RECUAR);
                break;

            case 1:
                atacando = true;
                break;
        }
    }

    void controleArma(int id)
    {
        if (!podeUsarArma()) return;

        foreach (GameObject o in armas)
        {
            o.SetActive(false);
        }
        armas[id].SetActive(true);
    }
    
    public bool podeUsarArma()  // Verifica se o inimigo pode usar a arma
    {
        return !levandoDano && estadoInimigoAtual != estadoInimigo.RECUAR;
    }

}