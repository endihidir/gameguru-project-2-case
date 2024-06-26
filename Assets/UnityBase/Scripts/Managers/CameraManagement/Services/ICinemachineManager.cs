using Cinemachine;
using UnityBase.Manager;
using UnityEngine;
using CameraState = UnityBase.Manager.CameraState;

namespace UnityBase.Service
{
    public interface ICinemachineManager
    {
        public void ChangeCamera(GameState gameState);
        public CinemachineVirtualCamera GetVirtualCam(CameraState cameraState);
        public void SetGameplayTargetParent(Transform parent);
        public void SetGameplayTargetPosition(Vector3 position);
        public void SetGameplayTargetLocalPosition(Vector3 position);
        public void SetGameplayTargetRotation(Quaternion rotation);
        public void SetGameplayTargetLocalRotation(Quaternion rotation);
        public void RotateGameplayTarget(float speed, float deltaTime);
        public void ResetGameplayTarget(bool resetInLocal);
    }
}