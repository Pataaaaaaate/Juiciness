using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float Health;
    public float MaxSpeed;
    public float AccelerationRate;

    // Private Variables
    float Speed;
    float DriftFactor;
    GameObject Player;
    Vector2 PlayerDirection;
    Vector2 PreviousPlayerDirection;
    Rigidbody2D rb;
    BoxCollider2D col;

    public GameObject deathVFX;
    public GameObject dmgVFX;
    public GameObject death2VFX;


    
    public AudioSource dmgSound;
    //public MeshRenderer ennemi;

    bulletSoundManager bulletSoundManager;
    void Start()
    {

        bulletSoundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<bulletSoundManager>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        Player = GameObject.FindWithTag("Player");
        DriftFactor = 1;
    }

    void Update()
    {
        //Should I rotate towards Player ?
        PlayerDirection = Player.transform.position - transform.position;
        if(Mathf.Sign(PlayerDirection.x) != Mathf.Sign(PreviousPlayerDirection.x))
        {
            RotateTowardsPlayer();
        }
        PreviousPlayerDirection = PlayerDirection;

        //Go towards Player
        rb.linearVelocity = new Vector2(transform.forward.z * DriftFactor * Speed * Time.fixedDeltaTime, rb.linearVelocity.y);

        //Die
        if(Health <= 7) // meurt au boout de 3 coups
        {
            Destroy(gameObject);
            bulletSoundManager.PlaydeathAudioSource(bulletSoundManager.DeathSound);
            Instantiate(death2VFX, transform.position, Quaternion.identity);
            Instantiate(deathVFX, transform.position, Quaternion.identity);
        }

        if(Speed <= 0)
        {
            StartCoroutine(GetToSpeed(MaxSpeed));
        }
        //Debug.Log(Speed);
    }

    public void GetDamage(float dmg)
    {
        Health -= dmg;
        dmgSound.Play();
        Instantiate(dmgVFX, transform.position, Quaternion.identity);
        //ennemi.material.SetColor("_Color", Color.red);
    }

    void RotateTowardsPlayer()
    {
        if (PlayerDirection.x < 0)
        {
            transform.rotation = new Quaternion(0, 180, 0, 0);
        }
        else
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        DriftFactor = -1;
        StartCoroutine(GetToSpeed(0));
    }

    IEnumerator GetToSpeed( float s)
    {
        //Debug.Log(s);
        float baseSpeed = Speed;
        float SignMultiplier = Mathf.Sign(s - Speed);
        for(float f=baseSpeed; f*SignMultiplier<=s; f += AccelerationRate*SignMultiplier)
        {
            Speed = f;
            yield return null;
        }
        DriftFactor = 1;
    }
}
