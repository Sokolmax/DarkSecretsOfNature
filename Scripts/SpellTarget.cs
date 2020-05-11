using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpellTarget : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if(!GameManagerScript.instance.isPlayerTurn)
            return;

        CardControllerScript spell = eventData.pointerDrag.GetComponent<CardControllerScript>(),
                             target = GetComponent<CardControllerScript>();

        if(spell && spell.thisCard.isSpell && spell.isPlayerCard && target.thisCard.isPlaced 
            && GameManagerScript.instance.playerEnergy >= spell.thisCard.cost)
        {
            var spellCard = (SpellCard)spell.thisCard;

            if((spellCard.spellTarget == SpellCard.TargetType.ALLY_CARD_TARGET && target.isPlayerCard)
                || spellCard.spellTarget == SpellCard.TargetType.ENEMY_CARD_TARGET && !target.isPlayerCard)
            {
                GameManagerScript.instance.ReduceEnergy(true, spell.thisCard.cost);
                spell.UseSpell(target);
                GameManagerScript.instance.CheckCardsForManaAvailability();
            }
        }
    }
}
