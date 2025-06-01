using UnityEngine;

public class PieceSelector : MonoBehaviour
{
    public enum EstadoJogo { SelecionarPeca, MoverPeca }
    public EstadoJogo estadoAtual = EstadoJogo.SelecionarPeca;

    public GameObject[] piecePrefabs;
    private int indiceAtual = 0;
    private GameObject pecaAtual;

    public Transform pontoSpawn;

    private int gridX = 0;
    private int gridZ = 0;

    private GameObject ultimaPecaColocada;
    private Material[] materiaisOriginais;

    void Start()
    {
        AtualizarPecaVisual();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (estadoAtual == EstadoJogo.SelecionarPeca)
            {
                AvancarPeca();
            }
            else if (estadoAtual == EstadoJogo.MoverPeca)
            {
                AvancarCelula();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (estadoAtual == EstadoJogo.SelecionarPeca)
            {
                estadoAtual = EstadoJogo.MoverPeca;
            }
            else if (estadoAtual == EstadoJogo.MoverPeca)
            {
                ColocarPeca();
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            RemoverUltimaPeca();
        }

        if (estadoAtual == EstadoJogo.MoverPeca)
        {
            if (pecaAtual == null || pecaAtual.transform == null)
            {
                AtualizarPecaVisual(); // recria se tiver sido destruída
            }
            AtualizarPosicaoPreview();
        }
    }

    void AvancarPeca()
    {
        gridX = 0;
        gridZ = 0;
        indiceAtual = (indiceAtual + 1) % piecePrefabs.Length;
        AtualizarPecaVisual();
    }

    void AtualizarPecaVisual()
    {
        if (pecaAtual != null && pecaAtual.transform != null)
        {
            Destroy(pecaAtual);
            pecaAtual = null;
        }

        if (pontoSpawn == null)
        {
            Debug.LogWarning("pontoSpawn não está atribuído.");
            return;
        }

        Quaternion rotacaoDeitada = Quaternion.Euler(90f, 0f, 0f);
        pecaAtual = Instantiate(piecePrefabs[indiceAtual], pontoSpawn.position, rotacaoDeitada);
        pecaAtual.tag = "PreviewPiece";

        Renderer[] renderers = pecaAtual.GetComponentsInChildren<Renderer>();
        materiaisOriginais = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            materiaisOriginais[i] = new Material(renderers[i].material);
        }
    }

    void AvancarCelula()
    {
        gridX++;
        if (gridX >= GridManager.Instance.width)
        {
            gridX = 0;
            gridZ++;
        }

        if (gridZ >= GridManager.Instance.height)
        {
            gridZ = 0;
        }
    }

    void AtualizarPosicaoPreview()
    {
        if (pecaAtual == null || pecaAtual.transform == null) return;

        Vector3 basePos = GridManager.Instance.GetWorldPosition(gridX, gridZ);
        float yTopo = 0.05f;
        float alturaPeca = pecaAtual.GetComponentInChildren<Renderer>().bounds.size.y;
        Vector3 finalPos = new Vector3(basePos.x, yTopo + alturaPeca / 2f, basePos.z);
        pecaAtual.transform.position = finalPos;

        Bounds bounds = pecaAtual.GetComponentInChildren<Renderer>().bounds;
        Vector3 extentsReduzidos = bounds.extents * 0.9f;

        Collider[] colisores = Physics.OverlapBox(
            bounds.center,
            extentsReduzidos,
            pecaAtual.transform.rotation
        );

        bool encontrouObstaculo = false;

        foreach (Collider col in colisores)
        {
            if (col.transform.root == pecaAtual.transform) continue;
            if (col.CompareTag("Obstaculo") || col.CompareTag("PlacedPiece"))
            {
                encontrouObstaculo = true;
                break;
            }
        }

        Renderer[] renderers = pecaAtual.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = encontrouObstaculo ? Color.red : materiaisOriginais[i].color;
        }
    }

    void ColocarPeca()
    {
        if (pecaAtual == null || pecaAtual.transform == null) return;

        gridX = Mathf.Clamp(gridX, 0, GridManager.Instance.width - 1);
        gridZ = Mathf.Clamp(gridZ, 0, GridManager.Instance.height - 1);

        if (!GridManager.Instance.IsInsideGrid(gridX, gridZ))
        {
            Debug.Log("Posição base fora do tabuleiro.");
            return;
        }

        Bounds bounds = pecaAtual.GetComponentInChildren<Renderer>().bounds;
        Vector3 extentsReduzidos = bounds.extents * 0.9f;

        Collider[] colisores = Physics.OverlapBox(
            bounds.center,
            extentsReduzidos,
            pecaAtual.transform.rotation
        );

        foreach (Collider col in colisores)
        {
            if (col.transform.root == pecaAtual.transform) continue;
            if (col.CompareTag("Obstaculo") || col.CompareTag("PlacedPiece"))
            {
                Debug.Log("Não podes colocar a peça aqui — espaço ocupado.");
                AtualizarPosicaoPreview();
                return;
            }
        }

        Vector3 basePos = GridManager.Instance.GetWorldPosition(gridX, gridZ);
        float yTopo = 0.05f;
        float alturaPeca = pecaAtual.GetComponentInChildren<Renderer>().bounds.size.y;
        Vector3 finalPos = new Vector3(basePos.x, yTopo + alturaPeca / 2f, basePos.z);
        pecaAtual.transform.position = finalPos;

        pecaAtual.tag = "PlacedPiece";
        ultimaPecaColocada = pecaAtual;
        pecaAtual = null;
        estadoAtual = EstadoJogo.SelecionarPeca;
    }

    void RemoverUltimaPeca()
    {
        GameObject[] colocadas = GameObject.FindGameObjectsWithTag("PlacedPiece");

        if (estadoAtual == EstadoJogo.SelecionarPeca)
        {
            // Não apagar a peça atual, apenas prevenir e recriar se foi destruída
            if (pecaAtual == null || pecaAtual.transform == null)
            {
                Debug.Log("Peça de pré-visualização não encontrada, a recriar...");
                AtualizarPecaVisual();
            }
            else
            {
                Debug.Log("Não podes apagar a peça atual de pré-visualização.");
            }
            return;
        }

        if (colocadas.Length == 0)
        {
            Debug.Log("Não há peças no tabuleiro para remover.");
            return;
        }

        GameObject ultima = colocadas[colocadas.Length - 1];
        Destroy(ultima);

        if (pecaAtual == ultima)
        {
            pecaAtual = null;
        }

        estadoAtual = EstadoJogo.SelecionarPeca;

        if (pecaAtual == null || pecaAtual.transform == null)
        {
            AtualizarPecaVisual();
        }
    }


}
