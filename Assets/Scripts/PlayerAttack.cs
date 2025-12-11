using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class PlayerAttack : MonoBehaviour 
{
    [Header("Obrigatório")]
    public Transform firePoint;      
    public GameObject arrowPrefab;   

    [Header("Atributos de Combate")]
    public float fireRate = 0.5f; 
    public float range = 15f;
    public float weaponDamage = 20f; 
    public float weaponKnockback = 10f; 

    [Header("Áudio")]
    public AudioClip shootSound;
    [Range(0f, 1f)] public float volume = 0.5f;

    [Header("Estado")]
    public bool useMouse = true; 

    private float nextFireTime = 0f;
    private Animator anim;
    private AudioSource audioSource;
    private bool canAim = false; // Trava de segurança inicial

    void Start() 
    {
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        if(anim) anim.SetFloat("AttackSpeed", 1f);

        // --- CORREÇÃO DO "SPAWN DEITADO" ---
        // Espera 0.1 segundos antes de deixar o script girar o boneco.
        // Isso dá tempo da animação "Idle" colocar a Erika em pé.
        Invoke("EnableAiming", 0.1f);
    }

    void EnableAiming()
    {
        canAim = true;
    }

    void Update() 
    {
        // Se ainda está carregando a postura, não faz nada
        if (!canAim) return;

        // Alternar mira
        if (Keyboard.current.altKey.wasPressedThisFrame) 
        {
            useMouse = !useMouse;
            Debug.Log("Modo de Mira: " + (useMouse ? "MOUSE" : "AUTOMÁTICO"));
        }

        // Metrônomo do Tiro
        if (Time.time >= nextFireTime)
        {
            if (useMouse) TryShootAtMouse();
            else TryShootAuto();
        }
    }

    void TryShootAtMouse()
    {
        if (Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane ground = new Plane(Vector3.up, transform.position); 
        
        if (ground.Raycast(ray, out float dist))
        {
            Vector3 worldMousePos = ray.GetPoint(dist);
            
            // 1. Gira o CORPO (Mantendo a altura dos pés para não tombar)
            Vector3 bodyTarget = worldMousePos;
            bodyTarget.y = transform.position.y;
            transform.LookAt(bodyTarget);

            // 2. Define o alvo do TIRO (Mantendo a altura da arma para voar reto)
            Vector3 shootTarget = worldMousePos;
            if(firePoint != null) shootTarget.y = firePoint.position.y;

            Shoot(shootTarget);
            nextFireTime = Time.time + fireRate;
        }
    }

    void TryShootAuto()
    {
        GameObject enemy = FindClosestEnemy();
        Vector3 targetPos;

        // Decide o alvo (Inimigo ou Aleatório)
        if (enemy != null && Vector3.Distance(transform.position, enemy.transform.position) <= range)
        {
            targetPos = enemy.transform.position;
        }
        else
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized * 5f; 
            targetPos = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        }

        // 1. Gira o CORPO (Sem tombar)
        Vector3 bodyTarget = targetPos;
        bodyTarget.y = transform.position.y;
        transform.LookAt(bodyTarget);

        // 2. Define alvo do TIRO (Reto)
        if(firePoint != null) targetPos.y = firePoint.position.y;

        Shoot(targetPos);
        nextFireTime = Time.time + fireRate;
    }

    void Shoot(Vector3 arrowTargetPos)
    {
        // Sincroniza Velocidade da Animação
        if (anim != null) 
        {
            float safeRate = Mathf.Max(fireRate, 0.1f); 
            float animSpeed = 1f / safeRate; 
            anim.SetFloat("AttackSpeed", animSpeed);
            anim.SetTrigger("Shoot");
        }

        // Cria a Flecha
        if (arrowPrefab && firePoint)
        {
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
            
            // Flecha olha para o alvo
            arrow.transform.LookAt(arrowTargetPos);
            
            // Passa os status
            var script = arrow.GetComponent<ArrowBehavior>();
            if (script != null)
            {
                script.damage = weaponDamage;
                script.maxDistance = range;
                script.knockbackForce = weaponKnockback;
            }
        }

        if (shootSound && audioSource) audioSource.PlayOneShot(shootSound, volume);
    }

    GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float minDist = Mathf.Infinity;
        foreach (GameObject e in enemies)
        {
            float d = Vector3.Distance(transform.position, e.transform.position);
            if (d < minDist) { closest = e; minDist = d; }
        }
        return closest;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}