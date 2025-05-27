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
                estadoAtual = EstadoJogo.SelecionarPeca;
                AvancarPeca();
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

        AtualizarPosicaoPreview();
    }

    void AvancarPeca()
    {
        indiceAtual = (indiceAtual + 1) % piecePrefabs.Length;
        AtualizarPecaVisual();
    }

    void AtualizarPecaVisual()
    {
        if (pecaAtual != null)
        { 
            Destroy(pecaAtual);
            pecaAtual = null;
        }

        Quaternion rotacaoDeitada = Quaternion.Euler(90f, 0f, 0f); // roda a peça 90º para deitar
        pecaAtual = Instantiate(piecePrefabs[indiceAtual], pontoSpawn.position, rotacaoDeitada);
        pecaAtual.tag = "PreviewPiece";
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
            gridZ = 0; // recomeça
        }
    }

    void AtualizarPosicaoPreview()
    {
        if (pecaAtual == null || !pecaAtual) return;

        if (estadoAtual == EstadoJogo.MoverPeca)
        {
            Vector3 basePos = GridManager.Instance.GetWorldPosition(gridX, gridZ);
            float yTopo = 0.05f;
            float alturaPeca = pecaAtual.GetComponentInChildren<Renderer>().bounds.size.y;
            Vector3 finalPos = new Vector3(basePos.x, yTopo + alturaPeca / 2f, basePos.z);
            pecaAtual.transform.position = finalPos;

            // Verifica se está a colidir com um obstáculo
            Bounds bounds = pecaAtual.GetComponentInChildren<Renderer>().bounds;

            Collider[] colisores = Physics.OverlapBox(
                bounds.center,
                bounds.extents,
                pecaAtual.transform.rotation
            );

            bool encontrouObstaculo = false;

            foreach (Collider col in colisores)
            {
                if (col.CompareTag("Obstaculo"))
                {
                    encontrouObstaculo = true;
                    break;
                }
            }

            // Muda a cor com base no resultado
            Renderer rend = pecaAtual.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                if (encontrouObstaculo)
                    rend.material.color = Color.red;
                else
                    rend.material.color = Color.white;
            }
        }
    }

    void ColocarPeca()
    {
        if (pecaAtual != null)
        {
            // Verifica colisão com obstáculos
            Bounds bounds = pecaAtual.GetComponentInChildren<Renderer>().bounds;

            Collider[] colisores = Physics.OverlapBox(
                bounds.center,
                bounds.extents,
                pecaAtual.transform.rotation
            );

            foreach (Collider col in colisores)
            {
                if (col.CompareTag("Obstaculo"))
                {
                    Debug.Log("Não podes colocar a peça aqui — há um obstáculo.");
                    return; // impede a colocação
                }
            }

            // Se não houve colisão com obstáculos, coloca a peça
            pecaAtual.tag = "PlacedPiece";
            pecaAtual = null;

            estadoAtual = EstadoJogo.SelecionarPeca;
            gridX = 0;
            gridZ = 0;
            AvancarPeca();
        }
    }

    void RemoverUltimaPeca()
    {
        GameObject[] colocadas = GameObject.FindGameObjectsWithTag("PlacedPiece");
        if (colocadas.Length > 0)
        {
            GameObject ultima = colocadas[colocadas.Length - 1];
            Destroy(ultima);

            //Se a peça atual for esta que foi destruída, limpa a referência
            if (pecaAtual == ultima)
            {
                pecaAtual = null;
            }
        }
    }

    void RodarPeca()
    {
        if (pecaAtual != null)
        {
            // Roda em torno do eixo global Y, mantendo a peça deitada
            pecaAtual.transform.Rotate(Vector3.up * 90f, Space.World);
        }
    }
}
