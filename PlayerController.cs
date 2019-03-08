using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // component variables
    Rigidbody2D P_rb;
    Collider2D P_CColl;
    SpriteRenderer P_ShieldSprite;
    CollisionDirectionDetect dir;

    // variables relating to input
    public Vector2 P_PlayerDirection;

    // The Variable that holds the current state of the player and one that holds all possible states so i can print it if id like to
    // The states are: "controlled" "frozen" "dash" "shield". I am the president of the united states of "PlayerController.cs"
    public enum P_States {controlled, dash, shield, frozen};
    public P_States P_CurrentState;

    // variables for the "controlled" state
    public float P_WalkSpeed;

    // variables for the "dash" state
    public float P_DashTime;
    public float P_DashDistance;
    public float P_DashDelay;
    public float P_DashDanceTimeFrame;
    private float P_DashTimer;

    // inputs
    Button P_dash;
    Joystick P_axis;


    //-----------------------------------------------------------------------------------------------------------------------------------------------//
    
    private void Start ()
    {
        //gets all required components
        P_rb = GetComponent<Rigidbody2D>();
        P_CColl = GetComponent<Collider2D>();
        P_ShieldSprite = GameObject.FindGameObjectWithTag("Shield").GetComponent<SpriteRenderer>();
        dir = GetComponent<CollisionDirectionDetect>();

        //sets the input variables
        P_PlayerDirection = Vector2.zero;
        
        //now set the state from the start
        P_CurrentState = P_States.controlled;

        //set timer to 0
        P_DashTimer = 0.0f;

        //setup inputs
        string[] temp0 = new string[2];
        temp0[0] = "CTRL - A";
        temp0[1] = "Dash";
        P_dash = new Button(temp0);

        string[] temp1 = new string[3];
        temp1[0] = "CTRL - LSX";
        temp1[1] = "CTRL - DPADX";
        temp1[2] = "Horizontal";
        string[] temp2 = new string[3];
        temp2[0] = "CTRL - LSY";
        temp2[1] = "CTRL - DPADY";
        temp2[2] = "Vertical";

        P_axis = new Joystick(temp1, temp2);

    }
    
    private void FixedUpdate()
    {
        //manually update the input
        P_dash.update();

        if (P_CurrentState != P_States.dash)
        {
            P_PlayerDirection = P_axis.get();
        }

        //checks to see when the player can enter each state
        P_HandleStates();
        

        //now handles one of the 4 states or prints to the command line if i messed up
        if (P_CurrentState == P_States.controlled)
        { /// the "controlled" state
            P_HandleControl();
        }
        else if (P_CurrentState == P_States.dash) 
        { /// the "dash" state
            P_HandleDash();
        }
        else if (P_CurrentState == P_States.shield)
        { /// the "shield" state
            P_HandleShield();
        }
        else if (P_CurrentState == P_States.frozen)
        { /// the "frozen" state
            P_HandleFreeze();
        }
        else
        {
            Debug.Log("Unknown State: " + P_CurrentState);
        }

        //Debug.Log("current state: " + P_CurrentState);
        
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------//
    
    private void P_HandleStates()
    {
        // the controlled state
        if (P_CurrentState == P_States.controlled)
        {
            // if the dash button is pressed
            if (P_dash.getDown())
            {
                // checks if there is a direction for the player to dash in
                if (P_PlayerDirection != Vector2.zero && P_DashTimer <= 0.0f)
                {
                    // regular dash
                    P_CurrentState = P_States.dash;
                    return;
                }
                // if there is no direction to dash in then you will start shielding
                if (P_PlayerDirection == Vector2.zero)
                {
                    P_CurrentState = P_States.shield;
                    return;
                }
            }
        } // the controlled state
        
        // the dash state
        if (P_CurrentState == P_States.dash)
        {
            if (P_axis.get() != Vector2.zero && P_DashTimer >= P_DashTime-P_DashDanceTimeFrame && P_dash.getDown())
            {
                P_DashTimer = 0.0f;
                P_PlayerDirection = P_axis.get();
                Debug.Log("HI");
            }
        }// the dash state
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------//

    private void P_HandleControl()
    {
        //get current position
        Vector2 newPos = transform.position;
        //add the displacement
        newPos += P_PlayerDirection * P_WalkSpeed * Time.fixedDeltaTime;
        //apply new position
        P_rb.MovePosition(newPos);

        //handle the dash delay
        if (P_DashTimer > 0.0f)
        {
            P_DashTimer -= Time.fixedDeltaTime;
        }
        if (P_DashTimer < 0.0f)
        {
            P_DashTimer = 0.0f;
        }
    }

    private void P_HandleDash()
    {
        //get current position
        Vector2 newPos = transform.position;
        //add the displacement
        newPos += P_PlayerDirection * P_DashDistance * (Time.fixedDeltaTime/P_DashTime);
        //apply new position
        P_rb.MovePosition(newPos);

        //handle the timer
        P_DashTimer += Time.fixedDeltaTime;
        if (P_DashTimer >= P_DashTime)
        {
            P_CurrentState = P_States.controlled;
            P_DashTimer = P_DashDelay;
        }
    }

    private void P_HandleShield()
    {
        if (P_dash.getHold())
        {
            P_CColl.isTrigger = true;
            P_ShieldSprite.enabled = true;
        }
        else
        {
            P_CColl.isTrigger = false;
            P_ShieldSprite.enabled = false;
            P_CurrentState = P_States.controlled;
        }
    }

    private void P_HandleFreeze()
    {

    } /* TODO: add functionality if needed */

    //-----------------------------------------------------------------------------------------------------------------------------------------------//
    
    public bool Freeze()
    {
        if (P_CurrentState == P_States.controlled)
        {
            P_CurrentState = P_States.frozen;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UnFreeze()
    {
        if (P_CurrentState == P_States.frozen)
        {
            P_CurrentState = P_States.controlled;
        }
    }
}
