using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardInfoScript : MonoBehaviour
{
    public Card SelfCard;
    public Image Logo;
    public Text Name, Attack, Helth;
    public TextMeshProUGUI Cost;
    public GameObject HideObj, HighlitedObj;
    public bool IsPlayer;
    //public Color NormalCol, TargetCol;

    public void HideCardInfo(Card card) //скрытие руки соперника
    {
        SelfCard = card;
        HideObj.SetActive(true);
        Helth.text = "";
        Attack.text = "";
        Cost.text = "";
        IsPlayer = false;
    }

    public void ShowCardInfo(Card card, bool isPlayer)
    {
        IsPlayer = isPlayer;
        HideObj.SetActive(false);
        SelfCard = card;

        Logo.sprite = card.Logo;
        Logo.preserveAspect = true;
        Name.text = card.Name;

        RefreshData();
    }

    public void RefreshData()
    {
        Attack.text = SelfCard.Attack.ToString();
        Helth.text = SelfCard.Helth.ToString();
        Cost.text = SelfCard.Cost.ToString();
    }

    public void HighlightCard() // включение подсветки
    {
        HighlitedObj.SetActive(true);
    }

    public void DeHighlightCard() // отключение подсветки
    {
        HighlitedObj.SetActive(false);
    }

    public void CheckForAvailability(int currentEnergy)//прозрачность карт, которые нельзя разыграть
    {
        GetComponent<CanvasGroup>().alpha = currentEnergy >= SelfCard.Cost ? 1: 0.7f; 
    }

    public void HighlightAsTarget(bool highlite)// подсветка карт для атаки
    {
        HighlitedObj.SetActive(highlite);
    }
}
