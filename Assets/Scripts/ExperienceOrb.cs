using UnityEngine;

public class ExperienceOrb : MonoBehaviour
{
    [Header("Configurações do XP")]
    public float flySpeed = 15f; // Mude aqui a velocidade de voo do orbe

    // Essa variável fica escondida pois quem define o valor é o Inimigo que morreu
    [HideInInspector] public float xpValue; 
    
    private bool isMagnetized = false;
    private Transform targetPlayer;

    void Update()
    {
        // Se foi imantado, voa na direção do player
        if (isMagnetized && targetPlayer != null)
        {
            // Move da posição atual -> para o player -> com a velocidade configurada
            transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, flySpeed * Time.deltaTime);
        }
    }

    // Chamado pelo Player quando entra no raio de coleta
    public void Magnetize(Transform playerParams)
    {
        if (!isMagnetized)
        {
            targetPlayer = playerParams;
            isMagnetized = true;
        }
    }

    // Só coleta quando bate no corpo do player
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Tenta pegar o script de XP do jogador
            PlayerExperience playerXP = other.GetComponent<PlayerExperience>();
            
            if (playerXP != null)
            {
                playerXP.GainExperience(xpValue);
            }

            Destroy(gameObject); // Orbe some
        }
    }
}