using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchFlicker : MonoBehaviour
{
    private Light2D torchLight;
    
    [Header("Configurações de Intensidade")]
    [SerializeField] private float minIntensity = 0.8f;
    [SerializeField] private float maxIntensity = 1.5f;
    [SerializeField] private float smoothSpeed = 5f;
    
    [Header("Configurações de Movimento (Opcional)")]
    [SerializeField] private bool moveLight = false;
    [SerializeField] private float maxMovement = 0.1f;
    
    private float targetIntensity;
    private Vector3 originalPosition;
    private Vector3 targetPosition;
    
    void Start()
    {
        torchLight = GetComponent<Light2D>();
        originalPosition = transform.localPosition;
        targetIntensity = torchLight.intensity;
    }
    
    void Update()
    {
        // Varia a intensidade suavemente
        torchLight.intensity = Mathf.Lerp(torchLight.intensity, targetIntensity, Time.deltaTime * smoothSpeed);
        
        // Quando chega perto do valor alvo, escolhe um novo alvo
        if (Mathf.Abs(torchLight.intensity - targetIntensity) < 0.05f)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity);
        }
        
        // Movimento da luz (opcional - simula chama tremendo)
        if (moveLight)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * smoothSpeed);
            
            if (Vector3.Distance(transform.localPosition, targetPosition) < 0.01f)
            {
                targetPosition = originalPosition + new Vector3(
                    Random.Range(-maxMovement, maxMovement),
                    Random.Range(-maxMovement, maxMovement),
                    0
                );
            }
        }
    }
}