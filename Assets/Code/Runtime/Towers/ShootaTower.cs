using TowerDefense.Runtime.Projectiles;
using TowerDefense.Runtime.Utility;
using UnityEngine;

namespace TowerDefense.Runtime.Towers
{
    public class ShootaTower : Tower
    {
        public Projectile.SpawnArgs spawnArgs;
        public float shootAngle = 5f;

        [Space]
        public Projectile projectilePrefab;
        public Transform muzzle;

        protected override void Awake()
        {
            base.Awake();
            projectilePrefab.gameObject.SetActive(false);
        }

        private void OnValidate()
        {
            if (!projectilePrefab) projectilePrefab = GetComponentInChildren<Projectile>();
            if (!muzzle) muzzle = transform.Search("Muzzle");
        }

        protected override void Tick()
        {
            if (!target) return;

            var direction = (target.transform.position - muzzle.position).normalized;
            var dot = Vector3.Dot(muzzle.forward, direction);
            var angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            if (angle < shootAngle)
            {
                Projectile.Spawn(projectilePrefab, muzzle.position, muzzle.forward, spawnArgs);
            }
        }
    }
}