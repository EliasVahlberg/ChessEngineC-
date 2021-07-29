using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HistoryItem : MonoBehaviour
{
    public InputField inputField;
    public Button button;
    public Text buttonText;
    public Image movedImg;
    public Image capturedImg;
    public Image redX;
    public void init(string message, string fen, Sprite movedImgSprite)
    {
        buttonText.text = message;
        inputField.text = fen;
        movedImg.sprite = movedImgSprite;
        movedImg.gameObject.SetActive(true);
        capturedImg.gameObject.SetActive(false);
        redX.gameObject.SetActive(false);

    }
    public void init(string message, string fen, Sprite movedImgSprite, Sprite capturedImgSprite)
    {
        buttonText.text = message;
        inputField.text = fen;
        movedImg.sprite = movedImgSprite;
        capturedImg.sprite = capturedImgSprite;
        movedImg.gameObject.SetActive(true);
        capturedImg.gameObject.SetActive(true);
        redX.gameObject.SetActive(true);

    }
}
