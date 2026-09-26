using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] GameObject startPanel;
    [SerializeField] GameObject gameOverPanel;

    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOverPanel()
    {
        startPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    public void HideAll()
    {
        startPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }
}