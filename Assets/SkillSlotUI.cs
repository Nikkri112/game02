// ��� ��� �� SkillSlotUI.cs, ������� ��� ����������� � ���������� ������.
// ���������� ��� � ������� �� 4 ������ � SelectedSkillsContainer.
using UnityEngine;
using UnityEngine.UI;
using TMPro; // ���� ����������� TextMeshPro

public class SkillSlotUI : MonoBehaviour
{
    public int slotNumber; // ���������� � ���������� (1, 2, 3, 4)
    public Image skillIcon;
    public Image cooldownOverlay; // �����������, ������� ����� �����������/������������
    public TextMeshProUGUI cooldownText; // ��� ����������� ������� �����������
    public TextMeshProUGUI slotNumberText; // ��� ����������� ������ �����

    private Skill assignedSkill;

    void Awake()
    {
        // �������� �������������� ������� ��� ������ �����
        Button slotButton = GetComponent<Button>();
        if (slotButton != null)
        {
            slotButton.onClick.AddListener(() => FindObjectOfType<SkillSelectionUI>()?.OnSelectedSlotClicked(slotNumber));
            // ��� ����� ��������� ����� ������, ���� SkillSelectionUI ��������� � ������ �����
        }
    }

    void Start()
    {
        if (slotNumberText != null) slotNumberText.text = slotNumber.ToString();
        UpdateUI();
    }

    void Update()
    {
        UpdateCooldownUI();
    }

    public void UpdateUI()
    {
        assignedSkill = SkillManager.Instance.GetSkillInSlot(slotNumber);

        if (assignedSkill != null)
        {
            if (skillIcon != null)
            {
                skillIcon.sprite = assignedSkill.icon;
                skillIcon.enabled = true;
            }
        }
        else
        {
            if (skillIcon != null) skillIcon.enabled = false;
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0;
            if (cooldownText != null) cooldownText.text = "";
        }
    }

    private void UpdateCooldownUI()
    {
        if (assignedSkill != null)
        {
            float currentCooldown = SkillManager.Instance.GetSkillCooldown(assignedSkill);
            float maxCooldown = assignedSkill.cooldown;

            if (currentCooldown > 0)
            {
                if (cooldownOverlay != null) cooldownOverlay.fillAmount = currentCooldown / maxCooldown;
                if (cooldownText != null) cooldownText.text = currentCooldown.ToString("F1");
            }
            else
            {
                if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0;
                if (cooldownText != null) cooldownText.text = "";
            }
        }
        else
        {
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0;
            if (cooldownText != null) cooldownText.text = "";
        }
    }
}
