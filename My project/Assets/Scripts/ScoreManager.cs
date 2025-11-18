using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text uiScore; // Referência ao componente UI Text
    public Text uiFinalScore; // Referência ao componente UI Text
    public Text uiRestarting; // Referência ao componente UI Text
    public float Points = 0;
    public float Multiplier = 1;
    public float FinalScore = 0;

    private void OnTriggerEnter(Collider other)
    {

        // Corrigido: comparar o tag do objeto que colidiu
        if (other.CompareTag("Green"))
        {
            Destroy(other.gameObject);
            Points += 100;       // Adiciona 100 pontos            
            Multiplier += 0.1f;   // Aumenta o multiplicador
        }

        if (other.gameObject.CompareTag("Red"))
        {
            Destroy(other.gameObject);
            Multiplier = 1;   // reseta o multiplicador
        }

        if (other.gameObject.CompareTag("Blue"))
        {
            FinalScore = Points * Multiplier;  // Calcula o score final
            uiFinalScore.text = $"{FinalScore}pts"; // mostra o score final
            uiRestarting.text = $"reiniciando...";
            Destroy(other.gameObject);

            StartCoroutine(ReiniciarCenaComDelay(5f));
        }
    }

    private IEnumerator ReiniciarCenaComDelay(float delay)
    {
        Debug.Log("Reiniciando em " + delay + " segundos...");
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Titulo");
    }

    private void Update()
    {
        // Atualiza o texto da UI a cada frame
        uiScore.text = $"{Points}pts {Multiplier}x";


    }
}
