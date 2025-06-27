using UnityEngine;
using System.Collections.Generic;
using System.Linq; // ��� ������������� LINQ

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance; // Singleton ��� ������� �������

    public GameObject availableSkills;

    [Header("��� ��������� �����������")]
    public List<Skill> allAvailableSkills = new List<Skill>();

    [Header("��������� �����������")]
    // ������� ��� �������� ��������� ������������ � �� ������� �����������
    public Dictionary<int, Skill> selectedSkills = new Dictionary<int, Skill>();
    private Dictionary<Skill, float> skillCooldowns = new Dictionary<Skill, float>();

    [Header("�������� ������")]
    public KeyCode[] skillKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };

    [Tooltip("������, ������� ���������� ����������� (������ �����)")]
    public GameObject userGameObject;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ��������� �������� ����� �������
        }
        else
        {
            Destroy(gameObject);
        }

        // ������������� �������� �����������
        foreach (var skill in allAvailableSkills)
        {
            skillCooldowns.Add(skill, 0f);
        }
    }

    void Update()
    {
        // ������������ �����������
        List<Skill> skillsToUpdate = new List<Skill>(skillCooldowns.Keys);
        foreach (var skill in skillsToUpdate)
        {
            if (skillCooldowns[skill] > 0)
            {
                skillCooldowns[skill] -= Time.deltaTime;
            }
        }

        // ��������� ������������ �� ������� ������
        for (int i = 0; i < skillKeys.Length; i++)
        {
            if (Input.GetKeyDown(skillKeys[i]))
            {
                ActivateSelectedSkill(i + 1); // +1, ������ ��� ����� 1, 2, 3, 4
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            availableSkills.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            availableSkills.SetActive(false);
        }

    }

    /// <summary>
    /// ���������� ��������� ����������� �� ������ ����� (1, 2, 3, 4).
    /// </summary>
    /// <param name="slotNumber">����� ����� ����������� (1-4).</param>
    public void ActivateSelectedSkill(int slotNumber)
    {
        if (selectedSkills.ContainsKey(slotNumber))
        {
            Skill skillToActivate = selectedSkills[slotNumber];

            if (skillCooldowns[skillToActivate] <= 0)
            {
                // �������� ���� (���� ����)
                // if (PlayerStats.Instance.CurrentMana >= skillToActivate.manaCost)
                // {
                //     PlayerStats.Instance.SpendMana(skillToActivate.manaCost);
                //     skillToActivate.Activate(userGameObject);
                //     skillCooldowns[skillToActivate] = skillToActivate.cooldown;
                // }
                // else
                // {
                //     Debug.Log($"������������ ���� ��� ������������� {skillToActivate.skillName}!");
                // }

                // �������� ��� �������� ����
                skillToActivate.Activate(userGameObject);
                skillCooldowns[skillToActivate] = skillToActivate.cooldown;
            }
            else
            {
                Debug.Log($"����������� {skillToActivate.skillName} ��������� �� �����������. ��������: {skillCooldowns[skillToActivate]:F2} ���.");
            }
        }
        else
        {
            Debug.Log($"���� {slotNumber} ����.");
        }
    }

    /// <summary>
    /// ��������� ��� �������� ����������� � ��������� �����.
    /// </summary>
    /// <param name="skill">����������� ��� ����������.</param>
    /// <param name="slotNumber">����, � ������� ����� ��������� ����������� (1-4).</param>
    public void SelectSkill(Skill skill, int slotNumber)
    {
        if (slotNumber >= 1 && slotNumber <= 4)
        {
            if (selectedSkills.ContainsKey(slotNumber))
            {
                Debug.Log($"�������� ����������� � ����� {slotNumber}: {selectedSkills[slotNumber].skillName} �� {skill.skillName}");
                selectedSkills[slotNumber] = skill;
            }
            else
            {
                Debug.Log($"��������� ����������� {skill.skillName} � ���� {slotNumber}");
                selectedSkills.Add(slotNumber, skill);
            }
        }
        else
        {
            Debug.LogError($"�������� ����� �����: {slotNumber}. ������ ���� �� 1 �� 4.");
        }
    }

    /// <summary>
    /// ������� ����������� �� ���������� �����.
    /// </summary>
    /// <param name="slotNumber">����, �� �������� ����� ������ ����������� (1-4).</param>
    public void DeselectSkill(int slotNumber)
    {
        if (selectedSkills.ContainsKey(slotNumber))
        {
            Debug.Log($"������� ����������� �� ����� {slotNumber}: {selectedSkills[slotNumber].skillName}");
            selectedSkills.Remove(slotNumber);
        }
    }

    /// <summary>
    /// �������� ������� ����������� ��� �����������.
    /// </summary>
    /// <param name="skill">�����������, ��� ������� ����� �������� �����������.</param>
    /// <returns>���������� ����� �����������.</returns>
    public float GetSkillCooldown(Skill skill)
    {
        if (skillCooldowns.ContainsKey(skill))
        {
            return skillCooldowns[skill];
        }
        return 0f;
    }

    /// <summary>
    /// �������� ������� ����������� ��� ����������� � ��������� �����.
    /// </summary>
    /// <param name="slotNumber">����� �����.</param>
    /// <returns>���������� ����� �����������. ���� ����� ��� ��� �� ����, ���������� 0.</returns>
    public float GetSelectedSkillCooldown(int slotNumber)
    {
        if (selectedSkills.ContainsKey(slotNumber))
        {
            return GetSkillCooldown(selectedSkills[slotNumber]);
        }
        return 0f;
    }

    /// <summary>
    /// �������� ����������� �� ���������� �����.
    /// </summary>
    /// <param name="slotNumber">����� �����.</param>
    /// <returns>������ Skill ��� null, ���� ���� ����.</returns>
    public Skill GetSkillInSlot(int slotNumber)
    {
        if (selectedSkills.ContainsKey(slotNumber))
        {
            return selectedSkills[slotNumber];
        }
        return null;
    }
}
