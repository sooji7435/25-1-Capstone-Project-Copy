using UnityEngine;

public class SkillB_effect : MonoBehaviour
{
    // Ǯ������ ���� �ʿ�
    void OnEnable()
    {
        Invoke(nameof(Deactivate), 1f);
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
