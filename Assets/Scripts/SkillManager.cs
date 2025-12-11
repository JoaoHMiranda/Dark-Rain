using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    [Header("Banco de Dados")]
    public List<SkillTreeDef> allTrees;
    public List<FusionDefinition> allFusions;

    [Header("Configuração RNG")]
    public int weightForOwnedTrees = 4; 

    [Header("Estado")]
    public List<SkillSO> acquiredSkills = new List<SkillSO>();

    // Variável para saber o total de cartas REAIS que existem
    private int totalTreeSkillsCount = 0;

    void Awake() 
    { 
        instance = this; 
        
        // --- CORREÇÃO: CONTA SÓ O QUE EXISTE ---
        if (allTrees != null)
        {
            totalTreeSkillsCount = 0;
            foreach(var tree in allTrees)
            {
                // Só conta +1 se o campo não estiver vazio (null)
                if (tree.initialSkill != null) totalTreeSkillsCount++;
                if (tree.secondarySkillA != null) totalTreeSkillsCount++;
                if (tree.secondarySkillB != null) totalTreeSkillsCount++;
                if (tree.finalSkill != null) totalTreeSkillsCount++;
            }
        }
        Debug.Log("Total de cartas nas árvores: " + totalTreeSkillsCount);
    }

    // --- VERIFICAÇÃO DE MAX LEVEL ---
    public bool IsMaxLevelReached()
    {
        // Conta quantas skills DE ÁRVORE o jogador tem
        int ownedTreeSkills = 0;
        
        // Verifica árvore por árvore quais skills você já pegou
        foreach(var tree in allTrees)
        {
            if (tree.initialSkill != null && acquiredSkills.Contains(tree.initialSkill)) ownedTreeSkills++;
            if (tree.secondarySkillA != null && acquiredSkills.Contains(tree.secondarySkillA)) ownedTreeSkills++;
            if (tree.secondarySkillB != null && acquiredSkills.Contains(tree.secondarySkillB)) ownedTreeSkills++;
            if (tree.finalSkill != null && acquiredSkills.Contains(tree.finalSkill)) ownedTreeSkills++;
        }

        // Se o número que temos é igual ao total existente, acabou!
        return ownedTreeSkills >= totalTreeSkillsCount;
    }
    // ----------------------------------

    public List<SkillSO> GetLevelUpOptions()
    {
        List<SkillTreeDef> lotteryUrn = new List<SkillTreeDef>();

        foreach (var tree in allTrees)
        {
            if (GetNextSkillFromTree(tree) == null) continue;
            bool isStarted = acquiredSkills.Contains(tree.initialSkill);
            int tickets = isStarted ? weightForOwnedTrees : 1;
            for (int i = 0; i < tickets; i++) lotteryUrn.Add(tree);
        }

        ShuffleList(lotteryUrn);

        List<SkillSO> finalOptions = new List<SkillSO>();
        List<SkillTreeDef> usedTrees = new List<SkillTreeDef>(); 

        foreach (SkillTreeDef ticket in lotteryUrn)
        {
            if (finalOptions.Count >= 3) break; 
            if (usedTrees.Contains(ticket)) continue;

            SkillSO nextSkill = GetNextSkillFromTree(ticket);
            if (nextSkill != null)
            {
                finalOptions.Add(nextSkill);
                usedTrees.Add(ticket); 
            }
        }

        return finalOptions;
    }

    void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private SkillSO GetNextSkillFromTree(SkillTreeDef tree)
    {
        bool hasInitial = acquiredSkills.Contains(tree.initialSkill);
        bool hasSecA = acquiredSkills.Contains(tree.secondarySkillA);
        bool hasSecB = acquiredSkills.Contains(tree.secondarySkillB);
        bool hasFinal = acquiredSkills.Contains(tree.finalSkill);

        // Se não tem a inicial e ela existe -> Retorna Inicial
        if (tree.initialSkill != null && !hasInitial) return tree.initialSkill;

        // Se tem a inicial...
        if (hasInitial)
        {
            // Procura secundárias que existam e faltam
            List<SkillSO> available = new List<SkillSO>();
            if (tree.secondarySkillA != null && !hasSecA) available.Add(tree.secondarySkillA);
            if (tree.secondarySkillB != null && !hasSecB) available.Add(tree.secondarySkillB);
            
            if (available.Count > 0) return available[UnityEngine.Random.Range(0, available.Count)];

            // Se tem secundárias, tenta a final
            if (tree.finalSkill != null && !hasFinal)
            {
                // Regra: Só libera final se tiver as secundárias QUE EXISTEM
                bool reqA = tree.secondarySkillA == null || hasSecA;
                bool reqB = tree.secondarySkillB == null || hasSecB;
                
                if (reqA && reqB) return tree.finalSkill;
            }
        }

        return null; 
    }

    public void AddSkill(SkillSO skill)
    {
        if (!acquiredSkills.Contains(skill))
        {
            acquiredSkills.Add(skill);
            ApplyEffects(skill);
            CheckFusions();
        }
    }

    void ApplyEffects(SkillSO skill)
    {
        if (skill.type == SkillType.StatBoost)
        {
            PlayerHealth hp = GetComponent<PlayerHealth>();
            PlayerAttack atk = GetComponent<PlayerAttack>();
            PlayerMovement mov = GetComponent<PlayerMovement>();
            
            // --- REGRA DE TRÊS PARA A VIDA ---
            if (skill.statName == "MaxHealth" && hp != null) 
            { 
                // 1. Guarda a porcentagem atual (Ex: 0.7)
                float currentPercentage = hp.currentHealth / hp.maxHealth;

                // 2. Aumenta o Maximo
                hp.maxHealth += skill.valueAmount; 

                // 3. Aplica a porcentagem no novo maximo (Ex: 60 * 0.7 = 42)
                hp.currentHealth = hp.maxHealth * currentPercentage;

                // 4. Atualiza a tela
                hp.UpdateHealthUI(); 
            }
            // ---------------------------------

            if (skill.statName == "Damage" && atk) atk.weaponDamage += skill.valueAmount;
            if (skill.statName == "MoveSpeed" && mov) mov.moveSpeed += skill.valueAmount;
            if (skill.statName == "FireRate" && atk) atk.fireRate -= skill.valueAmount;
        }
    }

    void CheckFusions()
    {
        foreach (var fusion in allFusions)
        {
            if (acquiredSkills.Contains(fusion.resultSkill)) continue;
            bool hasAll = true;
            foreach (var req in fusion.requiredSkills) if (!acquiredSkills.Contains(req)) hasAll = false;
            if (hasAll) AddSkill(fusion.resultSkill);
        }
    }
    // Coloque isso dentro do SkillManager.cs se ainda não tiver
    public int GetTotalSkillsCount()
    {
        return totalTreeSkillsCount;
    }
}