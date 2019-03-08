using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GWBulletBomb : MonoBehaviour
{
    private float GWBB_TimerToExplode;
    private float GWBB_ExplosionTimer;
    private bool GWBB_ExplodeNow;
    private Vector2 GWBB_OverlapCenter;
    private Vector2 GWBB_OverlapSize;
    private Rigidbody2D GWBB_RigidBody;
    private SpriteRenderer GWBB_SpriteRenderer;
    private Collider2D[] GWBB_Player;
    private bool GWBB_ApplyDamage;
    private int GWBB_Damage;
    private int GWBB_NumberOfExplosions;
    private float GWBB_ExplosionRadius;
    public LayerMask GWBB_CanBeDamaged;
    public Explosion GWBB_Explosion;


    void Awake()
    {
        GWBB_TimerToExplode = Random.Range(1.0f, 5.0f);
        GWBB_RigidBody = gameObject.GetComponent<Rigidbody2D>();
        GWBB_SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        GWBB_ExplosionTimer = 1.0f;
        GWBB_ExplodeNow = false;
        GWBB_OverlapSize = new Vector2(2.0f, 13.0f);
        GWBB_NumberOfExplosions = 5;
        GWBB_Damage = 10;
        GWBB_ExplosionRadius = 1.12f;
        GWBB_ApplyDamage = true;
    }


    void Update()
    {
        if (GWBB_TimerToExplode > 0.0f)
        {
            //If the timer is less than 2.0f start slowing down
            if (GWBB_TimerToExplode < 2.0f)
            {
                //Going right
                if (GWBB_RigidBody.velocity.x > 0.01f)
                {
                    GWBB_RigidBody.velocity = new Vector2(GWBB_RigidBody.velocity.x - 0.01f, GWBB_RigidBody.velocity.y);
                }
                //Going left
                else if (GWBB_RigidBody.velocity.x < -0.01f)
                {
                    GWBB_RigidBody.velocity = new Vector2(GWBB_RigidBody.velocity.x + 0.01f, GWBB_RigidBody.velocity.y);
                }
            }
            GWBB_TimerToExplode -= Time.deltaTime;
        }
        else
        {
            GWBB_Explode();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(GWBB_OverlapCenter, GWBB_OverlapSize);
    }

    private void GWBB_Explode()
    {

        if (GWBB_ExplodeNow)
        {
            //Stop
            GWBB_RigidBody.velocity = new Vector2(0.0f, 0.0f);
            GWBB_OverlapCenter = gameObject.transform.position;

            for (int i = 0; i < GWBB_NumberOfExplosions; i += 2)
            {
                //Create explosions above and below the bullet
                Instantiate(GWBB_Explosion, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + GWBB_ExplosionRadius + i), gameObject.transform.rotation);
                Instantiate(GWBB_Explosion, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - GWBB_ExplosionRadius - i), gameObject.transform.rotation);
            }
            //Create the overlap box
            GWBB_Player = Physics2D.OverlapBoxAll(GWBB_OverlapCenter, GWBB_OverlapSize, 0.0f, GWBB_CanBeDamaged);
            for (int i = 0; i < GWBB_Player.Length; i++)
            {
                //Damage the player
                if (GWBB_ApplyDamage)
                {
                    GWBB_Player[i].GetComponent<Health>().H_DealDamage(GWBB_Damage);
                    GWBB_ApplyDamage = false;
                }
            }

            Destroy(gameObject);
        }
        else
        {
            if(GWBB_ExplosionTimer<=0.0f)
            {
                GWBB_ExplodeNow = true;
            }
            else
            {
                GWBB_SpriteRenderer.color = Color.red;

                GWBB_ExplosionTimer -= Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag.Equals("Player"))
        {
            GWBB_Explode();
            GWBB_TimerToExplode = 0.0f;
        }
    }
}

