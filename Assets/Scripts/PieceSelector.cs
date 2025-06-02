using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PieceSelector : MonoBehaviour
{
    public enum EstadoJogo { SelecionarPeca, MoverPeca }
    public EstadoJogo estadoAtual = EstadoJogo.SelecionarPeca;

    public GameObject[] piecePrefabs;
    private List<GameObject> pecasDisponiveis = new List<GameObject>();
    private int indiceAtual = 0;
    private GameObject pecaAtual;

    public Transform pontoSpawn;

    private int gridX = 0;
    private int gridZ = 0;

    private GameObject ultimaPecaColocada;
    private Material[] materiaisOriginais;
    private float tempoEntreMovimentos = 0.2f; // intervalo entre movimentos (em segundos)
    private float tempoDesdeUltimoMovimento = 0f;

    void Start()
    {
        pecasDisponiveis = new List<GameObject>(piecePrefabs);
        AtualizarPecaVisual();
    }

    void Update()
    {
        // Atualiza o timer
        tempoDesdeUltimoMovimento += Time.deltaTime;

        // Se o Tab está pressionado e passou o tempo necessário desde o último movimento
        if (Input.GetKey(KeyCode.Tab))
        {
            if (tempoDesdeUltimoMovimento >= tempoEntreMovimentos)
            {
                if (estadoAtual == EstadoJogo.SelecionarPeca)
                {
                    AvancarPeca();
                }
                else if (estadoAtual == EstadoJogo.MoverPeca)
                {
                    AvancarCelula();
                }

                tempoDesdeUltimoMovimento = 0f;
            }
        }

        // Restantes inputs mantêm-se a reagir apenas ao pressionar (GetKeyDown)
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
            if (estadoAtual == EstadoJogo.SelecionarPeca)
            {
                RemoverUltimaPeca();
            }
            else if (estadoAtual == EstadoJogo.MoverPeca)
            {
                RodarPeca();
            }
        }

        if (estadoAtual == EstadoJogo.MoverPeca)
        {
            if (pecaAtual == null || pecaAtual.transform == null)
            {
                AtualizarPecaVisual();
            }
            AtualizarPosicaoPreview();
        }
    }


    void AvancarPeca()
    {
        if (pecasDisponiveis.Count == 0)
        {
            Debug.Log("Todas as peças já foram colocadas.");
            return;
        }

        gridX = 0;
        gridZ = 0;
        indiceAtual = (indiceAtual + 1) % pecasDisponiveis.Count;
        AtualizarPecaVisual();
    }

    void AtualizarPecaVisual()
    {
        if (pecaAtual != null && pecaAtual.transform != null)
        {
            Destroy(pecaAtual);
            pecaAtual = null;
        }

        if (pontoSpawn == null || pecasDisponiveis.Count == 0) return;

        Quaternion rotacaoDeitada = Quaternion.Euler(90f, 0f, 0f);
        pecaAtual = Instantiate(pecasDisponiveis[indiceAtual], pontoSpawn.position, rotacaoDeitada);
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

        // Verifica colisões por cada cubo (BoxCollider)
        Collider[] colliders = pecaAtual.GetComponentsInChildren<Collider>();
        bool encontrouColisao = false;

        foreach (Collider c in colliders)
        {
            Vector3 centro = c.bounds.center;
            Vector3 metade = c.bounds.extents * 0.95f;

            Collider[] colisores = Physics.OverlapBox(centro, metade, c.transform.rotation);

            foreach (Collider col in colisores)
            {
                if (col.transform.root == pecaAtual.transform) continue;

                if (col.CompareTag("Obstaculo") || col.CompareTag("PlacedPiece"))
                {
                    Debug.Log("Colisão com: " + col.name + " (tag: " + col.tag + ")");
                    encontrouColisao = true;
                    break;
                }
            }

            if (encontrouColisao)
                break;
        }

        // Atualiza a cor da peça inteira com base na colisão
        Renderer[] renderers = pecaAtual.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = encontrouColisao ? Color.red : materiaisOriginais[i].color;
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

        // Verificar colisão em TODOS os cubos da peça
        Collider[] colliders = pecaAtual.GetComponentsInChildren<Collider>();
        bool encontrouColisao = false;

        foreach (Collider c in colliders)
        {
            Vector3 centro = c.bounds.center;
            Vector3 metade = c.bounds.extents * 0.95f;

            Collider[] colisores = Physics.OverlapBox(centro, metade, c.transform.rotation);

            foreach (Collider col in colisores)
            {
                if (col.transform.root == pecaAtual.transform) continue;

                if (col.CompareTag("Obstaculo") || col.CompareTag("PlacedPiece"))
                {
                    encontrouColisao = true;
                    break;
                }
            }

            if (encontrouColisao)
                break;
        }

        if (encontrouColisao)
        {
            Debug.Log("Não podes colocar a peça aqui — colisão detectada.");
            return;
        }

        // Colocação permitida
        Vector3 basePos = GridManager.Instance.GetWorldPosition(gridX, gridZ);
        float yTopo = 0.05f;
        float alturaPeca = pecaAtual.GetComponentInChildren<Renderer>().bounds.size.y;
        Vector3 finalPos = new Vector3(basePos.x, yTopo + alturaPeca / 2f, basePos.z);
        pecaAtual.transform.position = finalPos;

        // Marca toda a peça e os seus filhos com a tag "PlacedPiece"
        pecaAtual.tag = "PlacedPiece";
        foreach (Transform t in pecaAtual.GetComponentsInChildren<Transform>())
        {
            t.tag = "PlacedPiece";
        }
        ultimaPecaColocada = pecaAtual;

        if (indiceAtual >= 0 && indiceAtual < pecasDisponiveis.Count)
        {
            pecasDisponiveis.RemoveAt(indiceAtual);
            if (pecasDisponiveis.Count == 0)
            {
                pecaAtual = null;
                Debug.Log("Todas as peças foram colocadas.");
                return;
            }
            indiceAtual %= pecasDisponiveis.Count;
        }

        pecaAtual = null;
        estadoAtual = EstadoJogo.SelecionarPeca;
        AtualizarPecaVisual();
    }


    void RemoverUltimaPeca()
    {
        GameObject[] colocadas = GameObject.FindGameObjectsWithTag("PlacedPiece");

        if (estadoAtual == EstadoJogo.SelecionarPeca)
        {
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

        // Repor peça na lista se existir nos prefabs originais
        string nome = ultima.name.Replace("(Clone)", "").Trim();
        GameObject prefabOriginal = piecePrefabs.FirstOrDefault(p => p.name == nome);
        if (prefabOriginal != null && !pecasDisponiveis.Contains(prefabOriginal))
        {
            pecasDisponiveis.Add(prefabOriginal);
        }

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

    void RodarPeca()
    {
        if (pecaAtual != null)
        {
            pecaAtual.transform.Rotate(Vector3.up * 90f, Space.World);
        }
    }
}
