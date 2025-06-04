using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public float tempoTotal = 300f; // 5 minutos
    private float tempoAtual;
    private bool jogoTerminado = false;

    public TextMeshProUGUI textoTempo;

    public int totalPecasParaVitoria = 12;

    void Start()
    {
        tempoAtual = tempoTotal;
    }

    void Update()
    {
        if (jogoTerminado) return;

        tempoAtual -= Time.deltaTime;
        if (tempoAtual < 0f) tempoAtual = 0f;

        int minutos = Mathf.FloorToInt(tempoAtual / 60);
        int segundos = Mathf.FloorToInt(tempoAtual % 60);
        textoTempo.text = $"Tempo: {minutos:00}:{segundos:00}";


        if (ContarPecasColocadas() >= totalPecasParaVitoria)
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
        PlayerPrefs.SetFloat("Tempo", tempoUsado);
    }

    int ContarPecasColocadas()
    {
        GameObject[] colocadas = GameObject.FindGameObjectsWithTag("PlacedPiece");
        HashSet<Transform> pecasUnicas = new HashSet<Transform>();

        foreach (GameObject go in colocadas)
        {
            pecasUnicas.Add(go.transform.root);
        }

        return pecasUnicas.Count;
    }
}
