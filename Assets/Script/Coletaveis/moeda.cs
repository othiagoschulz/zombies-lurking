using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moeda : MonoBehaviour{
    private     _GameController _GameController;

    public      int     valor;

    void Start()
    {
        _GameController = FindObjectOfType(typeof(_GameController))as _GameController;
    }

    public void coletar(){
        _GameController.moeda += valor; //LINHA QUE FAZ A VARIAVEL MOEDA RECEBER O VALOR "1" A CADA MOEDA COLETADA
        Destroy(this.gameObject);
    }
}