using SeekiosApp.Model.APP.OneSignal;
using SeekiosApp.Services;
using System.Collections.Generic;

namespace SeekiosApp.Interfaces
{
    public interface IOneSignal
    {
        void SetLogLevel(LogLevelEnum logLevel, LogLevelEnum visualLevel);

        void Init();

        OneSignalBuilder StartInit(string appID, string googleProjectNumber = "");

        void RegisterForPushNotifications();

        void SendTag(string tagName, string tagValue);

        void SendTags(IDictionary<string, string> tags);

        void GetTags();

        void DeleteTag(string key);

        void DeleteTags(IList<string> keys);

        void IdsAvailable();

        void IdsAvailable(OneSignalBuilder.IdsAvailableCallback inIdsAvailableDelegate);

        void SetSubscription(bool enable);

        void PostNotification(Dictionary<string, object> data);

        void SyncHashedEmail(string email);

        void PromptLocation();
    }
}
