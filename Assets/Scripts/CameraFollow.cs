using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // O alvo (Player)
    public Vector3 offset;   // A distância da câmera

    void Start()
    {
        // Calcula a distância atual entre a câmera e o player e mantém ela fixa
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

    void LateUpdate() // LateUpdate é melhor para câmeras
    {
        if (target != null)
        {
            // Move a câmera para a posição do player + a distância original
            transform.position = target.position + offset;
        }
    }
}