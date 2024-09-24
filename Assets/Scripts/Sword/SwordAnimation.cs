using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAnimation : MonoBehaviour
{
    public GameObject SwordEffectPrefab;
    public GameObject Tracking;
    private SwordComponent SwordComponent;

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

    //public void AttackAnimOn()
    //{
    //    GameObject tmp = GameObject.Instantiate(SwordEffectPrefab, this.transform.position, Quaternion.identity);
    //    Destroy(tmp.gameObject, 2.0f);
    //}

    //public void PlaySwordEffect()
    //{
    //    if (swordEffectParticleSystem != null && !swordEffectParticleSystem.isPlaying)
    //    {
    //       SwordEffectPrefab.transform.rotation = Tracking.transform.rotation;

    //        // Set the position and rotation of the particle effect
    //        //if (SwordComponent.AttackType == 1)
    //        //{
    //        //    SwordEffectPrefab.transform.rotation = Tracking.transform.rotation;
    //        //}               
    //        //else if (SwordComponent.AttackType == 2)
    //        //{
    //        //    Vector3 currentEulerAngles = Tracking.transform.rotation.eulerAngles;
    //        //    currentEulerAngles.x += 90;
    //        //    Tracking.transform.rotation = Quaternion.Euler(currentEulerAngles);
    //        //    SwordEffectPrefab.transform.rotation = Tracking.transform.rotation;

    //        //}


    //        // Play the particle effect
    //        swordEffectParticleSystem.Play();
    //        var emission = swordEffectParticleSystem.emission;
    //        emission.enabled = true;
    //    }
    //}

    public void PlaySwordEffect()
    {
        if (swordEffectParticleSystem != null && !swordEffectParticleSystem.isPlaying)
        {
            // Start the coroutine to play the sword effect after 1 second delay
            StartCoroutine(PlaySwordEffectWithDelay(0.5f));  // 1 second delay
        }
    }

    // Coroutine to handle the delay
    private IEnumerator PlaySwordEffectWithDelay(float delay)
    {
        // Wait for the specified delay (1 second in this case)
        yield return new WaitForSeconds(delay);

        // Set the rotation of the particle effect to match the Tracking object's rotation
        SwordEffectPrefab.transform.rotation = Tracking.transform.rotation;

        // Play the particle effect
        swordEffectParticleSystem.Play();
        var emission = swordEffectParticleSystem.emission;
        emission.enabled = true;
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
