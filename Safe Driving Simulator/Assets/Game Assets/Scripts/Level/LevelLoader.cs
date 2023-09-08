using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public CanvasGroup levelLoaderImage;

    void Start()
    {
        ShowLoaderImage(false);
    }

    public void ShowLoaderImage(bool show)
    {
        levelLoaderImage.gameObject.SetActive(true);
        levelLoaderImage.alpha = show ? 0 : 1;

        DOVirtual.Float(levelLoaderImage.alpha, show ? 1 : 0, 5, newAlpha =>
        {
            levelLoaderImage.alpha = newAlpha;
        }).SetEase(Ease.InSine).OnComplete(() =>
        {
            levelLoaderImage.gameObject.SetActive(show);
        });
    }
}
