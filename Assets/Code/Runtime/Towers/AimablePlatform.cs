using System;
using TowerDefense.Runtime.Utility;
using UnityEngine;

namespace TowerDefense.Runtime.Towers
{
    public class AimablePlatform : MonoBehaviour
    {
        public PidController behaviour;
        public float torque;
        public float maxSpeed;
        public Vector3 axis;
        public float sweepSpeed;

        public DefaultPoseBehaviour defaultPoseBehaviour;
        
        [Range(-1f, 1f)]
        public float input;
        
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

            var targetPosition = tower.targetPosition ?? GetDefaultTargetPosition();

            var direction = (targetPosition - transform.position).normalized;
            targetOrientation = Quaternion.LookRotation(direction);
            var targetAngle = Vector3.Dot(targetOrientation.eulerAngles, axis);
            var angle = Vector3.Dot(transform.localEulerAngles, axis);
            var error = Mathf.DeltaAngle(angle, targetAngle);
            
            input = Mathf.Clamp(behaviour.ComputeForce( error, Time.deltaTime), -1f, 1f);
            var force = (input - velocity / maxSpeed) * torque;

            position += velocity;
            velocity += force * Time.deltaTime;
            position %= 360f;

            if (float.IsNaN(position)) position = 0f;
            if (float.IsNaN(velocity)) velocity = 0f;

            transform.localRotation = Quaternion.Euler(this.axis * position);
        }

        private Vector3 GetDefaultTargetPosition()
        {
            return defaultPoseBehaviour switch {
                DefaultPoseBehaviour.DoNothing => transform.position + targetOrientation * Vector3.forward,
                DefaultPoseBehaviour.MoveToZero => transform.position + Vector3.forward,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public enum DefaultPoseBehaviour
        {
            DoNothing,
            MoveToZero,
        }
    }
}