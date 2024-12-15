using UnityEngine;
using System.Collections;

public class QuitOnEscapeOrBack : MonoBehaviour
{
    private void Update()
    {
        // "back" button of phone equals "Escape". quit app if that's pressed
        if (ControlFreak2.CF2Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
