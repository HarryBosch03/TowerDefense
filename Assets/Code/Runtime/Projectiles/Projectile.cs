using System;
using TowerDefense.Runtime.Utility;
using UnityEngine;

namespace TowerDefense.Runtime.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        public float trailPersistTime = 1.0f;

        [Space]
        public GameObject trail;
        public GameObject hit;
        
        private Vector3 velocity;
        private SpawnArgs args;
        private bool dead;
        
        private void Awake()
        {
            if (hit) hit.SetActive(false);
        }

        private void OnValidate()
        {
            if (!trail) trail = gameObject.Find("Trail");
            if (!hit) hit = gameObject.Find("Hit");
        }

        private void FixedUpdate()
        {
            var ray = new Ray(transform.position, velocity);
            var step = velocity.magnitude * Time.deltaTime * 1.01f;

            var hits = Physics.RaycastAll(ray, step);
            ArrayUtils.Sort(hits, hit => hit.distance);
            for (var i = 0; i < hits.Length; i++)
            {
                ProcessHit(ray, hits[i]);
                if (dead) return;
            }

            transform.position += velocity * Time.deltaTime;
            velocity += Physics.gravity * args.gravityScale * Time.deltaTime;
            
            transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);
        }

        private void Die()
        {
            if (trail)
            {
                trail.name += " [DISPOSED]";
                trail.transform.SetParent(null, true);
                Destroy(trail, trailPersistTime);
            }
            
            dead = true;
            Destroy(gameObject);
        }

        private void ProcessHit(Ray ray, RaycastHit hit)
        {
            if (args.pierce > 0) args.pierce--;
            else
            {
                transform.position = hit.point;
                Die();
            }
        }

        public static void Spawn(Projectile projectile, Vector3 position, Vector3 direction, SpawnArgs spawnArgs)
        {
            direction.Normalize();
            
            var instance = Instantiate(projectile, position, Quaternion.LookRotation(direction));
            instance.gameObject.SetActive(true);
            instance.velocity = direction * spawnArgs.speed;
            instance.args = spawnArgs;
        }

        [System.Serializable]
        public struct SpawnArgs
        {
            public int damage;
            public float speed;
            public float gravityScale;
            public int pierce;
        }
    }
}