//using Android.Content;
//using Android.App;
//using System.Collections.Generic;
//using SeekiosApp.Model.DTO;
//using SeekiosApp.Constants;
//using SeekiosApp.Enum;
//using Newtonsoft.Json;
//using System.Linq;
//using System;
//using Android.Util;
//using System.Threading;

//namespace SeekiosApp.Droid.Services
//{
//    public class PowerOnBroadcastReceiver : BroadcastReceiver
//    {
//        public async override void OnReceive(Context context, Intent intent)
//        {
//            var lsSeekiosInDontMove = new List<SeekiosDTO>();

//            //Get saved data 
//            var _saveDataService = new SaveDataService();
//            _saveDataService.Init(context.ApplicationContext);

//            if (!_saveDataService.Contains(SaveDataConstants.UserEnvironment)) return;

//            var json = _saveDataService.GetData(SaveDataConstants.UserEnvironment);
//            if (string.IsNullOrEmpty(json)) return;

//            var userEnvironment = JsonConvert.DeserializeObject<UserEnvironmentDTO>(json);
//            if (userEnvironment == null) return;

//            //Check if there is any seekios in don't move mode
//            //lsSeekiosInDontMove = userEnvironment.LsSeekios.Where(
//            //    el => userEnvironment.LsMode.FirstOrDefault(m => m.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove && m.Seekios_idseekios == el.Idseekios) != null).ToList();
//            //if (lsSeekiosInDontMove.Count <= 0) return;

//            //If there is at least one seekios in don't move mode, we start the DontMoveBackgroundService
//            //Application.Context.StartService(new Intent(Application.Context, typeof(DontMoveBackgroundService)));
//            //context.StartService(new Intent(context, typeof(OneSignalNotificationService)));
//        }
//    }
//}