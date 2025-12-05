using UnityEngine;

public class AudioManager: MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Volume Settings")]
    [Range(0, 1)] public float masterVolume = 1f;
    [Range(0, 1)] public float musicVolume = 1f;
    [Range(0, 1)] public float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        musicSource.volume = musicVolume * masterVolume;
        sfxSource.volume = sfxVolume * masterVolume;
    }

    public void PlayMusic(AudioClip clip, bool loop= true)
    {
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
    }

    public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volume = 1f, float spatialBlend = 1f)
    {
        if (clip == null) return;

        GameObject audioObj = new GameObject("SFX3D_" + clip.name);
        audioObj.transform.position = position;

        AudioSource src = audioObj.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = sfxVolume * masterVolume * volume;
        src.spatialBlend = spatialBlend;
        src.minDistance = 1f;
        src.maxDistance = 20f;
        src.rolloffMode = AudioRolloffMode.Linear;

        // ?? AÑADIR FILTRO Y SCRIPT DE OCLUSIÓN
        var lp = audioObj.AddComponent<AudioLowPassFilter>();
        lp.cutoffFrequency = 20000f;

        var occlusion = audioObj.AddComponent<AudioOcclusion>();
        //occlusion.listener = Camera.main.transform;   // o el transform del jugador
        //occlusion.obstacleMask = LayerMask.GetMask("Obstacles");

        src.Play();
        Destroy(audioObj, clip.length + 0.1f);
    }
}
