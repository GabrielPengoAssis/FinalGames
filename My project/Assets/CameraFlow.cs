using UnityEngine;

public class RacingCameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target; // O carro que a câmera vai seguir
    
    [Header("Position Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2.5f, -6f);
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float height = 2f;
    [SerializeField] private float distance = 6f;
    [SerializeField] private bool lockVerticalPosition = true; // Trava a altura da câmera
    
    [Header("Distance Constraints")]
    [SerializeField] private bool enableMaxDistance = true;
    [SerializeField] private float maxDistance = 15f; // Distância máxima permitida
    [SerializeField] private float snapSpeed = 20f; // Velocidade de retorno quando ultrapassa o limite
    
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private bool lookAtTarget = true;
    [SerializeField] private Vector3 lookAtOffset = new Vector3(0f, 0.5f, 0f);
    
    [Header("Speed-Based FOV")]
    [SerializeField] private bool enableSpeedFOV = true;
    [SerializeField] private float baseFOV = 60f;
    [SerializeField] private float maxFOV = 80f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float fovChangeSpeed = 5f;
    
    [Header("Shake Settings")]
    [SerializeField] private bool enableShake = true;
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private float shakeSpeed = 10f;
    
    private Camera cam;
    private Rigidbody targetRigidbody;
    private Vector3 currentVelocity;
    private float currentFOV;
    private float shakeOffset;
    private float fixedHeight; // Altura fixa da câmera

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            currentFOV = baseFOV;
            cam.fieldOfView = currentFOV;
        }
        
        if (target != null)
        {
            targetRigidbody = target.GetComponent<Rigidbody>();
            // Define a altura inicial baseada no offset
            fixedHeight = target.position.y + offset.y;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;
        
        FollowTarget();
        EnforceMaxDistance();
        RotateCamera();
        UpdateFOV();
        ApplyShake();
    }

    private void FollowTarget()
    {
        // Calcula a posição desejada baseada no offset e na rotação do alvo
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        
        // Se a trava vertical estiver ativada, mantém a altura do offset
        if (lockVerticalPosition)
        {
            desiredPosition.y = target.position.y + offset.y;
        }
        
        // Suaviza o movimento usando SmoothDamp para uma transição mais fluida
        Vector3 newPosition = Vector3.SmoothDamp(
            transform.position, 
            desiredPosition, 
            ref currentVelocity, 
            1f / followSpeed
        );
        
        // Garante que a altura permaneça fixa
        if (lockVerticalPosition)
        {
            newPosition.y = target.position.y + offset.y;
        }
        
        transform.position = newPosition;
    }

    private void EnforceMaxDistance()
    {
        if (!enableMaxDistance) return;
        
        // Calcula a distância atual entre a câmera e o alvo (apenas no plano horizontal se lockVerticalPosition estiver ativo)
        Vector3 cameraPos = transform.position;
        Vector3 targetPos = target.position;
        
        if (lockVerticalPosition)
        {
            // Ignora a diferença de altura no cálculo da distância
            cameraPos.y = 0;
            targetPos.y = 0;
        }
        
        float currentDistance = Vector3.Distance(cameraPos, targetPos);
        
        // Se ultrapassou a distância máxima
        if (currentDistance > maxDistance)
        {
            // Calcula a direção do alvo para a câmera (apenas no plano horizontal)
            Vector3 directionToCamera = (transform.position - target.position);
            
            if (lockVerticalPosition)
            {
                directionToCamera.y = 0; // Remove componente vertical
            }
            
            directionToCamera = directionToCamera.normalized;
            
            // Define a nova posição no limite da distância máxima
            Vector3 constrainedPosition = target.position + directionToCamera * maxDistance;
            
            // Mantém a altura do offset
            if (lockVerticalPosition)
            {
                constrainedPosition.y = target.position.y + offset.y;
            }
            
            // Move a câmera de volta para o limite com uma velocidade de "snap"
            Vector3 newPosition = Vector3.Lerp(
                transform.position, 
                constrainedPosition, 
                snapSpeed * Time.deltaTime
            );
            
            // Garante que a altura permaneça fixa
            if (lockVerticalPosition)
            {
                newPosition.y = target.position.y + offset.y;
            }
            
            transform.position = newPosition;
        }
    }

    private void RotateCamera()
    {
        if (!lookAtTarget) return;
        
        // Ponto de foco com offset
        Vector3 lookPosition = target.position + lookAtOffset;
        
        // Calcula a rotação suave em direção ao alvo
        Quaternion targetRotation = Quaternion.LookRotation(lookPosition - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            targetRotation, 
            rotationSpeed * Time.deltaTime
        );
    }

    private void UpdateFOV()
    {
        if (!enableSpeedFOV || targetRigidbody == null || cam == null) return;
        
        // Calcula o FOV baseado na velocidade do veículo
        float speed = targetRigidbody.linearVelocity.magnitude;
        float speedRatio = Mathf.Clamp01(speed / maxSpeed);
        float targetFOV = Mathf.Lerp(baseFOV, maxFOV, speedRatio);
        
        // Suaviza a mudança de FOV
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, fovChangeSpeed * Time.deltaTime);
        cam.fieldOfView = currentFOV;
    }

    private void ApplyShake()
    {
        if (!enableShake || targetRigidbody == null) return;
        
        // Cria um shake sutil baseado na velocidade
        float speed = targetRigidbody.linearVelocity.magnitude;
        float speedRatio = Mathf.Clamp01(speed / maxSpeed);
        
        shakeOffset += Time.deltaTime * shakeSpeed;
        float shake = Mathf.Sin(shakeOffset) * shakeIntensity * speedRatio;
        
        // Aplica o shake apenas nas direções horizontais, não no Y
        if (lockVerticalPosition)
        {
            transform.position += transform.right * shake;
        }
        else
        {
            transform.position += transform.up * shake;
        }
    }

    // Método público para ajustar o offset dinamicamente
    public void SetCameraOffset(Vector3 newOffset)
    {
        offset = newOffset;
        if (lockVerticalPosition && target != null)
        {
            fixedHeight = target.position.y + offset.y;
        }
    }

    // Método para trocar o alvo da câmera
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            targetRigidbody = target.GetComponent<Rigidbody>();
            if (lockVerticalPosition)
            {
                fixedHeight = target.position.y + offset.y;
            }
        }
    }

    // Método para ajustar a distância máxima em runtime
    public void SetMaxDistance(float newMaxDistance)
    {
        maxDistance = newMaxDistance;
    }

    // Método para obter a distância atual
    public float GetCurrentDistance()
    {
        if (target == null) return 0f;
        
        if (lockVerticalPosition)
        {
            // Retorna distância apenas no plano horizontal
            Vector3 cameraPos = transform.position;
            Vector3 targetPos = target.position;
            cameraPos.y = 0;
            targetPos.y = 0;
            return Vector3.Distance(cameraPos, targetPos);
        }
        
        return Vector3.Distance(transform.position, target.position);
    }

    // Método para ativar/desativar a trava vertical
    public void SetLockVerticalPosition(bool lockState)
    {
        lockVerticalPosition = lockState;
        if (lockVerticalPosition && target != null)
        {
            fixedHeight = target.position.y + offset.y;
        }
    }

    // Visualização no Editor (Gizmos)
    private void OnDrawGizmosSelected()
    {
        if (target == null || !enableMaxDistance) return;
        
        // Desenha a esfera de distância máxima (no plano horizontal se lockVerticalPosition estiver ativo)
        Gizmos.color = Color.yellow;
        if (lockVerticalPosition)
        {
            // Desenha um círculo no plano horizontal
            DrawCircle(target.position, maxDistance, 64);
        }
        else
        {
            Gizmos.DrawWireSphere(target.position, maxDistance);
        }
        
        // Desenha uma linha da câmera até o alvo
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, target.position);
        
        // Mostra a distância atual
        float currentDist = GetCurrentDistance();
        Gizmos.color = currentDist > maxDistance ? Color.red : Color.green;
        Gizmos.DrawSphere(transform.position, 0.3f);
        
        // Desenha um plano horizontal no nível do offset
        if (lockVerticalPosition)
        {
            Gizmos.color = new Color(0, 1, 0, 0.2f);
            Vector3 planePos = target.position;
            planePos.y += offset.y;
            Gizmos.DrawCube(planePos, new Vector3(maxDistance * 2, 0.1f, maxDistance * 2));
        }
    }

    // Desenha um círculo no plano horizontal
    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);
        
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, offset.y, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}