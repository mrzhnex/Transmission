using System;

namespace AudioVisualizationLibrary.Classes
{
    public static class Fix
    {
        /// <summary>
        /// Belirtilen döngünün belirtilen noktadan itibaren tersine doğru çalışmasını sağlar
        /// </summary>
        /// <param name="index">Döngünün o anki ulaştığı noktanın sayısal değeri</param>
        /// <param name="center">Döngünün orta noktasının sayısal değeri</param>
        /// <param name="count">Döngünün bitiş noktasının sayısal değeri</param>
        /// <returns></returns>
        public static int Reverse(int index, int center, int count)
        {
            return index < center ? index : count - index;
        }

        /// <summary>
        /// Belirtilen döngünün belirtilen noktadan itibaren tersine doğru çalışmasını sağlar
        /// </summary>
        /// <param name="index">Döngünün o anki ulaştığı noktanın sayısal değeri</param>
        /// <param name="center">Döngünün orta noktasının sayısal değeri</param>
        /// <param name="count">Döngünün bitiş noktasının sayısal değeri</param>
        /// <returns></returns>
        public static double Reverse(double index, double center, double count)
        {
            return index < center ? index : count - index;
        }

        /// <summary>
        /// Belirtilen döngünün belirtilen modüler değere göre tersine doğru çalışmasını sağlar
        /// </summary>
        /// <param name="index">Döngünün o anki ulaştığı noktanın sayısal değeri</param>
        /// <param name="count">Döngünün bitiş noktasının sayısal değeri</param>
        /// <param name="modNum"></param>
        /// <param name="modVal">Döngünün tersine çalışacağı modüler aritmetik değerini belirtir</param>
        /// <returns></returns>
        public static int Reverse(int index, int count, int modNum, int modVal)
        {
            return modNum % modVal == 0 ? count - index : index;
        }

        /// <summary>
        /// Belirtilen döngünün belirtilen modüler değere göre tersine doğru çalışmasını sağlar
        /// </summary>
        /// <param name="index">Döngünün o anki ulaştığı noktanın sayısal değeri</param>
        /// <param name="count">Döngünün bitiş noktasının sayısal değeri</param>
        /// <param name="modNum">Döngünün tersine çalışacağı sayıyı belirtir</param>
        /// <param name="modVal">Döngünün tersine çalışacağı modüler aritmetik değerini belirtir</param>
        /// <returns></returns>
        public static double Reverse(double index, double count, double modNum, double modVal)
        {
            return modNum % modVal == 0 ? count - index : index;
        }
        public static int Repeat(int index, int center, int count)
        {
            //50 - (100 - 50) = 0
            //50 - (100 - 51) = 1
            //50 - (100 - 52) = 2
            return index < center ? index : center - (count - index);
        }
        public static double Repeat(double index, double center, double count)
        {
            return index < center ? index : center - (count - index);
        }
        /// <summary>
        /// Bu method bir döngü içinde belirtilen bir değerin belirtilen aralıklarla toplam geri dönüşünü sağlar.
        /// 
        /// Örnek:
        /// 
        /// double count = 100;
        /// double loopStep = 1.0 / count;
        /// double maxData = 1000.0;
        /// double dataStep = 1.0 / maxData;
        /// 
        /// for(int i = 0; i < count; i++)
        /// {
        ///     //0.0 ile 1.0 aralığı arasındaki değerleri çağır.
        ///     double range = GetRange(i, loopStep, dataStep, maxData);
        /// }
        /// </summary>
        /// <param name="loopIndex">Döngü sırasını belirtmenizi sağlar.</param>
        /// <param name="loopStep">Döngü aralığını belirtmenizi sağlar.</param>
        /// <param name="dataStep">Veri aralığını belirtmenizi sağlar.</param>
        /// <param name="dataMax">Veri toplamını belirtmenizi sağlar.</param>
        /// <returns></returns>
        public static double GetStep(double loopIndex, double loopStep, double dataStep, double dataMax)
        {
            double CurrentStep = loopStep * loopIndex;
            double CurrentDataStep = CurrentStep * dataMax;
            double TotalDataStep = dataStep * CurrentDataStep;

            return TotalDataStep;
        }
        public static double FixMax(this double num, double max)
        {
            return double.IsInfinity(num) || double.IsNaN(num) || num < 1 ? 1 : num < max ? num : max;
        }
        public static double FixNum(this double num)
        {
            return double.IsInfinity(num) || double.IsNaN(num) || num < 1 ? 1 : num;
        }
        public static void FixWave(this double[] list, int start, int count, bool skipStart = false)
        {
            int index = start;
            int skip = skipStart ? start : 0;

            for (int i = list.Length - (1 + skip); i > (list.Length - (1 + skip)) - count; i--)
            {
                list[i] = list[index++];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        /// <param name="increase">true = artırılmış değer olarak geri döner, false = azaltılmış değer olarak geri döner.</param>
        /// <returns></returns>
        public static int Round(this double num, bool increase)
        {
            double fark = Math.Round(num) < num ? num - Math.Round(num) : Math.Round(num) - num;

            return increase ? (int)(num + fark) : (int)(num - fark);
        }
    }
}
