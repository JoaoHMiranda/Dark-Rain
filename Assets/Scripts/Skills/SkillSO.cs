using UnityEngine;

public enum SkillType
{
    StatBoost,      // Aumenta números (Vida, Dano, Velocidade)
    SpecialEffect   // Efeitos complexos (Drones, Explosões, Caos)
}

[CreateAssetMenu(fileName = "NovaHabilidade", menuName = "DarkRain/Skill")]
public class SkillSO : ScriptableObject
{
    [Header("Visual")]
    public string skillName;
    [TextArea] public string description;
    public Sprite icon; 

    [Header("Configuração")]
    public SkillType type;
    
    // Se for StatBoost, usamos estes campos:
    public string statName;   // Ex: "MaxHealth", "Damage", "MoveSpeed", "FireRate"
    public float valueAmount; // Ex: 10, 0.5, 20
}