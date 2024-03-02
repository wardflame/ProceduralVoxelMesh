using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Essence.Entity.Player
{
    public class PlayerLook : MonoBehaviour
    {
        public PlayerKernel kernel;

        public float rotationPower = 1;
        private float rotationPowerCoefficient = 100;

        public int angleThresholdBottom = 290;
        public int angleThresholdTop = 70;

        private Vector2 rotationNorm;

        private bool isRotating;

        private float rotX;
        private float rotY;

        private void Awake()
        {
            Debug.Log("Look");

            kernel = GetComponent<PlayerKernel>();

            kernel.input.Movement.Look.performed += OnLook;
            kernel.input.Movement.Look.canceled += OnLook;
        }

        private void Update()
        {
            Look();
        }

        private void Look()
        {
            if (!isRotating) return;

            rotY += rotValue.x * Time.deltaTime * lookSensitivity;
            rotX -= rotValue.y * Time.deltaTime * (lookSensitivity / 2);

            rotX = Mathf.Clamp(rotX, -90f, 90f);

            playerManager.playerTransform.rotation = Quaternion.Euler(0, rotY, 0);
            cameraTarget.rotation = Quaternion.Euler(rotX, rotY, 0);

            #region Rotate Player
            var qX = Quaternion.AngleAxis(rotationNorm.x * rotationPower * Time.deltaTime, Vector3.up);

            var rot = transform.rotation;
            var rotQ = rot * qX;

            transform.rotation = Quaternion.Slerp(rot, rotQ, 0.1f);
            #endregion Rotate Player

            #region Rotate VCam Look Target
            var qY = Quaternion.AngleAxis(rotationNorm.y * rotationPower * Time.deltaTime, Vector3.right);

            var kernelVCam = kernel.cameraCMV;
            var kVCamRot = kernelVCam.Follow.transform.rotation;

            var kVCamRotQ = kVCamRot * Quaternion.Inverse(qY);

            kernel.cameraCMV.Follow.transform.rotation = Quaternion.Slerp(kVCamRot, kVCamRotQ, 0.1f);
            #endregion Rotate VCam Look Target

            #region Restrict Camera Angles
            var vCamEuler = kernelVCam.Follow.transform.localEulerAngles;
            vCamEuler.z = 0;

            var vCamEulerX = vCamEuler.x;

            if (vCamEulerX > 180 && vCamEulerX < angleThresholdBottom)
            {
                vCamEuler.x = angleThresholdBottom;
            }
            else if (vCamEulerX < 180 && vCamEulerX > angleThresholdTop)
            {
                vCamEuler.x = angleThresholdTop;
            }

            kernel.cameraCMV.Follow.localEulerAngles = vCamEuler;
            #endregion Restrict Camera Angles
        }

        private void OnLook(InputAction.CallbackContext value)
        {
            rotationNorm = value.ReadValue<Vector2>().normalized;
            isRotating = rotationNorm.magnitude > 0;
        }
    }
}
