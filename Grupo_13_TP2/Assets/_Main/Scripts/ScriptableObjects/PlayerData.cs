using UnityEngine;

namespace _Main.Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Main/Player/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        [field: SerializeField] public float MovementSpeed { get; private set; } = 2;
        [field: SerializeField] public float JumpForce { get; private set; } = 5;
        [field: SerializeField] public float CheckGroundRadius { get; private set; } = 0.5f;
        [field: SerializeField] public LayerMask CheckGroundLayerMask { get; private set; }
    }
}