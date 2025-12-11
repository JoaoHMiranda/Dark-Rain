using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCard : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text titleText;
    public TMP_Text descText;
    public Image iconImage;

    private SkillSO mySkill;
    private LevelUpManager manager;

    public void Setup(SkillSO skill, LevelUpManager lvlManager)
    {
        mySkill = skill;
        manager = lvlManager;

        if(titleText) titleText.text = skill.skillName;
        if(descText) descText.text = skill.description;
        
        if(iconImage)
        {
             if(skill.icon != null) iconImage.sprite = skill.icon;
             else iconImage.color = Color.gray;
        }
    }

    public void OnClick()
    {
        manager.SelectSkill(mySkill);
    }
}