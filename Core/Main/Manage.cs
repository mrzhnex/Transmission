using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Localization;
using Core.Server;

namespace Core.Main
{
    public static class Manage
    {
        public static Information DefaultInformation { get; private set; } = new Information();
        public static EventManager EventManager { get; private set; } = new EventManager();
        public static Logger Logger { get; private set; } = new Logger();
        public static Core.Application.Manager ApplicationManager { get; private set; } = new Core.Application.Manager();
        public static Manager LocalizationManager { get; private set; } = new Manager();
        public static Client.Session ClientSession { get; set; }
        public static Session ServerSession { get; set; }
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
        public static byte[] GetDataFromString(string key)
        {
            return Encoding.ASCII.GetBytes(key);
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
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }
    }
}