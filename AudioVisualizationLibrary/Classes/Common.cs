namespace AudioVisualizationLibrary.Classes
{
    public static class Common
    {
        public static string ShowHideMenuText;
        public static string ScreenshotText;
        public static string FormatDeviceName(string deviceName)
        {
            string name = deviceName;

            string[,] titles = new string[,]
            {
                { "Hoparl�r", "Hoparlör" },
                { "Kulakl�k", "Kulaklık" },
            };

            for (int i = 0; i < titles.GetLength(0); i++)
            {
                name = name.Replace(titles[i, 0], titles[i, 1]);
            }

            return name;
        }
    }
}
