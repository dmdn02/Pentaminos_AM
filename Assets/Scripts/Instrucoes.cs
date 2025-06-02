using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Instrucoes : MonoBehaviour
{
    public List<Button> botoes; // Ordem: Jogar, Instruções, Som, Sair
    public Color corNormal = new Color(0.86f, 0.22f, 0.22f, 1f);       // Vermelho normal
    public Color corSelecionado = new Color(0.86f, 0.22f, 0.22f, 0.3f); // Vermelho transparente

    private int indiceAtual = 0;

    void Start()
    {
        AtualizarSelecao();
    }

    void Update()
    {
        // Avança com TAB
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            indiceAtual = (indiceAtual + 1) % botoes.Count;
            AtualizarSelecao();
        }

        // Ativa com ENTER
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
            colors.normalColor = corNormal; // mantém a cor
            botoes[i].colors = colors;

            // Aumentar botão selecionado, manter os outros normais
            if (i == indiceAtual)
                botoes[i].transform.localScale = new Vector3(1.2f, 1.2f, 1f); // 20% maior
            else
                botoes[i].transform.localScale = Vector3.one; // tamanho normal (1,1,1)
        }

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(botoes[indiceAtual].gameObject);
        }
    }

    public void VoltarAoMenu()
    {
        SceneManager.LoadScene("Menu Inicial");
    }
}
