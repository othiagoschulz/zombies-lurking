using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Linq;

public class carregarArmas : MonoBehaviour
{
    private _GameController _GameController;

    public string nomeArquivoXml;       //NOME DO ARQUIVO DO XML
    public List<string> nomeArma;       //ARMAZENA O NOME DA ARMA, QUE SERA EXIBIDO NO INVENTARIO
    public List<string> nomeIconeArma;  //NOME DO ICONE NO ARQUIVO SPRITESHEET
    public List<Sprite> iconeArma;      //ICONE EXIBIDO NO INVENTARIO
    public List<string> categoriaArma;  //ESPADA OU ARCO
    public List<int> idClasseArma;   
    public List<int> danoMinArma;
    public List<int> danoMaxArma;
    public List<int> tipoDano;

    public List<Sprite> spriteArmas1;        //SPRITE DE ARMAS 1
    public List<Sprite> spriteArmas2;        //SPRITE DE ARMAS 2
    public List<Sprite> spriteArmas3;        //SPRITE DE ARMAS 3

    public List<Sprite> bancoSpriteArma;    //ARMAZENA TODOS OS SPRITES DAS ARMAS TEMPORARIAMENTE
    public Sprite[] SpriteSheetIconeArma;
    public Sprite[] espadas;
    public Sprite[] arcos;
    public Sprite[] machados;

    private Dictionary<string, Sprite> SpriteSheetArmas;

    public Texture SpriteSheetIcone;
    public Texture SpriteSheetEspada;
    public Texture SpriteSheetArco;
    public Texture SpriteSheetMachado;

    void Start()
    {
        _GameController = FindObjectOfType(typeof(_GameController)) as _GameController;

        LoadData();
    }

    void LoadData() //RESPONSAVEL PELA LEITURA DO ARQUIVO XML E CARREGAMENTO DAS IMAGENS
    {
        SpriteSheetIconeArma = Resources.LoadAll<Sprite>(SpriteSheetIcone.name); //CARREGA OS ICONES DAS ARMAS

        espadas = Resources.LoadAll<Sprite>(SpriteSheetEspada.name); //CARREGA AS ESPADAS
        arcos = Resources.LoadAll<Sprite>(SpriteSheetArco.name); //CARREGA OS ARCOS
        machados = Resources.LoadAll<Sprite>(SpriteSheetMachado.name); //CARREGA OS MACHADOS

        foreach (Sprite s in espadas) //CARREGA TODOS OS SPRITES DAS ESPADAS NO JOGO
        {
            bancoSpriteArma.Add(s);
        }

        foreach (Sprite s in arcos) //CARREGA TODOS OS SPRITES DOS ARCOS NO JOGO
        {
            bancoSpriteArma.Add(s);
        }

        foreach (Sprite s in machados) //CARREGA TODOS OS SPRITES DOS MACHADOS NO JOGO
        {
            bancoSpriteArma.Add(s);
        }

        SpriteSheetArmas = bancoSpriteArma.ToDictionary(x => x.name, x => x);

        //LEITURA DO XML
        TextAsset arquivoXML = (TextAsset)Resources.Load(_GameController.idiomaFolder[_GameController.idioma] + "/" + nomeArquivoXml);   //LE O ARQUIVO XML, CONVERTE PARA TEXTASSET E ARMAZENA NA VARIAVEL "arquivoXml"
        XmlDocument documentoXML = new XmlDocument();   //CRIANDO UMA NOVA VARIAVEL E DIZENDO QUE ELA É UM NOVO DOCUMENTO XML
        documentoXML.LoadXml(arquivoXML.text);

        foreach (XmlNode atributo in documentoXML["Armas"].ChildNodes)
        {
            string att = atributo.Attributes["atributo"].Value;

            foreach (XmlNode a in atributo["armas"].ChildNodes)
            {
                switch (att)
                {
                    case "nome":
                        nomeArma.Add(a.InnerText);
                        break;

                    case "icone":
                        nomeIconeArma.Add(a.InnerText);

                        //CARREGA O ICONE 
                        for (int i = 0; i < SpriteSheetIconeArma.Length; i++)
                        {
                            if (SpriteSheetIconeArma[i].name == a.InnerText)
                            {
                                iconeArma.Add(SpriteSheetIconeArma[i]);
                                break;
                            }
                        }
                        break;

                    case "categoria":
                        categoriaArma.Add(a.InnerText);
                        idClasseArma.Add(0);
                        break;

                    case "danoMin":
                        danoMinArma.Add(int.Parse(a.InnerText));
                        break;

                    case "danoMax":
                        danoMaxArma.Add(int.Parse(a.InnerText));
                        break;

                    case "tipoDano":
                        tipoDano.Add(int.Parse(a.InnerText));
                        break;
                }
            }
        }

        for (int i = 0; i < iconeArma.Count; i++)
        {
            spriteArmas1.Add(SpriteSheetArmas[nomeIconeArma[i] + "0"]);
            spriteArmas2.Add(SpriteSheetArmas[nomeIconeArma[i] + "1"]);
            spriteArmas3.Add(SpriteSheetArmas[nomeIconeArma[i] + "2"]);
        }

        atualizarGameController();
    }

    public void atualizarGameController()
    {
        _GameController.nomeArma = nomeArma;
        _GameController.idClasseArma = idClasseArma;
        _GameController.danoMinimo = danoMinArma;
        _GameController.danoMaximo = danoMaxArma;
        _GameController.tipoDanoArma = tipoDano;
        _GameController.imgInventario = iconeArma;

        _GameController.spriteArmas1 = spriteArmas1;
        _GameController.spriteArmas2 = spriteArmas2;
        _GameController.spriteArmas3 = spriteArmas3;
    }
}
