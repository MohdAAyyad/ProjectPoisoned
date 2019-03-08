using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	//Health
	protected int E_Health;
	protected float E_StaggeredTimer;
	public GameObject E_BloodSplatter;
	public GameObject E_BloodDie;

    //Raycasting
    //Layermask used to make the enemy ignore himself, bullets, and other enemie when raycasting
    [SerializeField] protected LayerMask E_LayerMask;

	//Line Cast
    protected Vector3 E_LineCastPos;
	protected bool E_IsBlockedVertical;
	protected bool E_IsBlockedHorizontal;

	//Components
	protected SpriteRenderer E_SpriteRenderer;
	protected Rigidbody2D E_RigidBody;
	protected Transform E_Transform;
	protected float E_Width;
	protected float E_Height;
	protected Animator E_Animator;
	protected Collider2D E_Collider;

	//Movement
	protected bool E_FacingRight;
	protected bool E_FacingDown;
	protected float E_SpeedVertical;
	protected float E_SpeedHorizontal;
	protected Vector3 E_Rotation;

	//Player
	protected GameObject E_FindPlayer;
	protected Vector3 E_PlayerPosition;
	protected Vector2 E_DistanceFromPlayer;

    //States 
	protected enum E_State
		//Patroling: searches for player
		//Attacking: heads towards the player
		//Special: Uses special ability
		//Reloading: A state where the enemy just stands still
		//Staggered: used when being damaged, or when being pushed away
	{
		Patrolling,
		Attacking,
		Special1,
        Special2,
        Special3,
        Special4,
        Reloading,
		Staggered
	};
	protected E_State E_CurrentState;
	protected E_State E_PreviousState;
    [SerializeField]
    protected bool E_HalfHealth; // boolean used for some enemies who would use new patterns after their health is halved

	protected float E_StateChangeTimer;
	[Range(0.0f, 5.0f)]
	[SerializeField] protected float E_RaycastBlocked;


	//Generic line casts, should be overriden in child classes if a different behavior is desired
	virtual protected void E_LineCast()
	{		

		// Returns true when the line collides with a collider.
		// The layer mask at the end makes sure the line does not return true if it collides with the object's own collider.
		if (E_CurrentState==E_State.Patrolling)
		{

			
			E_LineCastPos = E_Transform.position + Vector3.down * E_Height;
			E_IsBlockedVertical = Physics2D.Linecast(E_LineCastPos, E_LineCastPos - E_Transform.right * E_RaycastBlocked, E_LayerMask);
			Debug.DrawLine(E_LineCastPos, E_LineCastPos + Vector3.down * E_Height);

			E_LineCastPos = E_Transform.position + Vector3.left * E_Width;			
			E_IsBlockedHorizontal = Physics2D.Linecast(E_LineCastPos, E_LineCastPos - E_Transform.right * E_RaycastBlocked, E_LayerMask);
			Debug.DrawLine(E_LineCastPos, E_LineCastPos + Vector3.left * E_Width);




			if (E_IsBlockedVertical)
			{

				E_VerticalFlip();

			}

			else if (E_IsBlockedHorizontal)
			{

				E_HorizontalFlip();

			}
		}

	}




	protected void E_Start()
	{

		//E_RigidBody = gameObject.GetComponent<Rigidbody2D>();
		E_Transform = gameObject.GetComponent<Transform>();
		E_SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		E_RigidBody = gameObject.GetComponent<Rigidbody2D>();
		E_Animator = gameObject.GetComponent<Animator>();
		E_Collider = gameObject.GetComponent<Collider2D>();
		E_Width = E_SpriteRenderer.bounds.extents.x;
		E_Height = E_SpriteRenderer.bounds.extents.y;
		E_CurrentState = E_State.Patrolling;
		E_StateChangeTimer = 0.0f;
		E_StaggeredTimer = 0.05f;


		//Enemy starts looking down
		E_FacingRight = false;
		E_FacingDown = true;
		E_IsBlockedVertical = false;
		E_IsBlockedHorizontal = false;


        if (E_FindPlayer = GameObject.Find("Player"))
		{
			E_PlayerPosition = E_FindPlayer.GetComponent<Transform>().position;
		}
		else
		{
			Debug.Log("Player was not found");
		}
		//Linecast position is from the enemy's position to the left of it by the amount of its width and as high as its height
		E_LineCastPos = E_Transform.position - E_Transform.right * E_Width + E_Transform.up * E_Height;


		E_Rotation = E_Transform.eulerAngles;

		E_SpeedVertical = 2.0f;
		E_SpeedHorizontal = 2.0f;
		E_DistanceFromPlayer = new Vector2(0.0f, 0.0f);

        E_HalfHealth = false;



    }

	virtual protected void E_VerticalFlip()
	{
		//Rotate the enemy around the Y axis by 180 degrees
		E_FacingDown = !E_FacingDown;
		E_Rotation.x += 180;
		E_Transform.eulerAngles = E_Rotation;
	}

	virtual protected void E_HorizontalFlip()
	{
		//Rotate the enemy around the Y axis by 180 degrees
		E_FacingRight = !E_FacingRight;
		E_Rotation.y += 180;
		E_Transform.eulerAngles = E_Rotation;
	}

	public virtual void E_TakeDamage(int Damage)
	{
		GameObject E_Blood;
		Debug.Log(Damage + "  has been inflicted on this enemy");
		E_PreviousState = E_CurrentState;
		E_CurrentState = E_State.Staggered;
		E_SpriteRenderer.color = Color.red;
		E_Health -= Damage;
		if (E_Health > 0)
		{
			E_Blood = (GameObject)Instantiate(E_BloodSplatter, this.gameObject.transform.position, this.gameObject.transform.rotation);
			E_Blood.transform.localScale = gameObject.transform.localScale *3.0f;
			E_Blood.GetComponent<Animator>().SetBool("End", true);
			E_Animator.enabled = false;
		}
		else
		{
			E_Blood = (GameObject)Instantiate(E_BloodDie, this.gameObject.transform.position, this.gameObject.transform.rotation);
			E_Blood.transform.localScale = gameObject.transform.localScale * 3.0f;
			E_Blood.GetComponent<Animator>().SetBool("End", true);
			Destroy(gameObject);
		}
	}

	protected void E_Staggered()
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
			E_StaggeredTimer = 0.05f;
		}

	}

}
