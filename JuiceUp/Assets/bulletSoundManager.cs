using UnityEngine;

public class bulletSoundManager : MonoBehaviour
{
    
    [SerializeField] AudioSource bulletSoundSource;
    [SerializeField] AudioSource deathSoundSource;

    public AudioClip BulletSound;
    public AudioClip DeathSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void PlaybulletAudioSource(AudioClip clip)
    {
        bulletSoundSource.PlayOneShot(clip);
    }

    public void PlaydeathAudioSource(AudioClip clip)
    {
        deathSoundSource.PlayOneShot(clip);
    }


}
