using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public CanvasGroup alpha;
    public float duration = 1;

    void Start() {
        LoadLevelIn();
    }

    public void LoadLevelIn()
    {
        alpha.alpha = 1;
        FadeScreen(true, false, null);
    }

    public void LoadLevelOut(string name)
    {
        alpha.alpha = 0;
        FadeScreen(false, true,
        ()=>
        {
            SceneManager.LoadScene(name);
        });
    }

    private void FadeScreen(bool fadeIn, bool loadLevel, Action onComplete)
    {
        DOVirtual.Float(fadeIn? 1 : 0, fadeIn? 0 : 1, duration, 
        (x)=>
        {
            alpha.alpha = x;
        }).OnComplete(()=>
        {
            onComplete?.Invoke();
        });
    }

}
