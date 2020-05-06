using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackedCardScript : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if(!GameManagerScript.instance.isPlayerTurn)
            return;

        CardControllerScript attacker = eventData.pointerDrag.GetComponent<CardControllerScript>(),
                             defender = GetComponent<CardControllerScript>();

        if(attacker && attacker.thisCard.canAttack && defender.thisCard.isPlaced)
        {
            GameManagerScript.instance.CardsFight(attacker, defender);
        }
    }
}
