namespace SeekiosApp.Droid.Helper
{
    /**
     * Those two methods check if references in .net still exist within the jvm
     * if a ref exists in .net but has been garbage collected by the jvm, app is going to crash
     */
    public static class PeerConnectionHelper
    {
        public static bool HasPeerConnection(Java.Lang.Object jObj)
        {
            return !(jObj == null || jObj.Handle == System.IntPtr.Zero);
        }

        public static bool HasPeerConnection(Android.Runtime.IJavaObject jObj)
        {
            return !(jObj == null || jObj.Handle == System.IntPtr.Zero);
        }
    }
}