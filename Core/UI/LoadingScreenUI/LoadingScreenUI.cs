using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenUI : MonoBehaviour
{
    public GameObject LoadingScreenIcons;
    // Start is called before the first frame update
    void Start()
    {
        LoadingScreenIcons.SetActive(true);
    }
    
}
