using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class FixedScrollView : MonoBehaviour, IDragHandler, IPointerUpHandler
{
    public int focusItem;

    public bool horizontal;
    public RectTransform Content;
    public Transform[] ContentItems;

    private void Start()
    {
        if (Content.GetComponent<ContentSizeFitter>() == null)
            Content.gameObject.AddComponent<ContentSizeFitter>();
        Content.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.MinSize;
        Content.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;
    }

    //public void OnScroll(PointerEventData eventData)
    //{
    //    Debug.Log("SCROLL");
    //    Vector2 delta = eventData.scrollDelta;
    //    if(delta.x > 100)
    //    {
    //        focusItem += 1;
    //    }
    //    SetFocus();
    //}

    private void FixedUpdate()
    {
        
    }

    private void Move(Vector2 delta)
    {
        if (horizontal)
        {
            Content.anchoredPosition = new Vector2(Content.anchoredPosition.x + delta.x, 0);
            //if(Mathf.Abs(Content.anchoredPosition.x))
        }
        else
        {
            Content.anchoredPosition = new Vector2(0, Content.anchoredPosition.y + delta.y);

        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("SCROLL");
        Vector2 delta = eventData.scrollDelta;
        //if (delta.x > 100)
        //{
        //    focusItem += 1;
        //}
        Move(delta);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
