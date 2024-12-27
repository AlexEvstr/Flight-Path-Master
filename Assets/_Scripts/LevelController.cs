using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public Button[] levelButtons;
    private int bestLevel;
    private CustomSceneFader _customSceneFader;

    private void Start()
    {
        _customSceneFader = GetComponent<CustomSceneFader>();
        bestLevel = PlayerPrefs.GetInt("BestLevel", 1);
        SetupLevelButtons();
    }

    private void SetupLevelButtons()
    {
        foreach (Button button in levelButtons)
        {
            string levelName = button.gameObject.name;

            if (int.TryParse(levelName, out int levelNumber))
            {
                if (levelNumber > bestLevel)
                {
                    button.interactable = false;
                    button.transform.GetChild(0).gameObject.SetActive(true);
                    button.transform.GetChild(1).gameObject.SetActive(false);
                }
                else
                {
                    button.interactable = true;
                    button.transform.GetChild(0).gameObject.SetActive(false);
                    button.transform.GetChild(1).gameObject.SetActive(true);
                }

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnLevelButtonClicked(levelName));
            }
        }
    }

    private void OnLevelButtonClicked(string levelName)
    {
        PlayerPrefs.SetInt("CurrentLevel", int.Parse(levelName));
        PlayerPrefs.Save();
        _customSceneFader.TriggerSceneChange("Game");

    }
}