using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiCompas : MonoBehaviour
{
    public Rect r;
   void OnGUI()
    {
        if (GameControls.I.Player.transform.rotation.y >= 66)
        {
            GUILayout.Label("\nNorth");
        }
        else if (GameControls.I.Player.transform.rotation.y <= -100)
        {
            GUILayout.Label("\nEast");
        }
        else if (GameControls.I.Player.transform.rotation.y <= 0)
        {
            GUILayout.Label("\nSouth");
        }
        else if (GameControls.I.Player.transform.rotation.y >= 145)
        {
            GUILayout.Label("\nWest");
        }
    }
    
}
