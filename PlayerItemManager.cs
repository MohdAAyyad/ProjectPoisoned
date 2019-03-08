using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemManager : MonoBehaviour {

	//Used to identify which object the player is currently touching
	private GameObject PIM_ItemToBePicked;
	//Used to verify whether the player can press the pickup button
	private bool PIM_PlayerInContactWithItem;

	private Inventory PIM_Inventory;

	// Use this for initialization
	void Start () {
		PIM_ItemToBePicked = null;
		PIM_PlayerInContactWithItem = false;

		if (PIM_Inventory = GameObject.Find("Inventory").GetComponent<Inventory>())
		{
			//Do nothing
		}
		else
		{
			Debug.Log("Inventory Not Found");
		}
	}
	
	// Update is called once per frame
	void Update () {
		//If the player is in contact with an item and presses B/circle, add the item to their inventory and destroy the item from the scene
		if(PIM_PlayerInContactWithItem)
		{
			if (Input.GetButton("CTRL - Y") || Input.GetKeyDown(KeyCode.X))
			{
				PIM_Inventory.IN_AddNewItemToPlayer(PIM_ItemToBePicked.tag, true);
				Destroy(PIM_ItemToBePicked);
			}

			else if(Input.GetButton("CTRL - B") || Input.GetKeyDown(KeyCode.V))
			{
				PIM_Inventory.IN_AddNewItemToPlayer(PIM_ItemToBePicked.tag, false);
				Destroy(PIM_ItemToBePicked);
			}
			
		}
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag.Equals("DaggerPoison"))
		{
			PIM_PlayerInContactWithItem = true;
			PIM_ItemToBePicked = col.gameObject;
		}
		else if (col.gameObject.tag.Equals("DaggerBleeding"))
		{
			PIM_PlayerInContactWithItem = true;
			PIM_ItemToBePicked = col.gameObject;
		}
		else if (col.gameObject.tag.Equals("Lightning"))
		{
			PIM_PlayerInContactWithItem = true;
			PIM_ItemToBePicked = col.gameObject;
		}
		else if (col.gameObject.tag.Equals("Fire"))
		{
			PIM_PlayerInContactWithItem = true;
			PIM_ItemToBePicked = col.gameObject;
		}
		else if (col.gameObject.tag.Equals("Ice"))
		{
			PIM_PlayerInContactWithItem = true;
			PIM_ItemToBePicked = col.gameObject;
		}
		else if (col.gameObject.tag.Equals("GravityBall"))
		{
			PIM_PlayerInContactWithItem = true;
			PIM_ItemToBePicked = col.gameObject;
		}
		else if (col.gameObject.tag.Equals("Key"))
		{
			PIM_PlayerInContactWithItem = true;
			PIM_ItemToBePicked = col.gameObject;
		}
	}

	private void OnTriggerExit2D(Collider2D col)
	{
		 if (col.gameObject.tag.Equals("DaggerPoison"))
		{
			PIM_PlayerInContactWithItem = false;
			PIM_ItemToBePicked = null;
		}
		else if (col.gameObject.tag.Equals("DaggerBleeding"))
		{
			PIM_PlayerInContactWithItem = false;
			PIM_ItemToBePicked = null;
		}
		else if (col.gameObject.tag.Equals("Lightning"))
		{
			PIM_PlayerInContactWithItem = false;
			PIM_ItemToBePicked = null;
		}
		else if (col.gameObject.tag.Equals("Fire"))
		{
			PIM_PlayerInContactWithItem = false;
			PIM_ItemToBePicked = null;
		}
		else if (col.gameObject.tag.Equals("Ice"))
		{
			PIM_PlayerInContactWithItem = false;
			PIM_ItemToBePicked = null;
		}
		else if (col.gameObject.tag.Equals("GravityBall"))
		{
			PIM_PlayerInContactWithItem = false;
			PIM_ItemToBePicked = null;
		}
		else if (col.gameObject.tag.Equals("Key"))
		{
			PIM_PlayerInContactWithItem = false;
			PIM_ItemToBePicked = null;
		}

	}
}
