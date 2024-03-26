using System;
using TowerDefense.Runtime.Utility;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TowerDefense.Runtime.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        public float trailPersistTime = 1.0f;

        [Space]
        public ParticleSystem trail;
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
            if (!trail) trail = transform.Find<ParticleSystem>("Trail");
            if (!hit) hit = gameObject.Find("Hit");
        }

        private void Start()
        {
            trail.Emit(1);
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

            if (trail) trail.Emit(1);

            transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);
        }

        private void Die()
        {
            if (trail)
            {
                trail.Emit(1);
                trail.name += " [DISPOSED]";
                trail.transform.SetParent(null, true);
                Destroy(trail.gameObject, trailPersistTime);
            }
            
            dead = true;
            Destroy(gameObject);
        }

        private void ProcessHit(Ray ray, RaycastHit hit)
        {
            var damageable = hit.collider.GetComponentInParent<Damageable>();
            if (damageable != null)
            {
                damageable.Damage(args.damage);
            }

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

            var orientation = Quaternion.LookRotation(direction);
            var px = Random.Range(-Mathf.PI, Mathf.PI);
            var py = Random.Range(0f, spawnArgs.spreadAngle);
            var offset = new Vector2(Mathf.Cos(px), Mathf.Sin(px)) * py;
            orientation *= Quaternion.Euler(offset.x, offset.y, 0f);
            direction = orientation * Vector3.forward;
            
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
            public float spreadAngle;
        }
    }
}