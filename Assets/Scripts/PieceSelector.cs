using UnityEngine;
using System.Collections.Generic;

public class PieceSelector : MonoBehaviour
{
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

                // Só roda a peça se estiver em modo mover, pressionar Backspace, e NÃO estiver a carregar Tab
                if (Input.GetKeyDown(KeyCode.Backspace) && !Input.GetKey(KeyCode.Tab))
                {
                    RodarPeca();
                }
            }

            AtualizarPosicaoPeca();
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
    }

    void AvancarPeca()
    {
        if (pecasDisponiveis.Count == 0) return;
        indicePecaAtual = (indicePecaAtual + 1) % pecasDisponiveis.Count;
        CriarPecaAtual();
    }

    void AvancarPosicao()
    {
        int largura = GridManager.Instance.width;
        int altura = GridManager.Instance.height;

        for (int i = 1; i <= largura * altura; i++)
        {
            int proximaX = (gridX + i) % largura;
            int proximaZ = (gridZ + (gridX + i) / largura) % altura;

            if (PecaCabeNoTabuleiro(proximaX, proximaZ))
            {
                gridX = proximaX;
                gridZ = proximaZ;
                return;
            }
        }
    }

    bool PecaCabeNoTabuleiro(int x, int z)
    {
        if (pecaAtual == null) return false;

        Vector3 posBase = GridManager.Instance.GetWorldPosition(x, z);
        float yPos = 0.05f + GetAlturaPeca() / 2f;

        foreach (Transform cubo in pecaAtual.transform)
        {
            Vector3 posCuboNoMundo = posBase + (cubo.localPosition);
            int gridXDoCubo = Mathf.RoundToInt(posCuboNoMundo.x / GridManager.CellSize);
            int gridZDoCubo = Mathf.RoundToInt(posCuboNoMundo.z / GridManager.CellSize);

            if (!GridManager.Instance.IsInsideGrid(gridXDoCubo, gridZDoCubo))
            {
                return false;
            }
        }

        return true;
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

    void TentarColocarPeca()
    {
        if (!PecaCabeNoTabuleiro(gridX, gridZ))
        {
            Debug.Log("Peça não cabe no tabuleiro aqui!");
            return;
        }

        if (VerificarColisao())
        {
            Debug.Log("Colisão detectada! Não podes colocar aqui.");
            return;
        }

        // Colocar peça na cena, marca como colocada
        pecaAtual.tag = "PlacedPiece";
        ultimaPecaColocada = pecaAtual;

        // Remove a peça da lista disponível e prepara para próxima
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
        int largura = GridManager.Instance.width;
        int altura = GridManager.Instance.height;

        for (int i = 1; i <= largura * altura; i++)
        {
            int index = ((gridZ * largura + gridX) - i + largura * altura) % (largura * altura);
            int anteriorX = index % largura;
            int anteriorZ = index / largura;

            if (PecaCabeNoTabuleiro(anteriorX, anteriorZ))
            {
                gridX = anteriorX;
                gridZ = anteriorZ;
                return;
            }
        }
    }


    void RemoverUltimaPeca()
    {
        if (ultimaPecaColocada != null)
        {
            pecasDisponiveis.Add(ultimaPecaColocada);
            Destroy(ultimaPecaColocada);
            ultimaPecaColocada = null;
            CriarPecaAtual();
        }
        else
        {
            Debug.Log("Não há peça para remover.");
        }
    }

    void RodarPeca()
    {
        if (pecaAtual == null) return;

        pecaAtual.transform.Rotate(0, 90, 0);

        if (!PecaCabeNoTabuleiro(gridX, gridZ))
        {
            pecaAtual.transform.Rotate(0, -90, 0);
            Debug.Log("Rotação inválida - sairia fora do tabuleiro.");
        }
    }
}
