using UnityEngine;

public class MusicaFundo : MonoBehaviour
{
    public static MusicaFundo Instance;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("MusicaFundo: Nenhum AudioSource encontrado no GameObject!");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AlternarSom()
    {
        if (audioSource == null)
        {
            Debug.LogError("MusicaFundo: AudioSource está nulo ao tentar alternar o som.");
            return;
        }

        audioSource.mute = !audioSource.mute;
        Debug.Log("Som agora está: " + (audioSource.mute ? "DESLIGADO" : "LIGADO"));
    }

    public bool EstaAtivo()
    {
        return audioSource != null && !audioSource.mute;
    }
}
