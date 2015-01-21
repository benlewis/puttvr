﻿using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	public AudioClip wallSound;
	public AudioClip clubSound;
	public AudioClip holeWallSound;
	public AudioClip sandSound;
	public AudioClip waterSound;


	private int restingInBounds = 0;

	private float maxOutOfBoundsSeconds = 3.0f;
	private float outOfBoundsTime = 0.0f;
	private bool isOutOfBounds = false;
	private bool resetShot = false;
	private Vector3 shotPosition;
	private Vector3? warpPosition = null;
	
	// Use this for initialization
	void Start () {

	}
	
	public void SetWarpPosition(Vector3 pos) {
		warpPosition = pos;
	}
	
	void FixedUpdate() {
		if (warpPosition != null) {
			transform.position = (Vector3) warpPosition;
			warpPosition = null;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (isOutOfBounds) {
			outOfBoundsTime -= Time.deltaTime;
			if (outOfBoundsTime <= 0.0f) {
				resetShot = true;
			}	
		}
	}

	public void StartShot() {
		resetShot = false;
		isOutOfBounds = false;
		shotPosition = transform.position;
		warpPosition = null;
	}

	public bool ResetShot() {
		if (resetShot) {
			transform.position = shotPosition;
		}
		return resetShot;
	}
	
	public bool StillInBounds() {
		return !isOutOfBounds;
	}
	
	// Keep track of what object our 
	void OnCollisionEnter(Collision collisionInfo) {
		Transform surface = collisionInfo.transform;
		
		PlayCollisionClip(surface.tag);
		
		if (surface.CompareTag("Grass") ||
			surface.CompareTag ("Sand") ||
			surface.CompareTag("Hole Walls")) {
			restingInBounds += 1;	
			//Debug.Log("Inbounds on " + surface.tag + " RiB: " + restingInBounds);
		}

		if (restingInBounds == 0) {
			// We have hit a surface but we are not in bounds
			outOfBoundsTime = maxOutOfBoundsSeconds;
			isOutOfBounds = true;
		} else {
			isOutOfBounds = false;
		}
	}
	
	public void OnTriggerEnter(Collider collider) {
		PlayCollisionClip(collider.transform.tag);
		
		if (collider.transform.tag == "Water") {
			// Reset the shot immediately
			resetShot = true;
		}
	}
	
	private void PlayCollisionClip(string tag) {
		AudioClip hitClip = null;
		
		float volumeMin = 0.3f;
		
		switch (tag) {
			case "Walls": hitClip = wallSound; break;
			case "Sand": hitClip = sandSound; break;
			case "Hole": hitClip = holeWallSound; break;
			case "Water": hitClip = waterSound; volumeMin = 1.0f; break;
		}
		
		if (hitClip) {
			Debug.Log ("Playing sound with speed: " + rigidbody.velocity.sqrMagnitude);
			audio.PlayOneShot(hitClip, Mathf.Min(Mathf.Max(volumeMin, rigidbody.velocity.sqrMagnitude / 10.0f), 1.0f));
		}
	}
	
	public void Hit(float force, float max_force) {
		if (clubSound) {
			audio.PlayOneShot(clubSound, force / max_force);
		}
	}
	
	void OnCollisionExit(Collision collisionInfo) {
		Transform surface = collisionInfo.transform;
		
		if (surface.CompareTag("Grass") ||
		    surface.CompareTag ("Sand") ||
		    surface.CompareTag("Hole Walls")) {
		    //We have left the in bounds area
			restingInBounds -= 1;	
			//Debug.Log("Leaving " + surface.tag + " RiB: " + restingInBounds);
		}
	}
}
