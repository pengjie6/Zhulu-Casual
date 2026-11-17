/*
using HyperWe.CurrencyHolder;
using njector;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

/// <summary>
/// HUD 上的「免费金币」按钮：点击弹出广告确认框，看完抖音激励视频发放金币。
/// </summary>
[RequireComponent(typeof(AutoInjectTarget))]
[RequireComponent(typeof(Button))]
public class FreeCoinButton : MonoBehaviour
{
    [SerializeField]
    private int rewardAmount = 200;

    private CurrencyHolder wallet;
    private Button button;

    [Preserve]
    public void Inject(CurrencyHolder currencyHolder)
    {
        this.wallet = currencyHolder;
    }

    private void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnClick);
        }
    }

    public void OnClick()
    {
        Debug.Log("[广告] 点击免费金币按钮，弹出激励视频确认框");
        AdConfirm.Show(GrantReward);
    }

    private void GrantReward()
    {
        if (wallet == null)
        {
            Debug.LogError("[广告] 金币系统未注入，无法发放奖励");
            return;
        }

        wallet.AddCurrency(rewardAmount);
        Debug.Log($"[广告] 激励视频完成，发放金币 {rewardAmount}");
    }
}
*/