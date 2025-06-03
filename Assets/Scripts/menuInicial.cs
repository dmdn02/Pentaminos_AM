using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class menuInicial : MonoBehaviour
{
    public List<Button> botoes; // Ordem: Jogar, Instruções, Som, Sair
    public Color corNormal = new Color(0.86f, 0.22f, 0.22f, 1f);
    public Color corSelecionado = new Color(0.86f, 0.22f, 0.22f, 0.3f);

    private int indiceAtual = 0;

    [Header("Texto do Botão de Som")]
    public TextMeshProUGUI somTexto; // <-- Aqui colocas o componente de texto do botão "Som"

    void Start()
    {
        AtualizarSelecao();

        //  Adiciona esta verificação aqui
        if (MusicaFundo.Instance == null)
        {
            Debug.LogError("MusicaFundo.Instance é NULL no Start do menuInicial!");
        }
        else
        {
            AtualizarSomVisual(); // mostra o texto inicial correto
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            indiceAtual = (indiceAtual + 1) % botoes.Count;
            AtualizarSelecao();
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            botoes[indiceAtual].onClick.Invoke();
        }
    }

    void AtualizarSelecao()
    {
        for (int i = 0; i < botoes.Count; i++)
        {
            var colors = botoes[i].colors;
            colors.normalColor = corNormal;
            botoes[i].colors = colors;

            botoes[i].transform.localScale = (i == indiceAtual) ? new Vector3(1.2f, 1.2f, 1f) : Vector3.one;
        }

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(botoes[indiceAtual].gameObject);
        }
    }

    public void Jogar()
    {
        SceneManager.LoadScene("Jogo");
    }

    public void AbrirInstrucoes()
    {
        SceneManager.LoadScene("Instrucoes");
    }

    public void ToggleSom()
    {
        Debug.Log("ToggleSom chamado em: " + Time.time);
        if (MusicaFundo.Instance == null)
        {
            Debug.LogError("MusicaFundo.Instance é NULL");
            return;
        }

        MusicaFundo.Instance.AlternarSom();
        AtualizarSomVisual();
    }




    void AtualizarSomVisual()
{
    if (somTexto != null)
    {
            somTexto.text = MusicaFundo.Instance.EstaAtivo() ? "Som: ON" : "Som: OFF";
    }
}

    public void Sair()
    {
        Application.Quit();
        Debug.Log("Jogo encerrado.");
    }

    public void VoltarAoMenu()
    {
        SceneManager.LoadScene("Menu Inicial");
    }
}
