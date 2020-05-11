using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public void MakeTurn()
    {
        StartCoroutine(EnemyTurn(GameManagerScript.instance.enemyHandCards));
    }

    IEnumerator EnemyTurn(List<CardControllerScript> cards)
    {
        yield return new WaitForSeconds(1);
        GameManagerScript gameManager = GameManagerScript.instance;

        int count = Random.Range(0, cards.Count + 1);

        for(int i = 0; i < count; i++) //выставление карт соперником для проверки
        {
            if (gameManager.enemyFieldCards.Count > 4/*баг с выкладыванием бльшего кол-ва карт*/ || GameManagerScript.instance.enemyEnergy == 0
                || gameManager.enemyHandCards.Count == 0)
                break;

            List<CardControllerScript> cardsList = cards.FindAll(x => gameManager.enemyEnergy >= x.thisCard.cost && !x.thisCard.isSpell); //карты с подходящей ценой в руке

            if(cardsList.Count == 0)
                break;

            cardsList[0].GetComponent<CardMovementScript>().MoveToField(gameManager.enemyField);

            yield  return new WaitForSeconds(0.51f);

            cardsList[0].transform.SetParent(gameManager.enemyField);

            cardsList[0].OnCast();
        }

        yield  return new WaitForSeconds(1);

        while(gameManager.enemyFieldCards.Exists(x => x.thisCard.canAttack)) // атака соперником для проверки
        {
            var activeCard = gameManager.enemyFieldCards.FindAll(x => x.thisCard.canAttack)[0];
            bool hasProvocation  = gameManager.playerFieldCards.Exists(x => x.thisCard.isProvocation);

            if(hasProvocation || Random.Range(0, 2) == 0 && gameManager.playerFieldCards.Count > 0)
            {
                CardControllerScript enemy;

                if(hasProvocation)
                    enemy = gameManager.playerFieldCards.Find(x => x.thisCard.isProvocation);
                else
                    enemy = gameManager.playerFieldCards[Random.Range(0, gameManager.playerFieldCards.Count)];

                //Debug.Log(activeCard.SelfCard.Name);

                activeCard.thisCard.canAttack = false;

                activeCard.movement.MoveToTurget(enemy.transform);
                yield return new WaitForSeconds(.75f);

                gameManager.CardsFight(enemy, activeCard);
            }
            else
            {
                activeCard.thisCard.canAttack = false;

                activeCard.GetComponent<CardMovementScript>().MoveToTurget(gameManager.playerHero.transform);
                yield return new WaitForSeconds(.75f);

                gameManager.DamageHero(activeCard, false);
            }
            yield return new WaitForSeconds(.2f);
        }
        yield return new WaitForSeconds(1);
        gameManager.ChangeTurn();
    }

}
