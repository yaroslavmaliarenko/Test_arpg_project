using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ItemCollectionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{

    public int index;//Порядковый номер ячейки начиная с 0
    public GameItem _gameItemRef;//Ссылка на объект связанный с данной ячейкой инвентаря
    public Sprite emptyButtonSprite;
    public Sprite SelectedButtonSprite;//Картинка для выделенной ячейки
    public Sprite NotSelectedButtonSprite;//Картинка для не выделенной ячейки
    public Color emptyButtonColor;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPointerEnter(PointerEventData eventData)
    {

        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = SelectedButtonSprite;
        if (_gameItemRef != null)
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

            if ((y - toolTipRT.rect.height - 3) < 0) y_toolTipWindow = y + toolTipRT.rect.height - height;
            else y_toolTipWindow = y - 3;


            Managers.inventory.ToolTipWindow.GetComponent<RectTransform>().SetPositionAndRotation(new Vector3(x_toolTipWindow, y_toolTipWindow, 0), new Quaternion(0, 0, 0, 0));
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

    public void OnPointerClick(PointerEventData eventData)
    {
        Managers.itemCollectionInterface.PickUpAnItem(index);
    }
}
