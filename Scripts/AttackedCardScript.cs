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
            if(GameManagerScript.instance.enemyFieldCards.Exists(x => x.thisCard.isProvocation) 
                && !defender.thisCard.isProvocation)
                return;
            else
                GameManagerScript.instance.CardsFight(attacker, defender);
        }
    }
}
