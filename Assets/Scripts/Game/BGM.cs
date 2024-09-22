using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    [SerializeField] SoundGenerator sound;
    [SerializeField] string soundName;
    void Start()
    {
        sound.GenerateSoundByID(soundName);
    }
}
