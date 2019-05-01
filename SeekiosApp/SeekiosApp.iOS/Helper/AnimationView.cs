using System;
using UIKit;

namespace SeekiosApp.iOS
{
	public class AnimationView
	{
		public static void StartAnimatinon(double duationTime)
		{
			UIView.BeginAnimations(string.Empty, System.IntPtr.Zero);
			UIView.SetAnimationDuration(duationTime);
		}

		public static void StopAnimatinon()
		{
			UIView.CommitAnimations();
		}
	}
}
