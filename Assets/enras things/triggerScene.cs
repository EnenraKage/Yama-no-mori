using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class triggerScene : MonoBehaviour
{
    public int sceneIndex;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SwitchScene());
    }

    IEnumerator SwitchScene()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(sceneIndex);
    }
}
