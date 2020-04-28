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
    public GameObject HideObj, HighlitedObj;
    public bool IsPlayer;

    public void HideCardInfo(Card card) //скрытие руки соперника
    {
        SelfCard = card;
        HideObj.SetActive(true);
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

        //if(card.CanAttack)
        //    HighlightCard();
    }

    public void RefreshData()
    {
        Attack.text = SelfCard.Attack.ToString();
        Helth.text = SelfCard.Helth.ToString();
    }

    public void HighlightCard() // включение подсветки
    {
        HighlitedObj.SetActive(true);
    }

    public void DeHighlightCard() // отключение подсветки
    {
        HighlitedObj.SetActive(false);
    }
}
