using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class arma : MonoBehaviour
{
    private _GameController _GameController;
    public  GameObject[]      armaColetar;
    private bool            coletado;
    void Start()
    {
        _GameController = FindObjectOfType(typeof(_GameController))as _GameController;
    }

    public void coletar(){
        if (coletado == false)
        {
            coletado = true;
            _GameController.coletarArma(armaColetar[Random.Range(0, armaColetar.Length)]);            
        }
        Destroy(this.gameObject);
    }
}
