using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class menuPausa : MonoBehaviour
{
    public GameObject painelPausa;           // Painel do menu de pausa (Canvas desativado por defeito)
    public List<Button> botoesPausa;         // Botões: Retomar (0), Sair (1)
    public Color corNormal = new Color(0.86f, 0.22f, 0.22f, 1f);

    private bool jogoPausado = false;
    private int indiceAtual = 0;

    void Start()
    {
        painelPausa.SetActive(false); // Esconde o menu no início

        botoesPausa[0].onClick.AddListener(Retomar);
        botoesPausa[1].onClick.AddListener(SairDoJogo);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (!jogoPausado)
                Pausar();
            else
                Retomar();
        }

        if (!jogoPausado) return;

        // Navegar com TAB
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            indiceAtual = (indiceAtual + 1) % botoesPausa.Count;
            AtualizarSelecao();
        }

        // Selecionar com ENTER
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            botoesPausa[indiceAtual].onClick.Invoke();
        }
    }

    void Pausar()
    {
        jogoPausado = true;
        painelPausa.SetActive(true);
        Time.timeScale = 0f; // Pausa o jogo
        indiceAtual = 0;
        AtualizarSelecao();
    }

    public void Retomar()
    {
        jogoPausado = false;
        painelPausa.SetActive(false);
        Time.timeScale = 1f; // Retoma o jogo
    }

     public void SairDoJogo()
    {
        Time.timeScale = 1f; // Garante que o tempo está normal ao sair
        Application.Quit();
        Debug.Log("Jogo encerrado.");
    }

    void AtualizarSelecao()
    {
        for (int i = 0; i < botoesPausa.Count; i++)
        {
            var colors = botoesPausa[i].colors;
            colors.normalColor = corNormal;
            botoesPausa[i].colors = colors;

            if (i == indiceAtual)
                botoesPausa[i].transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            else
                botoesPausa[i].transform.localScale = Vector3.one;
        }

        EventSystem.current.SetSelectedGameObject(botoesPausa[indiceAtual].gameObject);
    }
}
