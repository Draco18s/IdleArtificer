//! Created by Tom Lambert alias Koopakiller
//! Project started: 2011
//! License: Microsoft Reciprocal License (Ms-RL)
//! Project site: https://numerics.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Koopakiller.Numerics.Resources;

//[assembly: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "type", Target = "Koopakiller.Numerics._Extensions")]

namespace Koopakiller.Numerics
{
    /// <summary>
    /// Stellt Erweiterungsmethoden für .NET bereit.
    /// </summary>
    
    public static class _Extensions
    {
        /// <summary>
        /// Ersetzt alle doppelten Vorkommen eines Zeichens gegen eines, bis nur noch einzelne Zeichen vorkommen.
        /// </summary>
        /// <param name="value">Die Zeichenfolge, deren doppelten Vorkommen von <paramref name="character"/> ersetzt werden sollen.</param>
        /// <param name="character">Das zu suchende, doppelt vorkommende, Zeichen.</param>
        /// <returns>Eine neue <see cref="System.String"/>-Instanz, in der kein doppeltes Vorkommen von <paramref name="character"/> vorkommt.</returns>
        public static string SimplifyChars(this string value, char character)
        {
            string chars = string.Format(CultureInfo.CurrentCulture, "{0}{0}", character);
            string @charString = character.ToString();
            while (value.IndexOf(chars, StringComparison.CurrentCulture) >= 0)
                value = value.Replace(chars, @charString);
            return value;
        }

        /// <summary>
        /// Ersetzt alle doppelten Vorkommen eines Zeichens gegen eines bis, nur noch einzelne Zeichen in dieser Instanz enthalten sind.
        /// </summary>
        /// <param name="instance">Die Instanz, deren doppelten Vorkommen von <paramref name="character"/> ersetzt werden sollen.</param>
        /// <param name="character">Das zu suchende, doppelt vorkommende, Zeichen.</param>
        /// <returns>Die Instanz des <see cref="System.Text.StringBuilder"/>, dessen doppelten Vorkommen von <paramref name="character"/> nun nur noch einmal vorkommt.</returns>
        public static StringBuilder SimplifyChars(this StringBuilder instance, char character)
        {
            if (instance == null)
                throw new ArgumentNullException(ResourceManager.GetMessage("ArgNull_Param","instance"));

            string chars = string.Format(CultureInfo.CurrentCulture,"{0}{0}", character);
            string @charString = character.ToString();
            int oldlength = 0;
            while (oldlength != instance.Length)
            {
                oldlength = instance.Length;
                instance.Replace(chars, @charString);
            }
            return instance;
        }

        /// <summary>
        /// Überprüft ob eine Auflistung aus Elementen einer anderen Auflistung besteht.
        /// </summary>
        /// <typeparam name="TSource">Der Typ, von dem die Elemente der Auflistungen sind.</typeparam>
        /// <param name="list">Die Liste, deren Elemente geprüft werden sollen.</param>
        /// <param name="allowed">Die erlaubten Elemente.</param>
        /// <returns><c>True</c>, wenn die Liste nur Elemente der <paramref name="allowed"/>-Liste enthält. Andernfalls <c>False</c>.</returns>
        public static bool ContainsOnly<TSource>(this IEnumerable<TSource> list, IEnumerable<TSource> allowed)
        {
            if (list == null) throw new ArgumentNullException(ResourceManager.GetMessage("ArgNull_Param", "list"));
            if (allowed == null) throw new ArgumentNullException(ResourceManager.GetMessage("ArgNull_Param", "allowed"));

            foreach (TSource c in list)
                if (!allowed.Contains(c))
                    return false;
            return true;
        }
        /// <summary>
        /// Überprüft ob eine Auflistung keine Elemente einer anderen Auflistung besitzt.
        /// </summary>
        /// <typeparam name="TSource">Der Typ, von dem die Elemente der Auflistungen sind.</typeparam>
        /// <param name="list">Die Liste, deren Elemente geprüft werden sollen.</param>
        /// <param name="prohibited">Die verbotenen Elemente.</param>
        /// <returns><c>True</c>, wenn die Liste keine Elemente der <paramref name="prohibited"/>-Liste enthält. Andernfalls <c>False</c>.</returns>
        //[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool ContainsNothing<TSource>(this IEnumerable<TSource> list, IEnumerable<TSource> prohibited)
        {
            if (list == null) throw new ArgumentNullException(ResourceManager.GetMessage("ArgNull_Param", "list"));
            if (prohibited == null) throw new ArgumentNullException(ResourceManager.GetMessage("ArgNull_Param", "prohibited"));

            foreach (TSource c in list)
                if (prohibited.Contains(c))
                    return false;
            return true;
        }

        /// <summary>
        /// Überprüft ob eine <see cref="IEnumerable&lt;TSource&gt;"/> doppelte Elemente enthält.
        /// </summary>
        /// <typeparam name="TSource">Der Typ von dem die Elemente in der Auflistung sind.</typeparam>
        /// <param name="items">De IEnumerable&lt;TSource&gt; der Elemente.</param>
        /// <returns><c>True</c>, wenn die Liste Doppelte Elemente enthält, andernfalls <c>False</c>.</returns>
        public static bool ContainsDublicates<TSource>(this IEnumerable<TSource> items)
        {
            return items.Distinct().Count() != items.Count();
        }

        /// <summary>
        /// Erweitert ein Array auf die angegebene Länge. Wenn das Array länger als <paramref name="length"/> ist, wird nichts unternommen.
        /// </summary>
        /// <typeparam name="T">Der Typ der Elemente in dem Array.</typeparam>
        /// <param name="array">Das zu erweiternde Array.</param>
        /// <param name="length">Die Ziellänge des Arrays.</param>
        /// <returns>Ein Array mit der Länge <paramref name="length"/> und den Elementen aus <paramref name="array"/> im Bereich von 0 bis <paramref name="length"/> - 1.</returns>
        internal static T[] Extend<T>(this T[] array, int length)
        {
            if (array.Length > length)
                return array;
            else
            {
                T[] result = new T[length];
                Array.Copy(array, result, array.Length);
                //array = result;
                return result;
            }
        }

        /// <summary>
        /// Verschiebt die Bits in einem Array im den angegebenen Betrag.
        /// </summary>
        /// <param name="data">Die Byte-Daten deren Bits verschoben werden sollen.</param>
        /// <param name="shiftVal">Die Anzahl, um die die Bits verschoben werden sollen.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Wird ausgelöst, wenn <paramref name="shiftVal"/> kleiner als 0 ist.</exception>
        internal static uint[] ShiftLeft(this uint[] data, int shiftVal)
        {
            if (shiftVal < 0)
                throw new ArgumentOutOfRangeException("shiftVal", ResourceManager.GetMessage("ArgOutRang_ParamMustSmaller", "shiftVal", "0"));
            else if (shiftVal == 0)
                return data;

            //BigInteger result = BigInteger.Absolute(bi1);//Ergebnis aus Absolutem Betrag vom Parameter erstellen
            uint[] result = new uint[data.Length];
            Array.Copy(data, result, result.Length);

            int shiftAmount = 32;
            int bufLen = 0;

            for (int count = shiftVal; count > 0; )//Bits in 32er Schritten durchgehen
            {
                bufLen = result.Length; //Länge des Array neu ermitteln, falls Zielarray mehr als 32 Bit größer ist
                if (count < shiftAmount)//Solange um 1 uint verschieben, bis nur noch ein Restwert übrig ist, um den dann verschoben wird.
                    shiftAmount = count;

                //Bits verschieben
                ulong carry = 0;
                for (int i = 0; i < bufLen; i++)
                {
                    ulong val = ((ulong)result[i]) << shiftAmount;
                    val |= carry;

                    result[i] = (uint)(val & 0xFFFFFFFF);
                    carry = val >> 32;
                }

                if (carry != 0)//Wenn ein Überlauf existiert > diesen verschieben
                {
                    uint[] buffer2 = new uint[result.Length + 1];
                    Array.Copy(result, buffer2, result.Length);
                    buffer2[buffer2.Length - 1] = (uint)carry;//Um 1 Element erweitern
                    result = buffer2;

                }
                count -= shiftAmount;//32 Bit bzw. Rest abziehen, bis count 0 ist
            }
            return result;
        }

        /// <summary>
        /// Gibt die mit XOR verknüpften HashCodes der einzelnen Elemente wieder.
        /// </summary>
        /// <typeparam name="T">Der Typ von dem die Auflistung ist.</typeparam>
        /// <param name="items">Die Auflistung dessen gemeinsamer Hashcode ermittelt werden soll.</param>
        /// <returns>Ein mit XOR verknüpfter Hashcode aller Elemente von <paramref name="items"/>.</returns>
        public static int GetItemsHashCode<T>(this IEnumerable<T> items)
        {
            if (items == null || items.Count() == 0)
                return 0;
            if (items.Count() == 1)
                return items.ElementAt(0).GetHashCode();

            int result = items.ElementAt(0).GetHashCode();
            for (int i = 1; i < items.Count(); ++i)
                result ^= items.ElementAt(i).GetHashCode();
            return result;
        }



        [StructLayout(LayoutKind.Explicit)]
        private struct BitUnion//Zum konvertieren der Bytes zwischen Long und Double
        {
            //alle beim Offset 0, somit gibt es beim abrufen auch immer die selben Bits/Bytes
            [FieldOffset(0)]
            public double Double;
            [FieldOffset(0)]
            public long Long;
//            [FieldOffset(0)]
//            public ulong ULong;
        }

        /// <summary>
        /// Ruft die Bits eines Double-Wertes als Int64 ab.
        /// </summary>
        /// <param name="value">Ein Double-Wert.</param>
        /// <returns>Die Bits von <paramref name="value"/>.</returns>
        public static long ToInt64(this double value)
        {
            return new BitUnion { Double = value }.Long;
        }
        /// <summary>
        /// Erstellt einen neuen Doublewert und kopiert die Bits von <paramref name="value"/> in die neue Instanz.
        /// </summary>
        /// <param name="value">Die zu verwendenden Bits.</param>
        /// <returns>Ein Double mit den Bits von <paramref name="value"/>.</returns>
        public static double ToDouble(this long value)
        {
            return new BitUnion { Long = value }.Double;
        }

        /// <summary>
        /// Gibt die Mantisse sowie den Exponenten des Wertes mit doppelter Genauigkeit zurück.
        /// </summary>
        /// <param name="value">Der Wert, deren Mantisse und Exponent ermittelt werden sollen.</param>
        /// <param name="mantissa">Die Mantisse von <paramref name="value"/>.</param>
        /// <param name="exponent">Der Exponent von <paramref name="value"/>.</param>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#"), SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#")]
        public static void GetSegments(this double value, out long mantissa, out int exponent)
        {
            long bits = value.ToInt64(); // Einzelne Bits ermitteln //?  Siehe ggf.: http://dotnet-snippets.de/snippet/bits-zwischen-double-und-long-int64-konvertieren/2759
            bool negative = (bits < 0);
            exponent = (int)((bits >> 52) & 0x7ffL);
            mantissa = bits & 0xfffffffffffffL;

            if (exponent == 0)
                exponent++;
            else
                mantissa = mantissa | (1L << 52);

            exponent -= 1075; // 1023 (= 2 ^ 10 - 1) + 52

            if (mantissa == 0)
                return;

            // Normalisierung
            while ((mantissa & 1) == 0) // Solange Mantisse gerade ist
            {    
                mantissa >>= 1;
                ++exponent;
            }
            if (negative) // Mantisse negativ machen, wenn d auch negativ ist
                mantissa = -mantissa;
        }

        /// <summary>
        /// Berechnet die Summe der komplexen Zahlen in der Auflistung.
        /// </summary>
        /// <param name="cplxs">Eine Auflistung von Complex-Werten.</param>
        /// <returns>Die Summe der Werte von <paramref name="cplxs"/>.</returns>
        public static Complex Sum(this IEnumerable<Complex> cplxs)
        {
            if (cplxs == null) throw new ArgumentNullException(ResourceManager.GetMessage("ArgNull_Param", "cplxs"));

            Complex result = 0;
            foreach (Complex c in cplxs)
                result += c;
            return result;
        }

        /// <summary>
        /// Berechnet die Summe der komplexen Zahlen unter Ausführung von <paramref name="selector"/> für jeden Wert.
        /// </summary>
        /// <param name="cplxs">Eine Auflistung von Complex-Werten.</param>
        /// <param name="selector">Eine Funktion, mit der jeder Wert vor der Summierung geändert wird.</param>
        /// <returns>Die Summe der Werte von <paramref name="cplxs"/> unter Ausführung von <paramref name="selector"/>.</returns>
        public static Complex Sum(this IEnumerable<Complex> cplxs, Func<Complex, Complex> selector)
        {
            if (cplxs == null) throw new ArgumentNullException(ResourceManager.GetMessage("ArgNull_Param", "cplxs"));
            if (selector == null) throw new ArgumentNullException(ResourceManager.GetMessage("ArgNull_Param", "selector"));

            Complex result = 0;
            foreach (Complex c in cplxs)
                result += selector(c);
            return result;
        }
    }
}