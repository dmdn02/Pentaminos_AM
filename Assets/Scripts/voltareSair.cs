using UnityEngine;
using UnityEngine.SceneManagement;



public class voltareSair : MonoBehaviour
{
    public void SairdoJogo()
    {
        Application.Quit();
        Debug.Log("Jogo encerrado.");
    }

    public void VoltarMenu()
    {
        SceneManager.LoadScene("Menu Inicial");
    }
}
