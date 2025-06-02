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

    public int totalPecasParaVitoria = 12;

    private int cubosContados = 0;
    private int pecasCompletasContadas = 0; // peças completas = cubos / 5
    private int pontos = 0;

    void Start()
    {
        tempoAtual = tempoTotal;
        cubosContados = ContarCubosColocados();
        pecasCompletasContadas = cubosContados / 5;
        pontos = pecasCompletasContadas * 5; // pontos iniciais se houver peças já colocadas
        textoPontos.text = $"Pontos: {pontos}";
    }

    void Update()
    {
        if (jogoTerminado) return;

        tempoAtual -= Time.deltaTime;
        if (tempoAtual < 0f) tempoAtual = 0f;

        int minutos = Mathf.FloorToInt(tempoAtual / 60);
        int segundos = Mathf.FloorToInt(tempoAtual % 60);
        textoTempo.text = $"Tempo: {minutos:00}:{segundos:00}";

        int cubosAtuais = ContarCubosColocados();
        int pecasAtuais = cubosAtuais / 5;

        if (pecasAtuais > pecasCompletasContadas)
        {
            int novasPecas = pecasAtuais - pecasCompletasContadas;
            pontos += novasPecas * 5;
            pecasCompletasContadas = pecasAtuais;
            textoPontos.text = $"Pontos: {pontos}";
            Debug.Log($"Nova peça completa! Peças: {pecasCompletasContadas}, Pontos: {pontos}");
        }

        if (pecasCompletasContadas >= totalPecasParaVitoria)
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
        float tempoUsado = tempoTotal - tempoAtual;
        PlayerPrefs.SetInt("Pontos", pontos);
        PlayerPrefs.SetFloat("Tempo", tempoUsado);
    }

    int ContarCubosColocados()
    {
        return GameObject.FindGameObjectsWithTag("PlacedPiece").Length;
    }
}
