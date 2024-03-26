using System;
using System.Collections.Generic;
using TowerDefense.Runtime.Level;
using UnityEngine;
using UnityEngine.Serialization;

namespace TowerDefense.Runtime.Enemies
{
    public class Enemy : MonoBehaviour, Damageable
    {
        public int id;
        public float speed;
        public int health;
        public int maxHealth;
        public Path path;
        public float rotationSmoothing;

        public float position { get; private set; }

        public static event Action<Enemy> EndOfPathEvent;

        public static List<Enemy> enemies = new();
        
        private void Awake()
        {
            path = GetComponent<Path>();
        }

        private void OnEnable()
        {
            enemies.Add(this);
            health = maxHealth;
        }

        private void OnDisable()
        {
            enemies.Remove(this);
        }

        private void Start()
        {
            PlaceOnPath(out _);
        }

        private void FixedUpdate()
        {
            position += speed * Time.deltaTime;
            PlaceOnPath(out var pastEnd);

            if (pastEnd)
            {
                EndOfPathEvent?.Invoke(this);
                Destroy(gameObject);
            }
        }

        private void PlaceOnPath(out bool pastEnd)
        {
            var corner = path.Sample(position, out pastEnd);
            transform.position = corner.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, corner.rotation, Mathf.Pow(0.5f, rotationSmoothing));
        }

        public static Enemy Spawn(Enemy enemy, Path path)
        {
            var instance = Instantiate(enemy);
            instance.path = path;
            return instance;
        }

        public void Damage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            Destroy(gameObject);
        }
    }
}