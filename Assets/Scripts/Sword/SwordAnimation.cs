using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAnimation : MonoBehaviour
{
    public GameObject SwordEffectPrefab;
    public GameObject Tracking;
    public SwordComponent SwordComponent;

    private ParticleSystem swordEffectParticleSystem;
    private float DestroyTime;

    // Start is called before the first frame update
    void Start()
    {
        if (SwordEffectPrefab != null)
        {
            // Get the particle system from the SwordEffectPrefab
            swordEffectParticleSystem = SwordEffectPrefab.GetComponentInChildren<ParticleSystem>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySwordEffect()
    {
        if (swordEffectParticleSystem != null)
        {
            if (SwordComponent.AttackType == 1)
            {
                StartCoroutine(PlaySwordEffectWithDelay(0.5f));
            }
            else if (SwordComponent.AttackType == 2)
            {
                StartCoroutine(PlaySwordEffectWithDelay(1f));
            }
            // Start the coroutine to play the sword effect after 1 second delay
            /*StartCoroutine(PlaySwordEffectWithDelay(0.5f)); */ // 1 second delay
        }
    }

    // Coroutine to handle the delay
    private IEnumerator PlaySwordEffectWithDelay(float delay)
    {
        // Wait for the specified delay (1 second in this case)
        yield return new WaitForSeconds(delay);

        // Set the rotation of the particle effect to match the Tracking object's rotation
        //SwordEffectPrefab.transform.rotation = Tracking.transform.rotation;
        Debug.Log(SwordComponent.AttackType+"atttyp");

        if (SwordComponent.AttackType == 1)
        {
            SwordEffectPrefab.transform.localPosition = new(-0.589999974f, 0.100000001f + 0.3f, 0);
            SwordEffectPrefab.transform.localRotation = Quaternion.Euler(0, 0, 234.111542f);
            swordEffectParticleSystem.Play();
        }
        else if (SwordComponent.AttackType == 2)
        {
            SwordEffectPrefab.transform.localPosition = new(-0.589999974f, 0, 0);
            SwordEffectPrefab.transform.localRotation = Quaternion.Euler(0, 0, 0);
            swordEffectParticleSystem.Play();
        }
        //SwordEffectPrefab.transform.position = new(-0.589999974f, 0.100000001f + 1.0f, 0);
        //SwordEffectPrefab.transform.rotation = Quaternion.Euler(0, 0, 234.111542f);

        //var emission = swordEffectParticleSystem.emission;
        //emission.enabled = true;
    }
    public void StopSwordEffect()
    {
        if (swordEffectParticleSystem != null && swordEffectParticleSystem.isPlaying)
        {
            swordEffectParticleSystem.Stop();  // Stop playing the particle system when sword is inactive
            var emission = swordEffectParticleSystem.emission;
            emission.enabled = false;
        }
    }
}
