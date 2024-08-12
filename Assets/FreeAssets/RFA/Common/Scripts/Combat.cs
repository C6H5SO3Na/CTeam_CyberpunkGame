using UnityEngine;
using NaughtyCharacter;
using System.Collections;
using System.Collections.Generic;

namespace Retro.ThirdPersonCharacter
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Animator))]
    public class Combat : MonoBehaviour, IEventSource
    {
        private const string attackTriggerName = "Attack";
        private const string specialAttackTriggerName = "Ability";

        private Animator _animator;
        private PlayerInput _playerInput;

        public bool AttackInProgress {get; private set;} = false;

        SwordComponent swordComponent;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _playerInput = GetComponent<PlayerInput>();
            swordComponent = GetComponent<SwordComponent>();
        }

        private void Update()
        {
            if(_playerInput.AttackInput && !AttackInProgress)
            {
                Attack();
            }
            else if (_playerInput.SpecialAttackInput && !AttackInProgress)
            {
                SpecialAttack();
            }
        }

        private void SetAttackStart()
        {
            AttackInProgress = true;
        }

        private void SetAttackEnd()
        {
            AttackInProgress = false;
        }

        private void Attack()
        {
            _animator.SetTrigger(attackTriggerName);
        }

        private void SpecialAttack()
        {
            swordComponent.SetSwordActive();
            Debug.Log("Attack");
            _animator.SetTrigger(specialAttackTriggerName);
            
        }
    }
}