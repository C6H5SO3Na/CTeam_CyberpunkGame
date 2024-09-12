using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject canvas;  //UICanvas
    public GameObject textPrefab;  //文字を生成するためのprefab
    public GameObject imagePrefab;//画像を生成するためのprefab
    public GameObject buttonPrefab;  //ボタンを生成するためのprefab

    private string jsonFilePath = "Assets/Scenes/ui TEST/UIText.josn";

    void Start()
    {
        LoadUIFromJSON(jsonFilePath);
    }

    //JSONを読み取り
    void LoadUIFromJSON(string filePath)
    {
        if (File.Exists(filePath))
        {
            //JSONのマークを読み取り
            string jsonString = File.ReadAllText(filePath);

            //JSONを解析
            UIData uiData = JsonConvert.DeserializeObject<UIData>(jsonString);

            //全部のUIを生成
            foreach (UIElement element in uiData.uiElements)
            {
                if (element.uiType == "Text")
                {
                    CreateText(element.uiContent);
                }
                else if (element.uiType == "Button")
                {
                    CreateButton(element.uiContent);
                }
                else if (element.uiType == "Image")
                {
                    Debug.Log("image");
                    CreateImage(element.uiContent);
                }
            }
        }
        else
        {
            Debug.LogError("JSON file not found!");
        }
    }

    //番号によってuiを生成
    public void GenerateUIByID(string uiID)
    {
        if (File.Exists(jsonFilePath))
        {
            string jsonString = File.ReadAllText(jsonFilePath);
            UIData uiData = JsonConvert.DeserializeObject<UIData>(jsonString);

            foreach (UIElement element in uiData.uiElements)
            {
                if (element.uiID == uiID)
                {
                    if (element.uiType == "Text")
                    {
                        CreateText(element.uiContent);
                    }
                    else if (element.uiType == "Button")
                    {
                        CreateButton(element.uiContent);
                    }
                    else if (element.uiType == "Image")
                    {
                        CreateImage(element.uiContent);
                    }
                    break;
                }
            }
        }
    }

    //文字UIの生成
    void CreateText(string content)
    {
        if (textPrefab != null && canvas != null)
        {
            //TextMeshPro Prefabを生成
            GameObject textObj = Instantiate(textPrefab, canvas.transform);

            //TextMeshProUGUIを取得
            TextMeshProUGUI textComponent = textObj.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                textComponent.text = content;
            }
            else
            {
                Debug.LogError("The instantiated prefab does not contain a TextMeshProUGUI component.");
            }
        }
        else
        {
            Debug.LogError("Text prefab or Canvas is not assigned.");
        }
    }
    //画像UIの生成
    void CreateImage(string content)
    {
        GameObject textObj = Instantiate(imagePrefab, canvas.transform);
        Image ImageComponent = textObj.GetComponent<Image>();
    }
    //ボタンUIの生成
    void CreateButton(string content)
    {
        GameObject buttonObj = Instantiate(buttonPrefab, canvas.transform);
        Button buttonComponent = buttonObj.GetComponent<Button>();
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText != null)
        {
            buttonText.text = content;
        }
        else
        {
            Debug.LogError("The button prefab does not contain a TextMeshProUGUI component.");
        }

        //クリック
        buttonComponent.onClick.AddListener(() => OnButtonClick(content));
    }

    //クリック
    void OnButtonClick(string content)
    {
        Debug.Log("Button clicked: " + content);
    }
}

//UIの定義
[System.Serializable]
public class UIElement
{
    public string uiType;
    public string uiID;
    public string uiContent;
}

//UI内容の定義
[System.Serializable]
public class UIData
{
    public List<UIElement> uiElements;
}

