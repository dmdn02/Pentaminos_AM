using UnityEngine;

public class GerarTabuleiro : MonoBehaviour
{
    public GameObject quadradoPrefab;  // teu prefab preto atual
    public GameObject laranjaPrefab;   // novo prefab laranja
    public int linhas = 8;
    public int colunas = 8;

    void Start()
    {
        float espacamento = GridManager.CellSize; // mantém o valor do GridManager

        for (int x = 0; x < colunas; x++)
        {
            for (int z = 0; z < linhas; z++)
            {
                Vector3 pos = new Vector3(x * espacamento, 0, z * espacamento);

                // Alterna entre os prefabs com base na soma x + z (estilo tabuleiro xadrez)
                GameObject prefabParaInstanciar = ((x + z) % 2 == 0) ? quadradoPrefab : laranjaPrefab;

                GameObject quadrado = Instantiate(prefabParaInstanciar, pos, Quaternion.identity);
                quadrado.name = $"Quadrado_{x}_{z}";
                quadrado.tag = "Quadrado";

                if (!quadrado.TryGetComponent<Collider>(out _))
                {
                    quadrado.AddComponent<BoxCollider>();
                }
            }
        }
    }
}
