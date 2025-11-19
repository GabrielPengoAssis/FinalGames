using UnityEngine;

public class CarCameraFollow : MonoBehaviour
{
    [Header("Referências")]
    public Transform target; // O carro (player)
    public Rigidbody targetRb; // Rigidbody do carro para detectar direção

    [Header("Configurações de posição")]
    public Vector3 offset = new Vector3(0, 5, -5); // Posição da câmera em relação ao carro
    public float followSpeed = 5f;

    [Header("Limites de Distância")]
    public float minDistance = 4f;   // Distância mínima permitida
    public float maxDistance = 5f;  // Distância máxima permitida

    [Header("Efeito de inclinação")]
    public float tiltAmount = 2f; // Quanto a câmera move lateralmente
    public float tiltSpeed = 3f; // Velocidade da interpolação

    private float currentTilt = 0f;

    void LateUpdate()
    {
        if (!target || !targetRb) return;

        // Posição desejada baseada no offset
        Vector3 desiredPosition = target.position + target.transform.TransformDirection(offset);

        // Cálculo da distância entre câmera e target
        float currentDistance = Vector3.Distance(transform.position, target.position);

        // Ajusta a posição desejada para ficar dentro dos limites
        Vector3 directionToTarget = (desiredPosition - target.position).normalized;

        // Se distância futura estiver fora dos limites, corrige
        float desiredDistance = Vector3.Distance(desiredPosition, target.position);

        if (desiredDistance < minDistance)
        {
            desiredPosition = target.position + directionToTarget * minDistance;
        }
        else if (desiredDistance > maxDistance)
        {
            desiredPosition = target.position + directionToTarget * maxDistance;
        }

        // Move suavemente até a posição final corrigida
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        // Mantém a câmera olhando para o carro
        transform.LookAt(target);

        // Calcula direção lateral (curva)
        float steerInput = Vector3.Dot(targetRb.linearVelocity.normalized, target.transform.right);

        // Movimento lateral (tilt)
        float targetTilt = -steerInput * tiltAmount;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, tiltSpeed * Time.deltaTime);

        // Aplica leve movimento lateral
        transform.position += transform.right * currentTilt;
    }
}
