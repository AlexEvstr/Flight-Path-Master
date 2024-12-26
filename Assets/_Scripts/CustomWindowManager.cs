using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CustomWindowManager : MonoBehaviour
{
    [Header("Windows Settings")]
    public GameObject victoryPopup;
    public GameObject nextBtn;
    public GameObject defeatPopup;
    public GameObject tutorialPopup;
    public Text _levelText;

    [Header("Animation Settings")]
    public float popupAnimationDuration = 0.3f;

    private AudioGame _audioGame;


    private void Start()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        _levelText.text = "Level: " + currentLevel.ToString();
        _audioGame = GetComponent<AudioGame>();


    }

    public void ShowVictoryPopup()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        currentLevel++;

        int bestLevel = PlayerPrefs.GetInt("BestLevel", 1);
        if (currentLevel > bestLevel)
        {
            bestLevel = currentLevel;
            PlayerPrefs.SetInt("BestLevel", bestLevel);
        }
        if (currentLevel > 20)
        {
            currentLevel = 20;
            nextBtn.SetActive(false);
        }
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        StartCoroutine(AnimateAndShowVictoryPopup());
    }

    private IEnumerator AnimateAndShowVictoryPopup()
    {
        yield return StartCoroutine(AnimatePopupOpen(victoryPopup));
        _audioGame?.WinSound();
    }

    public void ShowDefeatPopup()
    {
        StartCoroutine(AnimateAndShowDefeatPopup());
    }

    private IEnumerator AnimateAndShowDefeatPopup()
    {
        yield return StartCoroutine(AnimatePopupOpen(defeatPopup));
        _audioGame?.LoseSound();
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
}