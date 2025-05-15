using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadMainSceneAfterDelay());
    }

    private IEnumerator LoadMainSceneAfterDelay()
    {
        AsyncOperation sceneLoading = SceneManager.LoadSceneAsync("main", LoadSceneMode.Single);
        sceneLoading.allowSceneActivation = false;

        yield return new WaitForSeconds(3);
        
        sceneLoading.allowSceneActivation = true;
    }
}
