using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	private Animator EXPl_Animator;

	// Use this for initialization
	void Start () {
		EXPl_Animator = this.gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
		//After explosion animation is done, destroy self.
		if(EXPl_Animator.GetCurrentAnimatorStateInfo(0).IsName("DestroySelf"))
		{
			Destroy(gameObject);
		}
	}
}
