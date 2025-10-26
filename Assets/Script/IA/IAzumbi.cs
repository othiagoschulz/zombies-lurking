using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum estadoInimigo
{
    PARADO,
    ALERTA,
    PATRULHA,
    ATACANDO
}
public class IAzumbi : MonoBehaviour
{
    private scriptPersonagem scriptPersonagem;
    private _GameController _GameController;
    private controleDanoInimigo controleDanoInimigo;
    public Rigidbody2D rBody;
    private Animator animator;
    public estadoInimigo estadoInimigoAtual;
    public estadoInimigo estadoInimigoInicial;

    public float velocidadeBase;
    public float velocidade;

    public float tempoParado;
    public float tempoRecuo;

    public float tempoEntreAtaques = 1f; // intervalo entre ataques
    public bool podeAtacar = true;

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
        controleDanoInimigo = FindObjectOfType(typeof(controleDanoInimigo)) as controleDanoInimigo;

        if (controleDanoInimigo != null && _GameController != null)
        {
            // Se o zumbi já foi morto, destrói ele antes de executar qualquer coisa
            if (_GameController.zumbisMortosIDs.Contains(controleDanoInimigo.idZumbi))
            {
                Destroy(gameObject);
                return;
            }

            // Garante que o zumbi conhece o GameController
            controleDanoInimigo._GameController = _GameController;
        }


        rBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

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

        if (estadoInimigoAtual != estadoInimigo.ATACANDO)
        {
            Debug.DrawRay(transform.position, dir * distanciaVerPersonagem, Color.blue);
            RaycastHit2D hitPersonagem = Physics2D.Raycast(transform.position, dir, distanciaVerPersonagem, layerPersonagem);
            RaycastHit2D hitPersonagemTras = Physics2D.Raycast(transform.position, -dir, distanciaVerPersonagem, layerPersonagem);

            if (hitPersonagem == true)
            {
                mudarEstado(estadoInimigo.ALERTA);
            }
            else if (hitPersonagemTras)
            {
                flip();
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

        if (estadoInimigoAtual == estadoInimigo.ALERTA)
        {
            RaycastHit2D hitObstaculo = Physics2D.Raycast(transform.position, dir, distanciaMudarRota, layerObstaculo);
            if (hitObstaculo == true)
            {
                mudarEstado(estadoInimigo.PARADO);
                return; // evita continuar o código e cair
            }
        }

        if (!levandoDano && estadoInimigoAtual != estadoInimigo.ATACANDO)
        {
            rBody.linearVelocity = new Vector2(velocidade, rBody.linearVelocity.y);
        }

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
            Vector3 posPlayer = scriptPersonagem.transform.position;
            float dist = Vector3.Distance(transform.position, scriptPersonagem.transform.position);

            RaycastHit2D hitVisao = Physics2D.Raycast(transform.position, dir, distanciaAtaque, layerPersonagem);

            if (dist <= distanciaAtaque)
            {
                // força ataque se o player estiver MUITO próximo (mesmo se o trigger falhar)

                if (!atacando && podeAtacar)
                {
                    mudarEstado(estadoInimigo.ATACANDO);
                    atacando = true;
                }
            }
            else if (dist <= distanciaVerPersonagem * 0.35f)
            {
                // player muito perto mas sem raycast (caso de ultrapassar o raio de visão)                
                if (!atacando && podeAtacar)
                {
                    mudarEstado(estadoInimigo.ATACANDO);
                    atacando = true;
                }
            }
            else if (dist >= distanciaSairAlerta)
            {
                atacando = false;
                mudarEstado(estadoInimigo.PARADO);
            }
        }
        if (estadoInimigoAtual != estadoInimigo.ALERTA)
        {
            balaoAlerta.SetActive(false);
        }
    }

    private void AtacarPlayer()
    {
        if (!podeAtacar) return;

        if (scriptPersonagem != null)
        {
            scriptPersonagem.emDano = true;
            scriptPersonagem.animacaoPersonagem.Play("Dano");
            scriptPersonagem.velocidade = 0;
            scriptPersonagem.personagemRb.linearVelocity = Vector2.zero;

            foreach (GameObject arma in scriptPersonagem.armas)
                arma.SetActive(false);

            scriptPersonagem.atacando = false;
            _GameController.vidaAtual -= 1;

            StartCoroutine(cooldownAtaque());
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            mudarEstado(estadoInimigo.ATACANDO);
            AtacarPlayer();
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player") && podeAtacar && !levandoDano)
        {
            if (!atacando)
            {
                mudarEstado(estadoInimigo.ATACANDO);
            }
        }
    }

    public void flip()
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

    IEnumerator AplicarDanoComAtraso(float atraso)
    {
        yield return new WaitForSeconds(atraso);

        // cancela o ataque se o zumbi estiver levando dano
        if (levandoDano)
        {
            atacando = false;
            yield break;
        }

        // garante que o player ainda está perto e pode receber dano
        float dist = Vector3.Distance(transform.position, scriptPersonagem.transform.position);
        if (dist <= distanciaAtaque && podeAtacar && !levandoDano)
        {
            AtacarPlayer();
        }

        // finaliza o ataque
        yield return new WaitForSeconds(0.4f);
        atacando = false;
    }

    IEnumerator cooldownAtaque()
    {
        podeAtacar = false;
        atacando = false;

        yield return new WaitForSeconds(tempoEntreAtaques);

        podeAtacar = true;

        // Se o player ainda estiver por perto, reataca automaticamente
        float dist = Vector3.Distance(transform.position, scriptPersonagem.transform.position);
        if (dist <= distanciaAtaque && !levandoDano)
        {
            mudarEstado(estadoInimigo.ATACANDO); // <- força novo ataque
        }
        else if (dist <= distanciaVerPersonagem)
        {
            mudarEstado(estadoInimigo.ALERTA);
        }
        else
        {
            mudarEstado(estadoInimigo.PARADO);
        }
    } 

    public void mudarEstado(estadoInimigo novoEstado)
    {
        if (levandoDano)
        {
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
                velocidade = velocidadeBase * 2;
                balaoAlerta.SetActive(true);
                break;

            case estadoInimigo.ATACANDO:
                velocidade = 0;

                if (podeAtacar && !atacando)
                {
                    atacando = true;
                    animator.ResetTrigger("ataque");
                    animator.SetTrigger("ataque");

                    StartCoroutine(AplicarDanoComAtraso(0.4f));
                }
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
                mudarEstado(estadoInimigo.ALERTA);
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

    public void encerrarAtaque()
    {
        atacando = false;
        foreach (GameObject o in armas)
            o.SetActive(false);
        animator.ResetTrigger("ataque");
        mudarEstado(estadoInimigo.ALERTA);
    }

    public IEnumerator ResetarDano()
    {
        yield return new WaitForSeconds(0.5f);
        levandoDano = false;
    }
    
    public bool podeUsarArma()  // Verifica se o inimigo pode usar a arma
    {
        return !levandoDano;
    }
}