using _main.Scripts.Services;
using _main.Scripts.Services.MicroServices.EventsServices;
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
        
        
        private float m_currRotX = 0f;
        private float m_currRotY = 0f;
        private float m_currMovementCount;

        private IEventService Service => ServiceLocator.Get<IEventService>();
        
        private void Start()
        {
            var l_rotation = parent.transform.rotation;
            maxAngleX += l_rotation.x;
            maxAngleY += l_rotation.y;

            SuscribeEvents();

        }

        private void SuscribeEvents()
        {
            var l_actions = inputs.actions;

            l_actions["Zoom"].performed += ZoomUpdate;
            l_actions["CameraRot"].performed += RotationUpdate;
        }
        



        private void RotationUpdate(InputAction.CallbackContext p_obj)
        {
            var l_mouseValue = p_obj.ReadValue<Vector2>();
            
            
            
            m_currRotX += l_mouseValue.x * rotSpeed * Time.deltaTime;
            m_currRotY -= l_mouseValue.y * rotSpeed * Time.deltaTime;

            m_currRotX = Mathf.Clamp(m_currRotX, -maxAngleX, maxAngleX);
            m_currRotY = Mathf.Clamp(m_currRotY, -maxAngleY, maxAngleY);

            parent.transform.rotation = Quaternion.Euler(m_currRotY, m_currRotX, 0);
        }

        private void ZoomUpdate(InputAction.CallbackContext p_obj)
        {
            var l_mouseWheel = p_obj.ReadValue<float>();
            
            

            switch (l_mouseWheel)
            {
                case > 0:
                    if(m_currMovementCount >= maxDepth) 
                        return;
                    var l_traslationPositive = transform.forward * (zoomSpeed * Time.deltaTime);
                    transform.position += l_traslationPositive;
                    m_currMovementCount += l_traslationPositive.magnitude;
                    
                    
                    Service.DispatchEvent(new CameraPos(transform.position));
                    
                    
                    break;
                
                
                case < 0:
                    if(m_currMovementCount <= 0) 
                        return;
                    
                    var l_traslationNegative = -transform.forward * (zoomSpeed * Time.deltaTime);
                    transform.position += l_traslationNegative;
                    m_currMovementCount -= l_traslationNegative.magnitude;
                    
                    
                    Service.DispatchEvent(new CameraPos(transform.position));
                    
                    
                    break;
            }
        }
        
        private struct CameraPos : ICustomEventData
        {
            public CameraPos(Vector3 p_pos)
            {
                Pos = p_pos;
            }
            public Vector3 Pos;
        }
    }
}