using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SalvareCarregar : MonoBehaviour
{
    public static SaveData dadosCarregados;
    private scriptPersonagem scriptPersonagem;
    private _GameController _GameController;

    void Start()
    {
        _GameController = FindObjectOfType<_GameController>();
        scriptPersonagem = FindObjectOfType<scriptPersonagem>();
    }

    void Update()
    {

    }

    //SALVAR E CARREGAR JOGO
    public void Salvar()
    {        
        if(_GameController == null)
        {            
            return;
        }

        SaveData data = new SaveData();
        data.idPersonagem = _GameController.idPersonagemAtual;
        data.moeda = _GameController.moeda;
        data.idioma = _GameController.idioma;
        data.qtdBandagens = _GameController.qtdBandagens;
        data.vidaAtual = _GameController.vidaAtual;
        data.idArmaAtual = _GameController.idArmaAtual;
        data.zumbisMortosIDs = _GameController.zumbisMortosIDs;
        data.bausAbertosIDs = _GameController.bausAbertosIDs; // IDs de todos os baús abertos
        data.tempoPartida = _GameController.tempoPartida;

        if (scriptPersonagem != null)
        {
            Vector3 pos = scriptPersonagem.transform.position;
            data.posX = pos.x;
            data.posY = pos.y;
            data.posZ = pos.z;
        }

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        Debug.Log("Save salvo em: " + Application.persistentDataPath + "/savefile.json");
    }

    public void Carregar()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            dadosCarregados = JsonUtility.FromJson<SaveData>(json);            

            SceneManager.LoadScene("Cena 1");
        }
        else
        {
            Debug.Log("Nenhum save encontrado");
        }
    }

    [System.Serializable]
    public class SaveData
    {
        public int idPersonagem;
        public int moeda;
        public int idioma;
        public int qtdBandagens;
        public int vidaAtual;
        public int idArmaAtual;
        public List<int> zumbisMortosIDs = new List<int>();
        public List<int> bausAbertosIDs = new List<int>(); // IDs de todos os baús abertos
        public float tempoPartida;

        //posição do personagem
        public float posX;
        public float posY;
        public float posZ;
    }
}