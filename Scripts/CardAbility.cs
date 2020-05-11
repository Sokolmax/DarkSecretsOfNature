using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility : MonoBehaviour
{
    public CardControllerScript cardController;

    public GameObject provocation;

    public void OnCast()
    {
        foreach(var ability in cardController.thisCard.abilities)
        {
            switch(ability)
            {
                case Card.AbilityType.INSTANT_ACTIVE:
                    cardController.thisCard.canAttack = true;

                    if(cardController.isPlayerCard)
                        cardController.info.HighlightCard(true);
                break;

                case Card.AbilityType.PROVOCATION:
                    provocation.SetActive(true);
                break;

            }

        }
    }
    
    public void OnDamageDeal()
    {
        foreach(var ability in cardController.thisCard.abilities)
        {
            switch(ability)
            {
                case Card.AbilityType.DOUBLE_ATTACK:
                    if(cardController.thisCard.timesDealeDamage == 1)
                    {
                        cardController.thisCard.canAttack = true;

                        if(cardController.isPlayerCard)
                            cardController.info.HighlightCard(true);
                    }
                break;
                    
            }

        }
    }

    public void OnDamageTake()
    {
    }

    public void OnNewTurn()
    {
        cardController.thisCard.timesDealeDamage = 0;
        foreach(var ability in cardController.thisCard.abilities)
        {
            switch(ability)
            {
                case Card.AbilityType.REGENERATION:
                    cardController.thisCard.helth += 2000;
                    cardController.info.RefreshData();
                break;

                case Card.AbilityType.STRENGTH_GAIN:
                    cardController.thisCard.attack += 2000;
                    cardController.info.RefreshData();
                break;
            }

        }
    }
}
