using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttackedHeroScript : MonoBehaviour, IDropHandler
{
    public enum HeroType
    {
        ENEMY,
        PLAYER
    }

    public HeroType Type;
    public GameManagerScript GameManager;
    public Color NormalCol, TurgetCol;

    public void OnDrop(PointerEventData eventData)
    {
        if(!GameManager.IsPlayerTurn)
            return;
        
        CardInfoScript card = eventData.pointerDrag.GetComponent<CardInfoScript>();

        if(card && card.SelfCard.CanAttack && Type == HeroType.ENEMY)
        {
            card.SelfCard.CanAttack = false;
            GameManager.DamageHero(card, true);
        }
    }

    public void HighlightHero(bool highlite)
    {
        GetComponent<Image>().color = highlite ? TurgetCol : NormalCol;
    }

}
