using UnityEngine;

public class GerarTabuleiro : MonoBehaviour
{
    public GameObject quadradoPrefab;
    public int linhas = 8;
    public int colunas = 8;
    public float espacamento = 1f;

    void Start()
    {
        for (int x = 0; x < colunas; x++)
        {
            for (int z = 0; z < linhas; z++)
            {
                Vector3 pos = new Vector3(x * espacamento, 0, z * espacamento);
                GameObject quadrado = Instantiate(quadradoPrefab, pos, Quaternion.identity);
                // Garante que tem a tag certa e um collider para interação
                quadrado.tag = "Quadrado";
                if (!quadrado.TryGetComponent<Collider>(out _))
                {
                    quadrado.AddComponent<BoxCollider>();
                }
            }
        }
    }
}
