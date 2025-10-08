using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

public class _GameController : MonoBehaviour
{
    private transicao transicao;
    public int idioma;
    public string[] idiomaFolder;
    private pauseScript pauseScript;
    private invScript invScript;
    private scriptPersonagem scriptPersonagem;
    public string[] tiposDano;
    public GameObject[] fxDano;
    public GameObject fxMorte;
    public int moeda;
    public TextMeshProUGUI dinheiroTXT;

    [Header("Informações do Personagem")]
    public int idPersonagem;
    public int idPersonagemAtual;
    public int vidaMaxima;
    public int vidaAtual;
    public int idArma, idArmaAtual;
    public int qtdBandagens;

    [Header("Banco de Personagem")]
    public string[] nome;
    public Texture[] nomeSpriteSheet;
    public int[] idClasse;
    public GameObject[] armaInicial;
    public int idArmaInicial;

    [Header("Banco de Dados Armas")]
    public List<string> nomeArma;
    public List<Sprite> imgInventario;
    public List<int> custoArma;
    public List<int> idClasseArma;
    public List<Sprite> spriteArmas1;
    public List<Sprite> spriteArmas2;
    public List<Sprite> spriteArmas3;
    public List<int> danoMinimo;
    public List<int> danoMaximo;
    public List<int> tipoDanoArma;

    [Header("Primeiro elemento de cada painel")]
    public Button primeiroPauseMenu;
    public Button primeiroInventario;
    public Button primeiroInfoInventario;

    [Header("HUD Zumbis")]
    public int zumbisMortos = 0;
    public TextMeshProUGUI zumbisMortosTXT;
    public List<int> zumbisMortosIDs = new List<int>();

    [Header("HUD Baús Floresta")]
    public int bausAbertos = 0;
    public List<int> bausAbertosIDs = new List<int>();
    public TextMeshProUGUI bausTXT;

    [Header("Tempo de Partida")]
    public float tempoPartida = 0f;
    public TextMeshProUGUI tempoTXT;
    private bool jogoFinalizado = false;

    [Header("Tela de Vitória")]
    public GameObject painelVitoria;
    public TextMeshProUGUI zumbisFinalTXT;
    public TextMeshProUGUI moedasFinalTXT;
    public TextMeshProUGUI tempoFinalTXT;

    [Header("Fim de Jogo")]
    public GameObject painelFimdeJogo;
    public Button botaoReset;
    public TextMeshProUGUI textoPiscar;
    public bool morto = false;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        transicao = FindObjectOfType(typeof(transicao)) as transicao;
        pauseScript = FindObjectOfType(typeof(pauseScript)) as pauseScript;
        invScript = FindObjectOfType(typeof(invScript)) as invScript;
        scriptPersonagem = FindObjectOfType(typeof(scriptPersonagem)) as scriptPersonagem;        

        if (FindObjectsOfType<_GameController>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        transicao.fadeOut();

        painelFimdeJogo.SetActive(false);
        painelVitoria.SetActive(false);

        idPersonagem = PlayerPrefs.GetInt("idPersonagem");

        if (SalvareCarregar.dadosCarregados == null)
        {
            invScript.itemInv.Add(armaInicial[idPersonagem]);

            GameObject tArma = Instantiate(armaInicial[idPersonagem]);
            invScript.itemCarregado.Add(tArma);

            armaInfo info = tArma.GetComponent<armaInfo>();
            if (info != null)
            {
                int id = idArmaInicial;
                info.danoMin = danoMinimo[id];
                info.danoMax = danoMaximo[id];
                info.tipoDano = tipoDanoArma[id];
            }

            idArmaInicial = tArma.GetComponent<item>().idItem;
        }

        vidaAtual = vidaMaxima;

        if (SalvareCarregar.dadosCarregados != null)
        {
            var data = SalvareCarregar.dadosCarregados;

            idPersonagemAtual = data.idPersonagem;
            moeda = data.moeda;
            idioma = data.idioma;
            qtdBandagens = data.qtdBandagens;
            vidaAtual = data.vidaAtual;
            idArmaAtual = data.idArmaAtual;
            tempoPartida = data.tempoPartida;
            scriptPersonagem.transform.position = new Vector3(data.posX, data.posY, data.posZ);

            zumbisMortosIDs = data.zumbisMortosIDs ?? new List<int>();
            zumbisMortos = zumbisMortosIDs.Count;
            if (zumbisMortosTXT != null)
                zumbisMortosTXT.text = zumbisMortos + "x";

            bausAbertosIDs = data.bausAbertosIDs ?? new List<int>();
            bausAbertos = FindObjectsOfType<bau>().Count(b => b.tag == "bauFloresta" && bausAbertosIDs.Contains(b.idBau));

            if (bausTXT != null)
                bausTXT.text = bausAbertos.ToString() + "x";

            // força atualização dos baús na cena
            foreach (var bau in FindObjectsOfType<bau>())
            {
                if (bausAbertosIDs.Contains(bau.idBau))
                {
                    bau.open = true;
                    var sr = bau.GetComponent<SpriteRenderer>();
                    sr.sprite = bau.imagemObjeto[1];
                    bau.GetComponent<Collider2D>().enabled = false;
                }
            }

            // Recria inventário pelo ID
            if (invScript != null)
            {
                invScript.itemInv.Clear();
                foreach (int id in data.inventarioIDs)
                {
                    GameObject prefab = EncontrarArmaPorID(id);
                    if (prefab != null)
                        invScript.itemInv.Add(prefab);
                }

                invScript.carregarInv();
            }      

            SalvareCarregar.dadosCarregados = null;
        }        
    }

    void Update()
    {
        string s = moeda.ToString("N0");

        dinheiroTXT.text = s.Replace(",", ".");

        validarArma();

        if (!jogoFinalizado)
        {
            tempoPartida += Time.deltaTime;

            if (tempoTXT != null)
                tempoTXT.text = FormatarTempo(tempoPartida);
        }

        if (vidaAtual <= 0 && !morto)
        {
            morto = true;
            vidaAtual = 0;
            scriptPersonagem.animacaoPersonagem.Play("Morte");

            scriptPersonagem.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

            scriptPersonagem.enabled = false;

            StartCoroutine(FimdeJogo());
        }

        PiscarTexto();
    }

    public void validarArma()
    {
        if (idClasseArma[idArma] != idClasse[idPersonagem])
        {     
            idArma = idArmaInicial;
        }
    }

    public void usarItemArma(int idArma)
    {
        idArmaAtual = idArma;
        scriptPersonagem.trocarArma(idArma);
    }

    public void equiparItens(int idSlot)
    {
        GameObject temp1 = invScript.itemInv[0];
        GameObject temp2 = invScript.itemInv[idSlot];

        invScript.itemInv[0] = temp2;
        invScript.itemInv[idSlot] = temp1;

        item itemInfo = temp2.GetComponent<item>();
        if (itemInfo != null)
            idArmaAtual = itemInfo.idItem;

        usarItemArma(idArmaAtual);
    

        pauseScript.voltarAoJogo();
    }

    public void coletarArma(GameObject objetoColetado)
    {
        invScript.itemInv.Add(objetoColetado);
    }

    public void usarBandagem()
    {
        if (!morto && qtdBandagens >= 1)
        {
            qtdBandagens -= 1;
            vidaAtual += 2;
            if (vidaAtual > vidaMaxima)
            {
                vidaAtual = vidaMaxima;
            }
        }
    }

    public string textoFormatado(string frase)
    {
        string tempcor = frase.Replace("cor=red", "<color=#FF0000FF>");
        tempcor = tempcor.Replace("fimcor", "</color>");
        tempcor = tempcor.Replace("negrito", "<b>");
        tempcor = tempcor.Replace("fimngt", "</b>");

        return tempcor;
    }

    public void ZumbiMorto(int idZumbi)
    {
        if (!zumbisMortosIDs.Contains(idZumbi))
        {
            zumbisMortos++;
            zumbisMortosIDs.Add(idZumbi);
            if (zumbisMortosTXT != null)
                zumbisMortosTXT.text = zumbisMortos.ToString() + "x";
        }
    }

    public void BauAberto(int idBau, string tagBau)
    {
        if (!bausAbertosIDs.Contains(idBau))
        {
            bausAbertosIDs.Add(idBau);

            if (tagBau == "bauFloresta")
            {
                bausAbertos++;
                if (bausTXT != null)
                    bausTXT.text = bausAbertos.ToString() + "x";

                if (bausAbertos >= 3 && !jogoFinalizado)
                {
                    StartCoroutine(Vitoria());
                }
            }
        }
    }

    private GameObject EncontrarArmaPorID(int idArma)
    {
        foreach (GameObject arma in Resources.LoadAll<GameObject>("Armas"))
        {
            if (arma == null) continue;
            item itemInfo = arma.GetComponent<item>();
            if (itemInfo != null && itemInfo.idItem == idArma)
                return arma;
        }
        return null;
    }

    IEnumerator FimdeJogo()
    {
        yield return new WaitForSeconds(1f);

        painelVitoria.SetActive(false);
        painelFimdeJogo.SetActive(true);

        CanvasGroup cg = painelFimdeJogo.GetComponent<CanvasGroup>();
        if (cg == null) cg = painelFimdeJogo.AddComponent<CanvasGroup>();
        cg.interactable = true;
        cg.blocksRaycasts = true;
        cg.alpha = 1f;

        Time.timeScale = 0f;
    }

    IEnumerator Vitoria()
    {
        jogoFinalizado = true;
        yield return new WaitForSeconds(1f);

        Time.timeScale = 0f;

        painelVitoria.SetActive(true);

        CanvasGroup cg = painelVitoria.GetComponent<CanvasGroup>();
        if (cg == null) cg = painelVitoria.AddComponent<CanvasGroup>();
        cg.interactable = true;
        cg.blocksRaycasts = true;
        cg.alpha = 1f;

        if (zumbisFinalTXT != null)
            zumbisFinalTXT.text = zumbisMortos.ToString();

        if (moedasFinalTXT != null)
            moedasFinalTXT.text = moeda.ToString("N0").Replace(",", ".");

        if (tempoFinalTXT != null)
            tempoFinalTXT.text = FormatarTempo(tempoPartida);
    }

    public void ResetarCena()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        painelFimdeJogo.SetActive(false);
    }

    private void PiscarTexto()
    {
        if (textoPiscar == null) return;

        float alpha = Mathf.Abs(Mathf.Sin(Time.unscaledTime * 3f)); // 3f = velocidade do piscar
        Color c = textoPiscar.color;
        c.a = alpha;
        textoPiscar.color = c;
    }

    public void btnJogarNovamente()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void btnVoltarAoMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public string FormatarTempo(float tempo)
    {
        int minutos = Mathf.FloorToInt(tempo / 60f);
        int segundos = Mathf.FloorToInt(tempo % 60f);
        return string.Format("{0:00}:{1:00}", minutos, segundos);
    }
}
