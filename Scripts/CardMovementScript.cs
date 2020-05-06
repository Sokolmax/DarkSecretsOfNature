using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class CardMovementScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CardControllerScript cardController;

    Camera main_Camera;
    Vector3 offset;
    public Transform defaultParent, defaultTempCardParent;
    GameObject temp_Card_GO;
    //public GameManagerScript GameManager; // для проверки хода
    public bool isDraggable; //проверка на возможность перетягивания карты по другим полям

    void Awake() // or start
    {
        main_Camera = Camera.allCameras[0];
        temp_Card_GO = GameObject.Find("TempCardGO");
    }

    public void OnBeginDrag(PointerEventData eventData) // начало движения карты
    {
        //return;
        offset = transform.position - main_Camera.ScreenToWorldPoint(eventData.position);

        defaultParent = defaultTempCardParent = transform.parent;
        
        isDraggable = GameManagerScript.instance.isPlayerTurn &&
                      (
                      (defaultParent.GetComponent<DropPlaceScript>().type == FieldType.SELF_HAND &&
                       GameManagerScript.instance.playerEnergy >= cardController.thisCard.cost) ||
                      (defaultParent.GetComponent<DropPlaceScript>().type == FieldType.SELF_FIELD &&
                       cardController.thisCard.canAttack)
                      );

        if(!isDraggable)
            return;

        if(cardController.thisCard.canAttack)
            GameManagerScript.instance.HighlightTargets(true);
        
        temp_Card_GO.transform.SetParent(defaultParent);
        temp_Card_GO.transform.SetSiblingIndex(transform.GetSiblingIndex());

        transform.SetParent(defaultParent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) // движение
    {
        if(!isDraggable)
            return;

        Vector3 newPos = main_Camera.ScreenToWorldPoint(eventData.position);
        transform.position = newPos + offset;

        if(temp_Card_GO.transform.parent != defaultTempCardParent)
            temp_Card_GO.transform.SetParent(defaultTempCardParent);
            
        if(defaultParent.GetComponent<DropPlaceScript>().type != FieldType.SELF_FIELD) //запрет перемещения карт по полю    
            CheckPosition(); 
    }

    public void OnEndDrag(PointerEventData eventData) // отпускание карты
    {
        if(!isDraggable)
            return;

        GameManagerScript.instance.HighlightTargets(false);

        transform.SetParent(defaultParent);
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        // замена позиции карты на позицию прототипа
        transform.SetSiblingIndex(temp_Card_GO.transform.GetSiblingIndex()); 
        temp_Card_GO.transform.SetParent(GameObject.Find("Canvas").transform);
        temp_Card_GO.transform.localPosition = new Vector3(800, 0);
    }


    void CheckPosition() // для замены и запоминание позиции карты через прототип
    {
        int newIndex = defaultTempCardParent.childCount;

        for(int i = 0; i < defaultTempCardParent.childCount; i++)
        {
            if(transform.position.x < defaultTempCardParent.GetChild(i).position.x)
            {
                newIndex = i;

                if(temp_Card_GO.transform.GetSiblingIndex() < newIndex)
                {
                    newIndex--;
                }
                break;
            }
        }

        temp_Card_GO.transform.SetSiblingIndex(newIndex);
    }

    public void MoveToField(Transform field)//плавный розыгрыш карты
    {
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.DOMove(field.position, .5f);
    }

    public void MoveToTurget(Transform target)// плавное перемещение карты
    {
        StartCoroutine(MoveToTurgetCor(target));
    }

    IEnumerator MoveToTurgetCor(Transform target)
    {
        Vector3 pos = transform.position;
        Transform parent = transform.parent;
        int index = transform.GetSiblingIndex();

        transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;

        transform.SetParent(GameObject.Find("Canvas").transform);

        transform.DOMove(target.position, .25f);

        yield return new WaitForSeconds(.25f);

        transform.DOMove(pos, .25f);

        yield return new WaitForSeconds(.25f);

        transform.SetParent(parent);
        transform.SetSiblingIndex(index);
        transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = true;
    }
}
