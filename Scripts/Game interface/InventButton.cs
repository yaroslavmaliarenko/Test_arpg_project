using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class InventButton : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IBeginDragHandler,IDragHandler,IEndDragHandler, IDropHandler
{
    public int index;//Порядковый номер ячейки начиная с 0
    public GameItem _gameItemRef;//Ссылка на объект связанный с данной ячейкой инвентаря
    public Sprite emptyButtonSprite;
    public Sprite SelectedButtonSprite;//Картинка для выделенной ячейки
    public Sprite NotSelectedButtonSprite;//Картинка для yt выделенной ячейки
    public Color emptyButtonColor;

    Transform beginDragT;
    Vector3 beginDragPos;
    [SerializeField]Transform canvasT;

    public void OnBeginDrag(PointerEventData eventData)
    {
        beginDragT = transform.parent;
        beginDragPos = transform.position;
        transform.SetParent(canvasT);
        GetComponent<CanvasGroup>().blocksRaycasts = false;

        
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(beginDragT);
        transform.position = beginDragPos;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        Managers.inventory.UpdateInventory();
    }

    public void OnDrop(PointerEventData eventData)
    {

        
        GameItem dragItem = eventData.pointerDrag.GetComponent<InventButton>()._gameItemRef;
        GameItem dropItem = _gameItemRef;

        Managers.inventory.ReplaceGameItem(dragItem, dropItem);


    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = SelectedButtonSprite;
        if(_gameItemRef!= null)
        {
            
            
            RectTransform invButtonRT = gameObject.GetComponent<RectTransform>();
            float x = invButtonRT.position.x;
            float y = invButtonRT.position.y;
            float width = invButtonRT.rect.width;
            float height = invButtonRT.rect.height;

            RectTransform toolTipRT = Managers.inventory.ToolTipWindow.GetComponent<RectTransform>();
            
            //Проверяем не заступает ли окно за края экрана

            //Debug.Log("x coord " + (x + width + toolTipRT.rect.width).ToString());// 
            Debug.Log("y coord " + y.ToString());// 
            float x_toolTipWindow = 0;
            float y_toolTipWindow = 0;

            if ((x + width + toolTipRT.rect.width + 3) > Screen.width) x_toolTipWindow = x - toolTipRT.rect.width - 3;
            else x_toolTipWindow = x + width + 3;

            if ((y - toolTipRT.rect.height - 3 ) < 0) y_toolTipWindow = y + toolTipRT.rect.height - height;
            else y_toolTipWindow = y - 3;


            Managers.inventory.ToolTipWindow.GetComponent<RectTransform>().SetPositionAndRotation(new Vector3(x_toolTipWindow, y_toolTipWindow, 0),new Quaternion(0,0,0,0));            
            Managers.inventory.ToolTipWindow.SetActive(true);
            Managers.inventory.ToolTipItemNameElement.GetComponent<Text>().text = _gameItemRef._itemFullName;
            Managers.inventory.ToolTipItemDescElement.GetComponent<Text>().text = _gameItemRef.itemDescription;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = NotSelectedButtonSprite;
        Managers.inventory.ToolTipWindow.SetActive(false);
        Managers.inventory.ToolTipItemNameElement.GetComponent<Text>().text = "";
        Managers.inventory.ToolTipItemDescElement.GetComponent<Text>().text = "";
    }

}
