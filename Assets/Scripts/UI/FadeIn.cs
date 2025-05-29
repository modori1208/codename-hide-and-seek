using System.Collections;
using UnityEngine;

/// <summary>
/// 메인 화면 UI 요소 페이드인 스크립트
/// </summary>
public class FadeIn : MonoBehaviour
{

    private static bool alreadyLoaded;

    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private float duration = 1.0f;

    void Start()
    {
        if (FadeIn.alreadyLoaded)
            return;

        FadeIn.alreadyLoaded = true;
        StartCoroutine(this.FadeCanvasGroup());
    }

    private IEnumerator FadeCanvasGroup()
    {
        this.canvasGroup.alpha = 0.0f;
        this.canvasGroup.interactable = false;
        this.canvasGroup.blocksRaycasts = false;

        float elapsed = 0f;
        while (elapsed <= this.duration)
        {
            this.canvasGroup.alpha = Mathf.Clamp01(elapsed / this.duration);
            yield return null;
            elapsed += Time.deltaTime;
        }

        this.canvasGroup.alpha = 1.0f;
        this.canvasGroup.interactable = true;
        this.canvasGroup.blocksRaycasts = true;
    }
}
