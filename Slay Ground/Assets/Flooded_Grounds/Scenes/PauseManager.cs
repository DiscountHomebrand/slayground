using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseManager : MonoBehaviour
{
    static bool gamePaused = false;

    public GameObject pauseScreen;
    public TMP_Text pauseTitle;
    public TMP_Text scoreText;
    // If the round is over, either victory or game over
    bool roundEnded = false;

    public static bool IsGamePaused() {
        return gamePaused;
    }

    void Start() {
        Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
    }

    void OnDestroy() {
        gamePaused = false;

        Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 1;
        AudioListener.pause = false;
    }

    void Update() {
        if (roundEnded) {
            // Don't allow pausing/unpausing when round is over
            return;
        }

        KeyCode pauseKey;
        if (Application.isEditor) {
            pauseKey = KeyCode.P;
        } else {
            pauseKey = KeyCode.Escape;
        }

        if (Input.GetKeyDown(pauseKey)) {
            if (gamePaused) {
                ResumeGame();
            } else {
                PauseGame();
            }
        }
    }

    void PauseGame() {
        gamePaused = true;
        Time.timeScale = 0;
        AudioListener.pause = true;
        pauseScreen.SetActive(true);

        Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
    }

    void ResumeGame() {
        gamePaused = false;
        Time.timeScale = 1;
        AudioListener.pause = false;
        pauseScreen.SetActive(false);

        Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
    }

    void OnApplicationFocus(bool hasFocus) {
		if (hasFocus && !gamePaused) {
			Cursor.visible=false;
			Cursor.lockState=CursorLockMode.Locked;
		}
	}

    public void GameOver() {
        if (!gamePaused) {
            PauseGame();
        }
        roundEnded = true;
        pauseTitle.text = "GAME OVER";
    }

    public void RoundVictory() {
        if (!gamePaused) {
            PauseGame();
        }
        roundEnded = true;
        pauseTitle.text = "VICTORY";
    }
}
