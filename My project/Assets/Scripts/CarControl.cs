using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SceneManagement;

// Script responsável pelo controle do carro
// Melhorias sugeridas nos comentários iniciais podem ser aplicadas neste código futuramente.

public class CarController : MonoBehaviour
{
    // Constantes para os nomes dos eixos de input configurados no Unity
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    // Variáveis que recebem o input do jogador
    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;     // Ângulo atual da direção
    private float currentBreakForce;     // Força atual de frenagem
    private bool isBreaking;             // Indica se o jogador está freando (barra de espaço)

    // Colliders das 4 rodas
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRighttWheelCollider;
    [SerializeField] private WheelCollider RearLeftWheelCollider;
    [SerializeField] private WheelCollider RearRightWheelCollider;

    // Transform das 4 rodas (parte gráfica)
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRighttWheelTransform;
    [SerializeField] private Transform RearLeftWheelTransformr;
    [SerializeField] private Transform RearRightWheelTransform;

    // Configurações gerais do carro
    [SerializeField] private float motorForce;       // Potência do motor
    [SerializeField] private float breakForce;       // Intensidade do freio
    [SerializeField] private float maxSteeringAngle; // Máximo que o volante pode girar
    [SerializeField] GameObject[] lights;            // Array contendo as luzes do carro

    // FixedUpdate é usado para física
    private void FixedUpdate()
    {
        GetInput();      // Captura o input do jogador
        HandleMotor();   // Aplica aceleração e freio
        HandleSteering(); // Controla a direção
        UpdateWheels();  // Atualiza posição e rotação das rodas
    }

    private void Update()
    {
        ActiveDesactiveLight(); // Liga/desliga luzes

        // Reinicia a cena ao apertar R
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Titulo");
        }
    }

    // Lê os comandos do jogador
    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL); // Direção esquerda/direita
        verticalInput = Input.GetAxis(VERTICAL);     // Acelerar/frear
        isBreaking = Input.GetKey(KeyCode.Space);    // Freio ao segurar espaço
    }

    // Responsável pelo motor e força das rodas
    private void HandleMotor()
    {
        // Aplica motor nas rodas dianteiras
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRighttWheelCollider.motorTorque = verticalInput * motorForce;

        // Se estiver freando, aplica a força definida
        currentBreakForce = isBreaking ? breakForce : 0f;

        // Aplica o freio nas 4 rodas
        ApplyBreaking();
    }

    // Aplica frenagem dividindo entre rodas dianteiras e traseiras
    private void ApplyBreaking()
    {
        // Freio dianteiro com maior força (60%)
        frontRighttWheelCollider.brakeTorque = (currentBreakForce * 0.6f);
        frontLeftWheelCollider.brakeTorque  = (currentBreakForce * 0.6f);

        // Freio traseiro com menor força (40%)
        RearLeftWheelCollider.brakeTorque   = (currentBreakForce * 0.4f);
        RearRightWheelCollider.brakeTorque  = (currentBreakForce * 0.4f);
    }

    // Controla a direção do carro
    private void HandleSteering()
    {
        // Calcula o ângulo proporcional ao input
        currentSteerAngle = maxSteeringAngle * horizontalInput;

        // Aplica direção nas rodas dianteiras
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRighttWheelCollider.steerAngle = currentSteerAngle;
    }

    // Atualiza visualmente as rodas (posição e rotação)
    private void UpdateWheels()
    {
        UpdateSingleWheelCollider(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheelCollider(frontRighttWheelCollider, frontRighttWheelTransform);
        UpdateSingleWheelCollider(RearRightWheelCollider, RearRightWheelTransform);
        UpdateSingleWheelCollider(RearLeftWheelCollider, RearLeftWheelTransformr);
    }

    // Atualiza um único conjunto de WheelCollider + Transform
    private void UpdateSingleWheelCollider(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;

        // Pega posição e rotação reais da roda física
        wheelCollider.GetWorldPose(out pos, out rot);

        // Atualiza modelo 3D da roda
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    // Liga ou desliga as luzes do carro
    private void ActiveDesactiveLight()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Toogling Lights");

            // Inverte o estado das duas luzes
            lights[0].SetActive(!lights[0].activeSelf);
            lights[1].SetActive(!lights[1].activeSelf);
        }
    }
}
