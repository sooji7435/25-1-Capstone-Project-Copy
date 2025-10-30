using UnityEngine;
using UnityEngine.UI;

public class SkillSelect : MonoBehaviour
{
    public GameObject skillSelectWindow;
    private System.Action<int> onSkillSelected;

    public void ShowSkillWindow(System.Action<int> callback)
    {
        skillSelectWindow.SetActive(true);
        onSkillSelected = callback;
    }

    public void CloseSkillWindow()
    {
        skillSelectWindow.SetActive(false);
    }

    public void OnClickSkillButton(int index)
    {
        Debug.Log($"��ų ��ư �Է�: {index}");
        CloseSkillWindow();
        GameManager.Instance.SetTimeScale(1f);

        onSkillSelected?.Invoke(index);
        onSkillSelected = null;
    }
}
