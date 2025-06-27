using UnityEngine;
using System.Collections; // ��� �������

public class MovingPlatform : MonoBehaviour
{
    // �����, ����� �������� ����� ��������� ���������
    public Vector2[] waypoints;
    // �������� �������� ���������
    public float speed = 2f;
    // �������� � �������� ������ (����� ��� ��� ��������� �������)
    public float waitTimeAtWaypoint = 1f;

    private int currentWaypointIndex = 0;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // ���� ��� �����, ��������� �� ����� ���������
        if (waypoints == null || waypoints.Length < 2)
        {
            Debug.LogWarning("MovingPlatform: ������� ���� �� 2 ����� ��� ��������.", this);
            enabled = false; // ��������� ������, ���� ����� ������������
            return;
        }

        // ��������, ��� ��������� ������� ��������� ������������� ������ ����� ��������
        // ���� �� ������, ����� ��������� �������� � ������� �������, ������ ������� ��� ������.
        //transform.position = waypoints[0]; // ����� ������������ ���, ���� ������, ����� ��������� ������ �������� � ������ �����
        // ������, ��� ���������� � ��������� �����, ����� ��������� ���������� ���, ��� �� � ���������.

        // ��������� �������� ��� ������ ��������
        StartCoroutine(MoveBetweenWaypoints());
    }

    IEnumerator MoveBetweenWaypoints()
    {
        while (true) // ����������� ���� ��� ����������� ��������
        {
            // �������� ������� ������� �����
            Vector2 targetPosition = waypoints[currentWaypointIndex];

            // ��������� � ������� �����
            while (Vector2.Distance(rb.position, targetPosition) > 0.05f) // ���������� ������ � �����
            {
                // ��������� ����� ���������
                Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.deltaTime);
                rb.MovePosition(newPosition);
                yield return null; // ���� ��������� ����
            }

            // ����������, ��� ��������� ����� � ������� �����, ����� �������� ���������� ������
            rb.position = targetPosition;

            // ���� � �������� �����
            yield return new WaitForSeconds(waitTimeAtWaypoint);

            // ��������� � ��������� �����
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // ����������� �������
        }
    }

    // ��� �����������: ����� ���������� ����� �������� � ��������� ��� ��������
    void OnDrawGizmos()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < waypoints.Length; i++)
            {
                Gizmos.DrawSphere(waypoints[i], 0.1f); // ������ ����� � ������ �����
                if (i < waypoints.Length - 1)
                {
                    Gizmos.DrawLine(waypoints[i], waypoints[i + 1]); // ������ ����� ����� �������
                }
            }
            // ������ ����� �� ��������� ����� � ������, ����� �������� ����
            if (waypoints.Length > 1)
            {
                Gizmos.DrawLine(waypoints[waypoints.Length - 1], waypoints[0]);
            }
        }
    }
}