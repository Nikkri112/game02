using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // Для TextMeshPro

public class SkillSelectionUI : MonoBehaviour
{
    public GameObject skillButtonPrefab; // Префаб кнопки для доступной способности
    public Transform availableSkillsParent; // Родительский объект для кнопок доступных способностей

    public List<SkillSlotUI> selectedSkillSlots; // Список скриптов для слотов 1-4

    private Skill selectedSkillToAssign; // Способность, которую мы хотим назначить
    private int currentTargetSlot = -1; // Слот, который мы хотим заполнить

    void Start()
    {
        PopulateAvailableSkills();
        UpdateSelectedSkillSlots(); // Обновить слоты при старте
    }

    // Заполняет список доступных способностей
    void PopulateAvailableSkills()
    {
        foreach (Transform child in availableSkillsParent)
        {
            Destroy(child.gameObject); // Очищаем старые кнопки
        }

        foreach (Skill skill in SkillManager.Instance.allAvailableSkills)
        {
            GameObject skillBtnGO = Instantiate(skillButtonPrefab, availableSkillsParent);
            SkillButtonUI skillButtonUI = skillBtnGO.GetComponent<SkillButtonUI>();
            Debug.Log("Добавил Скилл");
            if (skillButtonUI != null)
            {
                skillButtonUI.SetSkill(skill);
                skillButtonUI.button.onClick.AddListener(() => OnAvailableSkillClicked(skill));
            }
        }
    }

    // Вызывается при нажатии на доступную способность
    public void OnAvailableSkillClicked(Skill skill)
    {
        selectedSkillToAssign = skill;
        Debug.Log($"Выбрана способность: {skill.skillName}. Теперь выберите слот.");
        // Здесь можно подсветить слоты для выбора
    }

    // Вызывается при нажатии на слот выбранной способности
    public void OnSelectedSlotClicked(int slotNumber)
    {
        if (selectedSkillToAssign != null)
        {
            // Если выбрана способность из списка, назначаем ее
            SkillManager.Instance.SelectSkill(selectedSkillToAssign, slotNumber);
            selectedSkillToAssign = null; // Сбрасываем выбранную способность
            UpdateSelectedSkillSlots();
        }
        else
        {
            // Если способность не выбрана, возможно, хотим очистить слот или получить информацию
            Debug.Log($"Клик по слоту {slotNumber}. Текущая способность: {SkillManager.Instance.GetSkillInSlot(slotNumber)?.skillName ?? "Пусто"}");
            // Можно добавить логику для очистки слота, например, при клике правой кнопкой
            // SkillManager.Instance.DeselectSkill(slotNumber);
            // UpdateSelectedSkillSlots();
        }
    }

    // Обновляет UI для всех выбранных слотов
    public void UpdateSelectedSkillSlots()
    {
        foreach (var slotUI in selectedSkillSlots)
        {
            slotUI.UpdateUI();
        }
    }
}