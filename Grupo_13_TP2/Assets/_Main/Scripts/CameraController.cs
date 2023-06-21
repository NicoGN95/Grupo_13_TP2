using UnityEngine;
using UnityEngine.InputSystem;

namespace _Main.Scripts
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private GameObject parent;
        [SerializeField] private PlayerInput inputs;
        [SerializeField] private float rotSpeed;
        [SerializeField] private float zoomSpeed;
        [SerializeField] private float maxDepth;
        [SerializeField] private float maxAngleX;
        [SerializeField] private float maxAngleY;
        
        
        private float m_currRotX;
        private float m_currRotY;
        private float m_currMovementCount;
        private bool m_isCameraUnlocked;

        
        private void Start()
        {
            var l_rotation = parent.transform.rotation;
            maxAngleX += l_rotation.x;
            maxAngleY += l_rotation.y;
            m_currRotX = l_rotation.x;
            m_currRotY = l_rotation.y;
            SubscribeEvents();

        }

        private void SubscribeEvents()
        {
            var l_actions = inputs.actions;

            l_actions["Zoom"].performed += ZoomUpdate;
            l_actions["CameraRot"].performed += RotationUpdate;
            l_actions["UnlockCamera"].performed += UnlockCamera;
            l_actions["UnlockCamera"].canceled += UnlockCamera;
        }

        private void UnsubscribeEvents()
        {
            var l_actions = inputs.actions;

            l_actions["Zoom"].performed -= ZoomUpdate;
            l_actions["CameraRot"].performed -= RotationUpdate;
            l_actions["UnlockCamera"].performed -= UnlockCamera;
            l_actions["UnlockCamera"].canceled -= UnlockCamera;
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }


        private void RotationUpdate(InputAction.CallbackContext p_obj)
        {
            
            if(!m_isCameraUnlocked)
                return;
            var l_mouseValue = p_obj.ReadValue<Vector2>();
            
            
            
            m_currRotX += l_mouseValue.x * rotSpeed * Time.deltaTime;
            m_currRotY -= l_mouseValue.y * rotSpeed * Time.deltaTime;

            m_currRotX = Mathf.Clamp(m_currRotX, -maxAngleX, maxAngleX);
            m_currRotY = Mathf.Clamp(m_currRotY, -maxAngleY, maxAngleY);

            parent.transform.rotation = Quaternion.Euler(m_currRotY, m_currRotX, 0);
        }
        
        private void UnlockCamera(InputAction.CallbackContext p_obj)
        {
            var l_value = p_obj.ReadValue<float>();
            m_isCameraUnlocked = l_value > 0;

            Cursor.lockState = m_isCameraUnlocked ? CursorLockMode.Locked : CursorLockMode.Confined;

        }
        
        private void ZoomUpdate(InputAction.CallbackContext p_obj)
        {
            if(!m_isCameraUnlocked)
                return;
            
            var l_mouseWheel = p_obj.ReadValue<float>();
            
            

            switch (l_mouseWheel)
            {
                case > 0:
                    if(m_currMovementCount >= maxDepth) 
                        return;
                    
                    var l_traslationPositive = transform.forward * (zoomSpeed * Time.deltaTime);
                    transform.position += l_traslationPositive;
                    m_currMovementCount += l_traslationPositive.magnitude;
                    
                    break;
                
                
                case < 0:
                    if(m_currMovementCount <= 0) 
                        return;
                    
                    var l_traslationNegative = -transform.forward * (zoomSpeed * Time.deltaTime);
                    transform.position += l_traslationNegative;
                    m_currMovementCount -= l_traslationNegative.magnitude;

                    break;
            }
        }
    }
}