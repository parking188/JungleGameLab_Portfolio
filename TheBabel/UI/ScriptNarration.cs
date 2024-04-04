using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScriptNarration : MonoBehaviour
{
    public Image img;
    public List<TMP_Text> tmp_text;

    public void InitWithImage(Color color, string text1, string text2)
    {
        img.color = color;
        tmp_text[0].text = text1;
        tmp_text[1].text = text2;
    }

    public void InitWithoutImage(string text)
    {
        tmp_text[0].text = text;
    }

    void DeleteText()
    {
        gameObject.SetActive(false);
    }
}
