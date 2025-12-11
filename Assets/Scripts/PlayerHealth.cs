using UnityEngine;

public class PlayerHealth : MonoBehaviour 
{
    [Header("Configuração")]
    public float maxHealth = 100;
    
    [Header("Explosão de Morte")]
    public float deathRepelRadius = 10f; // Tamanho da explosão
    public float deathRepelForce = 20f;  // Força do empurrão
    
    [Header("Status")]
    public float currentHealth; 
    
    private Animator anim;
    private bool isDead = false;

    void Start() 
    {
        // --- A CORREÇÃO MÁGICA ---
        // Garante que o jogo despause sempre que o personagem nascer
        Time.timeScale = 1f; 
        // -------------------------

        currentHealth = maxHealth;
        // Pega o Animator que está no modelo filho (Erika)
        anim = GetComponentInChildren<Animator>();
        
        UpdateHealthUI();
    }

    public void TakeDamage(float amount) 
    {
        if(isDead) return;
        
        currentHealth -= amount;
        
        if(currentHealth <= 0) 
        {
            currentHealth = 0;
            Die();
        }

        UpdateHealthUI();
    }

    public void UpdateHealthUI()
    {
        var hud = FindFirstObjectByType<HUDManager>();
        if (hud != null) hud.UpdateHealthBar(currentHealth, maxHealth);
    }

    void Die() 
    {
        if (isDead) return;
        isDead = true;

        // --- 1. A GRANDE EXPLOSÃO DE MORTE ---
        // Procura tudo que está perto
        Collider[] enemies = Physics.OverlapSphere(transform.position, deathRepelRadius);
        
        foreach (Collider col in enemies)
        {
            if (col.CompareTag("Enemy"))
            {
                // Empurra o inimigo
                Rigidbody enemyRb = col.GetComponent<Rigidbody>();
                if (enemyRb != null)
                {
                    // Calcula a direção: Do Player -> Para o Inimigo
                    Vector3 direction = col.transform.position - transform.position;
                    direction.y = 0; // Não empurra pra cima, só pros lados
                    direction.Normalize();

                    // Aplica o chute (Impulse é força instantânea)
                    enemyRb.AddForce(direction * deathRepelForce, ForceMode.Impulse);
                }

                // (Opcional) Atordoa eles para não voltarem correndo na hora
                EnemyAI ai = col.GetComponent<EnemyAI>();
                if (ai != null) ai.ApplyStun(1.0f); // 1 segundo de stun
            }
        }
        // -------------------------------------

        Debug.Log("JOGADOR MORREU");

        if(anim) 
        {
            anim.SetTrigger("Die");
            anim.SetLayerWeight(1, 0); 
        }
        
        // Desliga controles
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerAttack>().enabled = false;
        
        // Desliga colisão e trava física
        GetComponent<CapsuleCollider>().enabled = false;
        
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; 
        
        // Cola no chão para não flutuar
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

        Invoke("CallGameOver", 3.2f);
    }

    void CallGameOver() 
    {
        var gm = FindFirstObjectByType<GameOverManager>();
        if(gm != null) gm.ShowGameOver();
    }
    
    // Desenha o raio da explosão no editor para você ver
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, deathRepelRadius);
    }
}