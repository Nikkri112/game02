using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Objects/Skill")]
public class Skill : ScriptableObject
{
    public string skillName = "New Skill";
    public Sprite icon; // Иконка способности для UI
    public float cooldown = 5f; // Перезарядка способности
    public float manaCost = 0f; // Стоимость маны (если применимо)
    [TextArea(3, 5)]
    public string description = "Skill description.";

    // Метод для активации способности. Будет переопределен для разных типов способностей.
    public virtual void Activate(GameObject user)
    {
        Debug.Log($"Активирована способность: {skillName}");
        // Здесь будет код для выполнения эффекта способности
    }
}
