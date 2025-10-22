using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        public bool loop;
        
        [HideInInspector] public AudioSource source;
    }

    [Header("Sound Settings")]
    [SerializeField] private List<Sound> sounds = new List<Sound>();
    
    private Dictionary<string, Sound> soundDictionary = new Dictionary<string, Sound>();

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSounds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSounds()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            
            soundDictionary[s.name] = s;
        }
    }

    /// <summary>
    /// Plays a one-shot sound effect
    /// </summary>
    public void PlayOneShot(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound s))
        {
            if (s.source != null && s.clip != null)
            {
                s.source.PlayOneShot(s.clip, s.volume);
            }
            else
            {
                Debug.LogWarning($"Sound '{soundName}' has no audio source or clip assigned.");
            }
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found in SoundManager.");
        }
    }

    /// <summary>
    /// Plays a looped sound
    /// </summary>
    public void PlayLooped(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound s))
        {
            if (s.source != null && s.clip != null)
            {
                if (!s.source.isPlaying)
                {
                    s.source.loop = true;
                    s.source.Play();
                }
            }
            else
            {
                Debug.LogWarning($"Sound '{soundName}' has no audio source or clip assigned.");
            }
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found in SoundManager.");
        }
    }

    /// <summary>
    /// Stops a looped sound
    /// </summary>
    public void StopLooped(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound s))
        {
            if (s.source != null && s.source.isPlaying)
            {
                s.source.Stop();
            }
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found in SoundManager.");
        }
    }

    /// <summary>
    /// Stops all currently playing sounds
    /// </summary>
    public void StopAll()
    {
        foreach (Sound s in sounds)
        {
            if (s.source != null && s.source.isPlaying)
            {
                s.source.Stop();
            }
        }
    }

    /// <summary>
    /// Checks if a sound is currently playing
    /// </summary>
    public bool IsPlaying(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound s))
        {
            return s.source != null && s.source.isPlaying;
        }
        return false;
    }

    /// <summary>
    /// Changes the volume of a specific sound
    /// </summary>
    public void SetVolume(string soundName, float volume)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound s))
        {
            if (s.source != null)
            {
                s.volume = Mathf.Clamp01(volume);
                s.source.volume = s.volume;
            }
        }
    }

    /// <summary>
    /// Fades out a sound over time
    /// </summary>
    public void FadeOut(string soundName, float duration)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound s))
        {
            if (s.source != null && s.source.isPlaying)
            {
                StartCoroutine(FadeOutCoroutine(s.source, duration));
            }
        }
    }

    private System.Collections.IEnumerator FadeOutCoroutine(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }
    
    /// <summary>
    /// Changes the pitch of a specific sound
    /// </summary>
    public void SetPitch(string soundName, float pitch)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound s))
        {
            if (s.source != null)
            {
                s.pitch = Mathf.Clamp(pitch, 0.1f, 3f);
                s.source.pitch = s.pitch;
            }
        }
    }

}

// Example Usage:
// SoundManager.Instance.PlayOneShot("explosion");
// SoundManager.Instance.PlayLooped("backgroundMusic");
// SoundManager.Instance.StopLooped("backgroundMusic");
// SoundManager.Instance.FadeOut("backgroundMusic", 2f);