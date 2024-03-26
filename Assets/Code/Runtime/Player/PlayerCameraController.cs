using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace TowerDefense.Runtime.Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        public float panSpeed;
        public float panAcceleration;

        [Space]
        public float cameraAngle;
        public float cameraMinDistance;
        public float cameraMaxDistance;
        public float cameraMinSize;
        public float cameraMaxSize;

        [Space] public float zoomKeySensitivity;
        [Space] public float zoomMouseSensitivity;
        [Space] public float zoomSmoothing;
        [Space] public float fieldOfView;
        
        private float targetZoom;
        private float currentZoom;
        private Camera mainCam;
        
        private Vector2 position;
        private Vector2 velocity;
        private Vector2 panInput;

        private void OnValidate()
        {
            zoomSmoothing = Mathf.Max(0, zoomSmoothing);
        }

        private void Awake()
        {
            mainCam = Camera.main;
        }
        
        private void Update()
        {
            panInput = GetPanInput();
            UpdateZoom();
            BindCamera();
        }

        private void UpdateZoom()
        {
            var kb = Keyboard.current;
            var m = Mouse.current;
            targetZoom += (kb.qKey.ReadValue() - kb.eKey.ReadValue()) * zoomKeySensitivity * Time.deltaTime;
            targetZoom -= m.scroll.ReadValue().y * zoomMouseSensitivity;
            targetZoom = Mathf.Clamp01(targetZoom);

            currentZoom = Mathf.Lerp(currentZoom, targetZoom, Mathf.Pow(0.5f, zoomSmoothing));
        }

        private void BindCamera()
        {
            var cameraDistance = Mathf.Lerp(cameraMinDistance, cameraMaxDistance, currentZoom);
            var size = Mathf.Lerp(cameraMinSize, cameraMaxSize, currentZoom);
            
            var rotation = transform.rotation * Quaternion.Euler(cameraAngle, 0f, 0f);
            mainCam.transform.position = transform.position - mainCam.transform.forward * cameraDistance;
            mainCam.transform.rotation = rotation;

            fieldOfView = Mathf.Atan(size / cameraDistance) * Mathf.Rad2Deg * 2f;
            mainCam.orthographic = false;
            mainCam.fieldOfView = fieldOfView;
        }

        private Vector2 GetPanInput()
        {
            var basis = Quaternion.Euler(0f, 0f, -mainCam.transform.eulerAngles.y);
            var kb = Keyboard.current;
            var raw = new Vector2()
            {
                x = kb.dKey.ReadValue() - kb.aKey.ReadValue(),
                y = kb.wKey.ReadValue() - kb.sKey.ReadValue(),
            };

            return basis * raw;
        }

        private void FixedUpdate()
        {
            position = new Vector2(transform.position.x, transform.position.z);

            var speed = Mathf.Tan(mainCam.fieldOfView * Mathf.Deg2Rad * 0.5f) * panSpeed;
            var force = (panInput * speed - velocity) * panAcceleration;
            
            position += velocity * Time.deltaTime;
            velocity += force * Time.deltaTime;
            
            transform.position = new Vector3(position.x, 0f, position.y);
        }
    }
}
