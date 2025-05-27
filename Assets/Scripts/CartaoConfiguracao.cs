using UnityEngine;

[System.Serializable]
public class CartaoConfiguracao
{
    public string nome;
    public Vector2Int[] posicoesBloqueadas = new Vector2Int[4]; // posi��es (X, Z) no tabuleiro
}
