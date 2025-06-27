using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButtonUI : MonoBehaviour
{
    public Button button;
    public Image skillIcon;
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillDescriptionText; // Опционально

    private Skill assignedSkill;

    void Awake()
    {
        if (button == null) button = GetComponent<Button>();
    }

    public void SetSkill(Skill skill)
    {
        assignedSkill = skill;
        if (skillIcon != null && skill.icon != null) skillIcon.sprite = skill.icon;
        if (skillNameText != null) skillNameText.text = skill.skillName;
        if (skillDescriptionText != null) skillDescriptionText.text = skill.description;
    }

    public Skill GetAssignedSkill()
    {
        return assignedSkill;
    }
}
