using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform plane;
    [SerializeField] private GameObject enterPanel;
    private float cameraMoveDuration = 2f;
    private Vector3 startPosition = new Vector3(0, 1, 0);
    private Vector3 targetPosition = new Vector3(0, 10, 0);
    private PlaneController planeController;
    [SerializeField] private GameObject[] _gameObjectsToTurnOn;
    [SerializeField] private GameObject _tutorial;

    void Start()
    {
        transform.position = startPosition;
        planeController = plane.GetComponent<PlaneController>();

        if (planeController != null)
        {
            planeController.enabled = false;
        }
    }

    public void StartGame()
    {
        enterPanel.SetActive(false);
        foreach (var item in _gameObjectsToTurnOn)
        {
            item.SetActive(true);
        }
        StartCoroutine(MoveCamera());
    }

    private IEnumerator MoveCamera()
    {
        float elapsedTime = 0f;

        while (elapsedTime < cameraMoveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / cameraMoveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        if (planeController != null)
        {
            planeController.enabled = true;
        }
        _tutorial.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        _tutorial.SetActive(false);
    }
}