//using System;
//using SeekiosApp.Interfaces;
//using SeekiosApp.Model.DTO;
//using Microsoft.Practices.ServiceLocation;

//namespace SeekiosApp.Droid.Services
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class FollowMeService : IFollowMeService, IDisposable
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        public event EventHandler<ConnexionStateChangedEventArgs> ConnexionStateChanged;

//        /// <summary>
//        /// 
//        /// </summary>
//        public static Action StartFollowMeBackgroundService { get; set; }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <returns></returns>
//        public bool TryToStartFollowMe(SeekiosDTO seekios)
//        {
//            if (StartFollowMeBackgroundService == null) return false;
            
//            FollowMeBackgroundService.ConnexionStateChanged += ConnexionStateChanged;
//            FollowMeBackgroundService.DataService = ServiceLocator.Current.GetInstance<IDataService>();
//            FollowMeBackgroundService.Seekios = seekios;

//            StartFollowMeBackgroundService.Invoke();

//            return true;
//        }

//        public void Dispose()
//        {
//            FollowMeBackgroundService.ConnexionStateChanged -= ConnexionStateChanged;
//        }
//    }
//}