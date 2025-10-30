using UnityEngine;

public class PlayerRoomTracker : MonoBehaviour
{
    public float roomSize = 10f; // ���� ���� �ʿ��� �� �ϳ� ũ�� (��: 10 ����)
    private Vector2Int currentRoomPos = Vector2Int.zero;

    void Start()
    {
        // ���� ���� �� ���۹� ǥ��
        MinimapManager.Instance.RevealRoom(currentRoomPos);
        MinimapManager.Instance.HighlightRoom(currentRoomPos);
    }

    void Update()
    {
        Vector2 playerPos = transform.position;

        // �÷��̾� ��ġ�� �� ������ ��ȯ
        Vector2Int roomPos = new Vector2Int(
            Mathf.RoundToInt(playerPos.x / roomSize),
            Mathf.RoundToInt(playerPos.y / roomSize)
        );

        // ���� �ٲ���� ���� ����
        if (roomPos != currentRoomPos)
        {
            currentRoomPos = roomPos;

            MinimapManager.Instance.RevealRoom(currentRoomPos);
            MinimapManager.Instance.HighlightRoom(currentRoomPos);
        }
    }
}
