using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Objects/Skill")]
public class Skill : ScriptableObject
{
    public string skillName = "New Skill";
    public Sprite icon; // ������ ����������� ��� UI
    public float cooldown = 5f; // ����������� �����������
    public float manaCost = 0f; // ��������� ���� (���� ���������)
    [TextArea(3, 5)]
    public string description = "Skill description.";

    // ����� ��� ��������� �����������. ����� ������������� ��� ������ ����� ������������.
    public virtual void Activate(GameObject user)
    {
        Debug.Log($"������������ �����������: {skillName}");
        // ����� ����� ��� ��� ���������� ������� �����������
    }
}
