using System;

public abstract class IAdPlatform
{
    /* 调起侧边栏 */
    public abstract void NavigateToScene(Action onSuccess, Action<string> onFail);

    /* 激励视频广告 */
    public abstract void ShowRewardedVideoAd(string adId, Action onSuccess, Action<string> onFail);

    /* 插屏广告 */
    public abstract void ShowInterstitialAd(string adId);

    /* 插屏广告（带回调）：onClose 在广告被关闭后触发，onFail 表示这次没能展示出来 */
    public virtual void ShowInterstitialAd(string adId, Action onClose, Action<string> onFail)
    {
        ShowInterstitialAd(adId);

        onClose?.Invoke();
    }

    /** 游戏分享 */
    public abstract void ShareGame(Action<bool> callBack);
}