using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class RectTest : MonoBehaviour
{
    public RectTransform _RectTransform;
    public float offset;
    public float multiplier=10f;
    // Start is called before the first frame update
    void Start()
    {
        

        _RectTransform= GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        //_RectTransform.anchoredPosition=new Vector2 (0,offset*multiplier);

        //_RectTransform.offsetMin=new Vector2 (_RectTransform.offsetMin.x,-offset*multiplier);
        _RectTransform.sizeDelta=new Vector2 (_RectTransform.sizeDelta.x,-(-offset*multiplier));
        _RectTransform.anchoredPosition=new Vector2 (_RectTransform.anchoredPosition.x,0);
        //_RectTransform.offsetMax=new Vector2 (offset*multiplier,0);
    }
}
