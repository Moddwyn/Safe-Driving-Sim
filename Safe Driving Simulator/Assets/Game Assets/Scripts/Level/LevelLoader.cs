using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void LoadLevel(string name)
    {
        StartCoroutine(LoadLevelWithTransition(name));
    }

    private IEnumerator LoadLevelWithTransition(string name)
    {
        animator.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(name);
    }

}
