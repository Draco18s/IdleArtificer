//! Created by Tom Lambert alias Koopakiller
//! Project started: 2011
//! License: Microsoft Reciprocal License (Ms-RL)
//! Project site: https://numerics.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Koopakiller.Numerics;
using Koopakiller.Numerics.Resources;

namespace Koopakiller.Numerics
{
    /// <summary>
    /// Stellt Methoden zum Konvertieren von Werten zwischen verschiedenen Zahlensystemen bereit.
    /// </summary>
    public static class NumberSystem
    {
        /// <summary>
        /// Repräsentiert das Binär(Dual)-System.
        /// </summary>
        public const int Binary = 2;
        /// <summary>
        /// Repräsentiert das Dezimal-System.
        /// </summary>
        public const int Decimal = 10;
        /// <summary>
        /// Repräsentiert das Hexadezimal-System.
        /// </summary>
        public const int Hexadecimal = 16;

        /// <summary>
        /// Konvertiert eine Zahl von einem Zahlensystem in ein anderes. Dabei werden maximal 15 Dezimalstellen generiert. Groß- und Kleinschreibung wird nicht beachtet, Periodische Zahlen werden nicht wiederhohlt. Als Ziffernzeichen werden die europäischen Zahlen und anschließend das großgeschriebene, lateinische Alphabet verwendet.
        /// </summary>
        /// <param name="num">Die zu konvertierende Zahl.</param>
        /// <param name="from">Das Zahlensystem, in dem <paramref name="num"/> angegeben ist. (Bereich: 2 bis 36)</param>
        /// <param name="to">Das Zahlensystem, in das <paramref name="num"/> konvertiert werden soll. (Bereich: 2 bis 36)</param>
        /// <returns>Der Wert von <paramref name="num"/> im Zielsystem (<paramref name="to"/>).</returns>
        public static string Convert(string num, int from, int to)
        {
            if (num == null) throw new ArgumentNullException(ResourceManager.GetMessage("ArgNull_Param", "num"));
            num = num.ToUpper();
            //? Fehlerbehandlung//Erstellung
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            CheckCommonErrors(num, from, to, chars);
            if (!num.ToCharArray().ContainsOnly((chars.Substring(0, from) + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator).ToCharArray()))
                throw new ArgumentException(ResourceManager.GetMessage("ArgOutRang_OnlyContainsCharsInRange", "num", "0", "from (" + from + ")", "chars"), "num");
            if (!chars.ToCharArray().ContainsNothing(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator.ToCharArray()))
                throw new ArgumentException(ResourceManager.GetMessage("Arg_CantContainsCharsFrom", "num", "info.DecimalSepareator"), "num");

            //? Vor- und Nachkommaanteile aufsplitten
            string[] nums = num.Split(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //? Zahl über interne Methode umrechnen.
            return InternalConvert(nums[0], nums.Length == 1 ? "" : nums[1], NumberFormatInfo.CurrentInfo.NumberDecimalSeparator, from, to, chars, 15, true, true);
        }

        /// <summary>
        /// Konvertiert eine Zahl von einem Zahlensystem in ein anderes. Dabei werden maximal 15 Dezimalstellen generiert. Groß- und Kleinschreibung wird nicht beachtet, Periodische Zahlen werden nicht wiederhohlt.
        /// </summary>
        /// <param name="num">Die zu konvertierende Zahl.</param>
        /// <param name="from">Das Zahlensystem, in dem <paramref name="num"/> angegeben ist. (Bereich: 2 bis <paramref name="chars"/>.Length)</param>
        /// <param name="to">Das Zahlensystem, in das <paramref name="num"/> konvertiert werden soll. (Bereich: 2 bis <paramref name="chars"/>.Length)</param>
        /// <param name="chars">Die zu verwendenden Zeichen.</param>
        /// <returns>Der Wert von <paramref name="num"/> im Zielsystem (<paramref name="to"/>).</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Argumente von öffentlichen Methoden validieren", MessageId = "3")]
        public static string Convert(string num, int from, int to, string chars)
        {
            if (num == null) throw new ArgumentNullException(ResourceManager.GetMessage("ArgNull_Param", "num"));
            num = num.ToUpper();
            //? Fehlerbehandlung/Erstellung
            CheckCommonErrors(num, from, to, chars);
            if (!num.ToCharArray().ContainsOnly((chars.Substring(0, from) + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator).ToCharArray()))
                throw new ArgumentException(ResourceManager.GetMessage("ArgOutRang_OnlyContainsCharsInRange", "num", "0", "from (" + from + ")", "chars"), "num");
            if (!chars.ToCharArray().ContainsNothing(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator.ToCharArray()))
                throw new ArgumentException(ResourceManager.GetMessage("Arg_CantContainsCharsFrom", "num", "NumberFormatInfo.CurrentInfo.DecimalSeparator"), "num");

            //? Vor- und Nachkommaanteile aufsplitten
            string[] nums = num.Split(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //? Zahl über interne Methode umrechnen.
            return InternalConvert(nums[0], nums.Length == 1 ? "" : nums[1], NumberFormatInfo.CurrentInfo.NumberDecimalSeparator, from, to, chars, 15, true, true);
        }

        /// <summary>
        /// Konvertiert eine Zahl von einem Zahlensystem in ein anderes.
        /// </summary>
        /// <param name="num">Die zu konvertierende Zahl.</param>
        /// <param name="from">Das Zahlensystem, in dem <paramref name="num"/> angegeben ist. (Bereich: 2 bis <paramref name="chars"/>.Length)</param>
        /// <param name="to">Das Zahlensystem, in das <paramref name="num"/> konvertiert werden soll. (Bereich: 2 bis <paramref name="chars"/>.Length)</param>
        /// <param name="chars">Die zu verwendenden Zeichen.</param>
        /// <param name="maxCount">Die maximale Anzahl an auszugebenden Nachkommastellen.</param>
        /// <param name="stopPeriod"><c>True</c>, wenn beim 2. Beginn einer Periodischen Zahlenfolge im Nachkommabereich die Zahl zurück gegeben werden soll. Wenn <c>False</c> angegeben wird, dann werden die Nachkommastellen bis auf <paramref name="maxCount"/> erweitert.</param>
        /// <param name="ignorecase"><c>True</c>, wenn die Groß- und Kleinschreibung bei der Eingabezahl nicht beachtet werden soll. Die Ausgabezahl hat die Schreibweise, wie Sie in angegeben <paramref name="chars"/> wurde.</param>
        /// <param name="info">Das Format, nach dem das Trennzeichen des Ganzzahligen- und des Deizmalteils getrennt werden sollen. Es wird nur die NumberDecimalSeparator-Eigenschaft beachtet. Kein darin enthaltenes Zeichen darf in <paramref name="chars"/> vorkommen.</param>
        /// <returns>Der Wert von <paramref name="num"/> im Zielsystem (<paramref name="to"/>).</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Argumente von öffentlichen Methoden validieren", MessageId = "3")]
        public static string Convert(string num, int from, int to, string chars, int maxCount, bool stopPeriod, bool ignorecase, NumberFormatInfo info)
        {
            if (num == null) throw new ArgumentNullException(ResourceManager.GetMessage("ArgNull_Param", "num"));
            //? Fehlerbehandlung/Erstellung
            CheckCommonErrors(num, from, to, chars);
            if (info == null)
                throw new ArgumentNullException(ResourceManager.GetMessage("ArgNull_ParamCantNull", "info"));
            if (string.IsNullOrEmpty(info.CurrencyDecimalSeparator))
                throw new ArgumentNullException(ResourceManager.GetMessage("ArgNull_ParamCantNull", "info.CurrencyDecimalSeparator"));
            if (!num.ToCharArray().ContainsOnly((chars.Substring(0, from) + info.NumberDecimalSeparator).ToCharArray()))
                throw new ArgumentException(ResourceManager.GetMessage("ArgOutRang_OnlyContainsCharsInRange", "num", "0", "from (" + from + ")", "chars"), "num");
            if (!chars.ToCharArray().ContainsNothing(info.NumberDecimalSeparator.ToCharArray()))
                throw new ArgumentException(ResourceManager.GetMessage("Arg_CantContainsCharsFrom", "num", "info.DecimalSeparator"), "num");

            if (num.StartsWith(info.NumberDecimalSeparator, StringComparison.CurrentCulture)) //? Ggf. 0 vor das Komma setzen.
                num = "0" + num;

            //? Vor- und Nachkommaanteile aufsplitten
            string[] nums = num.Split(info.NumberDecimalSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //? Zahl über interne Methode umrechnen.
            return InternalConvert(nums[0], nums.Length == 1 ? "" : nums[1], info.NumberDecimalSeparator, from, to, chars, maxCount, stopPeriod, ignorecase);
        }

        private static void CheckCommonErrors(string num, int from, int to, string chars)
        {
            if (string.IsNullOrEmpty(num))//Leerer String als Zahl
                throw new ArgumentException(ResourceManager.GetMessage("Arg_XMustBeANumber", "num (" + num + ")"), "num");
            if (from < 2 || from > chars.Length)
                throw new ArgumentOutOfRangeException("from", ResourceManager.GetMessage("ArgOutRang_ParamMustInRange", "from (" + from + ")", "2", "chars.Length (" + chars.Length + ")"));
            if (to < 2 || to > chars.Length)
                throw new ArgumentOutOfRangeException("to", ResourceManager.GetMessage("ArgOutRang_ParamMustInRange", "to (" + to + ")", "2", "chars.Length (" + chars.Length + ")"));
            if (chars.Length < 2)
                throw new ArgumentOutOfRangeException("chars", ResourceManager.GetMessage("ArgOutRang_ParamMustGreaterEqual", "chars.Length (" + chars.Length + ")", "2"));
        }

        #region Hilfsmethoden

        /// <param name="num1">Der Ganzzahlige Teil der Zahl.</param>
        /// <param name="num2">Der Nachkommateil der Zahl.</param>
        /// <param name="sep">Der Seprarator zwischen Ganzzahligem und Nachkommateil der Zahl.</param>
        /// <param name="from">Das Ursprungszahlensystem der Zahl. Muss größergleich 2 und kleiner als die Länge von chars sein.</param>
        /// <param name="to">Das Zielzahlensystem. Muss größergleich 2 und kleiner als die Länge von chars sein.</param>
        /// <param name="chars">Die zu verwendenden Zeichen. Diese werden im Ursprungssystem und im Zielsystem verwendet. </param>
        /// <param name="maxCount">Die maximale Anzahl der Nachkommastellen.</param>
        /// <param name="stopPeriod"><c>True</c>, wenn eine Periodische Zahl nach der letzten Stelle nicht fort geführt werden soll. Andernfalls <c>False</c>.</param>
        /// <param name="ignorecase"><c>True</c>, wenn die Groß- und Kleinschreibung bei der eingabezahl nicht beachtet werden soll. Andernfalls <c>False</c>.</param>
        static string InternalConvert(string num1, string num2, string sep, int from, int to, string chars, int maxCount, bool stopPeriod, bool ignorecase)
        {
            string chars2 = chars;
            if (ignorecase)
            {
                chars2 = chars2.ToUpper();
                num1 = num1.ToUpper();
            }
            if (chars.ToCharArray().ContainsDublicates())
                throw new ArgumentException(ResourceManager.GetMessage("Arg_CantContainsDuplicates", "chars"), "chars");

            //num1 in 10
            double p1 = Internal1To10(num1, from, chars);
            //num2 in 10
            double p2 = Internal2To10(num2, from, chars);

            //num1 in to
            string s1 = Internal1From10(p1, to, chars);
            //num2 in to
            string s2 = Internal2From10(p2, to, chars, stopPeriod, maxCount);

            return string.Format(CultureInfo.InvariantCulture, string.IsNullOrEmpty(s2) ? "{0}" : "{0}{1}{2}", s1, sep, s2);
        }

        /// <summary>
        /// Rechnet den Vorkommaanteil ins Dezimalsystem um.
        /// </summary>
        /// <param name="num">Die Zahl.</param>
        /// <param name="from">Das Zahlensystem, in dem die Zahl angegeben ist.</param>
        /// <param name="chars">Der Zeichensatz für die Zahlen.</param>
        static double Internal1To10(string num, int from, string chars)
        {
            double result = 0;
            for (int i = 0; i < num.Length; ++i)
            {
                result += chars.IndexOf(num[num.Length - i - 1]) * Math.Pow(from, i);
            }
            return result;
        }
        /// <summary>
        /// Rechnet den Nachkommaanteil ins Dezimalsystem um.
        /// </summary>
        /// <param name="num">Die Zahl.</param>
        /// <param name="from">Das Zahlensystem, in dem die Zahl angegeben ist.</param>
        /// <param name="chars">Der Zeichensatz für die Zahlen.</param>
        static double Internal2To10(string num, int from, string chars)
        {
            double result = 0;
            for (int i = 0; i < num.Length; ++i)
            {
                result += chars.IndexOf(num[num.Length - i - 1]) * Math.Pow(from, i);
            }
            result /= Math.Pow(from, num.Length);
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d">Die Zahl.</param>
        /// <param name="to">Das Zahlensystem, in das Konvertiert werden soll.</param>
        /// <param name="chars">Der Zeichensatz für die Zahlen.</param>
        static string Internal1From10(double d, int to, string chars)
        {
            string result = "";
            while (d != 0)
            {
                int r = (int)(d % to);//Rest ermitteln
                d = (int)(d / to);
                result = chars[r] + result;
            }
            if (string.IsNullOrEmpty(result))
                result = "0";
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d">Die Zahl.</param>
        /// <param name="to">Das Zahlensystem, in das Konvertiert werden soll.</param>
        /// <param name="chars">Der Zeichensatz für die Zahlen.</param>
        /// <param name="stopPeriod"><c>True</c>, wenn Periodische Zahlenbereiche nicht wiederhohlt werden sollen. <c>False</c>, wenn die Nachkommastellen bis <paramref name="maxCount"/> gefüllt werden sollen.</param>
        /// <param name="maxCount">Die maximale anzahl an Nachkommastellen.</param>
        static string Internal2From10(double d, int to, string chars, bool stopPeriod, int maxCount)
        {
            List<double> list = new List<double>();
            string result = "";
            int i = 0;
            while (d != 0 && (!list.Contains(d) || !stopPeriod) && i < maxCount)
            {
                list.Add(d);
                d *= to;
                result += chars[((int)d)];
                d -= (int)d;
                ++i;
            }
            return result;
        }

        #endregion
    }
}