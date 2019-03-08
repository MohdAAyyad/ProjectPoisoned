using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GWBomb : MonoBehaviour
{

    private float GWB_ExplosionRadius;
    private int GWB_Damage;
    private bool GWB_Exploded;
    private bool GWB_PlayExplosionAnimation;
    private float GWB_TimerToSelfExplode;
    private Animator GWB_Animator;
    public LayerMask GWB_PlayerToDamageMask; 

    // Start is called before the first frame update
    void Start()
    {
        GWB_ExplosionRadius = 1.12f;
        GWB_Exploded = false;
        GWB_Damage = 5;
        GWB_PlayExplosionAnimation = false;
        GWB_TimerToSelfExplode = 30.0f;
        GWB_Animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        if(GWB_PlayExplosionAnimation)
        {
            GWB_Animator.SetBool("GWB_Explode", true);
        }

        if(GWB_TimerToSelfExplode>0.0f)
        {
            GWB_TimerToSelfExplode -= Time.deltaTime;
        }
        else
        {
            GWB_PlayExplosionAnimation = true;
        }
    }


    public void GWB_Explode()
    {
        Collider2D[] PlayerToDamage = Physics2D.OverlapCircleAll(this.gameObject.transform.position, GWB_ExplosionRadius, GWB_PlayerToDamageMask);
        if (PlayerToDamage.Length > 0 && !GWB_Exploded)
        {
            PlayerToDamage[0].GetComponent<Health>().H_DealDamage(GWB_Damage);
            GWB_Exploded = true;
        }
    }

    public void GWB_ExplodeEnd()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag.Equals("Player"))
        {
            GWB_PlayExplosionAnimation = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.gameObject.transform.position, GWB_ExplosionRadius);

    }
}
