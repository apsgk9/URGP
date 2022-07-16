using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Test_ButtonScript : MonoBehaviour
{
    private void Start()
    {
        ChangeText();
    }

    private void OnValidate()
    {
        ChangeText();
    }

    private void ChangeText()
    {
        var tMP_Text = GetComponent<TMP_Text>();
        tMP_Text.text = transform.parent.GetSiblingIndex().ToString();
    }
}
