using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Android;

//public interface MeleeSwordTrail{}
public class SwordComponent : MonoBehaviour
{
    public float swordActiveTime;
    public int AttackType;

    SwordColliderController swordCollider;
    //MeleeSwordTrail swordTrail;

    public float swordActiveTimer;

    public List<TargetHitInfo> targetList = new List<TargetHitInfo>();

    private PlayerManager playerManager;

    public int SwordPower;

    private SwordAnimation SwordAnimation;

    public GameObject SwordReferences;

    private HashSet<ISwordTarget> hitTargets = new HashSet<ISwordTarget>();

    //public GameObject SwordEffectPrefab;
    //public GameObject Tracking;

    //private ParticleSystem swordEffectParticleSystem; // To store particle system

    // Start is called before the first frame update
    void Start()
    {
        swordCollider = GetComponentInChildren<SwordColliderController>();
        //swordTrail = GetComponentInChildren<MeleeSwordTrail>();
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        SwordAnimation = GetComponentInParent<SwordAnimation>();
        

        if (swordCollider!=null)
        {
            //Debug.Log("Start");
            swordCollider.OnSwordCollisionEvent += OnSwordCollision;
        }

        //if (SwordEffectPrefab != null)
        //{
        //    // Get the particle system from the SwordEffectPrefab
        //    swordEffectParticleSystem = SwordEffectPrefab.GetComponentInChildren<ParticleSystem>();
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (swordActiveTimer > 0)
        {
            swordActiveTimer -= Time.deltaTime;
        }
        else
        {
            //SwordAnimation.StopSwordEffect();
        }

        ////effect appear time
        //if(swordActiveTimer > 0)
        //{
        //    if(!swordTrail._emit)
        //    {
        //        swordTrail._emit = true;
        //    }
        //    swordActiveTimer -= Time.deltaTime;
        //}
        //else if (swordTrail._emit)
        //{
        //    swordTrail._emit = false;
        //}
    }
    public void SetSwordColliderOff()
    {
        SwordReferences.GetComponent<CapsuleCollider>().enabled = false;
    }
    public void SetSwordActive(int _AttactType)
    {
        SwordReferences.GetComponent<CapsuleCollider>().enabled = true;
        
        AttackType = _AttactType;
        if (AttackType == 1)
        {
            SwordPower = 10;
            swordActiveTime = 1.5f;
        }
        if (AttackType == 2)
        {
            SwordPower = 20;
            swordActiveTime = 2;
        }
        swordActiveTimer = swordActiveTime;
        hitTargets.Clear();
        //SwordAnimation.AttackAnimOn();
        SwordAnimation.PlaySwordEffect();
    }

    public void OnSwordCollision(IEventSource _source, ISwordTarget _target)
    {
        Debug.Log("sword hit somethisg");
        if (swordActiveTimer > 0 && !hitTargets.Contains(_target))
        {
            hitTargets.Add(_target);
            TargetHitInfo hitInfo = new TargetHitInfo(_source);
            _target.OnTargetHit(hitInfo, AttackType);  // Trigger target's hit logic
            targetList.Add(hitInfo);
            Debug.Log("sword hit somethisg and timer>0");

            // Check if the target implements IDamageable
            IDamageable damageable = _target as IDamageable;
            if (damageable != null)
            {
                // Deal damage to the target based on SwordPower
                damageable.TakeDamage(SwordPower);
                Debug.Log($"Dealt {SwordPower} damage to enemy.");
            }
            //if (!targetList.Contains(hitInfo))
            //{
                
            //}
            if (AttackType == 1 && playerManager != null)
            {
                PlayerManager.PlayerSPAdd(20);  // Reward player for successful hit
                Debug.Log("SP added to player.");
            }
        }
        else
        {
            targetList.Clear();
        }
    }

    private void OnDestroy()
    {
        if (swordCollider != null)
        {
            swordCollider.OnSwordCollisionEvent -= OnSwordCollision;
            //Debug.Log("Destroy");
        }
    }

    //private void PlaySwordEffect()
    //{
    //    if (swordEffectParticleSystem != null && !swordEffectParticleSystem.isPlaying)
    //    {
    //        // Set the position and rotation of the particle effect
    //        SwordEffectPrefab.transform.position = Tracking.transform.position;
    //        SwordEffectPrefab.transform.rotation = Tracking.transform.rotation;

    //        // Play the particle effect
    //        swordEffectParticleSystem.Play();
    //        var emission = swordEffectParticleSystem.emission;
    //        emission.enabled = true;
    //    }
    //}

    //private void StopSwordEffect()
    //{
    //    if (swordEffectParticleSystem != null && swordEffectParticleSystem.isPlaying)
    //    {
    //        swordEffectParticleSystem.Stop();  // Stop playing the particle system when sword is inactive
    //        var emission = swordEffectParticleSystem.emission;
    //        emission.enabled = false;
    //    }
    //}
}