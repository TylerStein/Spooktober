using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject uiPromptContainer;
    [SerializeField] private TextMeshProUGUI uiPromptText;

    public void SetPromptText(string text) {
        if (uiPromptContainer.activeSelf == false) uiPromptContainer.SetActive(true);
        uiPromptText.text = text;
    }

    public void ClearUIPrompt() {
        if (uiPromptContainer.activeSelf == true) uiPromptContainer.SetActive(false);
        uiPromptText.text = "";
    }
}
