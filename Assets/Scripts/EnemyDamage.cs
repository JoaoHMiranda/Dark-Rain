using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [Header("Configuração do Ataque")]
    public float damage = 10f; 

    // Se bater ou encostar...
    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHP = other.gameObject.GetComponent<PlayerHealth>();
            
            // Tenta dar dano. Se o player estiver invulnerável, o script do player ignora.
            if (playerHP != null)
            {
                playerHP.TakeDamage(damage);
            }
        }
    }
}