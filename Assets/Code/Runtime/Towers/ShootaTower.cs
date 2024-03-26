using System;
using System.Collections;
using TowerDefense.Runtime.Projectiles;
using TowerDefense.Runtime.Utility;
using UnityEngine;

namespace TowerDefense.Runtime.Towers
{
    public class ShootaTower : Tower
    {
        public Projectile.SpawnArgs spawnArgs;

        public int burstCount;
        public int waitCount;
        
        [Space]
        public Projectile projectilePrefab;
        public Transform muzzle;

        public Ray shootRay => new Ray(muzzle.position, muzzle.forward);
        
        protected override void Awake()
        {
            base.Awake();
            projectilePrefab.gameObject.SetActive(false);
        }

        private void OnValidate()
        {
            if (!projectilePrefab) projectilePrefab = GetComponentInChildren<Projectile>();
            if (!muzzle) muzzle = transform.Search("Muzzle");

            burstCount = Mathf.Max(1, burstCount);
            waitCount = Mathf.Max(0, waitCount);
        }

        protected override IEnumerator Tick()
        {
            while (!target || !HasLineOfSight(shootRay, target.gameObject)) yield return null;

            for (var i = 0; i < burstCount; i++)
            {
                Projectile.Spawn(projectilePrefab, muzzle.position, muzzle.forward, spawnArgs);
                yield return null;
            }

            for (var i = 0; i < waitCount; i++) yield return null;
        }
    }
}