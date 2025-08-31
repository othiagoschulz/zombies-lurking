using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class painelItemInfo : MonoBehaviour
{
    private pauseScript pauseScript;
    private _GameController _GameController;
    public int idSlot;
    public GameObject objetoSlot;

    [Header("Configuração dos Itens")]
    public Image imgItem;
    public TMP_Text nomeItem;
    public TMP_Text danoItem;
    public Button btnEquipar;
    public Button btnExcluir;

    void Start()
    {
        pauseScript = FindObjectOfType(typeof(pauseScript)) as pauseScript;
        _GameController = FindObjectOfType(typeof(_GameController)) as _GameController;
    }

    public void carregarInfoItem()
    {
        item itemInfo = objetoSlot.GetComponent<item>();
        int idArma = itemInfo.idItem;

        imgItem.sprite = _GameController.imgInventario[idArma];
        nomeItem.text = _GameController.nomeArma[idArma];

        string tipoDano = _GameController.tiposDano[_GameController.tipoDanoArma[idArma]];
        int danoMin = _GameController.danoMinimo[idArma];
        int danoMax = _GameController.danoMaximo[idArma];
        danoItem.text = "Dano: " + danoMin.ToString() + "-" + danoMax.ToString() + " / " + tipoDano;

        if (idSlot == 0)
        {
            btnEquipar.interactable = false;
            btnExcluir.interactable = false;
        }
        else
        {
            int idClasseArma = _GameController.idClasseArma[idArma];
            int idClassePersonagem = _GameController.idClasse[_GameController.idPersonagem];

            if (idClasseArma == idClassePersonagem)
            {
                btnEquipar.interactable = true;
                btnExcluir.interactable = true;
            }
            else
            {
                btnEquipar.interactable = false;                
            }
        }
    }

    public void btEquipar()
    {
        objetoSlot.SendMessage("usarItem", SendMessageOptions.DontRequireReceiver);
        _GameController.equiparItens(idSlot);
    }

    public void btExcluir()
    {
        pauseScript.excluirItem(idSlot);
    }
}
