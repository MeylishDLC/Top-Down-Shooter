

namespace _Support.GameAnalytics.Plugins.Scripts
{
    public interface IGameAnalyticsATTListener
    {
        void GameAnalyticsATTListenerNotDetermined();
        void GameAnalyticsATTListenerRestricted();
        void GameAnalyticsATTListenerDenied();
        void GameAnalyticsATTListenerAuthorized();
    }
}
