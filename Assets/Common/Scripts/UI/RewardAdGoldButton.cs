using OctoberStudio.Audio;
using OctoberStudio.Currency;
using UnityEngine;
using UnityEngine.UI;

namespace OctoberStudio.UI
{
    /// <summary>
    /// 大厅「看广告领奖励」按钮（jiliBtn）。
    /// 点击后弹出激励视频确认框，用户看完广告后发放金币到持久化货币 "gold"，
    /// 大厅的金币显示（Gold Indicator）会通过 onGoldAmountChanged 自动刷新。
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class RewardAdGoldButton : MonoBehaviour
    {
        [Tooltip("看完激励视频后发放的金币数量")]
        [SerializeField] int rewardAmount = 1000;

        [Tooltip("货币存档的 key，需要和 Currencies Database 中的一致")]
        [SerializeField] string currencySaveId = "gold";

        private Button button;

        private void Awake()
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

        /// <summary>
        /// 点击入口：弹出激励视频确认框，确认并看完广告后回调 GrantReward。
        /// 编辑器下没有真实广告，AdConfirm 会直接走成功回调，方便测试。
        /// </summary>
        public void OnClick()
        {
            if (GameController.AudioManager != null)
            {
                GameController.AudioManager.PlaySound(AudioManager.BUTTON_CLICK_HASH);
            }

            AdConfirm.Show(GrantReward);
        }

        private void GrantReward()
        {
            var currency = GetCurrency();

            if (currency == null)
            {
                Debug.LogError($"[广告] 找不到货币存档 \"{currencySaveId}\"，金币发放失败");
                return;
            }

            currency.Deposit(rewardAmount);

            if (GameController.SaveManager != null)
            {
                GameController.SaveManager.Save();
            }

            Debug.Log($"[广告] 激励视频观看完成，发放 {rewardAmount} 金币，当前余额 {currency.Amount}");
        }

        private CurrencySave GetCurrency()
        {
            if (GameController.SaveManager != null)
            {
                var save = GameController.SaveManager.GetSave<CurrencySave>(currencySaveId);

                if (save != null)
                {
                    return save;
                }
            }

            return GameController.Gold;
        }
    }
}
