using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorNamePanel : MonoBehaviour
{
    public GameObject newNamePanel;
    public void Okay()
    {
        newNamePanel.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
