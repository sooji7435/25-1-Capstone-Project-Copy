using UnityEngine;


public class UIManager : Singleton<UIManager>
{
    [Header("UI 오브젝트")]

    [SerializeField] GameObject InGameUI;
    [SerializeField] GameObject MainMenuUI;



    [Header("인게임")]
    public PlayerStatUI playerStatUI;
    public ParryStackUI parryStackUI;
    public SkillUI skillUI;
    public SkillSelect skillSelect;

    public AbilityRewardSystem abilityUI;
    public UI_BossUI bossUI;
    public UI_DeadInfo deadInfo;
    public PauseUI pauseUI;
    //public Pause pauseUI;
    protected override void Awake()
    {
        base.Awake();
        InGameUI.SetActive(false);
        MainMenuUI.SetActive(false);
    }
    public void SetActiveInGameUI(bool active)
    {
        InGameUI.SetActive(active);
    }
    public void SetActiveMainMenuUI(bool active)
    {
        MainMenuUI.SetActive(active);
    }


}
