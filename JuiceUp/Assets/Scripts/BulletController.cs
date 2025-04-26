using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float Damage;
    public float Velocity;
    [HideInInspector] public bool IsFlipped;

    // Private Variables
    Rigidbody2D rb;

    private Vector2 targetPosition;
    private bool hasReachedTarget = false;
    private float reachThreshold = 0.5f; // Distance à laquelle on considère que la balle est "arrivée"

    public GameObject destroyVFX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;

        //Rotate towards Mouse
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        targetPosition = mousePos;
        Vector2 myPos = transform.position;
        Vector2 dir = mousePos - myPos;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);

        rb.linearVelocity = transform.up * Velocity; // * Time.fixedDeltaTime;
    }


    void Update()
    {
        if (!hasReachedTarget)
        {
            float distanceToTarget = Vector2.Distance(transform.position, targetPosition);

            if (distanceToTarget <= reachThreshold)
            {
                // Arrivé à la cible donc activer gravité pour faire tomber la balle
                rb.gravityScale = 3.8f;
                //rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); // STOP la vitesse horizontale, garde seulement la chute
                hasReachedTarget = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
            Instantiate(destroyVFX, transform.position, Quaternion.identity);
        }
        else if (collision.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyController>().GetDamage(Damage);
            Destroy(gameObject);
        }
    }
}
