using System;
using System.Collections.Generic;
using TowerDefense.Runtime.Level;
using UnityEngine;
using UnityEngine.Serialization;

namespace TowerDefense.Runtime.Enemies
{
    public class Enemy : MonoBehaviour
    {
        public int id;
        public float speed;
        public Path path;

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
        }

        private void OnDisable()
        {
            enemies.Remove(this);
        }

        private void FixedUpdate()
        {
            position += speed * Time.deltaTime;
            var corner = path.Sample(position, out var pastEnd);
            transform.position = corner.position;
            transform.rotation = corner.rotation;

            if (pastEnd)
            {
                EndOfPathEvent?.Invoke(this);
                Destroy(gameObject);
            }
        }

        public static Enemy Spawn(Enemy enemy, Path path)
        {
            var instance = Instantiate(enemy);
            instance.path = path;
            return instance;
        }
    }
}