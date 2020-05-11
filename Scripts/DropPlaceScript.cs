using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public enum FieldType
{
    SELF_HAND,
    SELF_FIELD,
    ENEMY_HAND,
    ENEMY_FIELD,
}

public class DropPlaceScript : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public FieldType type;

    public void OnDrop(PointerEventData eventData)
    {
        if(type != FieldType.SELF_FIELD)
            return;

        CardControllerScript card = eventData.pointerDrag.GetComponent<CardControllerScript>();

        if(card && GameManagerScript.instance.playerFieldCards.Count < 5 && GameManagerScript.instance.isPlayerTurn
            && GameManagerScript.instance.playerEnergy >= card.thisCard.cost
            && !card.thisCard.isPlaced)
        { 
            if(!card.thisCard.isSpell)
                card.movement.defaultParent = transform;//изменение родителя карты при переносе
                
            card.OnCast();
        }
        
    }

    /*для перетягивания прототипа карты на позицию другого поля*/
    public void OnPointerEnter(PointerEventData eventData) // Наведение мыши на границу
    {
        if(eventData.pointerDrag == null || type == FieldType.ENEMY_FIELD 
            || type == FieldType.ENEMY_HAND || type == FieldType.SELF_HAND)
            return;

        CardMovementScript card = eventData.pointerDrag.GetComponent<CardMovementScript>();

        if(card)
            card.defaultTempCardParent = transform;
    }

    public void OnPointerExit(PointerEventData eventData) // Отвод мыши от границы
    {
        if(eventData.pointerDrag == null)
            return;

        CardMovementScript card = eventData.pointerDrag.GetComponent<CardMovementScript>();

        if(card && card.defaultTempCardParent == transform)
            card.defaultTempCardParent = card.defaultParent;
    }
}
