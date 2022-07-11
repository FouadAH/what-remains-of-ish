using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPromptController : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite pcButtonImage;
    public Sprite gamepadButtonImage;
    Image promptImage;

    void Start()
    {
        promptImage = GetComponent<Image>();
        GameManager.instance.player.GetComponent<Player_Input>().OnInputDeviceChanged += ButtonPromptController_OnInputDeviceChanged;
        SwitchPromptImage();
    }

    private void ButtonPromptController_OnInputDeviceChanged()
    {
        SwitchPromptImage();
    }

    void SwitchPromptImage()
    {
        if (promptImage != null)
        {
            if (GameManager.instance.player.GetComponent<Player_Input>().controllerConnected)
            {
                promptImage.sprite = gamepadButtonImage;
            }
            else
            {
                promptImage.sprite = pcButtonImage;
            }
        }
    }
}
