namespace SeekiosApp.Helper
{
    public static class SpinnerHelper
    {
        public static int GetValueSpinner(int value)
        {
            switch (value)
            {
                default: return 1;
                case 0: return 1;
                case 1: return 2;
                case 2: return 3;
                case 3: return 4;
                case 4:return 5;
                case 5: return 10;
                case 6: return 15;
                case 7: return 30;
                case 8: return 60;
                case 9: return 120;
                case 10: return 180;
                case 11: return 240;
                case 12: return 300;
                case 13: return 360;
                case 14: return 720;
                case 15: return 1440;
            }
        }

        public static int ReverseValueSpinner(int value)
        {
            switch (value)
            {
                default: return 8;
                case 1: return 0;
                case 2: return 1;
                case 3: return 2;
                case 4: return 3;
                case 5: return 4;
                case 10: return 5;
                case 15: return 6;
                case 30: return 7;
                case 60: return 8;
                case 120: return 9;
                case 180: return 10;
                case 240: return 11;
                case 300: return 12;
                case 360: return 13;
                case 720: return 14;
                case 1440: return 15;
            }
        }
    }
}
