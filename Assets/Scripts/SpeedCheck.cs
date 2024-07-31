using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeedCheck : MonoBehaviour
{
    public GameObject player;
    public TextMeshProUGUI textMeshPro;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 speed = player.GetComponent<Rigidbody>().velocity;
        textMeshPro.text = "Speed:" + speed.magnitude;
    }
}
