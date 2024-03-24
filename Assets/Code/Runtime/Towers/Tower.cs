using System;
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

        [Space]
        public bool lookAtMouse;

        private float tickTimer;
        private Camera mainCamera;
        
        public Enemy target { get; private set; }

        public Vector3? targetPosition
        {
            get
            {
                if (lookAtMouse)
                {
                    var plane = new Plane(Vector3.up, transform.position);
                    if (!mainCamera) return null;
                    var ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                    if (plane.Raycast(ray, out var enter))
                    {
                        return ray.GetPoint(enter);
                    }
                    return null;
                }
                return target ? target.transform.position : null;
            }
        }

        protected virtual void Awake()
        {
            mainCamera = Camera.main;
        }

        private void FixedUpdate()
        {
            tickTimer += Time.deltaTime;
            if (tickTimer > 1f / tickRate)
            {
                target = FindTarget();
                tickTimer -= 1f / tickRate;
                Tick();
            }
        }

        protected abstract void Tick();

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
                
                var score = enemy.position;
                if (score < bestScore) continue;
                
                target = enemy;
                bestScore = score;
            }

            return target;
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

    }
}