using UnityEngine;
using UnityEngine.Audio;
[System.Serializable]

public class Som
{
    public string nome; 
    public AudioClip clip;
    
    [Range(0f, 1f)] //Cria um slider no inspector para facilitar a regulagem do volume
    public float volume = 1f;

    [Range(0.5f, 1.5f)] //Cria um slider no inspector para facilitar a regulagem do pitch
    public float pitch = 1f;

    public bool loop = false; //Define se o áudio deve ser reproduzido em loop

    public tipoAudio tipoAudio = tipoAudio.EfeitoSonoro;

    [HideInInspector]   //Esconde o AudioSource no inspector
    public AudioSource source;
}

public enum tipoAudio 
{ 
    Musica, 
    EfeitoSonoro 
}

public class _AudioController : MonoBehaviour
{
    public static _AudioController instance;

    public Som[] musicas;       //lista de todas as musicas do jogo
    public Som[] efeitosSonoros; //lista de todos os efeitos sonoros do jogo

    public AudioMixerGroup grupoMusica;
    public AudioMixerGroup grupoEfeitosSonoros;

    public AudioSource musicSource;

    public AudioClip musicaFaseInicial;
    public AudioClip musicaFloresta;

    void Awake()
    {
        // Implementa o padrão Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (Som s in musicas)
        {
            s.source = gameObject.AddComponent<AudioSource>();


            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            s.source.outputAudioMixerGroup = grupoMusica;
        }

        foreach (Som s in efeitosSonoros)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            s.source.outputAudioMixerGroup = grupoEfeitosSonoros;
        }
    }

    void Start()
    {
        TocarMusica(musicaFaseInicial);
    }

    public void TocarSom(string nome)
    {
        Som s = System.Array.Find(efeitosSonoros, som => som.nome == nome);
        if (s == null)
        {
            Debug.LogWarning("Som: " + nome + " não encontrado!");
            return;
        }

        foreach (Som musica in musicas)
        {
            if (musica.source.isPlaying)
                musica.source.Stop();
        }

        s.source.Play();
    }

    public void TocarMusica(AudioClip novaMusica)
    {
        if (musicSource.clip == novaMusica && musicSource.isPlaying)
            return; // já está tocando

        musicSource.Stop();
        musicSource.clip = novaMusica;
        musicSource.Play();
    }

    public void TocarSomAleatorio(string[] nomes)
    {
        if(nomes.Length == 0) return;

        string nomeAleatorio = nomes[Random.Range(0, nomes.Length)];

        TocarSom(nomeAleatorio);
    }

    public void PararSom(string nome)
    {
        Som s = System.Array.Find(efeitosSonoros, som => som.nome == nome);
        if (s == null)
        {
            Debug.LogWarning("Som: " + nome + " não encontrado!");
            return;
        }

        s.source.Stop();
    }
}
