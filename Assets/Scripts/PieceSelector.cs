using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PieceSelector : MonoBehaviour
{
    public enum Jogador { Jogador1, Jogador2 }
    private Jogador jogadorAtual = Jogador.Jogador1;
    public TMPro.TextMeshProUGUI textoJogadorAtual;

    public GameObject[] piecePrefabs;
    private List<GameObject> pecasDisponiveis = new List<GameObject>();

    private GameObject pecaAtual;
    private int indicePecaAtual = 0;

    public Transform pontoSpawn;

    private int gridX = 0;
    private int gridZ = 0;

    private float tempoMovimento = 0f;
    private float delayMovimento = 0.2f;

    private bool moverPeca = false;

    private GameObject ultimaPecaColocada;
    private Material[] materiaisOriginais;

    void Start()
    {
        pecasDisponiveis = new List<GameObject>(piecePrefabs);
        CriarPecaAtual();
    }

    void Update()
    {
        if (moverPeca)
        {
            if (Input.GetKey(KeyCode.Tab) && Input.GetKey(KeyCode.Backspace))
            {
                tempoMovimento += Time.deltaTime;
                if (tempoMovimento >= delayMovimento)
                {
                    tempoMovimento = 0f;
                    RetrocederPosicao();
                }
            }
            else if (Input.GetKey(KeyCode.Tab))
            {
                tempoMovimento += Time.deltaTime;
                if (tempoMovimento >= delayMovimento)
                {
                    tempoMovimento = 0f;
                    AvancarPosicao();
                }
            }
            else
            {
                tempoMovimento = delayMovimento;

                if (Input.GetKeyDown(KeyCode.Backspace) && !Input.GetKey(KeyCode.Tab))
                {
                    RodarPeca();
                }
            }

            AtualizarPosicaoPeca();
            AtualizarCorDaPeca();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                AvancarPeca();
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                RemoverUltimaPeca();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!moverPeca)
            {
                moverPeca = true;
                gridX = 0;
                gridZ = 0;
            }
            else
            {
                TentarColocarPeca();
            }
        }
    }

    void CriarPecaAtual()
    {
        if (pecaAtual != null) Destroy(pecaAtual);
        if (pecasDisponiveis.Count == 0)
        {
            Debug.Log("Não há mais peças para jogar.");
            return;
        }
        pecaAtual = Instantiate(pecasDisponiveis[indicePecaAtual], pontoSpawn.position, Quaternion.Euler(90, 0, 0));
        pecaAtual.tag = "PreviewPiece";

        Renderer[] renderers = pecaAtual.GetComponentsInChildren<Renderer>();
        materiaisOriginais = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            materiaisOriginais[i] = new Material(renderers[i].material);
        }

        Color corJogador = jogadorAtual == Jogador.Jogador1 ? Color.blue : Color.green;

        foreach (Renderer rend in renderers)
        {
            rend.material.color = corJogador;
        }
    }

    void AvancarPeca()
    {
        if (pecasDisponiveis.Count == 0) return;
        indicePecaAtual = (indicePecaAtual + 1) % pecasDisponiveis.Count;
        CriarPecaAtual();
    }

    void AvancarPosicao()
    {
        gridX++;
        if (gridX > 7)
        {
            gridX = 0;
            gridZ++;
        }
        if (gridZ > 7)
        {
            gridX = 0;
            gridZ = 0;
        }
    }

    void AtualizarPosicaoPeca()
    {
        if (pecaAtual == null) return;

        Vector3 posBase = GridManager.Instance.GetWorldPosition(gridX, gridZ);
        float yPos = 0.05f + GetAlturaPeca() / 2f;
        pecaAtual.transform.position = new Vector3(posBase.x, yPos, posBase.z);
    }

    float GetAlturaPeca()
    {
        Renderer rend = pecaAtual.GetComponentInChildren<Renderer>();
        if (rend != null) return rend.bounds.size.y;
        return 1f;
    }

    void AtualizarCorDaPeca()
    {
        Renderer[] renderers = pecaAtual.GetComponentsInChildren<Renderer>();
        if (VerificarColisao())
        {
            foreach (Renderer rend in renderers)
                rend.material.color = Color.red;
        }
        else
        {
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].material.color = materiaisOriginais[i].color;
        }
    }

    void TentarColocarPeca()
    {
        if (!PecaCabeNoTabuleiro())
        {
            Debug.Log("A peça não está totalmente dentro do tabuleiro.");
            return;
        }

        if (VerificarColisao())
        {
            Debug.Log("Colisão detectada! Não podes colocar aqui.");
            return;
        }

        pecaAtual.tag = "PlacedPiece";
        foreach (Transform t in pecaAtual.GetComponentsInChildren<Transform>())
        {
            t.tag = "PlacedPiece";
        }

        ultimaPecaColocada = pecaAtual;
        pecasDisponiveis.RemoveAt(indicePecaAtual);

        if (pecasDisponiveis.Count == 0)
        {
            Debug.Log("Jogo terminado, não há mais peças.");
            pecaAtual = null;
            moverPeca = false;
            return;
        }
        else
        {
            indicePecaAtual %= pecasDisponiveis.Count;
        }

        pecaAtual = null;
        moverPeca = false;
        CriarPecaAtual();
        AlternarJogador();
    }

    bool VerificarColisao()
    {
        Collider[] cubos = pecaAtual.GetComponentsInChildren<Collider>();
        foreach (Collider c in cubos)
        {
            Vector3 centro = c.bounds.center;
            Vector3 metade = c.bounds.extents * 0.9f;
            Collider[] colisores = Physics.OverlapBox(centro, metade, c.transform.rotation);

            foreach (Collider col in colisores)
            {
                if (col.transform.root == pecaAtual.transform) continue;
                if (col.CompareTag("PlacedPiece") || col.CompareTag("Obstaculo"))
                    return true;
            }
        }
        return false;
    }

    void RetrocederPosicao()
    {
        gridX--;
        if (gridX < 0)
        {
            gridX = 7;
            gridZ--;
        }
        if (gridZ < 0)
        {
            gridX = 7;
            gridZ = 7;
        }
    }

    void RemoverUltimaPeca()
    {
        if (ultimaPecaColocada != null)
        {
            // Recuperar nome original da peça (sem o (Clone))
            string nome = ultimaPecaColocada.name.Replace("(Clone)", "").Trim();

            // Procurar o prefab original
            GameObject prefabOriginal = piecePrefabs.FirstOrDefault(p => p.name == nome);

            if (prefabOriginal != null && !pecasDisponiveis.Contains(prefabOriginal))
            {
                pecasDisponiveis.Add(prefabOriginal);
            }

            Destroy(ultimaPecaColocada);
            ultimaPecaColocada = null;
            CriarPecaAtual(); // Atualiza a visualização
        }
        else
        {
            Debug.Log("Não há peça para remover.");
        }
    }


    void RodarPeca()
    {
        if (pecaAtual == null) return;
        pecaAtual.transform.Rotate(Vector3.up * 90f, Space.World);
    }

    bool PecaCabeNoTabuleiro()
    {
        if (pecaAtual == null) return false;

        foreach (Transform cubo in pecaAtual.transform)
        {
            Vector3 posCubo = cubo.position;
            int gridXDoCubo = Mathf.FloorToInt(posCubo.x / GridManager.CellSize);
            int gridZDoCubo = Mathf.FloorToInt(posCubo.z / GridManager.CellSize);

            if (!GridManager.Instance.IsInsideGrid(gridXDoCubo, gridZDoCubo))
            {
                return false;
            }
        }

        return true;
    }

    void AlternarJogador()
    {
        jogadorAtual = jogadorAtual == Jogador.Jogador1 ? Jogador.Jogador2 : Jogador.Jogador1;
        Debug.Log("Agora é a vez do " + jogadorAtual.ToString());

        if (textoJogadorAtual != null)
            textoJogadorAtual.text = "Jogador: " + jogadorAtual.ToString();
    }

}
