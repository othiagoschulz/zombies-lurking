using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class armaInfo : MonoBehaviour
{

    public float danoMax;        //DANO MAXIMO
    public float danoMin;        //DANO MINIMO
    public int tipoDano;
    
    public void SetarDados(float min, float max, int tipo)
    {
        danoMin = min;
        danoMax = max;
        tipoDano = tipo;
    }
}
