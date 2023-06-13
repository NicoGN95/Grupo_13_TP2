using UnityEngine;

namespace _Main.Scripts
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private GameObject parent;
        [SerializeField] private float rotSpeed;
        [SerializeField] private float zoomSpeed;
        [SerializeField] private float maxAngleX;
        [SerializeField] private float maxAngleY;


        private float m_currRotX = 0f;
        private float m_currRotY = 0f;
        private GameManager m_instance;
        private void Start()
        {
            var l_rotation = parent.transform.rotation;
            maxAngleX += l_rotation.x;
            maxAngleY += l_rotation.y;

            m_instance = GameManager.Instance;
        }

        private void Update()
        {
            RotationUpdate();

            ZoomUpdate();
        }



        private void RotationUpdate()
        {
            var l_mouseX = Input.GetAxis("Mouse X");
            var l_mouseY = Input.GetAxis("Mouse Y");
            
            m_currRotX += l_mouseX * rotSpeed * Time.deltaTime;
            m_currRotY -= l_mouseY * rotSpeed * Time.deltaTime;

            m_currRotX = Mathf.Clamp(m_currRotX, -maxAngleX, maxAngleX);
            m_currRotY = Mathf.Clamp(m_currRotY, -maxAngleY, maxAngleY);

            parent.transform.rotation = Quaternion.Euler(m_currRotY, m_currRotX, 0);
        }

        private void ZoomUpdate()
        {
            var l_mouseWheel = Input.GetAxis("Mouse ScrollWheel");

            switch (l_mouseWheel)
            {
                case > 0:
                    transform.position += transform.forward * (zoomSpeed * Time.deltaTime);
                    m_instance.OnCameraZoomChange.Invoke(transform.position);
                    break;
                case < 0:
                    transform.position += -transform.forward * (zoomSpeed * Time.deltaTime);
                    m_instance.OnCameraZoomChange.Invoke(transform.position);
                    break;
            }
        }
    }
}