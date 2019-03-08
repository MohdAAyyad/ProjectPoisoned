using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunWoman : Enemy {

	//Attacking parameters
	private bool GW_Shooting;
	private Vector2 GW_BulletPos;
	private GameObject GW_InstaBullet;
	private bool GW_HasShot;
	private float GW_BetweenShotsTimer;
	private float GW_BulletInstantiationTimer;
	private Vector2 GW_BulletForce;
	private Vector2 GW_BulletForceVertical;
	private int GW_BulletDirection;
    private bool GW_HasChangedPosition;
    private Vector3 GW_PlayerPreviousPosition;

    //Special1 parameters
    public GameObject GW_Bullet;
    private int GW_ShotsCount;
	private Vector2 GW_CurrentPos;
	private bool GW_GoneUp;
	private float GW_ShootVerticalTimer;
	private int GW_HoizontalSpecialDirection;
	private ParticleSystem GW_ParticleSystem;
	private bool GW_iFrames;
    private bool GW_Special1Movement;

    //Special2 parameters
    public Crosshair GW_Crosshair;
    public GameObject GW_BulletHoming;
    private bool GW_InstantiatedCrossHair;

    //Special3 parameters
    public GWBomb GW_Bomb;
    private float GW_MaxDistanceX;
    private float GW_MinDistanceX;
    private float GW_MaxDistanceY;
    private float GW_MinDistanceY;
    private Vector2 GW_NextBombPos;
    private Vector2 GW_PreviousBombPos;
    private int GW_BombCount;

    //Special4 parameters
    public GameObject GW_BulletBomb;


	//Reloading parameters
	private float GW_ReloadingTimer;
	private Vector2 GW_ReloadingPos;
	public GameObject GW_ReloadingText;
	private GameObject GW_InstaReloading;
	private bool GW_HasReloading;
    private int GW_WhichSpecial;

	// Use this for initialization
	void Start () {

        //General
		E_Start();
		E_Health = 30;
		E_RaycastBlocked = 0.0f;
        GW_iFrames = false;

        //Attacking
        GW_BulletPos = new Vector2(gameObject.transform.position.x - 0.4f, gameObject.transform.position.y +0.3f);
		GW_HasShot = false;
		GW_Shooting = false;
		GW_BulletInstantiationTimer = 0.47f;
		GW_BetweenShotsTimer = 0.5f;
		GW_BulletForce = new Vector2(350.0f, 0.0f);
        GW_HasChangedPosition = false;
        GW_PlayerPreviousPosition = E_FindPlayer.transform.position;

        //Special1
        GW_BulletForceVertical = new Vector2(0.0f, 350.0f);
		GW_GoneUp = true;
		GW_ParticleSystem = gameObject.GetComponent<ParticleSystem>();
        GW_ShootVerticalTimer = 0.2f;
        GW_CurrentPos = gameObject.transform.position;
        GW_ShotsCount = 0;
        GW_Special1Movement = false;


        //Special2
        GW_InstantiatedCrossHair = false;

        //Special3
        GW_MaxDistanceX = 12.0f;
        GW_MinDistanceX = -4.0f;
        GW_MaxDistanceY =  4.0f;
        GW_MinDistanceY = -4.0f;
        GW_NextBombPos = new Vector2(0.0f,0.0f);
        GW_PreviousBombPos = GW_NextBombPos;
        GW_BombCount = 0;

        //Special4



        //Reloading
        GW_ReloadingTimer = 4.0f;
		GW_ReloadingPos = gameObject.transform.position;
		GW_HasReloading = false;
        GW_WhichSpecial = 0;




        //The original sprite is looking left.
        if (E_Transform.localScale.x>0.0f)
		{
			//Start facing left
			GW_BulletDirection = -1;
			GW_HoizontalSpecialDirection = 1;
		}
		else
		{
			//Start facing right
			GW_BulletDirection = 1;
			GW_HoizontalSpecialDirection = -1;
		}
	}

	// Update is called once per frame
	void Update()
	{
        //Check which state we are in and call the corrosponding function
        switch (E_CurrentState)
        {
            case E_State.Patrolling:
                GW_Patrolling();
               break;
            case E_State.Staggered:
                E_Staggered();
                break;
            case E_State.Reloading:
                GW_Reloading();
                break;
            case E_State.Attacking:
                GW_Attacking();
                break;
            case E_State.Special1:
                GW_Special1();
                break;
            case E_State.Special2:
                GW_Special2();
                break;
            case E_State.Special3:
                GW_Special3();
                break;
            case E_State.Special4:
                GW_Special4();
                break;
        }
	}

    //---------------------------------Patrolling---------------------------------//

        //Look for the player and if close go attacking
    private void GW_Patrolling()
    {
        GW_ParticleSystem.Stop();
        E_PlayerPosition = E_FindPlayer.transform.position;
        E_DistanceFromPlayer = new Vector2(E_PlayerPosition.x - this.gameObject.transform.position.x, E_PlayerPosition.y - this.gameObject.transform.position.y);

        if (Mathf.Abs(E_DistanceFromPlayer.x) <= 8.0f) //&& Mathf.Abs(E_DistanceFromPlayer.y) <= 5.0f)
        {
            //Player is close enough, start shooting
            E_CurrentState = E_State.Attacking;
        }
        else
        {
            //Reset everything
            E_CurrentState = E_State.Patrolling;
            GW_Shooting = false;
            E_Animator.SetBool("EGW_Shooting", GW_Shooting);
            GW_HasShot = false;
            E_Animator.SetBool("EGW_HasShot", GW_HasShot);
            GW_BulletInstantiationTimer = 0.47f;
            GW_BetweenShotsTimer = 0.5f;
            GW_ShotsCount = 0;
            GW_InstantiatedCrossHair = false;
        }

    }

    //---------------------------------Reloading---------------------------------//

        //Stay in place, turn off iFrames and spawn Reloading text below.

    private void GW_Reloading()
    {
        //No iframes
        GW_iFrames = false;
        GW_ParticleSystem.Stop();
        //Where to instantiate the word reloading
        GW_ReloadingPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 2.0f);
        if (GW_ReloadingTimer >= 0.0f)
        {
            if (!GW_HasReloading)
            {
                GW_InstaReloading = (GameObject)Instantiate(GW_ReloadingText, GW_ReloadingPos, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                GW_HasReloading = true;
            }
            GW_ReloadingTimer -= Time.deltaTime;
        }
        else
        {
            Destroy(GW_InstaReloading);
            GW_ReloadingTimer = 4.0f;
            GW_HasReloading = false;

            //If the enemy is at half health, randomize between special2 and special3 otherwise just use special2
            if (!E_HalfHealth)
            {
                E_CurrentState = E_State.Special2;
            }
            else
            {
                GW_WhichSpecial = Random.Range(0, 20);
                Debug.Log(GW_WhichSpecial);

                if(GW_WhichSpecial<7)
                {
                    E_CurrentState = E_State.Special3;
                    GW_CurrentPos = gameObject.transform.position;
                }
                else if(GW_WhichSpecial>=7 && GW_WhichSpecial <19)
                {
                    E_CurrentState = E_State.Special4;
                }
                else
                {
                    E_CurrentState = E_State.Special2;
                }
            }
        }
    }

    //---------------------------------Attacking---------------------------------//

        //Track the player's position and shoot

    private void GW_Attacking()
	{
        GW_BulletForce = new Vector2(350.0f, 0.0f);
        E_PlayerPosition = E_FindPlayer.transform.position;
        //Don't play the after images
        GW_ParticleSystem.Stop();
        E_Animator.SetBool("EGW_Shooting", GW_Shooting);



        //Update bullet instantiation position based on which direction the Gun Woman is facing
        if (GW_BulletDirection == 1)
        {
            GW_BulletPos = new Vector2(gameObject.transform.position.x + 0.4f, gameObject.transform.position.y + 0.3f);
        }
        else
        {
            GW_BulletPos = new Vector2(gameObject.transform.position.x - 0.4f, gameObject.transform.position.y + 0.3f);
        }

        GW_FollowPlayerY();

        GW_Shoot(GW_BulletForce, 8, GW_Bullet, E_State.Special1,0.0f,0.0f);


    }

    //---------------------------------Special1---------------------------------//

        //Move horizontally and shoot vertically

    private void GW_Special1()
	{
        E_SpriteRenderer.color = new Color(E_SpriteRenderer.color.r, E_SpriteRenderer.color.g, E_SpriteRenderer.color.b, 1.0f);
        //Boolean used to make sure GW only checks for the player once during special1
        if (!GW_Special1Movement)
        {
            E_PlayerPosition = E_FindPlayer.transform.position;

            //See where the player is and determine whether the GW should fire upwards or downwards
            if (E_PlayerPosition.y - gameObject.transform.position.y >= 0.5f)
            {
                GW_GoneUp = false;
            }
            else
            {
                GW_GoneUp = true;
            }

            //See where the player is and determine whether the GW should move right or left

            if (E_PlayerPosition.x > gameObject.transform.position.x)
            {
                GW_HoizontalSpecialDirection = -1;
            }
            else
            {
                GW_HoizontalSpecialDirection = 1;
            }
        }


        if (Mathf.Abs(gameObject.transform.position.x - GW_CurrentPos.x) < 10.0f)
        {
            //Activate iFrames when doing this special attack and play the particle system
            GW_iFrames = true;
            GW_ParticleSystem.Play();
            //Stop following the player
            GW_Special1Movement = true;
            //Add the velocity based on which directions the player is to the enemy
            E_RigidBody.velocity = new Vector2(-8.0f * GW_HoizontalSpecialDirection, 0.0f);
            //Shoot bullets vertically while moveing to the left
            GW_ShootVertically(GW_GoneUp);
        }
        else
        { 
                //Once the enemy has moved 10 units, stop moving and flip.
                E_RigidBody.velocity = new Vector2(0.0f, 0.0f);
				//flip
				Vector3 P_Scale = gameObject.transform.localScale;
				P_Scale.x *= -1;
				gameObject.transform.localScale = P_Scale;
				//flip bullet direction
				GW_BulletDirection *= -1;
				//Start reloading
				E_CurrentState = E_State.Reloading;
                //Make sure the GW's alpha is set to 1
                E_SpriteRenderer.color = new Color(E_SpriteRenderer.color.r, E_SpriteRenderer.color.g, E_SpriteRenderer.color.b, 1.0f);
		}
	}

    //---------------------------------Special2---------------------------------//

        //Stay in place and shoot six homing bulllets

    private void GW_Special2()
    {
        GW_BulletForce = new Vector2(0.0f, 0.0f);
        //Don't play the after images
        GW_ParticleSystem.Stop();

        //Instantiate crosshair
        if (!GW_InstantiatedCrossHair)
        {
            Instantiate(GW_Crosshair, gameObject.transform.position, gameObject.transform.rotation);
            GW_InstantiatedCrossHair = true;
        }

        //Play shooting animation and turn off iframes
        GW_Shooting = true;
        E_Animator.SetBool("EGW_Shooting", GW_Shooting);
        GW_iFrames = false;


        //Update bullet instantiation position based on which direction the enemy is facing
        if (GW_BulletDirection == 1)
        {
            GW_BulletPos = new Vector2(gameObject.transform.position.x + 0.4f, GW_BulletPos.y);
        }
        else
        {
            GW_BulletPos = new Vector2(gameObject.transform.position.x - 0.4f, GW_BulletPos.y);
        }

        GW_Shoot(GW_BulletForce, 6, GW_BulletHoming, E_State.Patrolling,0.0f,0.3f);
    }

    //---------------------------------Special3---------------------------------//

    //Move randomly and spawn bombs
    private void GW_Special3()
    {
        //Don't play the after images
        GW_ParticleSystem.Stop();
        //Store previous position
        GW_PreviousBombPos = GW_NextBombPos;
        //Choose the next position
        GW_NextBombPos = new Vector2(Random.Range(GW_MinDistanceX, GW_MaxDistanceX),Random.Range(GW_MinDistanceY, GW_MaxDistanceY));
        //If the previous position and the new position are very close, try again
        if(Mathf.Abs(GW_PreviousBombPos.x-GW_NextBombPos.x)<=0.5f || Mathf.Abs(GW_PreviousBombPos.y - GW_NextBombPos.y) <= 0.5f)
        {
            GW_Special3();
        }
        else
        {
            //If you haven't moved yet --> Fadeout
            if (E_SpriteRenderer.color.a <= 1.0f && E_SpriteRenderer.color.a > 0.0f && !GW_HasChangedPosition)
            {
                GW_iFrames = true;
                E_SpriteRenderer.color = new Color(E_SpriteRenderer.color.r, E_SpriteRenderer.color.g, E_SpriteRenderer.color.b, E_SpriteRenderer.color.a - 0.04f);
            }

            //Once you've faded out (alpha is zero), move to the next bomb position
            else if (E_SpriteRenderer.color.a <= 0.0f && !GW_HasChangedPosition)
            {
                //Summon bombs randomly around the player (GW_BulletDirection ensures the bombs are always at the side of the player)
                if (GW_BombCount < 25)
                {
                    gameObject.transform.position = new Vector2(GW_CurrentPos.x + GW_NextBombPos.x * GW_BulletDirection, GW_CurrentPos.y + GW_NextBombPos.y);
                }
                else
                {
                    gameObject.transform.position = GW_CurrentPos;
                }
                GW_HasChangedPosition = true;
            }
            //Once you've changed position, start fading in
            else if (E_SpriteRenderer.color.a < 1.0f && GW_HasChangedPosition)
            {
                E_SpriteRenderer.color = new Color(E_SpriteRenderer.color.r, E_SpriteRenderer.color.g, E_SpriteRenderer.color.b, E_SpriteRenderer.color.a + 0.04f);
            }
            //Once you've faded in, summon a bomb and turn off iframes
            else if (E_SpriteRenderer.color.a == 1.0f && GW_HasChangedPosition && GW_BombCount < 25)
            {
                Instantiate(GW_Bomb, gameObject.transform.position, gameObject.transform.rotation);
                GW_BombCount++;
                GW_iFrames = false;
                GW_HasChangedPosition = false;
            }
            //If 25 or more bombs were spawned, go back to patrolling
            else if (E_SpriteRenderer.color.a == 1.0f && GW_HasChangedPosition && GW_BombCount >= 25)
            {


                E_CurrentState = E_State.Special2;
                GW_ReloadingTimer = 6.0f; //Allow the player more time to hit the enemy
                GW_BombCount = 0;
            }

        }
    }

    //---------------------------------Special4---------------------------------//

     //Shoot a bomb bullet
    void GW_Special4()
    {
        //Update the force
        GW_BulletForce = new Vector2(100.0f, 0.0f);
        //Don't play the after images
        GW_ParticleSystem.Stop();
        E_PlayerPosition = E_FindPlayer.transform.position;
        //Play shooting animation and turn off iframes
        GW_Shooting = true;
        E_Animator.SetBool("EGW_Shooting", GW_Shooting);
        GW_iFrames = false;


        //Update bullet instantiation position based on which direction the enemy is facing
        if (GW_BulletDirection == 1)
        {
            GW_BulletPos = new Vector2(gameObject.transform.position.x + 0.4f, gameObject.transform.position.y + 0.3f);
        }
        else
        {
            GW_BulletPos = new Vector2(gameObject.transform.position.x - 0.4f, gameObject.transform.position.y + 0.3f);
        }

        GW_FollowPlayerY();

        GW_Shoot(GW_BulletForce, 6, GW_BulletBomb, E_State.Patrolling,0.0f,0.0f);
    }

    //---------------------------------Methods---------------------------------//

    public override void E_TakeDamage(int Damage)
    {
        //If iframes are active don't take damage
        if (!GW_iFrames)
        {
            base.E_TakeDamage(Damage);
        }
        else
        {
            //Do nothing
        }

        if(E_Health<=E_Health/2)
        {
            E_HalfHealth = true;
        }
    }


    private void GW_ShootVertically(bool HasGoneUp)
    {
        //if the enemy is down, shoot up, and vice versa.
        if (HasGoneUp)
        {
            if (GW_ShootVerticalTimer <= 0.0)
            {
                GW_BulletPos = gameObject.transform.position;
                GW_InstaBullet = (GameObject)Instantiate(GW_Bullet, GW_BulletPos, gameObject.transform.rotation);
                GW_InstaBullet.GetComponent<Rigidbody2D>().AddForce(GW_BulletForceVertical * -1.0f);
                GW_InstaBullet.transform.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
                GW_ShootVerticalTimer = 0.2f;
            }
            else
            {
                GW_ShootVerticalTimer -= Time.deltaTime;
            }
        }
        else
        {
            if (GW_ShootVerticalTimer <= 0.0)
            {
                GW_BulletPos = gameObject.transform.position;
                GW_InstaBullet = (GameObject)Instantiate(GW_Bullet, GW_BulletPos, gameObject.transform.rotation);
                GW_InstaBullet.GetComponent<Rigidbody2D>().AddForce(GW_BulletForceVertical);
                GW_InstaBullet.transform.eulerAngles = new Vector3(0.0f, 0.0f, -90.0f);
                GW_ShootVerticalTimer = 0.2f;
            }
            else
            {
                GW_ShootVerticalTimer -= Time.deltaTime;
            }
        }
    }

    private void GW_Shoot(Vector2 Force, int BulletCount, GameObject BulletToInstantiate, E_State NextState, float OffsetX, float OffsetY)
    {
        //If we are in attacking state, start shooting while alternating between shooting and between shots animations
        if (GW_Shooting && !GW_HasShot)
        {
         
            //Fire rate timer
            if (GW_BulletInstantiationTimer <= 0.0f)
            {
                GW_InstaBullet = (GameObject)Instantiate(BulletToInstantiate, GW_BulletPos, transform.rotation);
                GW_InstaBullet.GetComponent<Rigidbody2D>().AddRelativeForce(Force * GW_BulletDirection);
                GW_InstaBullet.transform.localScale = gameObject.transform.localScale;
                GW_ShotsCount++;
                GW_HasShot = true;
                GW_BulletInstantiationTimer = 0.47f;
                GW_Shooting = false;
                GW_BulletPos = new Vector2(GW_BulletPos.x + OffsetX, GW_BulletPos.y + OffsetY);

            }
            else
            {
                GW_BulletInstantiationTimer -= Time.deltaTime;
            }

        }
        //Give 0.5f delay between each shot enough for the between shots animation to play
        if (GW_HasShot)
        {
            if (GW_BetweenShotsTimer >= 0.0f)
            {
                GW_BetweenShotsTimer -= Time.deltaTime;
            }
            else
            {
                GW_HasShot = false;
                E_Animator.SetBool("EGW_HasShot", GW_HasShot);
                GW_BetweenShotsTimer = 0.5f;
            }
        }
        //If more than three shots have been shot, go into special state.
        if (GW_ShotsCount >= BulletCount)
        {
            E_CurrentState = NextState;
            GW_Special1Movement = false;
            GW_ShotsCount = 0;
            GW_Shooting = false;
            E_Animator.SetBool("EGW_Shooting", GW_Shooting);
            GW_HasShot = false;
            E_Animator.SetBool("EGW_HasShot", GW_HasShot);
            //Save the current position of the gunwoman
            GW_CurrentPos = gameObject.transform.position;
            GW_BulletInstantiationTimer = 0.47f;
            GW_BetweenShotsTimer = 0.5f;
            GW_InstantiatedCrossHair = false;
        }
    }

    private void GW_FollowPlayerY()
    {

        //If the player has not changed their Y position, attack without attempting to change position;
        if (GW_PlayerPreviousPosition.y == E_PlayerPosition.y)
        {
            GW_HasChangedPosition = true;
        }

        //If the you're not on the same Y coordinateas the player and you still have not changed your position accordingly --> Fadeout
        if (E_PlayerPosition.y != gameObject.transform.position.y && E_SpriteRenderer.color.a <= 1.0f && E_SpriteRenderer.color.a > 0.0f && !GW_HasChangedPosition)
        {
            GW_iFrames = true;
            E_SpriteRenderer.color = new Color(E_SpriteRenderer.color.r, E_SpriteRenderer.color.g, E_SpriteRenderer.color.b, E_SpriteRenderer.color.a - 0.04f);
        }

        //Once you've faded out (alpha is zero), move to the player's Y coordinate
        else if (E_PlayerPosition.y != gameObject.transform.position.y && E_SpriteRenderer.color.a <= 0.0f && !GW_HasChangedPosition)
        {

            gameObject.transform.position = new Vector2(gameObject.transform.position.x, E_PlayerPosition.y);
            GW_HasChangedPosition = true;
        }
        //Once you've changed position, start fading in
        else if (E_SpriteRenderer.color.a < 1.0f && GW_HasChangedPosition)
        {
            E_SpriteRenderer.color = new Color(E_SpriteRenderer.color.r, E_SpriteRenderer.color.g, E_SpriteRenderer.color.b, E_SpriteRenderer.color.a + 0.04f);
        }
        //Once you've faded in, play shooting animation and turn off iframes
        else if (E_SpriteRenderer.color.a == 1.0f && GW_HasChangedPosition)
        {

            GW_Shooting = true;
            GW_iFrames = false;
            GW_HasChangedPosition = false;
            GW_PlayerPreviousPosition = E_FindPlayer.transform.position;
        }

    }

}

