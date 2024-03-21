using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPromptController : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite pcButtonImage;
    public Sprite gamepadButtonImage;
    public GameSettingsSO gameSettings;

    Image promptImage;

    void Start()
    {
        promptImage = GetComponent<Image>();
        GetComponent<GameEventListener>().Response.AddListener(() => OnInputDeviceChanged());
        SwitchPromptImage();
    }

    public void OnInputDeviceChanged()
    {
        SwitchPromptImage();
    }

    void SwitchPromptImage()
    {
        if (promptImage != null)
        {
            if (gameSettings.controllerConnected)
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
