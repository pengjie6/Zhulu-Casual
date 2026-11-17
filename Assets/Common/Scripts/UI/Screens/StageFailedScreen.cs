using OctoberStudio.Audio;
using OctoberStudio.Easing;
using OctoberStudio.Input;
using OctoberStudio.Upgrades;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OctoberStudio.UI
{
    public class StageFailedScreen : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] Button reviveButton;
        [SerializeField] Button exitButton;

        [Header("Revive Ad")]
        [Tooltip("复活前是否必须先看一次插屏广告")]
        [SerializeField] bool requireAdToRevive = true;

        [Tooltip("广告拉不起来（未加载好 / 冷却中 / 平台异常）时是否仍然允许复活。默认关闭＝严格按「看完广告才能复活」")]
        [SerializeField] bool reviveIfAdUnavailable = false;

        private Canvas canvas;

        private bool revivedAlready = false;
        private bool waitingForAd = false;

        private void Awake()
        {
            canvas = GetComponent<Canvas>();

            reviveButton.onClick.AddListener(ReviveButtonClick);
            exitButton.onClick.AddListener(ExitButtonClick);

            revivedAlready = false;
            waitingForAd = false;
        }

        public void Show()
        {
            gameObject.SetActive(true);

            canvasGroup.alpha = 0;
            canvasGroup.DoAlpha(1, 0.3f).SetUnscaledTime(true);

            if (GameController.UpgradesManager.IsUpgradeAquired(UpgradeType.Revive) && !revivedAlready)
            {
                reviveButton.gameObject.SetActive(true);

                EventSystem.current.SetSelectedGameObject(reviveButton.gameObject);
            } else
            {
                reviveButton.gameObject.SetActive(false);
                EventSystem.current.SetSelectedGameObject(exitButton.gameObject);
            }

            GameController.InputManager.onInputChanged += OnInputChanged;
        }

        public void Hide(UnityAction onFinish)
        {
            canvasGroup.DoAlpha(0, 0.3f).SetUnscaledTime(true).SetOnFinish(() => {
                gameObject.SetActive(false);
                onFinish?.Invoke();
            });

            GameController.InputManager.onInputChanged -= OnInputChanged;
        }

        private void ReviveButtonClick()
        {
            if (waitingForAd || revivedAlready) return;

            GameController.AudioManager.PlaySound(AudioManager.BUTTON_CLICK_HASH);

            if (!requireAdToRevive)
            {
                Revive();
                return;
            }

#if UNITY_EDITOR
            // 编辑器里 TTSDK 是 mock，插屏永远拉不起来（也没有关闭回调），
            // 这里直接放行，保证本地能测通复活流程；真机上才会真正走广告。
            Debug.Log("[复活] 编辑器环境：插屏为 mock，跳过广告直接复活");
            Revive();
#else
            waitingForAd = true;
            if (reviveButton != null) reviveButton.interactable = false;

            var adManager = AdManager.Instance;
            if (adManager == null)
            {
                Debug.LogError("[复活] 场景里找不到 AdManager，插屏广告无法播放");
                OnReviveAdFailed("场景里找不到 AdManager");
                return;
            }

            Debug.Log("[复活] 播放插屏广告，广告关闭后才能复活");
            adManager.ShowInterstitialAd(OnReviveAdClosed, OnReviveAdFailed);
#endif
        }

        /// <summary>
        /// 插屏广告被关闭：视为“看完了”，执行复活。
        /// 插屏没有“是否看完”的概念，SDK 只提供 OnClose，所以关闭即放行。
        /// </summary>
        private void OnReviveAdClosed()
        {
            // 广告是异步的，期间玩家可能已经点了退出、界面随场景一起销毁
            if (this == null) return;

            waitingForAd = false;
            if (reviveButton != null) reviveButton.interactable = true;

            Revive();
        }

        /// <summary>
        /// 插屏没能展示出来（未加载完成 / 冷却中 / 平台异常）
        /// </summary>
        private void OnReviveAdFailed(string message)
        {
            if (this == null) return;

            waitingForAd = false;
            if (reviveButton != null) reviveButton.interactable = true;

            Debug.LogWarning($"[复活] 插屏广告没能展示：{message}");

            // 默认不放行：广告没看成就不给复活（玩家仍可点退出）
            if (reviveIfAdUnavailable)
            {
                Debug.LogWarning("[复活] 已开启「广告拉不起来也允许复活」，直接复活");
                Revive();
            }
        }

        private void Revive()
        {
            if (revivedAlready) return;

            revivedAlready = true;
            waitingForAd = false;
            Hide(StageController.ResurrectPlayer);
        }

        private void ExitButtonClick()
        {
            GameController.AudioManager.PlaySound(AudioManager.BUTTON_CLICK_HASH);
            Time.timeScale = 1;
            StageController.ReturnToMainMenu();

            GameController.InputManager.onInputChanged -= OnInputChanged;
        }

        private void OnInputChanged(InputType prevInput, InputType inputType)
        {
            if (prevInput == InputType.UIJoystick)
            {
                if (GameController.UpgradesManager.IsUpgradeAquired(UpgradeType.Revive) && !revivedAlready)
                {
                    EventSystem.current.SetSelectedGameObject(reviveButton.gameObject);
                }
                else
                {
                    EventSystem.current.SetSelectedGameObject(exitButton.gameObject);
                }
            }
        }
    }
}