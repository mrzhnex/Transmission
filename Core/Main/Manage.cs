﻿using System;
using System.ComponentModel;
using System.Text;
using Core.Client;
using Core.Server;

namespace Core.Main
{
    public static class Manage
    {
        public static Information DefaultInformation { get; private set; } = new Information();
        public static EventManager EventManager { get; private set; } = new EventManager();
        public static Logger Logger { get; private set; } = new Logger();
        public static Core.Application.Manager ApplicationManager { get; private set; } = new Core.Application.Manager();
        public static Localization.Manager LocalizationManager { get; private set; } = new Localization.Manager();
        public static ClientInfoBehaviour ClientInfoBehaviour { get; private set; } = new ClientInfoBehaviour();
        public static Client.Session ClientSession { get; set; }
        public static Server.Session ServerSession { get; set; }
        public static Application Application { get; set; }

        public static string GetStringFromData(byte[] data)
        {
            string result = string.Empty;
            int zerocount = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].ToString() == "0")
                {
                    zerocount++;
                }
                else
                {
                    for (int k = 0; k < zerocount; k++)
                    {
                        result += "0";
                    }
                    zerocount = 0;
                    result += data[i];
                }
            }
            if (zerocount > 100)
            {
                result = result + "+0x" + zerocount;
            }
            return result;
        }
        public static System.Windows.Media.Color ToMediaColor(this System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
        public static byte[] GetDataFromString(string key)
        {
            return Encoding.UTF8.GetBytes(key);
        }
        public static byte[] ParseKeyFromString(string key)
        {
            byte[] result = new byte[Manage.DefaultInformation.DataLength];
            byte[] data = GetDataFromString(key);
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = data[i];
            }
            return result;
        }
        public static ulong GetUlongFromBuffer(byte[] data)
        {
            ulong result = 0;
            for (int i = 0; i < data.Length; i++)
            {
                result += data[i];
            }
            return result;
        }
        public static T ToEnum<T>(this string value) where T : struct
        {
            if (string.IsNullOrEmpty(value)) return default;
            return Enum.TryParse<T>(value, true, out T result) ? result : default;
        }
        public static void Raise(this PropertyChangedEventHandler handler, object sender, string propertyName)
        {
            handler?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
        }
    }
}