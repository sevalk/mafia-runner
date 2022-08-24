using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AI.Core;
using AI.Movement;
using Dreamteck.Splines;
using Random = UnityEngine.Random;

namespace AI.Combat
{
    public class Fighter : MonoBehaviour , IAction
    {
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] public float weaponDamage = 5f;
        public void SetWeaponDamage(float damage)
        {
            weaponDamage = damage;
        }
        public Gun gun;

        private Animator _anim;
        private Mover _mover;
        private Health _target;
        private float _timeSinceLastAttack = Mathf.Infinity;

        private SplineFollower _follower;
        private void Start()
        {
            _mover = GetComponent<Mover>();
            _anim = GetComponent<Animator>();
            _follower = GameManager.Instance.splineFollower;
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            
           // _follower.followSpeed = 10f;
            if (!_target) return;
            if (_target.IsDead()) return;

           // _follower.followSpeed = 3f;
            if (!GetIsInRange())
            {
                _mover.MoveTo(_target.transform.position);
                _anim.SetTrigger("run");
            }
            else
            {
                _mover.Cancel();
                AttackBehavior();
               
            }
        }

        
        private void AttackBehavior()
        {
            transform.LookAt(_target.transform);
            if(_timeSinceLastAttack > timeBetweenAttacks)
            {
                //This will trigger the Hit() event
                TriggerAttack();
                _timeSinceLastAttack = 0;
               
            }

        }

        private void TriggerAttack()
        {
            _anim.ResetTrigger("stopAttack");
            _anim.SetTrigger("run");
            var rnd = Random.Range(0,2);
            _anim.SetTrigger("attack");
            if (rnd == 1)
            {
                _anim.SetTrigger("attackSit");
            }
            else
            {
                _anim.SetTrigger("attackSit");
               // _anim.SetTrigger("attackStand");
            }
            
        }

        //Animation controller
        void Hit()
        {
            if (_target == null)
            {
                
                return;
            }
            _target.TakeDamage(weaponDamage);
            gun.Fire(_target.transform);
            
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, _target.transform.position) < weaponRange;
        }

        public void Attack(GameObject combattarget)
        {
            //GetComponent<ActionSchedular>().StartAction(this);
            _target = combattarget.GetComponent<Health>();
           // print(gameObject.name+" attacking");
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (!combatTarget) return false;
            Health targetForControl = combatTarget.GetComponent<Health>();
            return !(!targetForControl) && !targetForControl.IsDead();

           
        }

        public void Cancel()
        {
            StopAttack();
            _target = null;

        }

        private void StopAttack()
        {
            _anim.ResetTrigger("attackSit");
            _anim.ResetTrigger("attackStand");
            _anim.ResetTrigger("attack");
            _anim.SetTrigger("stopAttack");
        }
    }
    
}
