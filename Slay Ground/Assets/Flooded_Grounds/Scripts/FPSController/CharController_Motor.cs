using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class CharController_Motor : MonoBehaviour {

	public float speed = 10.0f;
	public float sensitivity;
	public float WaterHeight = 15.5f;
	CharacterController character;
	public GameObject cam;
	float moveFB, moveLR;
	float rotX, rotY;



	



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

	int maxHealth = 100;
	int currentHealth;
	public Image healthBar;

	int currency = 0;
	public TMP_Text currencyText;

	// Seconds it takes to reload
	float reloadTime = 1.5f;
	// Remaining seconds on current reload
	float reloadTimer = 0.0f;
	public Image reloadBar;

	// Current y velocity of character
	float yVelocity = 0.0f;

	void Start() {
		UpdateAmmo(clipSize);
		UpdateHealth(maxHealth);
		
		character = GetComponent<CharacterController>();
		audioSource = GetComponent<AudioSource>();
	}

	void Update() {
		if (PauseManager.IsGamePaused()) {
			return;
		}

		moveFB = Input.GetAxis("Horizontal") * speed;
		moveLR = Input.GetAxis("Vertical") * speed;

		rotX = Input.GetAxis("Mouse X") * sensitivity;
		rotY = Input.GetAxis("Mouse Y") * sensitivity;


		if (rotX != 0 || rotY != 0) {
			//Debug.Log(rotX + " " + rotY);
		}

		//CheckForWaterHeight();


		CameraRotation (cam, rotX, rotY);

		if (IsReloading()) {
			UpdateReload();
		}
		else {
			if (shootTimer > 0.0f) {
				shootTimer -= Time.deltaTime;
			}

			if (shootTimer <= 0.0f && Input.GetMouseButton(0)) {
				Shoot();
			}

			if (Input.GetKeyDown(KeyCode.R)) {
				Reload();
			}
		}

		if (!character.isGrounded) {
			yVelocity -= 9.8f * Time.deltaTime;
		} else {
			yVelocity = -1.0f;
		}

		if (character.isGrounded && Input.GetKeyDown(KeyCode.Space)) {
			Jump();
			//dont think its good given complexity of map, may cause pathing bugs-h
		}

		Vector3 movement = new Vector3 (moveFB, yVelocity, moveLR);
		
		
		//this fixes diagonals causing doubled movement speed
		if (movement.magnitude > 10){
			movement = movement.normalized * 10;
		}
		movement = transform.rotation * movement;
		character.Move(movement * Time.deltaTime);
	}

	bool IsReloading() {
		return reloadTimer > 0.0f;
	}

	void UpdateReload() {
		reloadTimer -= Time.deltaTime;
		reloadBar.fillAmount = Mathf.Clamp01(reloadTimer / reloadTime);

		if (!IsReloading()) {
			reloadBar.gameObject.SetActive(false);
			UpdateAmmo(clipSize);
		}
	}

	void CameraRotation(GameObject cam, float rotX, float rotY) {		
		transform.Rotate (0, rotX, 0);

		cameraRotation = Math.Clamp(cameraRotation - rotY, -85.0f, 85.0f);
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

			// if zombie hit
			if (hitInfo.collider.gameObject.layer== 3){

				ZombieNav zombieNav = hitInfo.collider.transform.GetComponentInParent<ZombieNav>();
				//Debug.Log("health " +zombieNav.health);
				if(hitInfo.collider.gameObject.tag=="ZombieHead"){
					zombieNav.Headshot();
					//Debug.Log("Raycast hit headshot" );
				}else{
					zombieNav.Damage();
					//Debug.Log("Raycast hit body" );
       			}
        		
				if (zombieNav.IsDead()) {
					// Killed zombie, add money
					AddCurrency(zombieNav.GetCurrency());
				}
				
			}
			//Debug.Log("Raycast hit " + hitInfo.collider.gameObject.name);
		}

	}

	public void Damage(int amount) {
		int newHealth = Math.Clamp(currentHealth - amount, 0, maxHealth);
		UpdateHealth(newHealth);
	}

	public void AddCurrency(int amount) {
		UpdateCurrency(currency + amount);
	}

	void Reload() {
		reloadTimer = reloadTime;
		reloadBar.gameObject.SetActive(true);
		reloadBar.fillAmount = 1.0f;
	}

	void UpdateAmmo(int amount) {
		remainingAmmo = amount;
		ammoText.text = remainingAmmo.ToString() + "/" + clipSize.ToString();
	}

	void UpdateHealth(int value) {
		currentHealth = value;
		healthBar.fillAmount = (float)currentHealth / (float)maxHealth;
	}
	
	void UpdateCurrency(int amount) {
		currency = amount;
		currencyText.text = "$" + currency.ToString();
	}

	void Jump() {
		yVelocity = 5.0f;
	}

	public void lifesteal(){
		int temp = currentHealth + 2;
		if (temp>100){
			temp=100;
		}
		UpdateHealth(temp);
	}

	
}
