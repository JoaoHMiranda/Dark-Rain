using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Atributos do Inimigo")]
    public float maxHealth = 30f;
    
    [Header("Configuração de Drop")]
    public GameObject xpPrefab; // O modelo da bolinha de XP
    public float xpDropAmount = 10f; // Quanto de XP esse bicho dá (configurável)

    [Header("Debug")]
    public float currentHealth; 

    [Header("Reação ao Dano")]
    public float stunDuration = 0.2f;

    private EnemyAI enemyMovement;

    void Start()
    {
        currentHealth = maxHealth;
        enemyMovement = GetComponent<EnemyAI>();
    }

    public void TakeDamage(float damageAmount)
    {
        if (enemyMovement != null) enemyMovement.ApplyStun(stunDuration);

        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Cria a orbe de XP no lugar onde o inimigo morreu
        if (xpPrefab != null)
        {
            GameObject xp = Instantiate(xpPrefab, transform.position, Quaternion.identity);
            
            // Passa o valor do XP para a orbe
            ExperienceOrb orbScript = xp.GetComponent<ExperienceOrb>();
            if (orbScript != null)
            {
                orbScript.xpValue = xpDropAmount;
            }
        }

        Destroy(gameObject);
    }
}