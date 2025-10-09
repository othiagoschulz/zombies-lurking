using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptPersonagem : MonoBehaviour
{

    private pauseScript pauseScript;
    private _GameController _GameController;
    private IAzumbi IAzumbi;
    private armaInfo armaInfo;
    public Animator animacaoPersonagem;
    public Rigidbody2D personagemRb;
    public Transform chaoCheck;              //DETECTA SE O PERSONAGEM ESTÁ EM CIMA DE ALGO
    public LayerMask oQueEChao;              //INDICA O QUE É CHAO PARA O TESTE DO CHAO

    public bool emDano;

    public int vidaMax, vidaAtual;

    public float velocidade;             //INDICA A VELOCIDADE DO PERSONAGEM
    public float forcaPulo;              //INDICA A FORÇA APLICADA PARA GERAR O PULO DO PERSONAGEM
    public bool olhandoEsquerda;        //FAZ O PERSONAGEM OLHAR PARA O OUTRO LADO
    public bool Chao;                   //INDICA SE O PERSONAGEM ESTÁ PISANDO NO CHÃO
    public bool atacando;               //INDICA SE O PERSONAGEM ESTÁ EFETUANDO UM ATAQUE
    public int idAnimacao;             //INDICA O ID DA ANIMAÇÃO
    private float h, v;
    public Collider2D emPe, agachado;         //COLISOR EM PÉ E AGACHADO

    //INTERAÇÃO COM ITENS E OBJETOS
    public Transform mao;
    private Vector3 dir = Vector3.right;
    public LayerMask interacao;
    public GameObject objetoInteracao;

    //SISTEMA DAS ARMAS
    public int idArma;
    public int idArmaAtual;
    public GameObject[] armas, arcos, flechaArco;
    public Transform spawnFlecha;
    public GameObject balaoAlerta;

    void Start()
    {
        pauseScript = FindObjectOfType(typeof(pauseScript)) as pauseScript;
        _GameController = FindObjectOfType(typeof(_GameController)) as _GameController; //LINHA QUE FAZ O SCRIPTPERSONAGEM TER ACESSO AO SCRIPT _GAMECONTROLLER

        personagemRb = GetComponent<Rigidbody2D>();     //INICIALIZA O RIGIDBODY DENTRO DA VARIAVEL
        animacaoPersonagem = GetComponent<Animator>();  //INICIALIZA O ANIMATOR DENTRO DA VARIAVEL

        //CARREGA OS DADOS INICIAIS DO PERSONAGEM
        vidaMax = _GameController.vidaMaxima;
        vidaAtual = vidaMax;

        idArma = _GameController.idArmaAtual;
        trocarArma(idArma);

        armaInfo = FindObjectOfType(typeof(armaInfo)) as armaInfo;

        foreach (GameObject o in armas)
        {         //DESABILITA AS ARMAS QUANDO O JOGO INICIAR, PARA NÃO ACONTECER BUGS
            o.SetActive(false);
        }

        foreach (GameObject o in arcos)
        {         //DESABILITA OS ARCOS QUANDO O JOGO INICIAR, PARA NÃO ACONTECER BUGS
            o.SetActive(false);
        }
    }

    void FixedUpdate()
    {     //TAXA DE ATUALIZAÇÃO FIXA 0.02, CLASSE PARA CRIAR MOVIMENTOS FÍSICOS
        if (emDano)
        {
            personagemRb.linearVelocity = Vector2.zero;
            return;
        }


        if (pauseScript.estadoAtual != maquinaEstado.GAMEPLAY)
        {  //SE O ESTADO DE JOGO FOR QUALQUER UM QUE N SEJA GAMEPLAY, ELE PARA DE EXECUTAR O COMANDO E O CODIGO ABAIXO N É EXECUTADO
            return;
        }

        Chao = Physics2D.OverlapCircle(chaoCheck.position, 0.02f, oQueEChao);
        personagemRb.linearVelocity = (new Vector2(h * velocidade, personagemRb.linearVelocity.y));
        interagir();

    }

    void Update()
    {
        AnimatorStateInfo current = animacaoPersonagem.GetCurrentAnimatorStateInfo(0);
        if (emDano)
        {
            // Interrompe ataque e desativa armas/arcos ao entrar em dano
            if (atacando)
            {
                atacando = false;
                foreach (GameObject o in armas) o.SetActive(false);
                foreach (GameObject o in arcos) o.SetActive(false);
            }
            // Quando sair da animação de dano, restaura velocidade
            if (!current.IsTag("Dano"))
            {
                emDano = false;
                StartCoroutine(RestaurarVelocidade());
            }
            return;
        }

        if (pauseScript.estadoAtual == maquinaEstado.DIALOGO)
        {
            personagemRb.linearVelocity = new Vector2(0, personagemRb.linearVelocity.y);
            animacaoPersonagem.SetInteger("idAnimacao", 0);
            if (Input.GetButtonDown("Fire1"))
            {
                objetoInteracao.SendMessage("falar", SendMessageOptions.DontRequireReceiver);
            }
            return;
        }

        if (pauseScript.estadoAtual != maquinaEstado.GAMEPLAY)
        {
            return;
        } 

        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        if (h > 0 && olhandoEsquerda && !atacando)
            flip();
        else if (h < 0 && !olhandoEsquerda && !atacando)
            flip();

        // Atualiza animação de movimento
        if (v < 0)
        {
            idAnimacao = 2;
            if (Chao) h = 0;
        }
        else if (h != 0)
            idAnimacao = 1;
        else
            idAnimacao = 0;

        // Ataques    
        if (Input.GetButtonDown("Fire1") && v >= 0 && !atacando)
        {
            if (objetoInteracao == null)
                animacaoPersonagem.SetTrigger("ataque");
            else
            {
                if (objetoInteracao.tag == "porta")
                    objetoInteracao.GetComponent<porta>().tPersonagem = this.transform;

                objetoInteracao.SendMessage("interacao", SendMessageOptions.DontRequireReceiver);
            }
        }

        // Pulo
        if (Input.GetButtonDown("Jump") && Chao)
            personagemRb.AddForce(new Vector2(0, forcaPulo));

        // Troca de armas
        if (Input.GetKeyDown(KeyCode.Alpha1))
            trocarArma(0);

        if (atacando && Chao)
            h = 0;

        // Colisores de agachar
        if (v < 0 && Chao)
        {
            agachado.enabled = true;
            emPe.enabled = false;
        }
        else
        {
            agachado.enabled = false;
            emPe.enabled = true;
        }

        // Atualiza parâmetros do Animator
        animacaoPersonagem.SetBool("Chao", Chao);
        animacaoPersonagem.SetInteger("idAnimacao", idAnimacao);
        animacaoPersonagem.SetFloat("velocidadeY", personagemRb.linearVelocity.y);
        animacaoPersonagem.SetFloat("idClasseArma", _GameController.idClasseArma[_GameController.idArmaAtual]);        

        // Interações
        interagir();
    }

    void LateUpdate()
    {
        if (_GameController.idArma != _GameController.idArmaAtual)
        {
            trocarArma(_GameController.idArma);
        }
    }

    void flip()
    {
        olhandoEsquerda = !olhandoEsquerda;
        float x = transform.localScale.x * -1;
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);

        dir = olhandoEsquerda ? Vector3.left : Vector3.right;
    }

    void atack(int atk)
    {
        switch (atk)
        {
            case 0:
                atacando = false;
                armas[2].SetActive(false);
                break;

            case 1:
                atacando = true;
                break;
        }
    }

    void interagir()
    {
        Debug.DrawRay(mao.position, dir * 0.2f, Color.blue);
        RaycastHit2D hit = Physics2D.Raycast(mao.position, dir, 0.2f, interacao);

        if (hit == true)
        {
            objetoInteracao = hit.collider.gameObject;
            balaoAlerta.SetActive(true);
        }
        else
        {
            objetoInteracao = null;
            balaoAlerta.SetActive(false);
        }
    }

    void controleArma(int id)
    {
        foreach (GameObject o in armas)
        {
            o.SetActive(false);
        }
        armas[id].SetActive(true);
    }

    void controleArco(int id)
    {
        foreach (GameObject o in arcos)
        {
            o.SetActive(false);
        }
        arcos[id].SetActive(true);
    }
    void OnTriggerEnter2D(Collider2D col)
    {      //FUNÇÃO QUE DESTROI A MOEDA QUANDO O PERSONAGEM COLIDE COM ELA
        switch (col.gameObject.tag)
        {
            case "coletavel":
                col.gameObject.SendMessage("coletar", SendMessageOptions.DontRequireReceiver);
                break;
        }
    }

    public void trocarArma(int id)
    {
        idArma = id;

        _GameController.idArma = id;

        switch (_GameController.idClasseArma[id])
        {

            case 0: //ESPADA
                armas[0].GetComponent<SpriteRenderer>().sprite = _GameController.spriteArmas1[id];

                armaInfo tempArmaInfo = armas[0].GetComponent<armaInfo>();

                tempArmaInfo = armas[0].GetComponent<armaInfo>();
                tempArmaInfo.danoMin = _GameController.danoMinimo[idArma];
                tempArmaInfo.danoMax = _GameController.danoMaximo[idArma];
                tempArmaInfo.tipoDano = _GameController.tipoDanoArma[idArma];

                armas[1].GetComponent<SpriteRenderer>().sprite = _GameController.spriteArmas2[id];

                tempArmaInfo = armas[1].GetComponent<armaInfo>();
                tempArmaInfo.danoMin = _GameController.danoMinimo[idArma];
                tempArmaInfo.danoMax = _GameController.danoMaximo[idArma];
                tempArmaInfo.tipoDano = _GameController.tipoDanoArma[idArma];

                armas[2].GetComponent<SpriteRenderer>().sprite = _GameController.spriteArmas3[id];

                tempArmaInfo = armas[2].GetComponent<armaInfo>();
                tempArmaInfo.danoMin = _GameController.danoMinimo[idArma];
                tempArmaInfo.danoMax = _GameController.danoMaximo[idArma];
                tempArmaInfo.tipoDano = _GameController.tipoDanoArma[idArma];

                break;

            case 1: //ARCO

                arcos[0].GetComponent<SpriteRenderer>().sprite = _GameController.spriteArmas1[id];
                arcos[1].GetComponent<SpriteRenderer>().sprite = _GameController.spriteArmas2[id];
                arcos[2].GetComponent<SpriteRenderer>().sprite = _GameController.spriteArmas3[id];

                break;
        }
        _GameController.idArmaAtual = _GameController.idArma;
    }
    
    IEnumerator RestaurarVelocidade()
    {
        yield return new WaitForSeconds(0.3f);
        velocidade = 2f;
    }
}