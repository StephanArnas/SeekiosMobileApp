//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using Android.Support.V4.View;
//using Java.Lang;
//using Android;

//namespace SeekiosApp.Droid.View
//{
//    [Activity(Label = "CustomSwipeAdapter")]
//    public class CustomSwipeAdapter : PagerAdapter
//    {
//        private List<int> image_resources = new List<int>();
//        private Context ctx;
//        private LayoutInflater layoutInflater;

//        public CustomSwipeAdapter(Context ctx)
//        {
//            this.ctx = ctx;
//        }

//        public override int Count
//        {
//            get
//            {
//                return image_resources.Capacity/2;
//            }
//        }

//        public void image(int image)
//        {
//            image_resources.Add(image);
//        }

//        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object objectValue)
//        {
//        }

//        public override bool IsViewFromObject(Android.Views.View view, Java.Lang.Object objectValue)
//        {
//            return view == (LinearLayout)objectValue;
//        }

//        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
//        {
//            layoutInflater = (LayoutInflater)ctx.GetSystemService(Context.LayoutInflaterService);
//            Android.Views.View item_view = layoutInflater.Inflate(Resource.Layout.Swipe_Layout, container, false);
//            ImageView imageView = item_view.FindViewById<ImageView>(Resource.Id.image_view);

//            imageView.SetImageResource(image_resources[position]);
//            container.AddView(item_view);
//            return item_view;
//        }
//    }
//}