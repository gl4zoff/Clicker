using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    public void SetActivePanel()
    {
        panel.SetActive(true);
    }
}
