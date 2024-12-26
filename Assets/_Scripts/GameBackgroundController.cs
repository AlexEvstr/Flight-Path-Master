using UnityEngine;

public class GameBackgroundController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _bgSprite;
    [SerializeField] private Sprite[] _backgrounds;

    private void Start()
    {
        _bgSprite.sprite = _backgrounds[PlayerPrefs.GetInt("BackgroundIndex", 0)];
    }
}