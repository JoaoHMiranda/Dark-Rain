using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NovaFusao", menuName = "DarkRain/Fusion")]
public class FusionDefinition : ScriptableObject
{
    public string fusionName;       // Ex: "Choque Térmico"
    public SkillSO resultSkill;     // A habilidade que o player ganha
    public List<SkillSO> requiredSkills; // As habilidades necessárias para desbloquear
}