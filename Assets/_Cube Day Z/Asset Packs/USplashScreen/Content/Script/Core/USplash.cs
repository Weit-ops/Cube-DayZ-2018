using UnityEngine;
using System.Collections;

public class USplash : MonoBehaviour {

    [Header("Splash Settings")]
    public float DelayStart = 0.5f;
    [Header("Animation Settings")]
    public Animation m_animation;
    public string ShowAnimation = "Show";
    public string HideAnimation = "Hide";
    [Range(0.1f,10.0f)]
    public float ShowAnimSpeed = 1.0f;
    [Range(0.1f, 10.0f)]
    public float HideAnimSpeed = 1.0f;
    [Header("Sound Settings")]
    public AudioClip ShowSound;
    public AudioClip HideSound;
    [Range(0.0f, 1.0f)]
    public float m_volume;
    [Range(0.0f,2.0f)]
    public float m_pitch = 1.0f;
    /// <summary>
    /// 
    /// </summary>
    void OnEnable()
    {
        //On this is active, start function
        StartCoroutine(ShowCorrutine());
    }

    /// <summary>
    /// Call this when splash if end for hide it.
    /// </summary>
    public void Hide()
    {
        StartCoroutine(HideCorrutine());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator ShowCorrutine()
    {
        //If have delay for start, wait for it pass.
        if (DelayStart > 0.0f)
        {
            yield return new WaitForSeconds(DelayStart);
        }
        if (ShowSound)
        {
            PlayAudioClip(ShowSound, m_volume, m_pitch);
        }
        if (m_animation != null)
        {
            m_animation[ShowAnimation].speed = ShowAnimSpeed;
            m_animation.Play(ShowAnimation);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator HideCorrutine()
    {
        if (HideSound)
        {
            PlayAudioClip(HideSound, m_volume, m_pitch);
        }
        if (m_animation != null)
        {
            m_animation[HideAnimation].speed = HideAnimSpeed;
            m_animation.Play(HideAnimation);
            yield return new WaitForSeconds(m_animation[HideAnimation].length);
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <returns></returns>
    AudioSource PlayAudioClip(AudioClip clip, float volume, float pitch)
    {
        GameObject go = new GameObject("One shot audio");
        if (Camera.main != null)
        {
            go.transform.position = Camera.main.transform.position;
        }
        else
        {
            go.transform.position = Camera.current.transform.position;
        }
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
        Destroy(go, clip.length);
        return source;
    }

    /// <summary>
    /// 
    /// </summary>
}