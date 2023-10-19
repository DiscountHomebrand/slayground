using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class CharController_Motor : MonoBehaviour {

	public float speed = 10.0f;
	public float sensitivity = 30.0f;
	public float WaterHeight = 15.5f;
	CharacterController character;
	public GameObject cam;
	float moveFB, moveLR;
	float rotX, rotY;
	float gravity = -9.8f;

	// Pitch (x rotation) of the camera
	float cameraRotation = 0.0f;

	// Audio source for character audio
	AudioSource audioSource;

	// Prefab for player muzzle flash
	public GameObject muzzleFlashPrefab;
	// Prefab for bullet impact effect
	public GameObject bulletImpactPrefab;
	// Text for showing ammo count
	public TMP_Text ammoText;
	// Audio for firing gun
	public AudioClip fireGunAudio;
	// Audio for trying to fire with empty clip
	public AudioClip emptyGunAudio;

	// Cooldown between shots
	float shootTimer = 0.0f;

	int clipSize = 60;
	int remainingAmmo;


	void Start(){
		//LockCursor ();

		UpdateAmmo(clipSize);
		
		character = GetComponent<CharacterController>();
		audioSource = GetComponent<AudioSource>();

		if (Application.isEditor) {
			sensitivity = sensitivity * 5f;
		}
	}

	void OnApplicationFocus(bool hasFocus) {
		if (hasFocus) {
			Cursor.visible=false;
			Cursor.lockState=CursorLockMode.Locked;
		}
	}
	
	void CheckForWaterHeight(){
		if (transform.position.y < WaterHeight) {
			gravity = 0f;			
		} else {
			gravity = -9.8f;
		}
	}

	void Update(){
		moveFB = Input.GetAxis ("Horizontal") * speed;
		moveLR = Input.GetAxis ("Vertical") * speed;

		rotX = Input.GetAxis ("Mouse X") * sensitivity;
		rotY = Input.GetAxis ("Mouse Y") * sensitivity;

		CheckForWaterHeight();

		Vector3 movement = new Vector3 (moveFB, gravity, moveLR);

		CameraRotation (cam, rotX, rotY);

		if (shootTimer > 0.0f) {
			shootTimer -= Time.deltaTime;
		}

		if (shootTimer <= 0.0f && Input.GetMouseButton(0)) {
			Shoot();
		}

		if (Input.GetKeyDown(KeyCode.R)) {
			Reload();
		}

		movement = transform.rotation * movement;
		character.Move (movement * Time.deltaTime);
	}

	void CameraRotation(GameObject cam, float rotX, float rotY){		
		transform.Rotate (0, rotX * Time.deltaTime, 0);

		cameraRotation = Math.Clamp(cameraRotation - rotY * Time.deltaTime, -85.0f, 85.0f);
		cam.transform.localEulerAngles = new Vector3(cameraRotation, 0, 0);
	}

	void Shoot() {
		if (remainingAmmo == 0) {
			// Play empty clip sound
			//audioSource.PlayOneShot(emptyGunAudio, 0.2f);
			return;
		}

		// Set cooldown to next shot
		shootTimer = 0.1f;
		// Decrement bullets
		UpdateAmmo(remainingAmmo - 1);
		// Get muzzle placeholder object
		Transform muzzle = transform.Find("Camera/Rifle 1/Muzzle");
		// Instantiate new muzzle flash
		GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, muzzle);
		// Remove prefab's built in translation and scale down
		muzzleFlash.transform.localPosition = Vector3.zero;
		muzzleFlash.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		// Destroy flash after delay
		Destroy(muzzleFlash, 0.05f);

		// Play gun fire sound
		audioSource.PlayOneShot(fireGunAudio, 0.2f);

		// Calculate camera forward direction
		Vector3 camDirection = cam.transform.TransformDirection(Vector3.forward);

		// Cast ray from camera
		RaycastHit hitInfo;
		if (Physics.Raycast(cam.transform.position, camDirection, out hitInfo)) {
			// Instantiate new bullet impact pointing back at the camera
			Instantiate(bulletImpactPrefab, hitInfo.point, Quaternion.FromToRotation(Vector3.forward, -camDirection));
			//Debug.Log("Raycast hit " + hitInfo.collider.gameObject.name);
		}
	}

	void Reload() {
		UpdateAmmo(clipSize);
	}

	void UpdateAmmo(int amount) {
		remainingAmmo = amount;
		ammoText.text = remainingAmmo.ToString() + "/" + clipSize.ToString();
	}
}
