using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform   alvo;
    private Vector3     velocity = Vector3.zero;
    private float       smoothTime = 0.1f;
    void Start()
    {
        alvo = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPosition = alvo.position + new Vector3(0,0,-1);
        transform.position = Vector3.SmoothDamp(transform.position, cameraPosition, ref velocity, smoothTime);
    }
}
