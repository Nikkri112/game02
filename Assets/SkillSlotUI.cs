// Это тот же SkillSlotUI.cs, который был представлен в предыдущем ответе.
// Прикрепите его к каждому из 4 слотов в SelectedSkillsContainer.
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Если используете TextMeshPro

public class SkillSlotUI : MonoBehaviour
{
    public int slotNumber; // Установить в инспекторе (1, 2, 3, 4)
    public Image skillIcon;
    public Image cooldownOverlay; // Изображение, которое будет заполняться/опустошаться
    public TextMeshProUGUI cooldownText; // Для отображения времени перезарядки
    public TextMeshProUGUI slotNumberText; // Для отображения номера слота

    private Skill assignedSkill;

    void Awake()
    {
        // Добавьте прослушиватель событий для кнопки слота
        Button slotButton = GetComponent<Button>();
        if (slotButton != null)
        {
            slotButton.onClick.AddListener(() => FindObjectOfType<SkillSelectionUI>()?.OnSelectedSlotClicked(slotNumber));
            // Или более безопасно через ссылку, если SkillSelectionUI находится в другом месте
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
