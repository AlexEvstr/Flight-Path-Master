using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject _levels;

    public float popupAnimationDuration = 0.3f;

    public void ShowLevels()
    {
        StartCoroutine(AnimateAndShowLevels());
    }

    private IEnumerator AnimateAndShowLevels()
    {
        yield return StartCoroutine(AnimatePopupOpen(_levels));
    }

    private IEnumerator AnimatePopupOpen(GameObject popup)
    {
        popup.SetActive(true);
        Transform popupChild = popup.transform.GetChild(0);
        popupChild.localScale = Vector3.zero;

        float timer = 0f;
        while (timer < popupAnimationDuration)
        {
            timer += Time.deltaTime;
            float scale = Mathf.Clamp01(timer / popupAnimationDuration);
            popupChild.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }

        popupChild.localScale = Vector3.one;
    }

    public void CloseLevelsWindow()
    {
        StartCoroutine(CloseWindow(_levels));
    }

    private IEnumerator CloseWindow(GameObject window)
    {
        Transform child = window.transform.GetChild(0);
        float timer = popupAnimationDuration;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            float scale = Mathf.Clamp01(timer / popupAnimationDuration);
            child.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }

        child.localScale = Vector3.zero;
        window.SetActive(false);
    }
}