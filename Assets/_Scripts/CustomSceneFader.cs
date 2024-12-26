using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class CustomSceneFader : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeOverlay; // UI Image для затухания
    public float fadeDuration = 1f; // Длительность затухания

    private bool isFading = false;

    void Start()
    {
        Time.timeScale = 1f;
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        if (fadeOverlay != null)
        {
            fadeOverlay.raycastTarget = true; // Блокируем нажатия во время анимации
            StartCoroutine(FadeIn());
        }
    }

    /// <summary>
    /// Запускает переход на указанную сцену с эффектом затухания.
    /// </summary>
    /// <param name="sceneName">Название сцены для загрузки.</param>
    public void TriggerSceneChange(string sceneName)
    {
        if (isFading) return; // Избегаем двойного вызова
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    /// <summary>
    /// Запускает входное затухание (появление сцены).
    /// </summary>
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

        fadeOverlay.raycastTarget = false; // Разрешаем нажатия после затухания
        isFading = false;
    }

    /// <summary>
    /// Запускает исходящее затухание и загружает новую сцену.
    /// </summary>
    IEnumerator FadeOutAndLoad(string sceneName)
    {
        isFading = true;

        float timer = 0f;
        Color color = fadeOverlay.color;
        fadeOverlay.raycastTarget = true; // Блокируем нажатия во время затухания

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Clamp01(timer / fadeDuration);
            fadeOverlay.color = color;
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Универсальный метод для затухания (может использоваться с любым callback).
    /// </summary>
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

        onFadeComplete?.Invoke(); // Выполняем обратный вызов
        isFading = false;
    }
}
