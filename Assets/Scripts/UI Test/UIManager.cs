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
    public GameObject firstintoPrefab;  //firstintoを生成するためのprefab

    private string jsonFilePath = "Assets/Scenes/ui TEST/UIText.json";

    void Start()
    {
        // LoadUIFromJSON(jsonFilePath);
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
                    CreateText(element.uiContent, element.position, element.scale);
                }
                else if (element.uiType == "firstinto")
                {
                    Createfirstinto(element.uiContent, element.position, element.scale);
                }
                else if (element.uiType == "Image")
                {
                    Debug.Log("image");
                    CreateImage(element.uiContent, element.position, element.scale);
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
                        CreateText(element.uiContent, element.position, element.scale);
                    }
                    else if (element.uiType == "firstinto")
                    {
                        Createfirstinto(element.uiContent, element.position, element.scale);
                    }
                    else if (element.uiType == "Image")
                    {
                        CreateImage(element.uiContent, element.position, element.scale);
                    }
                    break;
                }
            }
        }
    }

    //文字UIの生成
    void CreateText(string content, Vector2? position, Vector2? scale)
    {
        if (textPrefab != null && canvas != null)
        {
            //TextMeshPro Prefabを生成
            GameObject textObj = Instantiate(textPrefab, canvas.transform);
            textObj.transform.SetSiblingIndex(canvas.transform.childCount - 1);
            //座標とスケールを取得
            RectTransform rectTransform = textObj.GetComponent<RectTransform>();


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
            //座標とスケールを設定されたら
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = position.HasValue ? position.Value : Vector2.zero; //座標を設定されたら設定
                Vector2 scaleValue = scale ?? new Vector2(1, 1);　                                  //スケール
                rectTransform.localScale = new Vector3(scaleValue.x, scaleValue.y, 1);
            }
        }
        else
        {
            Debug.LogError("Text prefab or Canvas is not assigned.");
        }
    }
    //画像UIの生成
    void CreateImage(string content, Vector2? position, Vector2? scale)
    {
        GameObject imageObj = Instantiate(imagePrefab, canvas.transform);
        Image ImageComponent = imageObj.GetComponent<Image>();
        Text imageText = imageObj.GetComponentInChildren<Text>();
        //座標とスケールを取得
        RectTransform rectTransform = imageObj.GetComponent<RectTransform>();
        if (imageText != null)
        {
            imageText.text = content;
        }
        //座標とスケールを設定されたら
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = position.HasValue ? position.Value : Vector2.zero; //座標を設定されたら設定
            Vector2 scaleValue = scale ?? new Vector2(1, 1);                                   //スケール
            rectTransform.localScale = new Vector3(scaleValue.x, scaleValue.y, 1);
        }
    }
    //firstintoUIの生成
    void Createfirstinto(string content, Vector2? position, Vector2? scale)
    {
        GameObject buttonObj = Instantiate(firstintoPrefab, canvas.transform);
        Button buttonComponent = buttonObj.GetComponent<Button>();
        //座標とスケールを取得
        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();

        //座標とスケールを設定されたら
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = position.HasValue ? position.Value : Vector2.zero; //座標を設定されたら設定
            Vector2 scaleValue = scale.HasValue ? scale.Value : new Vector2(1, 1);                       //スケール
            rectTransform.localScale = new Vector3(scaleValue.x, scaleValue.y, 1);
        }
    }
}

//UIの定義
[System.Serializable]
public class UIElement
{
    public string uiType;
    public string uiID;
    public string uiContent;
    public Vector2? position;  // 座標
    public Vector2? scale;     // スケール
}

//UI内容の定義
[System.Serializable]
public class UIData
{
    public List<UIElement> uiElements;
}

