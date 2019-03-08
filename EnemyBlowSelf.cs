using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBlowSelf : Enemy {

	//Tihs enemy moves in a square pattern
	//Attacks player whenever player is close
	//Blows self up near the player after a wind up timer. Glows red when it's going to blow itself

	//CheckVertical is made private as this enemy moves in a squre pattern
	private bool EBS_CheckVertical;

	//Explosion variables
	public LayerMask EBS_PlayerToDamageMask;
	public float EBS_ExplosionRadius;
	public int EBS_Damage;
    private Collider2D[] PlayerToDamage;

    //Explosion timers
    private float EBS_TimeToExplode;
	private float EBS_ChangingColorTimer;
	//Boolean used to make sure the explosion only affects the player once
	private bool EBS_Exploded;

	public GameObject EBS_Explosion;


	// Use this for initialization
	void Start () {
		E_Start();
		E_Health = 1;

		//Right is assumed true when enemy is facing down
		E_FacingRight = true;
		EBS_CheckVertical = true;
		EBS_ChangingColorTimer = 0.2f;
		EBS_TimeToExplode = 1.0f;
		EBS_Exploded = false;
        PlayerToDamage = new Collider2D[1];


    }
	
	// Update is called once per frame
	void Update () {

		if (E_CurrentState != E_State.Staggered)
		{

			//If the enemy is in special state, it will explode, and should not go back to patrolling or attacking states.
			if (E_CurrentState == E_State.Special1)
			{
				E_RigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
				if (EBS_TimeToExplode <= 0.0f)
				{
					//Change the color to red
					E_SpriteRenderer.color = Color.red;

					//Play explosion animation
					Instantiate(EBS_Explosion, this.gameObject.transform.position, this.gameObject.transform.rotation);

					//Create a circular AOE attack with the enemy as the center that only damages the player.
					PlayerToDamage = Physics2D.OverlapCircleAll(this.gameObject.transform.position, EBS_ExplosionRadius, EBS_PlayerToDamageMask);
					if (PlayerToDamage.Length > 0 && !EBS_Exploded)
					{
						PlayerToDamage[0].GetComponent<Health>().H_DealDamage(EBS_Damage);
						EBS_Exploded = true;

					}


					Destroy(this.gameObject);
				}
				else
				{
					EBS_TimeToExplode -= Time.deltaTime;
				}

				if (EBS_ChangingColorTimer <= 0.0f)
				{
					E_SpriteRenderer.color = Color.white;
					EBS_ChangingColorTimer = 0.2f;
				}
				else
				{
					EBS_ChangingColorTimer -= Time.deltaTime;
					E_SpriteRenderer.color = Color.red;
				}

			}
			else
			{

				if (E_CurrentState == E_State.Patrolling)
				{
					E_LineCast();
					E_RigidBody.velocity = new Vector2(E_SpeedHorizontal, E_SpeedVertical);

					//E_Transform.position = transform.position + new Vector3(E_SpeedHorizontal, E_SpeedVertical, 0.0f);
				}

				E_PlayerPosition = E_FindPlayer.transform.position;
				E_DistanceFromPlayer = new Vector2(E_PlayerPosition.x - this.gameObject.transform.position.x, E_PlayerPosition.y - this.gameObject.transform.position.y);


				if (Mathf.Abs(E_DistanceFromPlayer.x) <= 8.0f && Mathf.Abs(E_DistanceFromPlayer.y) <= 5.0f)
				{
					E_CurrentState = E_State.Attacking;
					E_RigidBody.velocity = E_DistanceFromPlayer.normalized * 4.0f;

					//Flip the sprite depending on the palyer's position
					if (E_DistanceFromPlayer.x <= 0.0f)
					{
						E_Transform.rotation = new Quaternion(E_Transform.rotation.x, 180.0f, 0.0f, 0.0f);
					}
					else
					{
						E_Transform.rotation = new Quaternion(E_Transform.rotation.x, 0.0f, 0.0f, 0.0f);
					}

					if (E_DistanceFromPlayer.y <= 0.0f)
					{

						E_Transform.rotation = new Quaternion(180.0f, E_Transform.rotation.y, 0.0f, 0.0f);
					}
					else
					{
						E_Transform.rotation = new Quaternion(0.0f, E_Transform.rotation.y, 0.0f, 0.0f);
					}

					//If you are close enough to the player, stop, and explode.
					if (E_DistanceFromPlayer.magnitude <= EBS_ExplosionRadius)
					{
						E_RigidBody.velocity = new Vector2(0.0f, 0.0f);
						E_CurrentState = E_State.Special1;
					}
				}
				else
				{
					E_CurrentState = E_State.Patrolling;
				}

			}
		}
		else
		{
			if (E_StaggeredTimer > 0.0f)
			{
				E_StaggeredTimer -= Time.deltaTime;
			}
			else
			{
				E_Animator.enabled = true;
				E_CurrentState = E_PreviousState;
				E_SpriteRenderer.color = Color.white;
				E_StaggeredTimer = 0.02f;
			}
		}
	}

	protected override void E_LineCast() 
	{
		
		E_LineCastPos = E_Transform.position + Vector3.down * E_Height;

		if (E_CurrentState == E_State.Patrolling)
		{

			if (EBS_CheckVertical)
			{
				if (E_FacingDown)
				{
					E_IsBlockedVertical = Physics2D.Linecast(E_LineCastPos + new Vector3(0.0f, 0.5f, 2.0f), E_LineCastPos + Vector3.down * E_Height, E_LayerMask);
					E_SpeedVertical = -2.0f;
					E_SpeedHorizontal = 0.0f;
					E_IsBlockedHorizontal = false; //Turn off horziontal checking
					

					Debug.DrawLine(E_LineCastPos + new Vector3(0.0f, 0.5f, 2.0f), E_LineCastPos + Vector3.down * E_Height);
				}
				else if (!E_FacingDown)
				{
					//Debug.Log("Checking Up");
					E_IsBlockedVertical = Physics2D.Linecast(E_LineCastPos + new Vector3(0.0f, 0.5f, 2.0f), E_LineCastPos + Vector3.up * E_Height * 3.0f, E_LayerMask);
					E_SpeedVertical = 2.0f;
					E_SpeedHorizontal = 0.0f;
					E_IsBlockedHorizontal = false;
					

					Debug.DrawLine(E_LineCastPos + new Vector3(0.0f, 0.5f, 2.0f), E_LineCastPos + Vector3.up * E_Height * 3.0f);
				}
			}
			else
			{ 
			 if (E_FacingRight)
				{
					E_IsBlockedHorizontal = Physics2D.Linecast(E_LineCastPos, E_LineCastPos + Vector3.right * E_Width * 2.0f, E_LayerMask);
					E_SpeedVertical = 0.0f;
					E_SpeedHorizontal = 2.0f;
					E_IsBlockedVertical = false; // turn off vertical checking
				

					Debug.DrawLine(E_LineCastPos, E_LineCastPos + Vector3.right * E_Width * 2.0f);
				}
				else if (!E_FacingRight)
				{
					E_IsBlockedHorizontal = Physics2D.Linecast(E_LineCastPos , E_LineCastPos + Vector3.left * E_Width * 2.0f, E_LayerMask);
					E_SpeedVertical = 0.0f;
					E_SpeedHorizontal = -2.0f;
					E_IsBlockedVertical = false;
				

					Debug.DrawLine(E_LineCastPos, E_LineCastPos + Vector3.left * E_Width * 2.0f);
				}
			}
			


			if (E_IsBlockedHorizontal)
			{

				E_VerticalFlip();
				EBS_CheckVertical = true;

			}
			else if(E_IsBlockedVertical)
			{
				E_HorizontalFlip();
				EBS_CheckVertical = false;
			}
		}

	}


	protected override void E_HorizontalFlip()
	{
		//Rotate the enemy around the Y axis by 180 degrees
		
	
		E_FacingRight = !E_FacingRight;
		E_Rotation.y += 180;
		E_Transform.eulerAngles = E_Rotation;
		E_IsBlockedVertical = false;
	}
	protected override void E_VerticalFlip()
	{
		//Rotate the enemy around the Y axis by 180 degrees
		E_FacingDown = !E_FacingDown;
		E_Rotation.x += 180;
		E_Transform.eulerAngles = E_Rotation;
		E_IsBlockedHorizontal = false;
	}

	private void OnDrawGizmosSelected()
	{
		if (EBS_TimeToExplode <= 0.0f)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(this.gameObject.transform.position, EBS_ExplosionRadius);
		}
	}
}
