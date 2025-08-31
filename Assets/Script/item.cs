using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item : MonoBehaviour
{
    private _GameController _GameController;
    public int  idItem;
    void Start()
    {
        _GameController = FindObjectOfType(typeof(_GameController))as _GameController;
    }

    public void usarItem(){ //COMANDOS QUANDO O ITEM FOR USADO
        print("esse item " + idItem + " foi utilizado");
        _GameController.usarItemArma(idItem);
    }
}
