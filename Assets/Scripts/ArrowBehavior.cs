using UnityEngine;

public class ArrowBehavior : MonoBehaviour 
{
    public float speed = 30f;
    
    // Escondido no Inspector, pois quem manda é o Player agora
    [HideInInspector] public float knockbackForce; 
    [HideInInspector] public float damage;
    [HideInInspector] public float maxDistance;

    private Vector3 start;

    void Start() 
    { 
        start = transform.position; 
        Destroy(gameObject, 5f); 
    }

    void Update() 
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        
        if(Vector3.Distance(start, transform.position) > maxDistance) 
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Enemy")) 
        {
            // Dano
            var enemyHP = other.GetComponent<EnemyHealth>();
            if(enemyHP != null) enemyHP.TakeDamage(damage);

            // Empurrão (Usando o valor recebido do Player)
            Rigidbody enemyRb = other.GetComponent<Rigidbody>();
            if(enemyRb != null)
            {
                Vector3 pushDir = transform.forward;
                pushDir.y = 0; 
                enemyRb.AddForce(pushDir * knockbackForce, ForceMode.Impulse);
            }

            // Stun
            var enemyAI = other.GetComponent<EnemyAI>();
            if(enemyAI != null) enemyAI.ApplyStun(0.2f); 

            Destroy(gameObject);
        }
        else if (!other.CompareTag("Player") && !other.CompareTag("XP") && !other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}