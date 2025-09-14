using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    [Header("UI Configuration")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image iconImage;

    private CardData currentCardData;

    public void Display(CardData data)
    {
        currentCardData = data;
        nameText.text = data.cardName;
        descriptionText.text = data.cardDescription;
        iconImage.sprite = data.cardIcon;
    }

    // This method is called when the button is clicked.
    public void OnCardSelected()
    {
        if (currentCardData != null && GameManager.Instance != null)
        {
            GameManager.Instance.ApplyUpgrade(currentCardData);
        }
    }
}