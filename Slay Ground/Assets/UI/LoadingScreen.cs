using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Image loadBar;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScene());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadScene() {
        AsyncOperation loadScene = SceneManager.LoadSceneAsync(2);

        while (!loadScene.isDone) {
            loadBar.fillAmount = loadScene.progress;
            yield return null;
        }
    }
}
