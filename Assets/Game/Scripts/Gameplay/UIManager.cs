using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject uiPromptContainer;
    [SerializeField] private TextMeshProUGUI uiPromptText;

    [SerializeField] private GameObject uiItemPrefab;
    [SerializeField] private RectTransform itemPanel;

    public void SetPromptText(string text) {
        if (uiPromptContainer.activeSelf == false) uiPromptContainer.SetActive(true);
        uiPromptText.text = text;
    }

    public void ClearUIPrompt() {
        if (uiPromptContainer.activeSelf == true) uiPromptContainer.SetActive(false);
        uiPromptText.text = "";
    }

    public void UpdateItemPanel(List<Item> items) {
        foreach (Transform child in itemPanel) {
            Destroy(child.gameObject);
        }

        foreach (Item item in items) {
            GameObject uiItem = Instantiate(uiItemPrefab, itemPanel);
            uiItem.GetComponent<Image>().sprite = item.itemSprite;
        }

        Image itemPanelImage = itemPanel.GetComponent<Image>();
        Color itemPanelColor = itemPanelImage.color;
        itemPanelColor.a = items.Count > 0 ? 1 : 0;
        itemPanelImage.color = itemPanelColor;
    }
}
