using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class selPersonagem : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void selecionarPersonagem(int idPersonagem){
        PlayerPrefs.SetInt("idPersonagem", idPersonagem);
        SceneManager.LoadScene("Cena 1");
    }
}
