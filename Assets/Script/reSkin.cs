using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class reSkin : MonoBehaviour{

    private _GameController _GameController;
    public  bool            vPersonagem;    //VERIFICA SE O SCRIPT ESTA CONECTADO AO PERSONAGEM JOGAVEL E NÃO AO ZUMBI
    private SpriteRenderer  sRender;
    public  Sprite[]        sprites;
    public  string          nomeSprite;     //NOME DO SPRITESHEET QUE IRA SER USADO
    public  string          nomeSpriteAtual;    //NOME DO SPRITESHEET ATUAL

    public  Dictionary<string, Sprite> spriteSheet; //VARIAVEL QUE VAI POSSIBILITAR FAZER A TROCA DA SKIN NO JOGO
    void Start(){
        _GameController = FindObjectOfType(typeof(_GameController))as _GameController;
        if(vPersonagem){
            nomeSprite = _GameController.nomeSpriteSheet[_GameController.idPersonagem].name;
        }
        
        sRender = GetComponent<SpriteRenderer>();
        carregarSprite();
    }


    void LateUpdate(){
        if(vPersonagem){
            if(_GameController.idPersonagem != _GameController.idPersonagemAtual){
                nomeSprite = _GameController.nomeSpriteSheet[_GameController.idPersonagem].name;
                _GameController.idPersonagemAtual = _GameController.idPersonagem;
            }
            _GameController.validarArma();
        }
        
        if(nomeSpriteAtual != nomeSprite){  //HOUVE UMA TROCA
            carregarSprite();
        } 
        sRender.sprite = spriteSheet[sRender.sprite.name];
    }

    private void    carregarSprite(){       //FUNCAO QUE VAI LER O SPRITESHEET DA PASTA PRA FAZER A TROCA
        sprites = Resources.LoadAll<Sprite>(nomeSprite);    //ESSA VARIAVEL VAI CARREGAR TODOS OS SPRITES QUE TENHAM O NOME QUE EU INSERIR
        spriteSheet =   sprites.ToDictionary(x => x.name, x => x);
        nomeSpriteAtual = nomeSprite;
    }
}
