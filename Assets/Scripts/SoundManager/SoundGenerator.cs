using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class SoundGenerator : MonoBehaviour
{
    public GameObject sePrefab;  // Prefab for Sound Effect (SFX)
    public GameObject bgmPrefab;  // Prefab for BGM
    //private string jsonFilePath = "Assets/Resources/soundData.json";


    private Dictionary<string, GameObject> soundDictionary = new Dictionary<string, GameObject>(); // Dictionary to manage both SFX and BGM objects

    // Generate Sound by ID
    public void GenerateSoundByID(string soundID, GameObject obj = null)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("SoundData");// Path to JSON file
        if (jsonFile != null)
        //if (File.Exists(jsonFilePath))
        {
            string jsonString = jsonFile.text;
            //string jsonString = File.ReadAllText(jsonFilePath);
            SoundData soundData = JsonConvert.DeserializeObject<SoundData>(jsonString);

            foreach (SoundElement element in soundData.soundElements)
            {
                if (element.soundID == soundID)
                {
                    if (element.soundType == "BGM")
                    {
                        CreateSound(soundID, element.soundPath, element.loop, bgmPrefab);
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

                        CreateSound(soundID, element.soundPath, element.loop, sePrefab, playPosition);
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

    // Create Sound (either SE or BGM)
    void CreateSound(string soundID, string path, bool loop, GameObject prefab, Vector3? position = null)
    {
        if (prefab != null)
        {
            // Check if sound with the same ID already exists
            if (soundDictionary.ContainsKey(soundID))
            {
                Debug.LogWarning("Sound with soundID " + soundID + " already exists. Skipping creation.");
                return;
            }

            GameObject soundObj = Instantiate(prefab, this.transform);
            AudioSource audioSource = soundObj.GetComponent<AudioSource>();

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
                    Debug.LogError("Audio clip not found at path: " + path);
                }
            }
            else
            {
                Debug.LogError("The instantiated prefab does not contain an AudioSource component.");
            }

            // Set position if provided
            if (position.HasValue)
            {
                soundObj.transform.position = position.Value;
            }

            // Add to dictionary
            soundDictionary[soundID] = soundObj;
        }
        else
        {
            Debug.LogError("Prefab is not assigned.");
        }
    }

    // Delete Sound by ID (works for both SE and BGM)
    public void DeleteSoundByID(string soundID)
    {
        if (soundDictionary.ContainsKey(soundID))
        {
            GameObject soundObj = soundDictionary[soundID];
            Destroy(soundObj);
            soundDictionary.Remove(soundID);
        }
        else
        {
            Debug.LogWarning("No sound found with soundID: " + soundID);
        }
    }

    // Delete all sounds
    public void DeleteAllSounds()
    {
        foreach (KeyValuePair<string, GameObject> kvp in soundDictionary)
        {
            Destroy(kvp.Value);
        }
        soundDictionary.Clear();
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
