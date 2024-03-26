using System;
using System.Collections;
using TowerDefense.Runtime.Enemies;
using TowerDefense.Runtime.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TowerDefense.Runtime.Towers
{
    public abstract class Tower : MonoBehaviour
    {
        public float tickRate;
        public float range;
        public TargetMode targetMode;

        private float tickTimer;
        private Camera mainCamera;
        private IEnumerator routine;
        
        public Enemy target { get; private set; }

        public Vector3? targetPosition { get; set; }

        protected virtual void Awake()
        {
            mainCamera = Camera.main;
        }

        private void FixedUpdate()
        {
            targetPosition = GetTargetPosition();
            
            tickTimer += Time.deltaTime;
            if (tickTimer > 1f / tickRate)
            {
                if (!HasTarget()) target = FindTarget();
                tickTimer -= 1f / tickRate;
                
                if (routine == null || !routine.MoveNext()) routine = Tick();
            }
        }

        protected virtual Vector3? GetTargetPosition() => target ? target.transform.position : null;

        private bool HasTarget()
        {
            return target && (target.transform.position - transform.position).sqrMagnitude < range * range;
        }

        protected abstract IEnumerator Tick();

        protected Enemy FindTarget()
        {
            var bestScore = 0f;
            var target = (Enemy)null;
            
            for (var i = 0; i < Enemy.enemies.Count; i++)
            {
                var enemy = Enemy.enemies[i];
                if (!enemy) continue;

                var length = (enemy.transform.position - transform.position).sqrMagnitude;
                if (length > range * range) continue;
                
                var score = GetScore(enemy);
                if (score < bestScore) continue;
                
                target = enemy;
                bestScore = score;
            }

            return target;
        }

        public float GetScore(Enemy enemy)
        {
            switch (targetMode)
            {
                case TargetMode.First:
                    return enemy.position;
                case TargetMode.Last:
                    return 1f / enemy.position;
                case TargetMode.Close:
                    return 1f / (enemy.transform.position - transform.position).sqrMagnitude;
                case TargetMode.Far:
                    return (enemy.transform.position - transform.position).sqrMagnitude;
                case TargetMode.Strong:
                    return enemy.maxHealth;
                case TargetMode.Weak:
                    return 1f / enemy.maxHealth;
                case TargetMode.Injured:
                    return 1f / enemy.health;
                case TargetMode.Healthy:
                    return enemy.health;
                case TargetMode.Fast:
                    return enemy.speed;
                case TargetMode.Slow:
                    return 1f / enemy.speed;
                default:
                    throw new System.Exception();
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            var lines = new ExtraGizmos.Line();
            Gizmos.matrix = transform.localToWorldMatrix;
            for (var i = 0; i < 32; i++)
            {
                var a = i / 16f * Mathf.PI;
                lines.Add(new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * range);
            }
            lines.DrawLoop(Color.red);
        }

        public static bool HasLineOfSight(Ray ray, GameObject target, float range = 100f)
        {
            return Physics.Raycast(ray, out var hit, range) && hit.collider.transform.IsChildOf(target.transform);
        }
        
        public enum TargetMode
        {
            First,
            Last,
            Close,
            Far,
            Strong,
            Weak,
            Injured,
            Healthy,
            Fast,
            Slow,
        }
    }
}