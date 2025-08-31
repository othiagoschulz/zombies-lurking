using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mudarFase : MonoBehaviour
{
    private transicao       transicao;
    public  string          cenaDestino;
    private _GameController _GameController;
    void Start()
    {
        transicao = FindObjectOfType(typeof(transicao)) as transicao;
        _GameController = FindObjectOfType(typeof(_GameController))as _GameController;
    }

    
    void Update()
    {
        
    }

    public void interacao(){        
        StartCoroutine("mudaCena");
    }

    IEnumerator mudaCena(){
        transicao.fadeIn();
        yield return new WaitForSeconds(0f);    //BUG DE TRAVAMENTO NA TROCA DE CENA

        if(cenaDestino == "selPersonagem"){
            Destroy(_GameController.gameObject);
        }

        SceneManager.LoadScene(cenaDestino);
    }
}
