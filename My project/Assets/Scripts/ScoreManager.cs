using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public GameObject player;
    public Text uiRestarting; // Referência ao componente UI Text
    public Text uiCombustivel;
    public Image Combustivel_imagem;
    public RectTransform barraCombustivel; // arraste a imagem aqui no Inspector

    public float Combustivel = 100f;
    public float DelayTime = 5f;

    private void OnTriggerEnter(Collider other)
    {

        // Corrigido: comparar o tag do objeto que colidiu
        if (other.CompareTag("Green"))
        {
            Destroy(other.gameObject);
            Combustivel += 10;
        }

        if (other.gameObject.CompareTag("Red"))
        {
            Destroy(other.gameObject);
            Combustivel -= 3;
        }

        if (other.gameObject.CompareTag("Blue"))
        {
            uiRestarting.text = $"Parabens";
            Destroy(other.gameObject);
            StartCoroutine(ReiniciarCenaComDelay(DelayTime));
        }
    }

    private IEnumerator ReiniciarCenaComDelay(float delay)
    {
        player.GetComponent<CarController>().enabled = false;
        uiCombustivel.enabled = false;
        Combustivel_imagem.enabled = false;
        Debug.Log("Reiniciando em " + delay + " segundos...");
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Titulo");
    }

    private void Update()
    {
        Combustivel = Combustivel - 0.05f;

        Vector2 tamanho = barraCombustivel.sizeDelta;
        tamanho.x = Combustivel;
        barraCombustivel.sizeDelta = tamanho;

        // Atualiza o texto da UI a cada frame mostrando apenas número inteiro
        uiCombustivel.text = $"Combustivel: {(int)Combustivel}%"; // mostra o combustivel como número inteiro

        if(Combustivel <= 0)
        {
            uiRestarting.text = $"Game Over";
            StartCoroutine(ReiniciarCenaComDelay(DelayTime));
        }


    }
}