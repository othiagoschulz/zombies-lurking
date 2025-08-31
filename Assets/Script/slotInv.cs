using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slotInv : MonoBehaviour
{
    private pauseScript pauseScript;
    public  GameObject  objetoSlot;
    private painelItemInfo painelItemInfo;
    public int idSlot;
    void Start()
    {
        painelItemInfo = FindObjectOfType(typeof(painelItemInfo)) as painelItemInfo;
        pauseScript = FindObjectOfType(typeof(pauseScript))as pauseScript;
    }
    void Update()
    {
        
    }

    public void    usarItem(){        
        if(objetoSlot != null){            
            painelItemInfo.objetoSlot = objetoSlot;
            painelItemInfo.idSlot = idSlot;
            painelItemInfo.carregarInfoItem();
            pauseScript.abrirInfoInventario();
        }
    }
}
