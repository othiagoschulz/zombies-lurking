using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controleDanoInimigo : MonoBehaviour
{
    public _GameController _GameController;
    private scriptPersonagem scriptPersonagem;
    private SpriteRenderer sRender;
    private Animator animator;

    [Header("Configuração da vida")]
    public int vidaInimigo;        //VIDA DO INIMIGO
    public int vidaAtual;
    public GameObject barrasVida;         //OBJETO QUE CONTEM AS BARRAS
    public Transform barraVida;          //OBJETO QUE INDICA A VIDA
    public Color[] corInimigo;         //CONTROLE DE COR DO INIMIGO
    private float contVida;           //CONTROLE DA VIDA  
    public GameObject danoTextoPefab;     //OBJETO QUE EXIBIRA O DANO
    public bool atacando = false;


    [Header("Configuração de resistencia")]
    public float[] ajusteDano;
    public bool IolhandoEsquerda, playerEsquerda;
    //KNOCKBACK
    [Header("Configuração do Knockback")]
    public GameObject forcaKnockPrefab;   //força da repulsão
    public Transform posicaoKnock;       //posição de origem
    public float knockX;             //valor padrão do position X
    private float kx;
    private bool verDano;            //VERIFICA SE TOMOU DANO
    public bool morto;              //INDICA SE O INIMIGO ESTÁ MORTO

    [Header("Configuração do Chão")]
    public Transform chaoCheck;          //INDICA SE O INIMIGO ESTA SOBRE ALGUMA SUPERFICIE
    public LayerMask oQueEChao;

    [Header("Configuração do loot")]
    public GameObject loots;

    void Start()
    {
        _GameController = FindObjectOfType(typeof(_GameController)) as _GameController;
        scriptPersonagem = FindObjectOfType(typeof(scriptPersonagem)) as scriptPersonagem;
        sRender = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        barrasVida.SetActive(false);
        vidaAtual = vidaInimigo;
        barraVida.localScale = new Vector3(1, 1, 1);

        sRender.color = corInimigo[0];

        if (IolhandoEsquerda == true)
        {
            float x = transform.localScale.x;
            x *= -1;                                    //INVERTE O SINAL DO SCALE X
            transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
            barrasVida.transform.localScale = new Vector3(x, barrasVida.transform.localScale.y, barrasVida.transform.localScale.z);

        }
    }

    void Update()
    {
        //VERIFICAÇÃO SE O PERSONAGEM ESTÁ A DIREITA OU A ESQUERDA DO INIMIGO
        float playerX = scriptPersonagem.transform.position.x;
        if (playerX < transform.position.x)
        {
            playerEsquerda = true;
        }
        else if (playerX > transform.position.x)
        {
            playerEsquerda = false;
        }

        if (IolhandoEsquerda == true && playerEsquerda == true)
        {
            kx = knockX;
        }
        else if (IolhandoEsquerda == false && playerEsquerda == true)
        {
            kx = knockX * -1;
        }
        else if (IolhandoEsquerda == true && playerEsquerda == false)
        {
            kx = knockX * -1;
        }
        else if (IolhandoEsquerda == false && playerEsquerda == false)
        {
            kx = knockX;
        }


        posicaoKnock.localPosition = new Vector3(kx, posicaoKnock.localPosition.y, 0);

        animator.SetBool("Chao", true);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (morto == true) { return; }      //ESSE IF FAZ COM QUE NADA ABAIXO SEJA EXECUTADO
        switch (col.gameObject.tag)
        {
            case "armaPersonagem":

                if (verDano == false)
                {
                    verDano = true;
                    barrasVida.SetActive(true);
                    armaInfo armaInfo = col.gameObject.GetComponent<armaInfo>();
                    float danoArma = Random.Range(armaInfo.danoMin, armaInfo.danoMax);    //ATRIBUI UM DANO MINIMO OU MAXIMO DE 1 A 5 (1,2,3,4,5)
                    int tipoDano = armaInfo.tipoDano;

                    animator.SetTrigger("dano");

                    float danoTomado = danoArma + (danoArma * (ajusteDano[tipoDano] / 100));

                    vidaAtual -= Mathf.RoundToInt(danoTomado); //REDUZ A VIDA DO INIMIGO PELO DANO TOMADO

                    contVida = (float)vidaAtual / (float)vidaInimigo;
                    if (contVida < 0)
                    {
                        contVida = 0;
                    }

                    barraVida.localScale = new Vector3(contVida, 1, 1);

                    if (vidaAtual <= 0)
                    {
                        morto = true;

                        //zera a velocidade do zumbi ao morrer
                        IAzumbi ia = GetComponent<IAzumbi>();
                        ia.velocidade = 0;
                        ia.rBody.linearVelocity = Vector2.zero;
                        ia.levandoDano = true;

                        //bloqueia qualquer ação futura
                        ia.levandoDano = true;

                        //inicia animação de morte
                        animator.SetTrigger("morte");

                        ia.rBody.simulated = false;          // impede que o Rigidbody2D interaja
                        Collider2D colisor = GetComponent<Collider2D>();
                        if(colisor != null) colisor.enabled = false;        // opcional, desativa colisão

                        StartCoroutine("loot");          //COMEÇA A COROTINA LOOT
                    }

                    GameObject danotextoTemporario = Instantiate(danoTextoPefab, transform.position, transform.localRotation);          //INSTANCIANDO O OBJETO DE TEXTO AO DA DANO
                    danotextoTemporario.GetComponent<TextMesh>().text = Mathf.RoundToInt(danoTomado).ToString();            //ATRIBUI O DANO TOMADO CONVERTIDO PRA STRING NA VARIAVEL DANO TEMPORARIO
                    danotextoTemporario.GetComponent<MeshRenderer>().sortingLayerName = "HUD";          //COLOCA O TEXTO DE DANO NA LAYER "HUD" PRA FICAR NA FRENTE DE TUDO

                    GameObject fxTemporario = Instantiate(_GameController.fxDano[tipoDano], transform.position, transform.localRotation);       //CRIANDO A ANIMAÇÃO DE HIT
                    Destroy(fxTemporario, 0.3f);        //TEMPO QUE LEVA ATÉ A ANIMAÇÃO DE HIT ACABAR

                    int repulsaoX = 50;
                    if (playerEsquerda == false)
                    {        //SISTEMA DE VERIFICAÇÃO PRA SABER SE O PERSONAGEM ESTÁ A DIREITA DO INIMIGO
                        repulsaoX *= -1;                // <SE O INIMIGO ESTIVER A DIREITA O VALOR DO X INVERTE E O TEXTO É ARREMEÇADO PARA O LADO OPOSTO>
                    }
                    danotextoTemporario.GetComponent<Rigidbody2D>().AddForce(new Vector2(repulsaoX, 180));      //ADICIONA UMA FORÇA AO TEXTO 
                    Destroy(danotextoTemporario, 0.8f);     //DESTROI A VARIAVEL DANO TEXTO TEMPORARIO DEPOIS DE 0,8 SEGUNDOS, PARA NÃO SOBRECARREGAR O JOGO

                    GameObject knockTemporario = Instantiate(forcaKnockPrefab, posicaoKnock.position, posicaoKnock.localRotation);      //INSTANCIANDO O OBJETO PARA CAUSAR KNOCKBACK
                    Destroy(knockTemporario, 0.02f);

                    StartCoroutine("invulneravel");

                    this.gameObject.SendMessage("dano", SendMessageOptions.DontRequireReceiver);
                }
                break;
        }
    }
    void flip()
    {
        IolhandoEsquerda = !IolhandoEsquerda;         //INVERTE O VALOR DA VARIAVEL BOLEANA
        float x = transform.localScale.x;
        x *= -1;                                    //INVERTE O SINAL DO SCALE X
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
        barrasVida.transform.localScale = new Vector3(x, barrasVida.transform.localScale.y, barrasVida.transform.localScale.z);
    }

    IEnumerator loot()
    {
        yield return new WaitForSeconds(1);
        GameObject fxMorte = Instantiate(_GameController.fxMorte, chaoCheck.position, transform.localRotation); //INSTANCIA A ANIMAÇÃO DE MORTE
        yield return new WaitForSeconds(0.4f);
        sRender.enabled = false;

        //CONTROLE DE LOOT
        int qtdMoeda = Random.Range(1, 5);   //ADICIONA UM NUMERO ALEATORIO DE MOEDAS AO INIMIGO DE 1 A 5
        for (int l = 0; l < qtdMoeda; l++)
        {
            GameObject lootTemporario = Instantiate(loots, transform.position, transform.localRotation);    //INSTANCIANDO A MOEDA NO INIMIGO
            lootTemporario.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-30, 30), 70));    //ADICIONANDO FORÇA PARA CAUSAR UM EFEITO LEGAL NA MOEDA
            yield return new WaitForSeconds(0.1f);  //TEMPO DE DROPE ENTRE CADA MOEDA
        }

        yield return new WaitForSeconds(0.3f);

            // INCREMENTA O CONTADOR DE ZUMBIS MORTOS
        if (_GameController != null)
        {
            _GameController.ZumbiMorto();
        }

        Destroy(fxMorte);                   //OPERAÇÃO QUE DESTROI A ANIMAÇÃO DE MORTE
        Destroy(this.gameObject);           //OPERAÇÃO QUE DESTROI O LOOT
    }
    IEnumerator invulneravel()      //SISTEMA DE INVULNERABILIDADE, TROCA A OPACIDADE DO INIMIGO AO RECEBER DANO
    {             

        IAzumbi ia = GetComponent<IAzumbi>();
        ia.levandoDano = true;

        ia.mudarEstado(estadoInimigo.RECUAR);        

        sRender.color = corInimigo[1];
        yield return new WaitForSeconds(0.3f);
        sRender.color = corInimigo[0];
        yield return new WaitForSeconds(0.3f);
        sRender.color = corInimigo[1];
        yield return new WaitForSeconds(0.3f);
        sRender.color = corInimigo[0];

        verDano = false;                    //A VARIAVEL VERDANO RECEBE FALSO PARA QUE O INIMIGO POSSA RECEBER OUTRO DANO
        ia.levandoDano = false;
    }
}
