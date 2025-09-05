using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewNamePanel : MonoBehaviour
{
    public tk2dUITextInput inputField;
    public GameObject errorNamePanel;
    void Start()
    {
        inputField.Text = "Игрок-" + UnityEngine.Random.Range(10, 10000);
    }

    public void SetChanges()
    {
        if (inputField.Text.Length >= 4)
        {
            PHPNetwork.I.ChangeName(inputField.Text);
            JsSpeeker.I._vk_nick(inputField.Text.Replace("_", " "));
            gameObject.SetActive(false);
        }else if (inputField.Text.Length < 4)
        {
            errorNamePanel.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
