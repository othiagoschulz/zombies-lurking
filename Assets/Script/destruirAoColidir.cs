using UnityEngine;

public class destruirAoColidir : MonoBehaviour
{
    public  LayerMask   destruir;
    void Start()
    {
        
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 0.1f, destruir);        

        if(hit == true){
            Destroy(this.gameObject);
        }
    }
}
