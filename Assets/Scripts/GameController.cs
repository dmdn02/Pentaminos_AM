using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    public float tempoTotal = 300f; // 5 minutos
    private float tempoAtual;
    private bool jogoTerminado = false;

    public TextMeshProUGUI textoTempo;
    public TextMeshProUGUI textoPontos;

    private int pecasNoInicio = 0;

    public int totalPecasParaVitoria = 12;

    void Start()
    {
        tempoAtual = tempoTotal;
        pecasNoInicio = ContarPecasColocadas();
        textoPontos.text = "Pontos: 0";
    }

    void Update()
    {
        if (jogoTerminado) return;

        tempoAtual -= Time.deltaTime;
        if (tempoAtual < 0f)
            tempoAtual = 0f;

        int pecasAtuais = ContarPecasColocadas();
        int pecasColocadasAgora = pecasAtuais - pecasNoInicio;
        if (pecasColocadasAgora < 0)
            pecasColocadasAgora = 0;

        int pontos = pecasColocadasAgora * 5;
        textoPontos.text = $"Pontos: {pontos}";

        int minutos = Mathf.FloorToInt(tempoAtual / 60);
        int segundos = Mathf.FloorToInt(tempoAtual % 60);
        textoTempo.text = $"Tempo: {minutos:00}:{segundos:00}";

        if (pecasColocadasAgora >= totalPecasParaVitoria)
        {
            VerificarVitoria();
        }

        if (tempoAtual <= 0f)
        {
            Derrota();
        }
    }

    public void VerificarVitoria()
    {
        if (jogoTerminado) return;

        GuardarEstadoFinal();
        jogoTerminado = true;
        SceneManager.LoadScene("MenuVitoria");
    }

    public void Derrota()
    {
        if (jogoTerminado) return;

        GuardarEstadoFinal();
        jogoTerminado = true;
        SceneManager.LoadScene("MenuDerrota");
    }

    void GuardarEstadoFinal()
    {
        int pecasAtuais = ContarPecasColocadas();
        int pecasColocadasAgora = pecasAtuais - pecasNoInicio;
        if (pecasColocadasAgora < 0)
            pecasColocadasAgora = 0;

        int pontos = pecasColocadasAgora * 5;
        float tempoUsado = tempoTotal - tempoAtual;

        PlayerPrefs.SetInt("Pontos", pontos);
        PlayerPrefs.SetFloat("Tempo", tempoUsado);
    }

    int ContarPecasColocadas()
    {
        return GameObject.FindGameObjectsWithTag("PlacedPiece").Length;
    }
}
