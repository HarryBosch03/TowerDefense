using System;
using TowerDefense.Runtime.Enemies;
using UnityEngine;

namespace TowerDefense.Runtime.Level
{
    public class Spawner : MonoBehaviour
    {
        public Path path;
        public Enemy prefab;
        public float delay;

        private float timer;

        private void FixedUpdate()
        {
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                timer -= delay;
                Enemy.Spawn(prefab, path);
            }
        }

        private void Reset()
        {
            path = FindObjectOfType<Path>();
        }
    }
}