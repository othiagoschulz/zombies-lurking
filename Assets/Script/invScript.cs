using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class invScript : MonoBehaviour
{
    private _GameController _GameController;
    public Button[] slot;
    public Image[]  iconeItem;
    public TextMeshProUGUI  qtdBandagem, qtdFlechaN, qtdFlechaF, qtdFlechaO;
    public int      qBandagem, qFlechaN, qFlechaF, qFlechaO;
    public List<GameObject> itemInv;
    public List<GameObject> itemCarregado;
    void Start(){
        _GameController = FindObjectOfType(typeof(_GameController))as _GameController;

    }

    public void carregarInv(){

        limparItensCarregados();
        foreach(Button botao in slot){
            botao.interactable = false;
        }

        foreach(Image icon in iconeItem){
            icon.sprite = null;
            icon.gameObject.SetActive(false);
        }

        qtdBandagem.text = "x " + _GameController.qtdBandagens.ToString();
        qtdFlechaN.text = "x " + _GameController.qtdFlechas[0].ToString();
        qtdFlechaF.text = "x " + _GameController.qtdFlechas[1].ToString();
        qtdFlechaO.text = "x " + _GameController.qtdFlechas[2].ToString();

        int s = 0;  //ID SLOT
        foreach(GameObject item in itemInv){
            GameObject temp = Instantiate(item);

            item itemInfo = temp.GetComponent<item>();

            itemCarregado.Add(temp);

            slot[s].GetComponent<slotInv>().objetoSlot = temp;
            slot[s].interactable = true;

            iconeItem[s].sprite = _GameController.imgInventario[itemInfo.idItem];
            iconeItem[s].gameObject.SetActive(true);

            s++;
        }
    }

    public void limparItensCarregados(){
        foreach(GameObject ic in itemCarregado){
            Destroy(ic);
        }
        itemCarregado.Clear();
    }
}
