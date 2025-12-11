using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    [Header("Status")]
    public int currentLevel = 1;
    public float currentXP = 0;
    public float xpToNextLevel = 100;
    
    public bool isMaxLevel = false;

    [Header("Configuração")]
    public float collectionRadius = 5f;
    public LayerMask xpLayer;

    private HUDManager hud;
    private LevelUpManager levelManager;

    void Start()
    {
        hud = FindAnyObjectByType<HUDManager>();
        levelManager = FindAnyObjectByType<LevelUpManager>();
        if (hud != null) hud.SetFinalTargetXP(0, xpToNextLevel);
    }

    void Update()
    {
        // --- MUDANÇA AQUI: Removemos o "if (!isMaxLevel)" ---
        // Agora ele sempre procura orbes, para dar o efeito visual de coleta
        CheckForOrbs();
    }

    void CheckForOrbs()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, collectionRadius, xpLayer);
        foreach (Collider hit in hits)
        {
            ExperienceOrb orb = hit.GetComponent<ExperienceOrb>();
            if (orb != null) orb.Magnetize(transform);
        }
    }

    // Função pública para travar tudo
    public void SetMaxLevelInstant()
    {
        if (isMaxLevel) return;
        isMaxLevel = true;
        if (hud != null) hud.ShowMaxLevelVisuals();
    }

    public void GainExperience(float amount)
    {
        // Se já travou, só sai (mas o orbe já foi coletado visualmente no Update)
        if (isMaxLevel) return;

        currentXP += amount;

        // Enquanto tiver XP sobrando...
        while (currentXP >= xpToNextLevel)
        {
            // 1. ANTES DE SUBIR: Verifica se ainda temos cartas
            // Se NÃO tiver mais cartas, paramos TUDO aqui.
            if (GetComponent<SkillManager>().IsMaxLevelReached())
            {
                // Trava a matemática
                isMaxLevel = true;
                
                // Trava o XP no máximo (para a barra ficar cheia)
                currentXP = xpToNextLevel; 

                // Força o visual agora
                if (hud != null) hud.ShowMaxLevelVisuals();
                
                return; // Sai da função. Não sobe level, não zera barra.
            }

            // 2. Se tem cartas, sobe de nível normal
            currentXP -= xpToNextLevel;
            currentLevel++; 
            xpToNextLevel *= 1.2f;       

            if (hud != null) hud.QueueLevelUp(currentLevel);

            if (levelManager != null) levelManager.AddLevelUpCredit();
        }

        // Atualiza a barra normal enquanto não é Max
        if (!isMaxLevel && hud != null) 
        {
            hud.SetFinalTargetXP(currentXP, xpToNextLevel);
        }
    }
}