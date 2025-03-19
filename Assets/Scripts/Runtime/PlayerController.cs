using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime
{
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 10f;
        
        public Vector3 cameraOffset;
    
        private Camera mainCamera;
    
        private void Awake()
        {
            mainCamera = Camera.main;      
        }
    
        private void FixedUpdate()
        {
            Move();
        }

        private void LateUpdate()
        {
            mainCamera.transform.position = transform.position + cameraOffset;
            mainCamera.transform.LookAt(transform.position, Vector3.up);
        }

        private void Move()
        {
            var kb = Keyboard.current;
            var input = new Vector2()
            {
                x = kb.dKey.ReadValue() - kb.aKey.ReadValue(),
                y = kb.wKey.ReadValue() - kb.sKey.ReadValue(),
            };
            input = Vector2.ClampMagnitude(input, 1);

            var rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up), Vector3.up);
            var target = rotation * new Vector3(input.x, 0, input.y);
        
            transform.position += target * moveSpeed * Time.deltaTime;
        }
    }
}
