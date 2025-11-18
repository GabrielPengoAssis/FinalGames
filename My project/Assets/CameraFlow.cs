using UnityEngine;

public class CarCameraFollow : MonoBehaviour
{
    [Header("Referências")]
    public Transform target; // O carro (player)
    public Rigidbody targetRb; // Rigidbody do carro para detectar direção

    [Header("Configurações de posição")]
    public Vector3 offset = new Vector3(0, 5, -10); // Posição da câmera em relação ao carro
    public float followSpeed = 5f;

    [Header("Efeito de inclinação")]
    public float tiltAmount = 2f; // Quanto a câmera move lateralmente
    public float tiltSpeed = 3f;  // Velocidade da interpolação

    private float currentTilt = 0f;

    void LateUpdate()
    {
        if (!target || !targetRb)
            return;

        // Segue o carro suavemente
        Vector3 desiredPosition = target.position + target.transform.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Mantém a câmera olhando para o carro
        transform.LookAt(target);

        // Calcula a direção lateral (curva)
        float steerInput = Vector3.Dot(targetRb.linearVelocity.normalized, target.transform.right);

        // Movimento lateral (tilt)
        float targetTilt = -steerInput * tiltAmount;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, tiltSpeed * Time.deltaTime);

        // Aplica o leve movimento lateral na posição da câmera
        transform.position += transform.right * currentTilt;
    }
}
