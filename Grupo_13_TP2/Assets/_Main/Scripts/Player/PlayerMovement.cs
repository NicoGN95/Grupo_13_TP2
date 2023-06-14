using _Main.Scripts.ScriptableObjects;
using _Main.Scripts.Utilities;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace _Main.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private PlayerData data;
        [SerializeField] private Transform checkGroundTransform;

        private Rigidbody m_rigidbody;
        private PlayerInput m_playerInput;
        private Vector3 m_dir;
        
        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            Assert.IsNotNull(m_rigidbody);
            
            m_playerInput = GetComponent<PlayerInput>();
            Assert.IsNotNull(m_playerInput);

            SubscribeEvents();
        }

        private void Update()
        {
            transform.position += m_dir * (data.MovementSpeed * Time.deltaTime);
        }

        private void SubscribeEvents()
        {
            var l_actions = m_playerInput.actions;
            l_actions["Jump"].performed += OnJumpInputHandler;
            l_actions["Move"].performed += OnMoveInputHandler;
            l_actions["Move"].canceled += OnMoveInputHandler;
        }

        private void UnsubscribeEvents()
        {
            var l_actions = m_playerInput.actions;
            l_actions["Jump"].performed -= OnJumpInputHandler;
            l_actions["Move"].performed -= OnMoveInputHandler;
            l_actions["Move"].canceled -= OnMoveInputHandler;
        }
        
        private void OnMoveInputHandler(InputAction.CallbackContext p_obj)
        {
            var l_newInput = p_obj.ReadValue<Vector2>();
            m_dir = l_newInput.X0Z();
        }
        
        private void OnJumpInputHandler(InputAction.CallbackContext p_obj)
        {
            var l_isGrounded = Physics.CheckSphere(checkGroundTransform.position, data.CheckGroundRadius, data.CheckGroundLayerMask);
            
            if (l_isGrounded)
                m_rigidbody.AddForce(Vector3.up * data.JumpForce, ForceMode.Impulse);
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }
    }
}
