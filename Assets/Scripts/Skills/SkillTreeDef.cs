using UnityEngine;

[CreateAssetMenu(fileName = "NovaArvore", menuName = "DarkRain/Tree Definition")]
public class SkillTreeDef : ScriptableObject
{
    public string treeName; // Ex: "O Avatar do Caos"
    
    [Header("Estrutura Diamante")]
    public SkillSO initialSkill;      
    public SkillSO secondarySkillA;   
    public SkillSO secondarySkillB;   
    public SkillSO finalSkill;        
}