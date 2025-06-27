using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Для использования LINQ

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance; // Singleton для легкого доступа

    public GameObject availableSkills;

    [Header("Все доступные способности")]
    public List<Skill> allAvailableSkills = new List<Skill>();

    [Header("Выбранные способности")]
    // Словарь для хранения выбранных способностей и их текущей перезарядки
    public Dictionary<int, Skill> selectedSkills = new Dictionary<int, Skill>();
    private Dictionary<Skill, float> skillCooldowns = new Dictionary<Skill, float>();

    [Header("Привязки клавиш")]
    public KeyCode[] skillKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };

    [Tooltip("Объект, который использует способности (обычно игрок)")]
    public GameObject userGameObject;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Сохраняем менеджер между сценами
        }
        else
        {
            Destroy(gameObject);
        }

        // Инициализация словарей перезарядки
        foreach (var skill in allAvailableSkills)
        {
            skillCooldowns.Add(skill, 0f);
        }
    }

    void Update()
    {
        // Отслеживание перезарядки
        List<Skill> skillsToUpdate = new List<Skill>(skillCooldowns.Keys);
        foreach (var skill in skillsToUpdate)
        {
            if (skillCooldowns[skill] > 0)
            {
                skillCooldowns[skill] -= Time.deltaTime;
            }
        }

        // Активация способностей по нажатию клавиш
        for (int i = 0; i < skillKeys.Length; i++)
        {
            if (Input.GetKeyDown(skillKeys[i]))
            {
                ActivateSelectedSkill(i + 1); // +1, потому что слоты 1, 2, 3, 4
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
    /// Активирует выбранную способность по номеру слота (1, 2, 3, 4).
    /// </summary>
    /// <param name="slotNumber">Номер слота способности (1-4).</param>
    public void ActivateSelectedSkill(int slotNumber)
    {
        if (selectedSkills.ContainsKey(slotNumber))
        {
            Skill skillToActivate = selectedSkills[slotNumber];

            if (skillCooldowns[skillToActivate] <= 0)
            {
                // Проверка маны (если есть)
                // if (PlayerStats.Instance.CurrentMana >= skillToActivate.manaCost)
                // {
                //     PlayerStats.Instance.SpendMana(skillToActivate.manaCost);
                //     skillToActivate.Activate(userGameObject);
                //     skillCooldowns[skillToActivate] = skillToActivate.cooldown;
                // }
                // else
                // {
                //     Debug.Log($"Недостаточно маны для использования {skillToActivate.skillName}!");
                // }

                // Временно без проверки маны
                skillToActivate.Activate(userGameObject);
                skillCooldowns[skillToActivate] = skillToActivate.cooldown;
            }
            else
            {
                Debug.Log($"Способность {skillToActivate.skillName} находится на перезарядке. Осталось: {skillCooldowns[skillToActivate]:F2} сек.");
            }
        }
        else
        {
            Debug.Log($"Слот {slotNumber} пуст.");
        }
    }

    /// <summary>
    /// Добавляет или заменяет способность в выбранном слоте.
    /// </summary>
    /// <param name="skill">Способность для добавления.</param>
    /// <param name="slotNumber">Слот, в который нужно поместить способность (1-4).</param>
    public void SelectSkill(Skill skill, int slotNumber)
    {
        if (slotNumber >= 1 && slotNumber <= 4)
        {
            if (selectedSkills.ContainsKey(slotNumber))
            {
                Debug.Log($"Заменяем способность в слоте {slotNumber}: {selectedSkills[slotNumber].skillName} на {skill.skillName}");
                selectedSkills[slotNumber] = skill;
            }
            else
            {
                Debug.Log($"Добавляем способность {skill.skillName} в слот {slotNumber}");
                selectedSkills.Add(slotNumber, skill);
            }
        }
        else
        {
            Debug.LogError($"Неверный номер слота: {slotNumber}. Должен быть от 1 до 4.");
        }
    }

    /// <summary>
    /// Убирает способность из выбранного слота.
    /// </summary>
    /// <param name="slotNumber">Слот, из которого нужно убрать способность (1-4).</param>
    public void DeselectSkill(int slotNumber)
    {
        if (selectedSkills.ContainsKey(slotNumber))
        {
            Debug.Log($"Убираем способность из слота {slotNumber}: {selectedSkills[slotNumber].skillName}");
            selectedSkills.Remove(slotNumber);
        }
    }

    /// <summary>
    /// Получить текущую перезарядку для способности.
    /// </summary>
    /// <param name="skill">Способность, для которой нужно получить перезарядку.</param>
    /// <returns>Оставшееся время перезарядки.</returns>
    public float GetSkillCooldown(Skill skill)
    {
        if (skillCooldowns.ContainsKey(skill))
        {
            return skillCooldowns[skill];
        }
        return 0f;
    }

    /// <summary>
    /// Получить текущую перезарядку для способности в выбранном слоте.
    /// </summary>
    /// <param name="slotNumber">Номер слота.</param>
    /// <returns>Оставшееся время перезарядки. Если слота нет или он пуст, возвращает 0.</returns>
    public float GetSelectedSkillCooldown(int slotNumber)
    {
        if (selectedSkills.ContainsKey(slotNumber))
        {
            return GetSkillCooldown(selectedSkills[slotNumber]);
        }
        return 0f;
    }

    /// <summary>
    /// Получить способность из выбранного слота.
    /// </summary>
    /// <param name="slotNumber">Номер слота.</param>
    /// <returns>Объект Skill или null, если слот пуст.</returns>
    public Skill GetSkillInSlot(int slotNumber)
    {
        if (selectedSkills.ContainsKey(slotNumber))
        {
            return selectedSkills[slotNumber];
        }
        return null;
    }
}
