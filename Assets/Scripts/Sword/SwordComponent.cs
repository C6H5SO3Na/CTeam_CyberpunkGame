using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

//public interface MeleeSwordTrail{}
public class SwordComponent : MonoBehaviour
{
    public float swordActiveTime;
    int AttackType;

    SwordColliderController swordCollider;
    //MeleeSwordTrail swordTrail;

    public float swordActiveTimer;

    public List<TargetHitInfo> targetList = new List<TargetHitInfo>();

    private PlayerManager playerManager;

    public int SwordPower;

    public GameObject SwordEffectPrefab;
    public GameObject Tracking;

    private ParticleSystem swordEffectParticleSystem; // To store particle system

    // Start is called before the first frame update
    void Start()
    {
        swordCollider = GetComponentInChildren<SwordColliderController>();
        //swordTrail = GetComponentInChildren<MeleeSwordTrail>();
        playerManager = GetComponentInParent<PlayerManager>();

        if (swordCollider!=null)
        {
            Debug.Log("Start");
            swordCollider.OnSwordCollisionEvent += OnSwordCollision;
        }

        if (SwordEffectPrefab != null)
        {
            // Get the particle system from the SwordEffectPrefab
            swordEffectParticleSystem = SwordEffectPrefab.GetComponentInChildren<ParticleSystem>();
        }
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
            StopSwordEffect();
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

    public void SetSwordActive(int _AttactType)
    {
        swordActiveTimer = swordActiveTime;
        AttackType = _AttactType;
        if (AttackType == 1)
        {
            SwordPower = 10;
        }
        if (AttackType == 2)
        {
            SwordPower = 20;
        }

        PlaySwordEffect();
    }

    public void OnSwordCollision(IEventSource _source, ISwordTarget _target)
    {
        Debug.Log("OnSwordCollision called");

        swordActiveTime = 2f;
        if (swordActiveTimer > 0)
        {
            TargetHitInfo hitInfo = new TargetHitInfo(_source);
            if (!targetList.Contains(hitInfo))
            {
                _target.OnTargetHit(hitInfo, AttackType);
                targetList.Add(hitInfo);
                Debug.Log("Target hit processed");

                IDamageable damageable = _target as IDamageable;
                if (damageable != null)
                {
                    damageable.TakeDamage(SwordPower);
                    Debug.Log($"Dealt {SwordPower} damage to enemy.");
                }

                if (AttackType == 1 && playerManager != null)
                {
                    PlayerManager.PlayerSPAdd(20);
                    Debug.Log("SP added to player.");
                }
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
            Debug.Log("Destroy");
        }
    }

    private void PlaySwordEffect()
    {
        if (swordEffectParticleSystem != null && !swordEffectParticleSystem.isPlaying)
        {
            // Set the position and rotation of the particle effect
            SwordEffectPrefab.transform.position = Tracking.transform.position;
            SwordEffectPrefab.transform.rotation = Tracking.transform.rotation;

            // Play the particle effect
            swordEffectParticleSystem.Play();
        }
    }

    private void StopSwordEffect()
    {
        if (swordEffectParticleSystem != null && swordEffectParticleSystem.isPlaying)
        {
            swordEffectParticleSystem.Stop();  // Stop playing the particle system when sword is inactive
        }
    }
}