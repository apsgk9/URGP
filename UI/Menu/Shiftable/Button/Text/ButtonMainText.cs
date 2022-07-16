using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonMainText : MonoBehaviour
{
    public TMP_Text TMPText {get;private set;} 
    private void Awake()
    {
        TMPText= GetComponent<TMP_Text>();
    }
    private void OnValidate()
    {
        TMPText= GetComponent<TMP_Text>();        
    }
}
