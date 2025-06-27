using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // ��� TextMeshPro

public class SkillSelectionUI : MonoBehaviour
{
    public GameObject skillButtonPrefab; // ������ ������ ��� ��������� �����������
    public Transform availableSkillsParent; // ������������ ������ ��� ������ ��������� ������������

    public List<SkillSlotUI> selectedSkillSlots; // ������ �������� ��� ������ 1-4

    private Skill selectedSkillToAssign; // �����������, ������� �� ����� ���������
    private int currentTargetSlot = -1; // ����, ������� �� ����� ���������

    void Start()
    {
        PopulateAvailableSkills();
        UpdateSelectedSkillSlots(); // �������� ����� ��� ������
    }

    // ��������� ������ ��������� ������������
    void PopulateAvailableSkills()
    {
        foreach (Transform child in availableSkillsParent)
        {
            Destroy(child.gameObject); // ������� ������ ������
        }

        foreach (Skill skill in SkillManager.Instance.allAvailableSkills)
        {
            GameObject skillBtnGO = Instantiate(skillButtonPrefab, availableSkillsParent);
            SkillButtonUI skillButtonUI = skillBtnGO.GetComponent<SkillButtonUI>();
            Debug.Log("������� �����");
            if (skillButtonUI != null)
            {
                skillButtonUI.SetSkill(skill);
                skillButtonUI.button.onClick.AddListener(() => OnAvailableSkillClicked(skill));
            }
        }
    }

    // ���������� ��� ������� �� ��������� �����������
    public void OnAvailableSkillClicked(Skill skill)
    {
        selectedSkillToAssign = skill;
        Debug.Log($"������� �����������: {skill.skillName}. ������ �������� ����.");
        // ����� ����� ���������� ����� ��� ������
    }

    // ���������� ��� ������� �� ���� ��������� �����������
    public void OnSelectedSlotClicked(int slotNumber)
    {
        if (selectedSkillToAssign != null)
        {
            // ���� ������� ����������� �� ������, ��������� ��
            SkillManager.Instance.SelectSkill(selectedSkillToAssign, slotNumber);
            selectedSkillToAssign = null; // ���������� ��������� �����������
            UpdateSelectedSkillSlots();
        }
        else
        {
            // ���� ����������� �� �������, ��������, ����� �������� ���� ��� �������� ����������
            Debug.Log($"���� �� ����� {slotNumber}. ������� �����������: {SkillManager.Instance.GetSkillInSlot(slotNumber)?.skillName ?? "�����"}");
            // ����� �������� ������ ��� ������� �����, ��������, ��� ����� ������ �������
            // SkillManager.Instance.DeselectSkill(slotNumber);
            // UpdateSelectedSkillSlots();
        }
    }

    // ��������� UI ��� ���� ��������� ������
    public void UpdateSelectedSkillSlots()
    {
        foreach (var slotUI in selectedSkillSlots)
        {
            slotUI.UpdateUI();
        }
    }
}