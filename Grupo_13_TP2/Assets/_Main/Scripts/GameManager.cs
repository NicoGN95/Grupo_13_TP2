using System;
using UnityEngine;

namespace _Main.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;


        public Action<Vector3> OnCameraZoomChange;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            
        }
    }
}