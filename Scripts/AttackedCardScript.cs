using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackedCardScript : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        CardInfoScript card = eventData.pointerDrag.GetComponent<CardInfoScript>(); // атакующая карта

        if(card && card.SelfCard.CanAttack 
            && transform.parent == GetComponent<CardMovementScript>().GameManager.EnemyField)
        {
            card.SelfCard.ChangeAttackState(false);

            if(card.IsPlayer)
                card.DeHighlightCard();

            GetComponent<CardMovementScript>().GameManager.CardsFight(card, GetComponent<CardInfoScript>());
        }
    }
}
