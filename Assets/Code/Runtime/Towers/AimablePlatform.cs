using System;
using TowerDefense.Runtime.Utility;
using UnityEngine;

namespace TowerDefense.Runtime.Towers
{
    public class AimablePlatform : MonoBehaviour
    {
        public PidController behaviour;
        public float rotationSpeed;
        public float torque;
        public Vector3 axis;

        private AimablePlatform parent;
        private Tower tower;

        private float position;
        private float velocity;

        public Quaternion targetOrientation;
        
        private void Awake()
        {
            parent = GetComponentInParent<AimablePlatform>();
            tower = GetComponentInParent<Tower>();
        }

        private void FixedUpdate()
        {
            if (!tower) return;

            if (!tower.targetPosition.HasValue) return;
            var targetPosition = tower.targetPosition.Value;

            var direction = (targetPosition - transform.position).normalized;
            targetOrientation = Quaternion.LookRotation(direction);
            var targetAngle = Vector3.Dot(targetOrientation.eulerAngles, axis);
            var angle = Vector3.Dot(transform.localEulerAngles, axis);
            var error = Mathf.DeltaAngle(angle, targetAngle);
            
            var input = Mathf.Clamp(behaviour.ComputeForce( error, Time.deltaTime), -1f, 1f);
            var force = (input * rotationSpeed - velocity) * torque;

            position += velocity;
            velocity += force * Time.deltaTime;
            position %= 360f;

            transform.localRotation = Quaternion.Euler(this.axis * position);
        }

        private Vector3 ToWorld(Vector3 axis)
        {
            return parent ? parent.targetOrientation * axis : axis;
        }
    }
}