using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trocaFlecha : MonoBehaviour
{
    private _GameController _GameController;
    private SpriteRenderer sRenderer;
    void Start()
    {
        _GameController = FindObjectOfType(typeof(_GameController)) as _GameController;
        sRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        sRenderer.sprite = _GameController.imgFlecha[_GameController.idFlechaEquipada];
    }
}
