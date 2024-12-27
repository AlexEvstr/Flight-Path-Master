using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class CustomSceneFader : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeOverlay;
    public float fadeDuration = 1f;

    private bool isFading = false;

    void Start()
    {
        Time.timeScale = 1f;
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        if (fadeOverlay != null)
        {
            fadeOverlay.raycastTarget = true;
            StartCoroutine(FadeIn());
        }
    }

    public void TriggerSceneChange(string sceneName)
    {
        if (isFading) return;
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    IEnumerator FadeIn()
    {
        isFading = true;

        float timer = fadeDuration;
        Color color = fadeOverlay.color;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            color.a = Mathf.Clamp01(timer / fadeDuration);
            fadeOverlay.color = color;
            yield return null;
        }

        fadeOverlay.raycastTarget = false;
        isFading = false;
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        isFading = true;

        float timer = 0f;
        Color color = fadeOverlay.color;
        fadeOverlay.raycastTarget = true;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Clamp01(timer / fadeDuration);
            fadeOverlay.color = color;
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }

    public IEnumerator FadeOut(System.Action onFadeComplete)
    {
        isFading = true;

        float timer = 0f;
        Color color = fadeOverlay.color;

        fadeOverlay.raycastTarget = true;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Clamp01(timer / fadeDuration);
            fadeOverlay.color = color;
            yield return null;
        }

        onFadeComplete?.Invoke();
        isFading = false;
    }
}