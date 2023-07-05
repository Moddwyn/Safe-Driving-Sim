using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public float transitionTime;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    public void LoadLevel(string name) {
        StartCoroutine(LoadLevelCoroutine(name));
    }

    private IEnumerator LoadLevelCoroutine(string name) {
        animator.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(name);
    }
}
