using UnityEngine;
using System.Collections; // Necessário para usar o tempo de espera (Coroutine)

public class EnemyAI : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float speed = 3f;
    private Transform playerTransform;
    
    // Variável para saber se ele está atordoado
    private bool isStunned = false;

    void Start()
    {
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        // SE estiver atordoado, NÃO faz nada (sai da função)
        if (isStunned) return;

        if (playerTransform != null)
        {
            transform.LookAt(playerTransform);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    // Função pública para alguém de fora chamar o Stun
    public void ApplyStun(float duration)
    {
        // Inicia a contagem de tempo em paralelo
        StartCoroutine(StunRoutine(duration));
    }

    // A rotina que espera o tempo passar
    IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration); // Espera X segundos
        isStunned = false;
    }
}