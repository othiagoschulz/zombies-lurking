using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bau : MonoBehaviour{
    
    public int idBau;
    private _GameController _GameController;
    private SpriteRenderer spriteRenderer;
    public  Sprite[]        imagemObjeto;
    public  bool            open;
    public int qtdMaxItem;
    public int qtdMinItem;
    public  GameObject[]      loots;
    private bool            gerouLoot;
    void Start()
    {
        _GameController = FindObjectOfType(typeof(_GameController)) as _GameController;

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (_GameController != null && _GameController.bausAbertosIDs.Contains(idBau))
        {
            open = true;
            spriteRenderer.sprite = imagemObjeto[1];
            GetComponent<Collider2D>().enabled = false;
        }
    }

    public void interacao() {
        if (open == false) {
            open = true;
            if (_GameController != null)
            {
                _GameController.BauAberto(idBau, tag);
            }
            spriteRenderer.sprite = imagemObjeto[1];
            StartCoroutine("gerarLoot");
            GetComponent<Collider2D>().enabled = false;
        }
    }
    IEnumerator gerarLoot(){        
        //CONTROLE DE LOOT
        gerouLoot = true;      
        int qtdMoeda = Random.Range(qtdMinItem, qtdMaxItem);   //ADICIONA UM NUMERO ALEATORIO DE MOEDAS AO INIMIGO DE 1 A 5
        for(int l = 0; l < qtdMoeda; l++){

            int rand = 0;
            int idLoot = 0;
            rand = Random.Range(0,100);

            GameObject lootTemporario = Instantiate(loots[idLoot], transform.position, transform.localRotation);    //INSTANCIANDO A MOEDA NO INIMIGO
            lootTemporario.GetComponent<Collider2D>().isTrigger = true;
            lootTemporario.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-30, 30), 70));    //ADICIONANDO FORÇA PARA CAUSAR UM EFEITO LEGAL NA MOEDA
            yield return new WaitForSeconds(0.1f);  //TEMPO DE DROPE ENTRE CADA MOEDA
        }
    }
}

