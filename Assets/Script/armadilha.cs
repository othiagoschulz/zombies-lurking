using UnityEngine;

public class armadilha : MonoBehaviour
{
    private _GameController _GameController;
    void Start()
    {
        _GameController = FindObjectOfType(typeof(_GameController)) as _GameController;
    }
    void OnTriggerEnter2D(Collider2D colisor)
    {
        if (colisor.CompareTag("Player"))
        {
            _GameController.vidaAtual = 0;
        }
    }
}
