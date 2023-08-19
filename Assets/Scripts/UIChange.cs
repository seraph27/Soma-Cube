using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChange : MonoBehaviour
{
    public GameObject OldUI;
    public GameObject NewUI;

    // Start is called before the first frame update
    public void UIchange()
    {
        OldUI.SetActive(false);
        NewUI.SetActive(true);
    }

}
