using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public enum maquinaEstado
{
    PAUSE,
    GAMEPLAY,
    INVENTARIO,
    DIALOGO,
    FIMDIALOGO
}

public class pauseScript : MonoBehaviour
{
    private invScript invScript;
    public maquinaEstado estadoAtual;
    public GameObject pauseMenu;
    public GameObject inventario;
    public GameObject infoInventario;
    private bool pause = false;     //VARIAVEL PARA VERIFICAR SE O JOGO ESTA PAUSADO
    private _GameController _GameController;
    void Start()
    {
        invScript = FindObjectOfType(typeof(invScript)) as invScript;
        _GameController = FindObjectOfType(typeof(_GameController)) as _GameController;
        Time.timeScale = 1f;
        EsconderPauseMenu();        //FAZ COM QUE O JOGO INICIE SEM ESTAR PAUSADO
        inventario.SetActive(false);    //
        infoInventario.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && estadoAtual != maquinaEstado.INVENTARIO)
        {   //VERIFICA SE A TECLA "ESQ" FOI CLICADA
            if (pause)
            {
                EsconderPauseMenu();   //SE O JOGO ESTIVER PAUSADO, ELE DESPAUSA
            }
            else
            {
                MostrarPauseMenu();    //SE O JOGO NÃO ESTIVER PAUSADO, ELE PAUSA
            }
        }
    }

    public void Voltar()
    {
        EsconderPauseMenu();
    }
    public void Sair()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    private void MostrarPauseMenu()
    {        //MOSTRAR O MENU DO PAUSE
        pause = true;
        pauseMenu.SetActive(true);
        mudarEstado(maquinaEstado.PAUSE);
        _GameController.primeiroPauseMenu.Select();
    }

    private void EsconderPauseMenu()
    {       //ESCONDE O MENU DO PAUSE
        pause = false;
        pauseMenu.SetActive(false);
        mudarEstado(maquinaEstado.GAMEPLAY);
    }

    public void mudarEstado(maquinaEstado novoEstado)
    {
        estadoAtual = novoEstado;
        switch (novoEstado)
        {
            case maquinaEstado.GAMEPLAY:
                Time.timeScale = 1f;    //FAZ OS OBJETOS E CRIATURAS DO JOGO VOLTAREM
                break;

            case maquinaEstado.PAUSE:
                Time.timeScale = 0;     //FAZ OS OBJETOS E CRIATURAS DO JOGO PAUSAREM
                break;

            case maquinaEstado.INVENTARIO:
                Time.timeScale = 0;
                break;

            case maquinaEstado.FIMDIALOGO:
                StartCoroutine("fimConversa");
                break;
        }
    }

    public void btInventario()
    {
        pauseMenu.SetActive(false);
        inventario.SetActive(true);
        invScript.carregarInv();
        mudarEstado(maquinaEstado.INVENTARIO);
        _GameController.primeiroInventario.Select();
    }

    public void voltarInventario()
    {
        inventario.SetActive(false);
        pauseMenu.SetActive(true);
        _GameController.primeiroPauseMenu.Select();
        invScript.limparItensCarregados();
        mudarEstado(maquinaEstado.PAUSE);
    }

    public void abrirInfoInventario()
    {
        infoInventario.SetActive(true);
        _GameController.primeiroInfoInventario.Select();
    }

    public void fecharInfoInventario()
    {
        infoInventario.SetActive(false);
    }

    public void voltarAoJogo()
    {
        infoInventario.SetActive(false);
        inventario.SetActive(false);
        pauseMenu.SetActive(false);
        mudarEstado(maquinaEstado.GAMEPLAY);
    }

    public void excluirItem(int idSlot)
    {
        invScript.itemInv.RemoveAt(idSlot);
        invScript.carregarInv();
        infoInventario.SetActive(false);
    }
    IEnumerator fimConversa()
    {
        yield return new WaitForEndOfFrame();
        mudarEstado(maquinaEstado.GAMEPLAY);
    }
}
