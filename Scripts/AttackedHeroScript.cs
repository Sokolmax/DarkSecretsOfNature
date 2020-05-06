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

    public HeroType type;
    //public GameManagerScript GameManager;
    public GameObject highlitedObj;

    public void OnDrop(PointerEventData eventData)
    {
        if(!GameManagerScript.instance.isPlayerTurn)
            return;
        
        CardControllerScript card = eventData.pointerDrag.GetComponent<CardControllerScript>();

        if(card && card.thisCard.canAttack && type == HeroType.ENEMY)
        {
            GameManagerScript.instance.DamageHero(card, true);
        }
    }

    public void HighlightHero(bool highlite)
    {
        highlitedObj.SetActive(highlite);
    }

}
