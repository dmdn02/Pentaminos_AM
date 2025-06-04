using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class MenusDerrota_Vitoria : MonoBehaviour
{
    public List<Button> botoes; // Ordem: Jogar Novamente, Menu Inicial
    public TextMeshProUGUI textoTempo;

    public Color corNormal = new Color(0.86f, 0.22f, 0.22f, 1f);        // Vermelho normal
    public Color corSelecionado = new Color(0.86f, 0.22f, 0.22f, 0.3f); // Vermelho transparente

    private int indiceAtual = 0;

    void Start()
    {
        AtualizarSelecao();

        // Liga os botões às respetivas funções
        botoes[0].onClick.AddListener(JogarNovamente);
        botoes[1].onClick.AddListener(Sair);

        // Obter tempo guardado no fim do jogo
        float tempo = PlayerPrefs.GetFloat("Tempo", 0f);

        textoTempo.text = "Tempo: " + tempo.ToString("F1") + "s";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            indiceAtual = (indiceAtual + 1) % botoes.Count;
            AtualizarSelecao();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
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

            if (i == indiceAtual)
                botoes[i].transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            else
                botoes[i].transform.localScale = Vector3.one;
        }

        EventSystem.current.SetSelectedGameObject(botoes[indiceAtual].gameObject);
    }

    public void JogarNovamente()
    {
        SceneManager.LoadScene("Jogo");
    }

    public void Sair()
    {
        Application.Quit(); // Fecha a aplicação na build
        Debug.Log("Jogo encerrado."); // Só aparece no Editor
    }
}
