using UnityEngine;

public class GestorCartoes : MonoBehaviour
{
    public CartaoConfiguracao[] cartoes; // preencher no Inspector
    public GameObject quadradoPretoPrefab;

    void Start()
    {
        SortearCartao();
    }

    public void SortearCartao()
    {
        int numeroDado = Random.Range(1, 7); // 1 a 6
        CartaoConfiguracao selecionado = cartoes[numeroDado - 1];

        foreach (Vector2Int pos in selecionado.posicoesBloqueadas)
        {
            Vector3 posMundo = GridManager.Instance.GetWorldPosition(pos.x, pos.y);
            Instantiate(quadradoPretoPrefab, posMundo + Vector3.up * 0.05f, Quaternion.identity);
        }

        Debug.Log($"Cartão sorteado: {selecionado.nome} (dado = {numeroDado})");
    }
}
