using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
public class SoundGanerator : MonoBehaviour
{
    public GameObject sePrefab;  // Prefab for Sound Effect (SFX)
    public GameObject bgmPrefab;  // Prefab for BGM (optional if BGM uses a different prefab)
    private string jsonFilePath = "Assets/Scenes/SoundManager/soundData.json"; // Path to JSON file
    private List<GameObject> sfxList = new List<GameObject>();  // List to manage SFX objects
    private GameObject bgmObject; // BGM object for handling BGM playback

    // Generate Sound by ID
    public void GenerateSoundByID(string soundID, GameObject obj = null)
    {
        if (File.Exists(jsonFilePath))
        {
            string jsonString = File.ReadAllText(jsonFilePath);
            SoundData soundData = JsonConvert.DeserializeObject<SoundData>(jsonString);

            foreach (SoundElement element in soundData.soundElements)
            {
                if (element.soundID == soundID)
                {
                    if (element.soundType == "BGM")
                    {
                        CreateBGM(element.soundPath, element.loop);
                    }
                    else if (element.soundType == "SE")
                    {
                        Vector3 playPosition = Vector3.zero;

                        if (element.playAtObjectPosition && obj != null)
                        {
                            playPosition = obj.transform.position;
                        }
                        else if (element.position.HasValue)
                        {
                            playPosition = element.position.Value;
                        }

                        CreateSE(element.soundPath, element.loop, playPosition);
                    }
                    break;
                }
            }
        }
        else
        {
            Debug.LogError("Sound JSON file not found!");
        }
    }

    // Create SFX
    void CreateSE(string path, bool loop, Vector3 position)
    {
        if (sePrefab != null)
        {
            GameObject seObj = Instantiate(sePrefab, this.transform);
            AudioSource audioSource = seObj.GetComponent<AudioSource>();

            if (audioSource != null)
            {
                AudioClip clip = Resources.Load<AudioClip>(path);
                if (clip != null)
                {
                    audioSource.clip = clip;
                    audioSource.loop = loop;
                    audioSource.Play();
                }
                else
                {
                    Debug.LogError("SFX audio clip not found at path: " + path);
                }
            }
            else
            {
                Debug.LogError("The instantiated prefab does not contain an AudioSource component.");
            }

            seObj.transform.position = position;
            sfxList.Add(seObj);
        }
        else
        {
            Debug.LogError("SFX prefab is not assigned.");
        }
    }

    // Create BGM
    void CreateBGM(string path, bool loop)
    {
        if (bgmPrefab != null)
        {
            // Destroy existing BGM if already playing
            if (bgmObject != null)
            {
                Destroy(bgmObject);
            }

            bgmObject = Instantiate(bgmPrefab, this.transform);
            AudioSource audioSource = bgmObject.GetComponent<AudioSource>();

            if (audioSource != null)
            {
                AudioClip clip = Resources.Load<AudioClip>(path);
                if (clip != null)
                {
                    audioSource.clip = clip;
                    audioSource.loop = loop;
                    audioSource.Play();
                }
                else
                {
                    Debug.LogError("BGM audio clip not found at path: " + path);
                }
            }
            else
            {
                Debug.LogError("The instantiated prefab does not contain an AudioSource component.");
            }
        }
        else
        {
            Debug.LogError("BGM prefab is not assigned.");
        }
    }
}
// JSON Data Classes
[System.Serializable]
public class SoundData
{
    public List<SoundElement> soundElements;
}

[System.Serializable]
public class SoundElement
{
    public string soundID;
    public string soundType; // "BGM" or "SE"
    public string soundPath;
    public bool loop;
    public Vector3? position;
    public bool playAtObjectPosition;
}
