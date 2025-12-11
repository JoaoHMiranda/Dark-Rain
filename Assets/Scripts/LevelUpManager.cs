using UnityEngine;
using System.Collections.Generic;

public class LevelUpManager : MonoBehaviour
{
    public GameObject panelLevelUp;
    public Transform container;
    public GameObject prefabCard;

    private int pendingLevelUps = 0;

    public void AddLevelUpCredit()
    {
        // Se já estamos no nível máximo, ignora novos créditos
        if (SkillManager.instance.IsMaxLevelReached()) return;

        pendingLevelUps++;
        if (!panelLevelUp.activeSelf) ShowNextLevelUp();
    }

    private void ShowNextLevelUp()
    {
        var options = SkillManager.instance.GetLevelUpOptions();

        if (options.Count == 0)
        {
            ForceGameFinish(); // Caso de segurança
            return;
        }

        Time.timeScale = 0f;
        panelLevelUp.SetActive(true);

        foreach(Transform child in container) Destroy(child.gameObject);

        foreach(SkillSO skill in options)
        {
            GameObject card = Instantiate(prefabCard, container);
            card.transform.localPosition = Vector3.zero; 
            card.transform.localScale = Vector3.one; 
            card.GetComponent<SkillCard>().Setup(skill, this);
        }
    }

    public void SelectSkill(SkillSO skill)
    {
        // 1. Aplica a skill
        SkillManager.instance.AddSkill(skill);
        pendingLevelUps--;

        // --- VERIFICAÇÃO DE PRIORIDADE: MAX LEVEL ---
        if (SkillManager.instance.IsMaxLevelReached())
        {
            Debug.Log("Última carta pega! Cancelando níveis pendentes e finalizando.");
            
            // ZERA os níveis pendentes para não abrir o menu de novo
            pendingLevelUps = 0; 
            
            ForceGameFinish();
            return;
        }
        // --------------------------------------------

        if (pendingLevelUps > 0)
        {
            ShowNextLevelUp();
        }
        else
        {
            panelLevelUp.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    void ForceGameFinish()
    {
        pendingLevelUps = 0;
        
        // 1. PRIMEIRO: Atualiza o HUD visualmente
        var hud = FindAnyObjectByType<HUDManager>();
        if(hud != null) hud.ShowMaxLevelVisuals();

        // 2. SEGUNDO: Avisa a lógica do player
        var playerXP = FindAnyObjectByType<PlayerExperience>();
        if(playerXP != null) playerXP.SetMaxLevelInstant();

        // 3. TERCEIRO: Fecha a janela
        panelLevelUp.SetActive(false);
        Time.timeScale = 1f;
    }
    
    
}