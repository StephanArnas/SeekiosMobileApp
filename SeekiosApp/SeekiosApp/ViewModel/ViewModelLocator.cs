using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using SeekiosApp.Services;
using SeekiosApp.Interfaces;
using Microsoft.AspNet.SignalR.Client;
using System;

namespace SeekiosApp.ViewModel
{
    public class ViewModelLocator
    {
        #region ===== Attributs ===================================================================

        private static bool _isInitilialized = false;

        #endregion

        #region ===== Constructor =================================================================

        public ViewModelLocator()
        {
            if (_isInitilialized) return;
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            Initialize();
        }

        public static void Initialize()
        {
            SimpleIoc.Default.Register<IDataService, DataService>(true);
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<LeftMenuViewModel>();
            SimpleIoc.Default.Register<AddSeekiosViewModel>();
            SimpleIoc.Default.Register<ListSeekiosViewModel>();
            SimpleIoc.Default.Register<DetailSeekiosViewModel>();
            SimpleIoc.Default.Register<AlertViewModel>();
            SimpleIoc.Default.Register<AlertSOSViewModel>();
            SimpleIoc.Default.Register<ListAlertsViewModel>();
            SimpleIoc.Default.Register<ModeZoneViewModel>();
            SimpleIoc.Default.Register<ModeDontMoveViewModel>();
            SimpleIoc.Default.Register<ModeTrackingViewModel>();
            SimpleIoc.Default.Register<ModeSelectionViewModel>();
            SimpleIoc.Default.Register<MapViewModel>();
            SimpleIoc.Default.Register<HistoricViewModel>();
            SimpleIoc.Default.Register<MapAllSeekiosViewModel>();
            SimpleIoc.Default.Register<MapViewModelBase>();
            SimpleIoc.Default.Register<ReloadCreditViewModel>();
            SimpleIoc.Default.Register<TransactionHistoricViewModel>();
            SimpleIoc.Default.Register<ParameterViewModel>();
            SimpleIoc.Default.Register<CreditsViewModel>();

            // community
            //SimpleIoc.Default.Register<TabListFriendsViewModel>();
            //SimpleIoc.Default.Register<AddFriendViewModel>();
            //SimpleIoc.Default.Register<TabRequestsViewModel>();
            //SimpleIoc.Default.Register<ListSharingsViewModel>();
            //SimpleIoc.Default.Register<ShareSeekiosViewModel>();

            _isInitilialized = true;
        }

        #endregion

        #region ===== Properties =================================================================

        public LoginViewModel Login
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoginViewModel>();
            }
        }

        public LeftMenuViewModel LeftMenu
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LeftMenuViewModel>();
            }
        }

        public AddSeekiosViewModel AddSeekios
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AddSeekiosViewModel>();
            }
        }

        public ListSeekiosViewModel ListSeekios
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ListSeekiosViewModel>();
            }
        }

        public DetailSeekiosViewModel DetailSeekios
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DetailSeekiosViewModel>();
            }
        }

        public AlertViewModel Alert
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AlertViewModel>();
            }
        }

        public AlertSOSViewModel AlertSOS
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AlertSOSViewModel>();
            }
        }

        public ListAlertsViewModel ListAlert
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ListAlertsViewModel>();
            }
        }

        public ModeZoneViewModel ModeZone
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ModeZoneViewModel>();
            }
        }

        public ModeDontMoveViewModel ModeDontMove
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ModeDontMoveViewModel>();
            }
        }

        public ModeTrackingViewModel ModeTracking
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ModeTrackingViewModel>();
            }
        }

        public ModeSelectionViewModel ModeSelection
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ModeSelectionViewModel>();
            }
        }

        public MapViewModel Map
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MapViewModel>();
            }
        }

        public HistoricViewModel Historic
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HistoricViewModel>();
            }
        }

        public MapAllSeekiosViewModel MapAllSeekios
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MapAllSeekiosViewModel>();
            }
        }

        public MapViewModelBase BaseMap
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MapViewModelBase>();
            }
        }

        public ReloadCreditViewModel ReloadCredit
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ReloadCreditViewModel>();
            }
        }

        public TransactionHistoricViewModel TransactionHistoric
        {
            get
            {
                return ServiceLocator.Current.GetInstance<TransactionHistoricViewModel>();
            }
        }

        public ParameterViewModel Parameter
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ParameterViewModel>();
            }
        }

        public CreditsViewModel Credits
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CreditsViewModel>();
            }
        }

        #region COMMUNITY

        //public AddFriendViewModel AddFriend
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<AddFriendViewModel>();
        //    }
        //}

        //public TabListFriendsViewModel TabListFriends
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<TabListFriendsViewModel>();
        //    }
        //}

        //public TabRequestsViewModel TabRequests
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<TabRequestsViewModel>();
        //    }
        //}

        //public ListSharingsViewModel ListSharings
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<ListSharingsViewModel>();
        //    }
        //}

        //public ShareSeekiosViewModel ShareSeekios
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<ShareSeekiosViewModel>();
        //    }
        //}

        #endregion

        #endregion
    }
}