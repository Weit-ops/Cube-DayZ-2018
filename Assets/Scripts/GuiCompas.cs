using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiCompas : MonoBehaviour
{
    public Rect r;
   void OnGUI()
    {
        if (GameControls.I.Player.transform.rotation.y >= 90 || GameControls.I.Player.transform.rotation.y <= -260)
        {
            GUILayout.Label("\nNorth");
        }
        else if (GameControls.I.Player.transform.rotation.y >= 130 || GameControls.I.Player.transform.rotation.y <= -295)
        {
            GUILayout.Label("\nEast");
        }
        else if (GameControls.I.Player.transform.rotation.y >= 250 || GameControls.I.Player.transform.rotation.y <= -30)
        {
            GUILayout.Label("\nSouth");
        }
        else if (GameControls.I.Player.transform.rotation.y >= 2 || GameControls.I.Player.transform.rotation.y >= 2)
        {
            GUILayout.Label("\nWest");
        }
    }
    
}
