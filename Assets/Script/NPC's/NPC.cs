using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using TMPro;

public class NPC : MonoBehaviour
{
    private _GameController _GameController;
    private pauseScript pauseScript;
    public string nomeArquivoXML;
    public GameObject CanvasNPC;    //BACKGROUND DO TEXTO
    public TMP_Text texto;      //TEXTO DO NPC

    public List<string> fala;   //VARIAVEL QUE ARMAZENA AS FALAS DO NPC
    public List<string> fala1;
    public List<string> linhasDialogo;

    public int idFala;      //IDENTIFICADOR AS FALAS
    public bool dialogoOn;      //VARIAVEL QUE VERIFICA SE O DIALOGO ESTA ACONTECENDO OU NÃO
    public int idDialogo;

    void Start()
    {
        pauseScript = FindObjectOfType(typeof(pauseScript)) as pauseScript;
        _GameController = FindObjectOfType(typeof(_GameController)) as _GameController;
        carregarDialogoXml();
    }

    void Update()
    {

    }

    public void interacao()
    {
        if (pauseScript.estadoAtual == maquinaEstado.GAMEPLAY) ;
        {
            pauseScript.mudarEstado(maquinaEstado.DIALOGO);

            idFala = 0;            
            prepararDialogo();
            dialogo();            
            CanvasNPC.SetActive(true);
            dialogoOn = true;            
        }        
    }

    public void dialogo()
    {
        if (idFala < linhasDialogo.Count)
        {
            texto.text = linhasDialogo[idFala];
        }
        else    //A CONVERSA TERMINA
        {
            switch (idDialogo)
            {
                case 0:
                    idDialogo = 1;      //MANDA A CONVERSA PARA O PROXIMO DIALOGO
                    break;
                case 1:

                    break;
            }
            CanvasNPC.SetActive(false);
            dialogoOn = false;
            pauseScript.mudarEstado(maquinaEstado.FIMDIALOGO);
        }
    }

    public void falar()
    {
        if (dialogoOn == true)
        {
            idFala += 1;
            dialogo();
        }
    }
    void prepararDialogo()
    {
        linhasDialogo.Clear();
        switch (idDialogo)
        {
            case 0:
                foreach (string s in fala)
                    linhasDialogo.Add(s);
                break;
            case 1:
                foreach (string s in fala1)
                    linhasDialogo.Add(s);
                break;
        }
    }

    void carregarDialogoXml()   //LE O ARQUIVO XML DO NPC
    {
        TextAsset arquivoXML = (TextAsset)Resources.Load(_GameController.idiomaFolder[_GameController.idioma] + "/" + nomeArquivoXML);   //LE O ARQUIVO XML, CONVERTE PARA TEXTASSET E ARMAZENA NA VARIAVEL "arquivoXml"
        XmlDocument documentoXML = new XmlDocument();   //CRIANDO UMA NOVA VARIAVEL E DIZENDO QUE ELA É UM NOVO DOCUMENTO XML
        documentoXML.LoadXml(arquivoXML.text);

        foreach (XmlNode dialogo in documentoXML["dialogos"].ChildNodes)
        {
            string nomeDialogo = dialogo.Attributes["name"].Value;

            foreach (XmlNode f in dialogo["falas"].ChildNodes)
            {
                switch (nomeDialogo)
                {
                    case "fala":
                        fala.Add(_GameController.textoFormatado(f.InnerText));
                        break;
                    case "fala1":
                        fala1.Add(_GameController.textoFormatado(f.InnerText));
                        break;
                }
            }
        }
    }    
}
