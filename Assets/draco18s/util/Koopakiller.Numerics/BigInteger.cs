//! Created by Tom Lambert alias Koopakiller
//! Project started: 2011
//! License: Microsoft Reciprocal License (Ms-RL)
//! Project site: https://numerics.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Koopakiller.Numerics.Resources;

//? 1 uint = 4 Bytes = 32 Bit

namespace Koopakiller.Numerics
{
    /// <summary>
    /// Stellt eine Zahl ohne Nachkommastellen dar, welche Positiv oder Negativ sein darf und theoretisch unendlich große Werte abspeichern kann (Begrenzt auf -(2^32)² &lt; x &lt; (2^32)²).
    /// </summary>
    [DebuggerDisplay("{ToString(10)}")]
    public struct BigInteger : IBigNumber<BigInteger>, INumber, IFormattable, IComparable, IComparable<BigInteger>, IEquatable<BigInteger>, IMathComparison<BigInteger>
    {
        #region .ctor

        ///// <summary>
        ///// Initialisiert eine neue Instanz der BigInteger-Klasse. Der Wert wird 0 und das Vorzeichen <c>null</c> sein.
        ///// </summary>
        //[DebuggerStepThrough()]
        //public BigInteger()
        //{
        //    //this.InitValues();
        //    this._sign = null;
        //    this._data = new uint[0];
        //}

        /// <summary>
        /// Initialisiert eine neue Instanz der BigInteger-Klasse unter zuweisung des Wertes mit einem Parameter.
        /// </summary>
        /// <param name="value">Der Wert, welcher der neue Instanz zugewiesen werden soll.</param>
        [DebuggerStepThrough()]
        
        public BigInteger(uint value)
            : this()
        {
            //this.InitValues();
            if (value == 0)
                this._sign = null;
            else
                this._sign = true;

            this.data = new uint[1] { value & 0xFFFFFFFF };
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der BigInteger-Klasse unter zuweisung des Wertes mit einem Parameter.
        /// </summary>
        /// <param name="value">Der Wert, welcher der neue Instanz zugewiesen werden soll.</param>
        [DebuggerStepThrough()]
        
        public BigInteger(ulong value)
            : this()
        {
            //this.InitValues();
            if (value == 0)
                this._sign = null;
            else
                this._sign = true;

            if (value >> 32 == 0)//Nur 4 Byte belegt
            {
                this.data = new uint[1] { (uint)value & 0xFFFFFFFF };
            }
            else
            {
                this.data = new uint[2] { (uint)value & 0xFFFFFFFF, (uint)(value >> 32) & 0xFFFFFFFF };
            }
        }

		public BigInteger(long mantissa, long exponent):this(mantissa) {
			for(int p = 1; p <= exponent; p++) {
				this *= 10;
			}
		}

		/// <summary>
		/// Initialisiert eine neue Instanz der BigInteger-Klasse unter zuweisung des Wertes mit einem Parameter.
		/// </summary>
		/// <param name="value">Der Wert, welcher der neue Instanz zugewiesen werden soll.</param>
		[DebuggerStepThrough()]
        public BigInteger(int value)
            : this()
        {
            //this.InitValues();
            if (value < 0)
            {
                this._sign = false;
            }
            else if (value == 0)
                this._sign = null;
            else
                this._sign = true;

            this.data = new uint[1] { (uint)value & 0xFFFFFFFF };
        }

		/// <summary>
		/// Initialisiert eine neue Instanz der BigInteger-Klasse unter zuweisung des Wertes mit einem Parameter.
		/// </summary>
		/// <param name="value">Der Wert, welcher der neue Instanz zugewiesen werden soll.</param>
		[DebuggerStepThrough()]
        public BigInteger(long value)
            : this()
        {
            //this.InitValues();
            if (value == 0)
            {
                this._sign = null; return;
            }
            else if (value < 0)
                this._sign = false;
            else
                this._sign = true;

            if (value >> 32 == 0)//Nur 4 Byte belegt
            {
                this.data = new uint[1] { (uint)value & 0xFFFFFFFF };
            }
            else
            {
                uint var1 = (uint)(value >> 32) & 0xFFFFFFFF;//ggf. die ersten 32 Bits entfernen
                if (this._sign == false && var1 == 0xFFFFFFFF)
                    this.data = new uint[1] { (uint)value & 0xFFFFFFFF };
                else if (this._sign == true && var1 == 0x00000000)
                    this.data = new uint[1] { (uint)value & 0xFFFFFFFF };
                else
                    this.data = new uint[2] { (uint)value & 0xFFFFFFFF, var1 };//8 Bytes eintragen
            }
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der BigInteger-Klasse mit dem Wert einer anderen BigInteger-Klasse.
        /// </summary>
        /// <param name="value">Die Instanz einer BigInteger-Klasse, deren Wert in die neue Intanz übernommen werden soll.</param>
        [DebuggerStepThrough()]
        public BigInteger(BigInteger value)
            : this()
        {
            //if ((value ?? BigInteger.NullValue).IsNull)
            //    throw new ArgumentNullException("value", "value must be a non null value");

            //this.InitValues();
            this._data = new uint[value.data.Length];
            Array.Copy(value.data, this.data, this.data.Length);
            this._sign = value._sign;
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der BigInteger-Klasse unter zuweisung des Wertes mithilfe der einzelnen Bytes.
        /// </summary>
        /// <param name="values">Die Bytes, welche die Daten des Wertes repräsentieren. Das 1. Bit wird nicht für das Vorzeichen verwendet.</param>
        /// <param name="sign"><c>True</c>, wenn der Wert Positiv ist, <c>False</c>, wenn der Wert negativ ist und <c>null</c> wenn der Wert 0 ist.</param>
        [DebuggerStepThrough()]
        
        public BigInteger(uint[] values, bool? sign)
            : this()
        {
            this.data = values.Reverse().ToArray();
            this._sign = sign;
            this.RemoveEmptyDataFields();
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der BigInteger-Klasse unter Zuweisung des Wertes mithilfe der einzelnen Bytes.
        /// </summary>
        /// <param name="values">Die Bytes, welche die Daten des Wertes repräsentieren. Das 1. Bit wird nicht für das Vorzeichen verwendet.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Argumente von öffentlichen Methoden validieren", MessageId = "0"), DebuggerStepThrough()]
        
        public BigInteger(uint[] values) : this(values, (values.Length == 0 || (from x in values where x == 0 select x).Count() == values.Length) ? null : (bool?)true) { }

        //public BigInteger(double value)
        //    : this()
        //{
        //    long x = value.ToInt64();
        //}

        #endregion

        #region Parse / TryParse

        /// <summary>
        /// Erstellt eine neue Instanz der BigInteger-Klasse mit dem angegebenen Wert.
        /// </summary>
        /// <remarks>Der Abgleich von <paramref name="value"/> und dem verwendeten Zeichensatz erfolgt nicht unter beachtung der Groß- und Kleinschreibung.</remarks>
        /// <remarks>
        /// Wenn die Zeichenfolge mit '-' beginnt, dann wird die neu erstellte Instanz negativ. Beginnt die Zahl mit '+', dann wird dies ignoriert.
        /// Sollte die Zeichenfolge mit 0x (hinter dem Vorzeichen) beginnen, dann wird die Zeichenfolge als Hexadezimal interpretiert. Andernfalls als Dezimal.
        /// </remarks>
        /// <param name="value">Eine Zahl, welche als Zeichenfolge angegeben ist. Beachten Sie die Hinweise.</param>
        /// <returns>Eine neue Instanz der BigInteger-Klasse, welche den Wert aus der angegebenen Zeichenfolge enthält.</returns>
        public static BigInteger Parse(string value)
        {
            return BigInteger.Parse(value, BigInteger.DigitSet);
        }
        /// <summary>
        /// Erstellt eine neue Instanz der BigInteger-Klasse mit dem angegebenen Wert.
        /// </summary>
        /// <remarks>Der Abgleich von <paramref name="value"/> und dem verwendeten Zeichensatz erfolgt nicht unter beachtung der Groß- und Kleinschreibung.</remarks>
        /// <param name="value">Die Darstellung der Zahl als Zeichenfolge im Zahlensystem <paramref name="radix"/>.</param>
        /// <param name="radix">Das Zahlensystem, in dem die Zahl in der Zeichenfolge gespeichert ist.</param>
        /// <returns>Eine neue Instanz der BigInteger-Klasse, welche den Wert aus der angegebenen Zeichenfolge enthält.</returns>
        public static BigInteger Parse(string value, int radix)
        {
            return BigInteger.Parse(value, BigInteger.DigitSet, radix);
        }
        /// <summary>
        /// Erstellt eine neue Instanz der BigInteger-Klasse mit dem angegebenen Wert.
        /// </summary>
        /// <remarks>Der Abgleich von <paramref name="value"/> und <paramref name="digits"/> erfolgt nicht unter beachtung der Groß- und Kleinschreibung.</remarks>
        /// <remarks>
        /// Wenn die Zeichenfolge mit '-' beginnt, dann wird die neu erstellte Instanz negativ. Beginnt die Zahl mit '+', dann wird dies ignoriert.
        /// Sollte die Zeichenfolge mit 0x (hinter dem Vorzeichen) beginnen, dann wird die Zeichenfolge als Hexadezimal interpretiert. Andernfalls als Dezimal.
        /// </remarks>
        /// <param name="value">Eine Zahl, welche als Zeichenfolge angegeben ist. Beachten Sie die Hinweise.</param>
        /// <param name="digits" >Die Zeichen, welche in der Zeichenfolge für die Darstellung der Zahl verwendet wurden.</param>
        /// <returns>Eine neue Instanz der BigInteger-Klasse, welche den Wert aus der angegebenen Zeichenfolge enthält.</returns>
        public static BigInteger Parse(string value, string digits)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value", ResourceManager.GetMessage("ArgNull_ParamCantNull", "value"));
            if (value.StartsWith("+", StringComparison.OrdinalIgnoreCase))
                value = value.Remove(0, 1);

            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                return BigInteger.Parse(value.Remove(0, 2), digits, 16);
            else if (value.StartsWith("-0x", StringComparison.OrdinalIgnoreCase))
                return BigInteger.Parse(value.Remove(1, 2), digits, 16);
            else
                return BigInteger.Parse(value, digits, 10);
        }
        /// <summary>
        /// Erstellt eine neue Instanz der BigInteger-Klasse mit dem angegebenen Wert.
        /// </summary>
        /// <remarks>Der Abgleich von <paramref name="value"/> und <paramref name="digits"/> erfolgt nicht unter beachtung der Groß- und Kleinschreibung.</remarks>
        /// <param name="value">Die Darstellung der Zahl als Zeichenfolge im Zahlensystem <paramref name="radix"/>.</param>
        /// <param name="digits" >Die Zeichen, welche in der Zeichenfolge für die Darstellung der Zahl verwendet wurden.</param>
        /// <param name="radix">Das Zahlensystem, in dem die Zahl in der Zeichenfolge gespeichert ist.</param>
        /// <returns>Eine neue Instanz der BigInteger-Klasse, welche den Wert aus der angegebenen Zeichenfolge enthält.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Wird geworfen, wenn <paramref name="radix"/> größer ist als die Länge von <paramref name="digits"/> oder 
        /// <paramref name="value"/> Zeichen enthält die nicht im Bereich von 0 bis <paramref name="radix"/> in <paramref name="digits"/> liegt oder <paramref name="radix"/> kleiner als 2 ist.</exception>
        /// <exception cref="System.ArgumentNullException">Wird geworfen, wenn <paramref name="value"/> <c>null</c> oder <c>String.Empty</c> ist.</exception>
        public static BigInteger Parse(string value, string digits, int radix)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value", ResourceManager.GetMessage("ArgNull_ParamCantNull", "value"));
            switch (value.ToLower())
            {
                case "∞":
                case "+∞":
                case "infinity":
                case "+infinity":
                    return PositiveInfinity;
                case "-∞":
                case "-infinity":
                    return NegativeInfinity;
                case "nan":
                    return NaN;
            }
            if (string.IsNullOrEmpty(digits))
                throw new ArgumentNullException("digits", ResourceManager.GetMessage("ArgNull_ParamCantNull", "digits"));
            value = value.ToUpper();
            digits = digits.ToUpper();
            if (digits.Length < radix)
                throw new ArgumentOutOfRangeException("digits", ResourceManager.GetMessage("ArgOutRang_ParamMustSmaller", "digits.Length", "radix"));
            if (!value.ToCharArray().ContainsOnly((digits.Substring(0, radix) + "+-").ToCharArray()))
                throw new ArgumentOutOfRangeException("value", ResourceManager.GetMessage("ArgOutRang_OnlyContainsCharsInRange", "value", "0", "radix (" + radix + ")", "digits"));
            if (radix < 2)
                throw new ArgumentOutOfRangeException("radix", "radix >= 2!");
            if (digits.ToCharArray().ContainsDublicates())
                throw new ArgumentDoubleException(ResourceManager.GetMessage("ArgDbl_CantContainsDuplicates", "digits"));


            BigInteger multiplier = new BigInteger(1);
            BigInteger result = new BigInteger();
            value = (value.ToUpper()).Trim();

            bool isNeg = false;

            if (value[0] == '-')
            {
                if (value.Replace(digits[0].ToString(), "") == "-")
                {
                    throw (new ArithmeticException(ResourceManager.GetMessage("Arith_InvalidChar", "value")));
                }
                else
                {
                    isNeg = true;
                    value = value.Remove(0, 1);
                    result._sign = true;
                }
            }
            else if (value[0] == '+')
            {
                value = value.Remove(0, 1);
                result._sign = true;
            }
            else if (string.IsNullOrEmpty(value.Replace(digits[0].ToString(), "")))
            {
                return new BigInteger();
            }
            else
            {
                result._sign = true;
            }

            for (int i = value.Length - 1; i >= 0; i--)
            {
                int posVal = digits.IndexOf(value[i]);

                if (posVal >= radix || posVal < 0)
                    throw (new ArithmeticException(ResourceManager.GetMessage("Arith_InvalidChar", "value")));
                else
                {
                    result = result + (multiplier * posVal);

                    multiplier = multiplier * radix;
                }
            }

            if (isNeg)
                return -result;
            return result;
        }

        /// <summary>
        /// Überprüft ob eine Zeichenfolge eine Valide Zahl enthält. Wenn die Prüfung positiv ist wird der Wert in <paramref name="bi"/> gespeichert. Bei einer negativen Prüfung ist <paramref name="bi"/> 0.
        /// </summary>
        /// <param name="value">Die zu testende Zeichenolge.</param>
        /// <param name="radix">Die Basis des Zahlensystems in dem die Zahl dargestellt ist.</param>
        /// <param name="digits">Das verwendete Ziffernset.</param>
        /// <param name="bi">Eine Instanz der BigInteger-Klasse die nach der Analyse den dargestellten Wert enthält. Sollte die Zeichenfolge keine Zahl darstellen, wird der Instanz 0 zugewiesen.</param>
        /// <returns><c>True</c>, wenn <paramref name="value"/> eine Zahl im angegebenen Zahlensystem ist oder <c>False</c> wenn die Zeichenfolge keine Zahl ist.</returns>
        public static bool TryParse(string value, int radix, string digits, out BigInteger bi)
        {
            try
            {
                bi = BigInteger.Parse(value, digits, radix);
                return true;
            }
            catch (ArgumentException)//ArgumentOutOfRangeException, ArgumentNullException
            {
                bi = BigInteger.Empty;
                return false;
            }
        }

        #endregion

        #region Felder

        uint[] data
        {
            get
            {
                if (_data == null)
                    _data = new uint[0];
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        uint[] _data; // Die Daten der Zahl
        bool? _sign;// = null; // Vorzeichen

        #endregion

        #region Eigenschaften

        /// <summary>
        /// Ruft einen Wert ab, der angibt ob die Zahl 0 ist.
        /// </summary>
        public bool IsZero
        {
            get
            {
                if (this.SpecialValue != SpecialValues.None)
                    return false;
                return this._sign == null || this.data == null || this.data.Length == 0 || (from x in this.data where x == 0 select x).Count() == this.data.Length;
            }
        }
        /// <summary>
        /// Ruft einen Wert ab, der angibt ob die Zahl positiv ist.
        /// </summary>
        public bool IsPositiv
        {
            get
            {
                switch (this.SpecialValue)
                {
                    case SpecialValues.NegativeInfinity:
                    case SpecialValues.NaN:
                        return false;
                    case SpecialValues.PositiveInfinity:
                        return true;
                    default:
                        return _sign == true;
                }
            }
        }
        /// <summary>
        /// Ruft einen Wert ab, der angibt ob die Zahl negativ ist.
        /// </summary>
        public bool IsNegative
        {
            get
            {
                switch (this.SpecialValue)
                {
                    case SpecialValues.NegativeInfinity:
                    case SpecialValues.NaN:
                        return true;
                    case SpecialValues.PositiveInfinity:
                        return false;
                    default:
                        return _sign == false;
                }
            }
        }

        /// <summary>
        /// Ruft einen Wert ab, der angibt ob der Wert der gespeicherten Zahl gerade ist.
        /// </summary>
        public bool IsEven
        {
            get
            {
                if (this.SpecialValue != SpecialValues.None)
                    return false;
                if (this.IsZero)
                    return true;
                else
                    return ((int)this._data[this._data.Length - 1] & 1) == 0;
            }
        }
        /// <summary>
        /// Ruft einen Wert ab, der angibt ob der Wert der gespeicherten Zahl ungerade ist.
        /// </summary>
        public bool IsOdd
        {
            get
            {
                if (this.SpecialValue != SpecialValues.None)
                    return false;
                if (this.IsZero)
                    return false;
                else
                    return ((int)this._data[this._data.Length - 1] & 1) != 0;
            }
        }

        /// <summary>
        /// Ruft die einzelnen Bits des Wertes ab.
        /// </summary>
        /// <param name="index">Der Index des Bits, das abgefragt werden soll.</param>
        /// <returns><c>True</c>, wenn das Bit gesetzt ist, andernfalls <c>False</c>.</returns>
        public bool this[int index]
        {
            get
            {
                return GetBit(index);
            }
        }

        /// <summary>
        /// Hilfsmethode für Indexer
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        private bool GetBit(int index)
        {
            if (index >= this._data.Length * 32)
                throw new IndexOutOfRangeException(ResourceManager.GetMessage("IndexOutRange"));
            uint n = this.data[(index - index % 32) / 32];
            return (n & (1 << (index % 32))) == (1 << (index % 32));
        }

        ///// <summary>
        ///// Ruft einen Wert ab, der angibt ob das letzte Bit gesetzt ist.
        ///// </summary>
        //internal bool IsLastBitSet
        //{
        //    get
        //    {
        //        return this.IsZero ? false : ((this._data[0] & (1 << 32 - 1)) != 0);
        //    }
        //}

        #endregion

        #region Statische Methoden

        /// <summary>
        /// Berechnet den Absoluten Wert einer Zahl.
        /// </summary>
        /// <remarks>Wenn die Zahl 0 ist, dann wird auch 0 zurück gegeben. In diesem Fall ist weder die IsPositiv noch die IsNegative-Eigenschaft <c>True</c>.
        /// Wenn die Zahl positiv oder negativ ist, wird der Wert der Zahl mit positivem Vorzeichen zurück gegeben.</remarks>
        /// <param name="value">Die Zahl, von der der Absolute Betrag berechnet werden soll.</param>
        /// <returns>Der Absolute Wert (mit Positivem oder neutralem Vorzeichen) der übergebenen Zahl.</returns>
        [DebuggerStepThrough()]
        public static BigInteger Abs(BigInteger value)
        {
            switch (value.SpecialValue)
            {
                case SpecialValues.NegativeInfinity:
                case SpecialValues.PositiveInfinity:
                    return PositiveInfinity;
                case SpecialValues.NaN:
                    return NaN;
            }
            if (value._sign == false)
                return -value;
            else
                return value;//0 und Positive Werte zurück geben
        }

        /// <summary>
        /// Potenziert eine Zahl mit einer anderen.
        /// </summary>
        /// <remarks>Sollte die Basis sowie der Exponent 0 sein, so wird eine </remarks>
        /// <param name="base">Die Basis der zu berechnenden Potenz</param>
        /// <param name="exponent">Der Exponent der zu berechnenden Potenz. Dieser Wert muss größer gleich 0 sein.</param>
        /// <returns>Die Potenz der beiden übergebenen Werte.</returns>
        // [DebuggerStepThrough()]
        public static BigInteger Pow(BigInteger @base, BigInteger exponent)
        {
            if (@base.SpecialValue == SpecialValues.NaN)
                return NaN;
            switch (exponent.SpecialValue)
            {
                case SpecialValues.NaN:
                    return NaN;
                case SpecialValues.PositiveInfinity:
                    if (@base.IsZero)
                        return 0;
                    if (@base == 1)
                        return 1;
                    return PositiveInfinity;
                case SpecialValues.NegativeInfinity:
                    if (@base.IsZero)
                        return NaN;
                    if (@base == 1)
                        return 1;
                    return 0;
            }

            if (exponent.IsZero)
                return 1;

            if (exponent.IsNegative)
            {
                if (@base == 0)
                    return NaN;
                if (exponent == -1)
                    return 1;
                else
                    return 0;
                //throw new ArgumentOutOfRangeException("exponent", ResourceManager.GetMessage("ArgOutRang_ParamMustGreaterEqual", "exponent", "0"));
            }

            BigInteger bi = new BigInteger(1);
            for (BigInteger i = new BigInteger(0); i < exponent; ++i)
            {
                bi *= @base;
            }
            return bi;
        }

        /// <summary>
        /// Addiert 2 Instanzen der BigInteger-Klasse.
        /// </summary>
        /// <param name="bi1">Der erste Summand.</param>
        /// <param name="bi2">Der zweite Summand.</param>
        /// <returns>Die Summe aus <paramref name="bi1"/> und <paramref name="bi2"/>.</returns>
        [DebuggerStepThrough()]
        public static BigInteger Add(BigInteger bi1, BigInteger bi2)
        {
            if (bi1.SpecialValue != SpecialValues.None || bi2.SpecialValue != SpecialValues.None)
                switch (bi1.SpecialValue)
                {
                    case SpecialValues.NaN:
                        return NaN;
                    case SpecialValues.PositiveInfinity:
                        if (bi2.SpecialValue == SpecialValues.NegativeInfinity)
                            return NaN;
                        if (bi2 == 0)
                            return 0;
                        return PositiveInfinity;
                    case SpecialValues.NegativeInfinity:
                        if (bi2.SpecialValue == SpecialValues.PositiveInfinity)
                            return NaN;
                        if (bi2 == 0)
                            return 0;
                        return NegativeInfinity;
                    default:
                        switch (bi2.SpecialValue)
                        {
                            case SpecialValues.NaN:
                                return NaN;
                            case SpecialValues.PositiveInfinity:
                                if (bi1.SpecialValue == SpecialValues.NegativeInfinity)
                                    return NaN;
                                if (bi1 == 0)
                                    return 0;
                                return PositiveInfinity;
                            case SpecialValues.NegativeInfinity:
                                if (bi1.SpecialValue == SpecialValues.PositiveInfinity)
                                    return NaN;
                                if (bi1 == 0)
                                    return 0;
                                return NegativeInfinity;
                        }
                        break;
                }
            return AddWithoutSpecial(bi1, bi2);
        }

        private static BigInteger AddWithoutSpecial(BigInteger bi1, BigInteger bi2)
        {
            //result._data = new uint[Math.Max(bi1._data.Length, bi2._data.Length) + 1];//Größe der Zieldaten ermitteln + 1 Element für carry, also den Überfluss
            uint[] data = new uint[Math.Max(bi1.data.Length, bi2.data.Length) + 1];

            uint[] data1 = new uint[Math.Max(bi1.data.Length, bi2.data.Length) + 1];
            uint[] data2 = new uint[Math.Max(bi1.data.Length, bi2.data.Length) + 1];

            //Arrays mit den Null-Daten auffüllen
            for (int i = 0; i < data1.Length; ++i)
            {
                if (bi1.IsNegative)
                    data1[i] = 0xFFFFFFFF;
                else
                    data1[i] = 0x00000000;
                if (bi2.IsNegative)
                    data2[i] = 0xFFFFFFFF;
                else
                    data2[i] = 0x00000000;
            }

            //? Array.Copy(bi1._data, 0, data1, 0/*1*/, bi1._data.Length);
            //? Array.Copy(bi2._data, 0, data2, 0/*1*/, bi2._data.Length);
            Array.Copy(bi1.data.Reverse().ToArray(), 0, data1, data1.Length - bi1.data.Length, bi1.data.Length);
            Array.Copy(bi2.data.Reverse().ToArray(), 0, data2, data2.Length - bi2.data.Length, bi2.data.Length);

            long carry = 0;
            //for (int i = data.Length - 1; i >= 0; --i)
            //{
            //    long sum = (long)data1[i] + (long)data2[i] + carry;//2 Werte mit dem Rest addieren. Die 2 Werte sind je 4 Bytes groß, sodass nie die vollen 8 Bytes erreicht werden können.

            //    carry = sum >> 32;//Den Rest ermitteln, der nicht mehr ins aktuelle data passt.
            //    data[i] = (uint)(sum & 0xFFFFFFFF);//Daten ins Array eintragen
            //}
            for (int j = data.Length - 1; j >= 0; --j)//j für Zieldata da ctor datenarray dreht
            {
                long sum = (long)data1[j] + (long)data2[j] + carry;//2 Werte mit dem Rest addieren. Die 2 Werte sind je 4 Bytes groß, sodass nie die vollen 8 Bytes erreicht werden können.

                carry = sum >> 32;//Den Rest ermitteln, der nicht mehr ins aktuelle data passt.
                data[j] = (uint)(sum & 0xFFFFFFFF);//Daten ins Array eintragen
            }

            if (data.Length == 0 || (from x in data where x == 0x00000000 select x).Count() == data.Length)//Ergebnis ist 0
                return new BigInteger();

            bool? sign = null;
            //   uint[] resultData = null;
            //   int c = 0;

            if ((data.First() & 0x80000000) != 0)//IsBitSet(data.First(), 0)) //x if (carry == 1)//Last statt first, da data in ctor gedreht wird
            {
                sign = false;
            }
            else
            {
                sign = true;
            }

            BigInteger result = new BigInteger(data, sign);
            //result.RemoveEmptyDataFields();//Nicht nötig, da es im Konstruzktor aufgerufen wird
            //result._data = resultData;//Erzeugt, bevor Konstruktorüberladung existierte
            //result._sign = sign;
            return result;
        }

        /// <summary>
        /// Subtrahiert 2 Instanzen der BigInteger-Klasse von einander.
        /// </summary>
        /// <param name="bi1">Der Minuend.</param>
        /// <param name="bi2">Der Subtrahend.</param>
        /// <returns>Die Differenz zwischen <paramref name="bi1"/> und <paramref name="bi2"/>.</returns>
        [DebuggerStepThrough()]
        public static BigInteger Subtract(BigInteger bi1, BigInteger bi2)
        {
            return bi1 - bi2;
        }

        /// <summary>
        /// Multipliziert 2 Instanzen der BigInteger-Klasse.
        /// </summary>
        /// <param name="bi1">Der erste Faktor.</param>
        /// <param name="bi2">Der zweite Faktor.</param>
        /// <returns>Das Produkt aus <paramref name="bi1"/> und <paramref name="bi2"/>.</returns>
        [DebuggerStepThrough()]
        public static BigInteger Multiply(BigInteger bi1, BigInteger bi2)
        {
            if (bi1.SpecialValue != SpecialValues.None || bi2.SpecialValue != SpecialValues.None)
                switch (bi1.SpecialValue)
                {
                    case SpecialValues.NaN:
                        return NaN;
                    case SpecialValues.PositiveInfinity:
                        if (bi2.SpecialValue == SpecialValues.NaN)
                            return NaN;
                        if (bi2.IsZero)
                            return 0;
                        if (bi2.IsNegative)
                            return NegativeInfinity;
                        else
                            return PositiveInfinity;
                    case SpecialValues.NegativeInfinity:
                        if (bi2.SpecialValue == SpecialValues.NaN)
                            return NaN;
                        if (bi2.IsZero)
                            return 0;
                        if (bi2.IsNegative)
                            return PositiveInfinity;
                        else
                            return NegativeInfinity;
                    default:
                        switch (bi2.SpecialValue)
                        {
                            case SpecialValues.NaN:
                                return NaN;
                            case SpecialValues.PositiveInfinity:
                                if (bi1.IsZero)
                                    return 0;
                                if (bi1.IsNegative)
                                    return NegativeInfinity;
                                else
                                    return PositiveInfinity;
                            case SpecialValues.NegativeInfinity:
                                if (bi1.IsZero)
                                    return 0;
                                if (bi1.IsNegative)
                                    return PositiveInfinity;
                                else
                                    return NegativeInfinity;
                            default:
                                throw new ArithmeticException(ResourceManager.GetMessage("Arith_System"));
                        }
                }

            return MultiplyWithoutSpecial(bi1, bi2);
        }
        private static BigInteger MultiplyWithoutSpecial(BigInteger bi1, BigInteger bi2)
        {

            if (bi1.IsZero || bi2.IsZero)//Wenn ein oder beide Werte 0 sind, dann 0 zurück geben.
                return new BigInteger();

            bool bi1Neg = false, bi2Neg = false;

            //Absolute Werte berechnen
            if (bi1.IsNegative)     // bi1 negativ
                bi1Neg = true;
            if (bi2.IsNegative)     // bi2 negativ
                bi2Neg = true;

            bi1 = BigInteger.Abs(bi1);
            bi2 = BigInteger.Abs(bi2);

            BigInteger result = new BigInteger();//Rückgabewert
            result.data = new uint[bi1.data.Length + bi2.data.Length];
            result._sign = true;

            // Absolute Werte multiplizieren
            for (int i = 0; i < bi1.data.Length; i++)
            {
                if (bi1.data[i] == 0) continue;

                ulong mcarry = 0;
                for (int j = 0, k = i; j < bi2.data.Length; j++, k++)
                {
                    // k = i + j
                    ulong val = ((ulong)bi1.data[i] * (ulong)bi2.data[j]) + (ulong)result.data[k] + mcarry;

                    result.data[k] = (uint)(val & 0xFFFFFFFF);
                    mcarry = (val >> 32);
                }

                if (mcarry != 0)
                    result.data[i + bi2.data.Length] = (uint)mcarry;
            }
            result.RemoveEmptyDataFields();

            if (bi1Neg != bi2Neg) //Unterschiedliche Werte > Negatives Ergebnis
                return -result;
            return result;
        }

        /// <summary>
        /// Negiert den Wert einer BigInteger-Instanz.
        /// </summary>
        /// <param name="bi">Der zu negierende Wert.</param>
        /// <returns>Der Negierte Wert von <paramref name="bi"/>.</returns>
        [DebuggerStepThrough()]
        public static BigInteger Negate(BigInteger bi)
        {
            return -bi;
        }

        /// <summary>
        /// Berechnet den Quotienten aus 2 Instanzen der BigInteger-Klasse.
        /// </summary>
        /// <param name="bi1">Der Dividend.</param>
        /// <param name="bi2">Der Divisor.</param>
        /// <returns>Der Quotient aus <paramref name="bi1"/> und <paramref name="bi2"/>.</returns>
        /// <remarks>
        /// Wenn das ergebnis Nachkommastellen hat, dann werden diese einfach abgeschnitten.
        /// </remarks>
        [DebuggerStepThrough()]
        public static BigInteger Divide(BigInteger bi1, BigInteger bi2)
        {
            return bi1 / bi2;
        }

        /// <summary>
        /// Berechnet den Rest einer Division.
        /// </summary>
        /// <param name="bi1">Der Dividend.</param>
        /// <param name="bi2">Der Divisor.</param>
        /// <returns>Der Rest, der bei der Division von <paramref name="bi1"/> durch <paramref name="bi2"/> übrig bleibt.</returns>
        [DebuggerStepThrough()]
        public static BigInteger Mod(BigInteger bi1, BigInteger bi2)
        {
            return bi1 % bi2;
        }

        /// <summary>
        /// Erhöht den Wert einer BigInteger-Instanz um 1.
        /// </summary>
        /// <param name="bi">Die zu erniedrigende BigInteger-Instanz.</param>
        /// <returns>Der Wert von <paramref name="bi"/> erhöht um 1.</returns>
        public static BigInteger Increment(BigInteger bi)
        {
            return ++bi;
        }

        /// <summary>
        /// Erniedrigt den Wert einer BigInteger-Instanz um 1.
        /// </summary>
        /// <param name="bi">Die zu erniedrigende BigInteger-Instanz.</param>
        /// <returns>Der Wert von <paramref name="bi"/> niedrigt um 1.</returns>
        public static BigInteger Decrement(BigInteger bi)
        {
            return --bi;
        }

        /// <summary>
        /// Verschiebt die Datenbits um die angegebenen Stellen nach links. Die rechten Stellen werden mit 0 aufgefüllt.
        /// </summary>
        /// <param name="bi1">Der Wert, deren Daten verschoben werden sollen.</param>
        /// <param name="shiftVal">Die Anzahl der Stellen, um die die Datenbits verschoben werden sollen.</param>
        public static BigInteger LeftShift(BigInteger bi1, int shiftVal)
        {
            return bi1 << shiftVal;
        }

        /// <summary>
        /// Verschiebt die Datenbits um die angegebenen Stellen nach rechts. Die rechten Stellen werden mit 0 aufgefüllt.
        /// </summary>
        /// <param name="bi1">Der Wert, deren Daten verschoben werden sollen.</param>
        /// <param name="shiftVal">Die Anzahl der Stellen, um die die Datenbits verschoben werden sollen.</param>
        public static BigInteger RightShift(BigInteger bi1, int shiftVal)
        {
            return bi1 >> shiftVal;
        }

        /// <summary>
        /// Kehrt alle Bits im Datenpuffer um und tauscht das Vorzeichen. Sollte de Wert 0 sein, wird 0 zurück gegeben.
        /// </summary>
        /// <param name="bi1">Der Wert, deren Bits und Vorzeichen umgekehrt werden sollen.</param>
        public static BigInteger OnesComplement(BigInteger bi1)
        {
            return ~bi1;
        }

        /// <summary>
        /// Setzt ein Bit im Ergebnis auf True, wenn genau ein Bit an der selben Position in <paramref name="bi1"/> oder <paramref name="bi2"/> gesetzt ist.
        /// </summary>
        /// <param name="bi1">Der erste Operand.</param>
        /// <param name="bi2">Der zweite Operand.</param>
        public static BigInteger Xor(BigInteger bi1, BigInteger bi2)
        {
            return bi1 ^ bi2;
        }

        /// <summary>
        /// Setzt ein Bit im Ergebnis auf True, wenn die jeweiligen Bits in <paramref name="bi1"/> und <paramref name="bi2"/> gesetzt sind.
        /// </summary>
        /// <param name="bi1">Der erste Operand.</param>
        /// <param name="bi2">Der zweite Operand.</param>
        public static BigInteger BitwiseAnd(BigInteger bi1, BigInteger bi2)
        {
            return bi1 & bi2;
        }

        /// <summary>
        /// Setzt ein Bit im Ergebnis auf True, wenn mindestens eines der jeweiligen Bits in <paramref name="bi1"/> und <paramref name="bi2"/> gesetzt ist.
        /// </summary>
        /// <param name="bi1">Der erste Operand.</param>
        /// <param name="bi2">Der zweite Operand.</param>
        public static BigInteger BitwiseOr(BigInteger bi1, BigInteger bi2)
        {
            return bi1 | bi2;
        }

        /// <summary>
        /// Berechnet eine Fakultät.
        /// </summary>
        /// <param name="bi">Ein Wert, deren Fakulät berechnet werden soll.</param>
        /// <returns>Die Fakulät von <paramref name="bi"/>.</returns>
        public static BigInteger Factorial(BigInteger bi)
        {
            if (bi.SpecialValue == SpecialValues.PositiveInfinity)
                return PositiveInfinity;
            if (bi.IsNegative || bi.SpecialValue == SpecialValues.NaN) // (n)! = n.d. ; n < 0 oder n = n.d.
                //throw new ArgumentOutOfRangeException("n", ResourceManager.GetMessage("ArgOutRang_ParamMustGreaterEqual", "n", "0"));
                return NaN;
            if (bi.IsZero) // 0! = 1
                return new BigInteger(1);
            BigInteger result = new BigInteger(1);
            for (; bi > 0; --bi)
                result *= bi;
            return result;
        }

        /// <summary>
        /// Ermittelt die Primfaktoren eines Wertes.
        /// </summary>
        /// <param name="bi">Der Wert, dessen Primfaktoren ermittelt werden sollen.</param>
        /// <returns>Die Primfaktoren von <paramref name="bi"/>.</returns>
        public static ListSet<BigInteger> PrimeFactors(BigInteger bi)
        {
            if (bi.SpecialValue != SpecialValues.None || bi == 1)//http://de.wikipedia.org/wiki/Primfaktorzerlegung
                return ListSet<BigInteger>.Empty;
            if (bi < 1)
                //throw new ArgumentOutOfRangeException("x", ResourceManager.GetMessage("ArgOutRang_ParamMustGreaterEqual", "bi", "1"));
                return ListSet<BigInteger>.NaN;
            ListSet<BigInteger> list = new ListSet<BigInteger>() { AcceptDuplicates = true, };

            while (bi % 2 == 0) //? 2 entfernen
            {
                list.Add(2);
                bi /= 2;
            }

            for (BigInteger i = 3; i <= bi && bi != 1; ) //? Ungerade Zahlen auf Primzahlen prüfen und ggf. entfernen/auflisten
            {
                if (bi % i == 0) // ist Zahl durch i Teilbar?
                {
                    list.Add(i); // Zur Liste hinzufügen
                    bi /= i; // Faktor von Zahl entfernen
                }
                else
                {
                    i += 2; // Nächste ungerade Zahl
                }
            }
            return list; // Als Array zurück geben
        }

        /// <summary>
        /// Überprüft ob in einem Array von Zahlen ein spezieller Wert enthalten ist.
        /// </summary>
        private static bool CheckIfContainsNotNoneValue(BigInteger[] numbers)
        {
            return numbers.Contains(BigInteger.NaN, new LambdaEqualityComparer<BigInteger>((x, y) => x.SpecialValue == y.SpecialValue, x => (int)x.SpecialValue))
             || numbers.Contains(BigInteger.NegativeInfinity, new LambdaEqualityComparer<BigInteger>((x, y) => x.SpecialValue == y.SpecialValue, x => (int)x.SpecialValue))
             || numbers.Contains(BigInteger.PositiveInfinity, new LambdaEqualityComparer<BigInteger>((x, y) => x.SpecialValue == y.SpecialValue, x => (int)x.SpecialValue));
        }

        /// <summary>
        /// Ermittelt das kleinste gemeinsamme Vielfache von belibig vielen Werten.
        /// </summary>
        /// <remarks>Der zurück gegebene Wert ist immer Positiv. Unabhängig von den übergebenen Werten.</remarks>
        /// <param name="numbers">Die Werte, deren kleinstes, gemeinsammes Vielfaches ermittelt werden soll.</param>
        /// <returns>Das kleinste gemeinsame Vielfache von <paramref name="numbers"/>.</returns>
        public static BigInteger SmallestCommonMultiple(ListSet<BigInteger> numbers)
        {
            return SmallestCommonMultiple(numbers.ToArray());
        }

        /// <summary>
        /// Ermittelt das kleinste gemeinsamme Vielfache von belibig vielen Werten.
        /// </summary>
        /// <remarks>Der zurück gegebene Wert ist immer Positiv. Unabhängig von den übergebenen Werten.</remarks>
        /// <param name="numbers">Die Werte, deren kleinstes, gemeinsammes Vielfaches ermittelt werden soll.</param>
        /// <returns>Das kleinste gemeinsame Vielfache von <paramref name="numbers"/>.</returns>
        public static BigInteger SmallestCommonMultiple(params BigInteger[] numbers)
        {
            if (numbers == null)
                throw new ArgumentNullException("numbers", ResourceManager.GetMessage("ArgNull_ArrayNull", "numbers"));
            if (numbers.Length == 0)
                throw new ArgumentOutOfRangeException("numbers", ResourceManager.GetMessage("ArgOutRang_ParamMustGreater", "numbers.Length", "0"));

            if (numbers.Contains(0))
                throw new ArithmeticException(ResourceManager.GetMessage("Arith_SCMContainsZero"));

            if (CheckIfContainsNotNoneValue(numbers))
                return BigInteger.NaN;

            BigInteger[][] lists = (from x in numbers
                                    where x != 1 // 1 hat keine Auswirkungen
                                    select PrimeFactors(BigInteger.Abs(x)).ToArray()).ToArray();//Alle Primfaktoren ermitteln

            Dictionary<BigInteger, uint> list = new Dictionary<BigInteger, uint>();

            foreach (BigInteger[] l in lists)//Alle Primfaktoren-Listen durchgehen
            {
                uint n = 1;
                BigInteger cur = 0;
                if (l.Length == 0)
                    continue;// Zahl ist eine 1
                foreach (BigInteger i in l)//Alle Primfaktoren durchgehen
                {
                    if (cur == i)
                        ++n;
                    else
                    {
                        if (cur != 0)
                        {
                            if (list.ContainsKey(cur))//Enthält die Liste bereits die Zahl?
                            {
                                if (list[cur] <= n)//Zahl bereits enthalten und bisherige Anzahl kleiner als die aktuelle
                                    list[cur] = n;//Neue Zuweisung der Anzahl.
                            }
                            else
                            {
                                list.Add(cur, n);//Neue Zahl zur Liste hinzufügen.
                            }
                        }
                        n = 1;
                        cur = i;
                    }
                }
                if (list.ContainsKey(cur))//Enthält die Liste bereits die Zahl?
                {
                    if (list[cur] <= n)//Zahl bereits enthalten und bisherige Anzahl kleiner als die aktuelle
                        list[cur] = n;//Neue Zuweisung der Anzahl.
                }
                else
                {
                    list.Add(cur, n);//Neue Zahl zur Liste hinzufügen.
                }
            }
            BigInteger result = 1;
            foreach (KeyValuePair<BigInteger, uint> l in list)//Alle Werte durchgehen
                result *= Pow(l.Key, l.Value);//Potenzieren und multiplizieren der Werte

            return result;//Wert zurück geben
        }

        /// <summary>
        /// Ermittelt den größten gemeinsamen Teiler von beliebig vielen Werten. 
        /// </summary>
        /// <remarks>Der Ergebnis ist immer Positiv.
        /// Wenn nur der Wert 0 (egal wie oft= übergeben wird, wird eine ArithmeticException ausgelöst.</remarks>
        /// <param name="numbers">Die Zahlen, dessen ggT ermittelt werden soll.</param>
        /// <returns>Der ggT von <paramref name="numbers"/>.</returns>
        public static BigInteger GreatestCommonDivisor(ListSet<BigInteger> numbers)
        {
            return GreatestCommonDivisor(numbers.ToArray());
        }

        /// <summary>
        /// Ermittelt den größten gemeinsamen Teiler von 2 ganzen Zahlen.
        /// </summary>
        /// <param name="num1">Die erste Zahl.</param>
        /// <param name="num2">Die zweite Zahl.</param>
        /// <returns>Der ggT von <paramref name="num1"/> und <paramref name="num2"/>.</returns>
        public static BigInteger GreatestCommonDivisor(BigInteger num1, BigInteger num2)
        {
            num1 = BigInteger.Abs(num1);
            num2 = BigInteger.Abs(num2);

            if (num1 == 1)
                return 1;
            if (num2 == 1)
                return 1;
            if (num1.IsZero && num2.IsZero)
                throw new ArithmeticException(ResourceManager.GetMessage("Arith_GCDConatinsOnlyZero"));

            Prime primes = new Prime(true);
            int i = 0;
            BigInteger result = new BigInteger(1);
            BigInteger test1 = new BigInteger(1), test2 = new BigInteger(1);
            while (!primes.Contains(num1) && !primes.Contains(num2) && num1 != 1 && num2 != 1)
            {
                if (num1 % primes[i] == 0)
                {
                    test1 *= primes[i];
                    num1 /= primes[i];
                    if (num2 % primes[i] == 0)
                    {
                        test2 *= primes[i];
                        num2 /= primes[i];
                        result *= primes[i];
                    }
                }
                else if (num2 % primes[i] == 0)
                {
                    test2 *= primes[i];
                    num2 /= primes[i];
                }
                else
                {
                    ++i;
                }
            }
            if (num1 % primes[i] == 0 && num2 % primes[i] == 0)
            {
                result *= primes[i];
            }
            return result;
        }

        /// <summary>
        /// Ermittelt den größten gemeinsamen Teiler von beliebig vielen Werten. 
        /// </summary>
        /// <remarks>Der Ergebnis ist immer Positiv.
        /// Wenn nur der Wert 0 (egal wie oft= übergeben wird, wird eine ArithmeticException ausgelöst.</remarks>
        /// <param name="numbers">Die Zahlen, dessen ggT ermittelt werden soll.</param>
        /// <returns>Der ggT von <paramref name="numbers"/>.</returns>
        public static BigInteger GreatestCommonDivisor(params BigInteger[] numbers)
        {
            if (numbers == null)
                throw new ArgumentNullException("numbers", ResourceManager.GetMessage("ArgNull_ArrayNull", "numbers"));
            if (numbers.Length == 0)
                throw new ArgumentOutOfRangeException("numbers", ResourceManager.GetMessage("ArgOutRang_ParamMustGreater", "numbers.Length", "0"));

            if (numbers.Contains(BigInteger.NaN, new LambdaEqualityComparer<BigInteger>((x, y) => x.SpecialValue == y.SpecialValue, x => (int)x.SpecialValue)))
                return BigInteger.NaN;

            if (numbers.Length == 1)
                if (numbers[0] == 0)
                    throw new ArithmeticException(ResourceManager.GetMessage("Arith_GCDConatinsOnlyZero"));
                else
                    if (numbers[0].SpecialValue == SpecialValues.None)
                        return numbers[0];
                    else
                        return NaN;

            List<BigInteger>[] lists = (from x in numbers
                                        where x != 1 && x != 0 && x.SpecialValue != SpecialValues.NegativeInfinity && x.SpecialValue != SpecialValues.PositiveInfinity // 1 hat keine Auswirkungen und 0 auch nicht (Inf auch nicht)
                                        select PrimeFactors(BigInteger.Abs(x)).ToList()).ToArray();//Alle Primfaktoren ermitteln
            if (lists.Length == 0)//Keine Elemente von denen ein ggT ermittelt werden kann/braucht
            {
                if (numbers.Contains(1))//Enthält 1
                    return 1;
                else if (numbers.Contains(0))
                    return 0;
                else
                    return NaN;
                //throw new ArithmeticException(ResourceManager.GetMessage("Arith_GCDConatinsOnlyZero"));
            }

            //x List<BigInteger> factors = new List<BigInteger>();//Faktor

            //Ermitteln, welche Faktoren in jeder Zahl enthalten sind.
            BigInteger result = new BigInteger(1);
            foreach (BigInteger factor in lists[0])
            {
                bool flag = true;
                for (int i = 1; i < lists.Length; ++i)
                {
                    if (lists[i].Contains(factor))
                    {
                        lists[i].Remove(factor);
                    }
                    else
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                    result *= factor;
                //foreach (BigInteger[] el in lists)
                //{
                //    if (!el.Contains(factor))
                //    {
                //        flag = false;
                //        break;
                //    }
                //}
                //if (flag)
                //    factors.Add(factor);
            }

            //Die minimalen Vorkommen der Faktoren zählen
            //BigInteger result = new BigInteger(1);
            //for (int i = 0; i < factors.Count; ++i)
            //{
            //    uint c = uint.MaxValue;
            //    foreach (List<BigInteger> el in lists)
            //    {
            //        c = (uint)Math.Min(c, el.Count(x => x == factors.ElementAt(i)));
            //    }
            //    result *= BigInteger.Pow(factors.ElementAt(i), c);
            //}
            return result;
        }

        /// <summary>
        /// Gibt den kleinsten, aufgelisteten Wert von <paramref name="numbers"/> zurück.
        /// </summary>
        /// <param name="numbers">Eine Liste von Werten.</param>
        /// <returns>Der kleinste Wert, welcher in <paramref name="numbers"/> enthalten ist.</returns>
        public static BigInteger Min(params BigInteger[] numbers)
        {
            return numbers.Min();
        }

        /// <summary>
        /// Gibt den größten, aufgelisteten Wert von <paramref name="numbers"/> zurück.
        /// </summary>
        /// <param name="numbers">Eine Liste von Werten.</param>
        /// <returns>Der größte Wert, welcher in <paramref name="numbers"/> enthalten ist.</returns>
        public static BigInteger Max(params BigInteger[] numbers)
        {
            return numbers.Max();
        }

        /// <summary>
        /// Ruft den Signierten Wert einer Ganzzahl ab.
        /// </summary>
        /// <param name="value">Die Zahl deren signierter Wert ermittelt werden soll.</param>
        /// <returns><c>-1</c> wenn <paramref name="value"/> kleiner ist als <c>0</c>. <para/>
        /// <c>0</c> wenn <paramref name="value"/> <c>0</c> ist.<para/>
        /// <c>1</c> wenn <paramref name="value"/> größer als <c>0</c> ist.</returns>
        public static BigInteger Sgn(BigInteger value)
        {
            if (value < 0)
                return -1;
            if (value > 0)
                return 1;
            return 0;
        }

        #endregion

        #region Statische Eigenschaften

        /// <summary>
        /// Ruft das standartmäßig zu verwendende Ziffernset ab oder legt dieses fest. 
        /// </summary>
        public const string DigitSet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Stellt eine unveränderbare Instanz der BigInteger-Struktur dar, deren Wert 0 ist.
        /// </summary>
        public static BigInteger Empty
        {
            get
            {
                return _Empty;
            }
        }
        private static readonly BigInteger _Empty = new BigInteger();// { get; private set; }

        /// <summary>
        /// Stellt eine unveränderbare Instanz der BigInteger-Struktur dar, deren Wert keine Zahl ist.
        /// </summary>
        /// <remarks>
        /// Zur Abfrage ob ein Wert keine Zahl ist, verwenden Sie die <see cref="SpecialValue"/>-Eigenschaft.
        /// </remarks>
        public static BigInteger NaN
        {
            get
            {
                return _NaN;
            }
        }
        private static readonly BigInteger _NaN = new BigInteger() { _SpecialValue = SpecialValues.NaN, };
        /// <summary>
        /// Stellt eine unveränderbare Instanz der BigInteger-Struktur dar, deren Wert positiv Unendlich ist.
        /// </summary>
        /// <remarks>
        /// Zur Abfrage ob ein Wert positiv Unendlich ist, verwenden Sie die <see cref="SpecialValue"/>-Eigenschaft.
        /// </remarks>
        public static BigInteger PositiveInfinity
        {
            get
            {
                return _PositiveInfinity;
            }
        }
        private static readonly BigInteger _PositiveInfinity = new BigInteger() { _SpecialValue = SpecialValues.PositiveInfinity, };
        /// <summary>
        /// Stellt eine unveränderbare Instanz der BigInteger-Struktur dar, deren Wert negativ Unendlich ist.
        /// </summary>
        /// <remarks>
        /// Zur Abfrage ob ein Wert negativ Unendlich ist, verwenden Sie die <see cref="SpecialValue"/>-Eigenschaft.
        /// </remarks>
        public static BigInteger NegativeInfinity
        {
            get
            {
                return _NegativeInfinity;
            }
        }
        private static readonly BigInteger _NegativeInfinity = new BigInteger() { _SpecialValue = SpecialValues.NegativeInfinity, };

        #endregion

        #region Methoden

        /// <summary>
        /// Gibt den Wert dieser Instanz in einem bestimmten Zahlensystem zurück.
        /// </summary>
        /// <param name="radix">Die Basis des Zahlensystems. Diese muss größer gleich 2 und kleiner gleich DigitSet.Length sein.</param>
        /// <returns>Eine Zeichenfolge welche zur angegebenen Basis den Wert dieser Instanz repräsentiert.</returns>
        
        public string ToString(uint radix, bool commaSeparated)
        {
            return this.ToString(radix, DigitSet, commaSeparated);
        }
        /// <summary>
        /// Gibt den Wert dieser Instanz in einem bestimmten Zahlensystem zurück.
        /// </summary>
        /// <param name="radix">Die Basis des Zahlensystems. Diese muss größer gleich 2 und kleiner gleich DigitSet.Length sein.</param>
        /// <param name="digits">Die zu verwendenden Zeichen in der zu erzegenden Zeichenfolge. Dieses Ziffernset darf keine doppelten Zeichen enthalten. Beim abgleich wird nicht auf die Groß- und Kleinschreibung geachtet.</param>
        /// <returns>Eine Zeichenfolge welche zur angegebenen Basis den Wert dieser Instanz repräsentiert.</returns>
        
        public string ToString(uint radix, string digits, bool commaSeparated)
        {
            switch (this.SpecialValue)
            {
                case SpecialValues.NaN:
                    return "NaN";
                case SpecialValues.PositiveInfinity:
                    return "+∞";
                case SpecialValues.NegativeInfinity:
                    return "-∞";
                default:
                    if (string.IsNullOrEmpty(digits))
                        throw new ArgumentNullException("digits", ResourceManager.GetMessage("ArgNull_ParamCantNull", "digits"));
                    if (radix < 2 || radix > digits.Length)
                        throw (new ArgumentOutOfRangeException("radix/digits", ResourceManager.GetMessage("ArgOutRang_ParamMustInRange", "radix", "2", "digits.Length (" + digits.Length + ")")));
                    if (digits.ToUpper().ToCharArray().ContainsDublicates())
                        throw new ArgumentException(ResourceManager.GetMessage("Arg_CantContainsDuplicates", "digits"), "digits");

                    if (this.IsZero)
                        return "0";

                    string result = "";

                    BigInteger a;
                    if (this.IsNegative)
                        a = -this;
                    else
                        a = this;

                    //x  a._data = a._data.Reverse().ToArray(); //Anpassen, da in meinem Typ die Elemente im Array "vekehrt" herum gespeichert werden.

                    BigInteger quotient = new BigInteger();
                    BigInteger remainder = new BigInteger();
                    ulong biRadix = radix;
					// BigInteger biRadix = radix;
					int i = 0;
                    while (a.data.Length >= 1 && !a.IsZero)
                    {
                        // DivideByte (ref a, ref biRadix, ref quotient, ref remainder);
                        ToStringHelper(a, biRadix, ref quotient, ref remainder);

                        result = digits[(int)remainder.data[0]] + result;

						a = quotient;

						if(commaSeparated && ++i == 3 && a.data.Length >= 1 && !a.IsZero) {
							result = "," + result;
							i = 0;
						}
                    }
                    if (this.IsNegative)
                        result = "-" + result;

                    return result;
            }
        }

        #endregion

        #region operators

        #region implicit

        /// <summary>
        /// Erstellt eine neue Instanz der BigInteger-Klasse mit dem angegebenen Wert.
        /// </summary>
        /// <param name="value">Der Wert, welcher der neuen Instanz zugewiesen werden soll.</param>
        /// <returns>Eine neue BigInteger-Instanz, welche den selben Wert wie <paramref name="value"/> aufweißt.</returns>
        public static implicit operator BigInteger(int value)
        {
            return new BigInteger(value);
        }

        /// <summary>
        /// Erstellt eine neue Instanz der BigInteger-Klasse mit dem angegebenen Wert.
        /// </summary>
        /// <param name="value">Der Wert, welcher der neuen Instanz zugewiesen werden soll.</param>
        /// <returns>Eine neue BigInteger-Instanz, welche den selben Wert wie <paramref name="value"/> aufweißt.</returns>
        
        public static implicit operator BigInteger(uint value)
        {
            return new BigInteger(value);
        }

        /// <summary>
        /// Erstellt eine neue Instanz der BigInteger-Klasse mit dem angegebenen Wert.
        /// </summary>
        /// <param name="value">Der Wert, welcher der neuen Instanz zugewiesen werden soll.</param>
        /// <returns>Eine neue BigInteger-Instanz, welche den selben Wert wie <paramref name="value"/> aufweißt.</returns>
        public static implicit operator BigInteger(long value)
        {
            return new BigInteger(value);
        }

        /// <summary>
        /// Erstellt eine neue Instanz der BigInteger-Klasse mit dem angegebenen Wert.
        /// </summary>
        /// <param name="value">Der Wert, welcher der neuen Instanz zugewiesen werden soll.</param>
        /// <returns>Eine neue BigInteger-Instanz, welche den selben Wert wie <paramref name="value"/> aufweißt.</returns>
        
        public static implicit operator BigInteger(ulong value)
        {
            return new BigInteger(value);
        }

        #endregion

        #region explicit

        /// <summary>
        /// Gibt die ersten 4 Bytes des Datenarrays der angegebenen BigInteger-Instanz zurück.
        /// </summary>
        /// <param name="value">Die zu verarbeitende BigInteger-Instanz.</param>
        /// <returns>Die ersten 4 Bytes von <paramref name="value"/>.</returns>
        
        public static explicit operator uint(BigInteger value)
        {
            if (value.SpecialValue != SpecialValues.None)
                throw new ArgumentOutOfRangeException(ResourceManager.GetMessage("ArgOutRang_InvalidSpecialValueCast"));

            if (value._sign == null)
                return 0;
            return value.data[0];//Erste 4 Bytes zurück geben.
        }

        /// <summary>
        /// Gibt die ersten 8 Bytes des Datenarrays der angegebenen BigInteger-Instanz zurück.
        /// </summary>
        /// <param name="value">Die zu verarbeitende BigInteger-Instanz.</param>
        /// <returns>Die ersten 8 Bytes von <paramref name="value"/>.</returns>
        
        public static explicit operator ulong(BigInteger value)
        {
            if (value.SpecialValue != SpecialValues.None)
                throw new ArgumentOutOfRangeException(ResourceManager.GetMessage("ArgOutRang_InvalidSpecialValueCast"));

            if (value._sign == null)
                return 0;
            if (value.data.Length == 1)
                return value.data[0];//Erste 4 Bytes zurück geben.
            return value.data[0] + ((ulong)value.data[1] << 32);//Erste 8 Bytes zurück geben.
        }

        #endregion

        #region unary, relation, shift, ...

        /// <summary>
        /// Addiert 2 Instanzen der <see cref="Koopakiller.Numerics.BigInteger"/>-Klasse.
        /// </summary>
        /// <param name="bi1">Der erste Summand der Rechnung.</param>
        /// <param name="bi2">Der zweite Summand der Rechnung.</param>
        /// <returns>Die Summe von <paramref name="bi1"/> und <paramref name="bi2"/>.</returns>
        public static BigInteger operator +(BigInteger bi1, BigInteger bi2)
        {
            return BigInteger.Add(bi1, bi2);

        }

        /// <summary>
        /// Erhöht den Wert einer Zahl um 1.
        /// </summary>
        /// <param name="bi1">Die zu erhöhende Zahl.</param>
        /// <returns><paramref name="bi1"/> erhöht um 1.</returns>
        public static BigInteger operator ++(BigInteger bi1)
        {
            return bi1 + 1;
        }

        /// <summary>
        /// Subtrahiert 2 Instanzen der <see cref="Koopakiller.Numerics.BigInteger"/>-Klasse.
        /// </summary>
        /// <param name="bi1">Der Minuend der Rechnung.</param>
        /// <param name="bi2">Der Subtrahend der Rechnung.</param>
        /// <returns>Die Differenz von <paramref name="bi1"/> und <paramref name="bi2"/>.</returns>
        public static BigInteger operator -(BigInteger bi1, BigInteger bi2)
        {
            //! #warning Effektivieren?
            return bi1 + (-bi2);
        }

        /// <summary>
        /// Negiert eine Zahl.
        /// </summary>
        /// <param name="bi1">Der zu negierende Betrag.</param>
        /// <returns>Der negierte Wert von <paramref name="bi1"/>.</returns>
        public static BigInteger operator -(BigInteger bi1)
        {
            if (bi1.SpecialValue != SpecialValues.None)
                switch (bi1.SpecialValue)
                {
                    case SpecialValues.NaN:
                        return NaN;
                    case SpecialValues.PositiveInfinity:
                        return NegativeInfinity;
                    case SpecialValues.NegativeInfinity:
                        return PositiveInfinity;
                }

            if (bi1.IsZero)//0 einfach wieder zurück geben.
                return bi1;

            BigInteger result = new BigInteger(bi1);

            //Einerkomplement erzeugen und anschließend 1 hinzufügen
            for (int i = 0; i < bi1.data.Length; i++)
                result.data[i] = (uint)(~(bi1.data[i]));

            long val, carry = 1;
            int index = 0;

            while (carry != 0 && index < bi1.data.Length)
            {
                val = (long)(result.data[index]);
                ++val;

                result.data[index] = (uint)(val & 0xFFFFFFFF);
                carry = val >> 32;

                index++;
            }
            result._sign = !(bool)bi1._sign;
            return result;
        }

        /// <summary>
        /// Erhöht den Wert einer BigInteger-Instanz um 1.
        /// </summary>
        /// <param name="bi">Die zu erniedrigende BigInteger-Instanz.</param>
        /// <returns>Der Wert von <paramref name="bi"/> erhöht um 1.</returns>
        public static BigInteger operator --(BigInteger bi)
        {
            return bi - 1;
        }

        /// <summary>
        /// Multipliziert 2 Instanzen der <see cref="Koopakiller.Numerics.BigInteger"/>-Klasse.
        /// </summary>
        /// <param name="bi1">Der erste Faktor der Rechnung.</param>
        /// <param name="bi2">Der zweite Faktor der Rechnung.</param>
        /// <returns>Das Produkt von <paramref name="bi1"/> und <paramref name="bi2"/>.</returns>
        public static BigInteger operator *(BigInteger bi1, BigInteger bi2)
        {
            return BigInteger.Multiply(bi1, bi2);
        }

        /// <summary>
        /// Berechnet den Rest einer Division von 2 Instanzen der BigInteger-Klasse.
        /// </summary>
        /// <param name="bi1">Der Dividend der Rechnung.</param>
        /// <param name="bi2">Der Divisor der Rechnung.</param>
        /// <returns>Der Rest der Division.</returns>
        public static BigInteger operator %(BigInteger bi1, BigInteger bi2)
        {
            if (bi1.SpecialValue != SpecialValues.None || bi2.SpecialValue != SpecialValues.None)
                switch (bi1.SpecialValue)
                {
                    case SpecialValues.NaN:
                    case SpecialValues.PositiveInfinity:
                    case SpecialValues.NegativeInfinity:
                        return NaN;
                    default:
                        switch (bi2.SpecialValue)
                        {
                            case SpecialValues.NaN:
                                return NaN;
                            case SpecialValues.PositiveInfinity: //  x / unendlich = 0
                            case SpecialValues.NegativeInfinity:
                                return bi1;
                            default:
                                if (bi2.IsZero)
                                    return NaN;
                                break;
                        }
                        break;
                }

            if (bi2.IsZero)
                return NaN;

            //Wenn der Dividend negativ ist, dann ist es auch das Ergebnis. Divisor spielt keine Rolle.
            bool dividendNeg = false;

            if (bi1.IsNegative)
            {
                bi1 = -bi1;
                dividendNeg = true;
            }
            if (bi2.IsNegative)
                bi2 = -bi2;

            //Variablen für die Rückgabe
            BigInteger quotient = new BigInteger();
            quotient._data = new uint[bi1._data.Length];
            BigInteger remainder = new BigInteger(bi1);

            if (bi1 < bi2)
                return remainder;//Der Rest entspricht dem Dividenden, da der Divisor größer ist als der Dividend.

            bi2.RemoveEmptyDataFields();//Sicher ist sicher
            if (bi2._data.Length == 1)//Wenn der Divisor höchstens 4 Bytes groß ist dann entsprechende Methode aufrufen.
                DivideByte(ref bi1, ref bi2, ref  quotient, ref  remainder);
            else
                DivideBytes(ref bi1, ref  bi2, ref quotient, ref  remainder);

            if (dividendNeg)//Ergebnis ist negativ.
                return -remainder;

            return remainder;

        }

        /// <summary>
        /// Berechnet den Quotienten von 2 Instanzen der BigInteger-Klasse.
        /// </summary>
        /// <param name="bi1">Der Dividend der Rechnung.</param>
        /// <param name="bi2">Der Divisor der Rechnung.</param>
        /// <returns>Der Quotient aus dem Dividenden <paramref name="bi1"/> und dem Divisor <paramref name="bi2"/>.</returns>
        /// <remarks>Nachkommastellen werden abgeschnitten.</remarks>
        public static BigInteger operator /(BigInteger bi1, BigInteger bi2)
        {
            if (bi1.SpecialValue != SpecialValues.None || bi2.SpecialValue != SpecialValues.None)
                switch (bi1.SpecialValue)
                {
                    case SpecialValues.NaN:
                        return NaN;
                    case SpecialValues.PositiveInfinity:
                        if (bi2.SpecialValue == SpecialValues.None)
                        {
                            if (bi2.IsZero)
                                return NaN;
                            if (bi2.IsNegative)
                                return NegativeInfinity;
                            else
                                return PositiveInfinity;
                        }
                        else
                            return NaN;
                    case SpecialValues.NegativeInfinity:
                        if (bi2.SpecialValue == SpecialValues.None)
                        {
                            if (bi2.IsZero)
                                return NaN;
                            if (bi2.IsNegative)
                                return PositiveInfinity;
                            else
                                return NegativeInfinity;
                        }
                        else
                            return NaN;
                    default:
                        switch (bi2.SpecialValue)
                        {
                            case SpecialValues.NaN:
                                return NaN;
                            case SpecialValues.PositiveInfinity: //  x / unendlich = 0
                            case SpecialValues.NegativeInfinity:
                                return 0;
                            default:
                                throw new ArithmeticException(ResourceManager.GetMessage("Arith_System"));
                        }
                }

            if (bi2.IsZero)
                return NaN;
			
            BigInteger quotient = new BigInteger();

			if(bi1.IsZero) return 0;

            quotient._data = new uint[bi1._data.Length];
            BigInteger remainder = new BigInteger();

            bool sign = true;//Ungleiche Vorzeichen: negatives Ergebnis
            if (bi1.IsNegative)
            {
                bi1 = -bi1;
                sign = !sign;
            }
            if (bi2.IsNegative)
            {
                bi2 = -bi2;
                sign = !sign;
            }

            if (bi1 < bi2)
                return BigInteger.Empty;//Ergbnis ist 0, da der Dividend kleiner ist als der Divisor.

            bi2.RemoveEmptyDataFields();//Sicher ist sicher
            if (bi2._data.Length == 1)//Wenn der Divisor höchstens 4 Bytes groß ist dann entsprechende Methode aufrufen.
                DivideByte(ref bi1, ref  bi2, ref  quotient, ref  remainder);
            else
                DivideBytes(ref bi1, ref bi2, ref quotient, ref  remainder);

            if (!sign)
                return -quotient;//Ergbnis negativ

            return quotient;

        }

        /// <summary>
        /// Überprüft, ob der linke Operand kleiner als der rechte ist.
        /// </summary>
        /// <param name="bi1">Der linke Operand.</param>
        /// <param name="bi2">Der rechte Operand.</param>
        /// <returns><c>True</c>, wenn <paramref name="bi1"/> kleiner ist als <paramref name="bi2"/>. Andernfalls <c>False</c>.</returns>
        public static bool operator <(BigInteger bi1, BigInteger bi2)
        {
            if ((bi1.SpecialValue == SpecialValues.NegativeInfinity && bi2.SpecialValue != SpecialValues.NaN && bi2.SpecialValue != SpecialValues.NegativeInfinity)// -Unendlich < Wert < +Unendlich
               || (bi1.SpecialValue != SpecialValues.PositiveInfinity && bi2.SpecialValue == SpecialValues.PositiveInfinity))
                return true;
            if (bi1.SpecialValue != SpecialValues.None || bi2.SpecialValue != SpecialValues.None)//min. einer hat Spezialwerte
                return false;

            if (bi1.IsNegative && !bi2.IsNegative)
                return true;
            if (bi1.IsPositiv && !bi2.IsPositiv)
                return false;
            if (bi1.IsZero)
                if (bi2.IsPositiv)
                    return true;
                else
                    return false;
            if (bi2.IsZero)
                if (bi1.IsNegative)
                    return true;
                else
                    return false;

            bool isPos = bi1.IsPositiv;
            bi1 = BigInteger.Abs(bi1);
            bi2 = BigInteger.Abs(bi2);

            if ((bi1.data.Length < bi2.data.Length))
                //    || (bi1._sign == null && bi2._sign == true)
                //    || (bi1._sign == false && bi2._sign != false))
                return isPos;// true;
            if ((bi1.data.Length > bi2.data.Length))
                //    || (bi1._sign == null && bi2._sign == false)
                //    || (bi1._sign != false && bi2._sign == false))
                return !isPos;//false;

            for (int i = bi1.data.Length - 1; i >= 0; --i)
            {
                if (bi1.data[i] < bi2.data[i])
                    return isPos;//true;
                else if (bi1.data[i] > bi2.data[i])
                    return !isPos;//false;
            }

            return !isPos;//false;
        }

        /// <summary>
        /// Überprüft, ob der linke Operand größer als der rechte ist.
        /// </summary>
        /// <param name="bi1">Der linke Operand.</param>
        /// <param name="bi2">Der rechte Operand.</param>
        /// <returns><c>True</c>, wenn <paramref name="bi1"/> größer ist als <paramref name="bi2"/>. Andernfalls <c>False</c>.</returns>
        public static bool operator >(BigInteger bi1, BigInteger bi2)
        {
			return !(bi1 <= bi2);
        }

        /// <summary>
        /// Überprüft, ob der linke Operand kleiner oder gleich dem rechten ist.
        /// </summary>
        /// <param name="bi1">Der linke Operand.</param>
        /// <param name="bi2">Der rechte Operand.</param>
        /// <returns><c>True</c>, wenn der Wert von <paramref name="bi1"/> kleiner oder gleich dem Wert von <paramref name="bi2"/> ist. Andernfalls <c>False</c>.</returns>
        public static bool operator <=(BigInteger bi1, BigInteger bi2)
        {
            return (bi1 < bi2) || (bi1 == bi2);
        }

        /// <summary>
        /// Überprüft, ob der linke Operand größer oder gleich dem rechten ist.
        /// </summary>
        /// <param name="bi1">Der linke Operand.</param>
        /// <param name="bi2">Der rechte Operand.</param>
        /// <returns><c>True</c>, wenn der Wert von <paramref name="bi1"/> größer oder gleich dem Wert von <paramref name="bi2"/> ist. Andernfalls <c>False</c>.</returns>
        public static bool operator >=(BigInteger bi1, BigInteger bi2)
        {
            return (bi1 > bi2) || (bi1 == bi2);
        }

        /// <summary>
        /// Überprüft ob 2 Operanden die gleichen Werte haben.
        /// </summary>
        /// <param name="bi1">Der linke Operand.</param>
        /// <param name="bi2">Der rechte Operand.</param>
        /// <returns><c>True</c>, wenn beide Werte <c>null</c> sind oder den gleichen Zahlenwert haben. Andernfalls <c>False</c>.</returns>
        public static bool operator ==(BigInteger bi1, BigInteger bi2)
        {
            if (bi1._SpecialValue != bi2.SpecialValue)//Unterschiedliche Spezialwerte
                return false;
            if (bi1.SpecialValue != SpecialValues.None)//Gleiche Spezialwerte, ungleich None //! NaN != NaN
                return false;

            //Einfach entscheidbare Fälle
            if (bi1.IsZero == true && bi2.IsZero == true)//Beide 0
                return true;
            if (bi1.IsZero != bi2.IsZero)//eines 0
                return false;
            if (bi1.IsNegative != bi2.IsNegative)//Unterschiedliche Vorzeichen
                return false;
            if (bi1.data.Length != bi2.data.Length)//selbe Länge der Daten
                return false;

            //Bytes durchgehen
            for (int i = 0; i < bi1.data.Length; ++i)
                if (bi1.data[i] != bi2.data[i])
                    return false;
            return true;//Keine unterschiedlichen Bytes gefunden
        }

        /// <summary>
        /// Überprüft ob 2 Operanden nicht die gleichen Werte haben.
        /// </summary>
        /// <param name="bi1">Der linke Operand.</param>
        /// <param name="bi2">Der rechte Operand.</param>
        /// <returns><c>True</c>, wenn nur ein Operand <c>null</c> ist oder die Zahlenwerte unterschiedlich sind. Andernfalls <c>False</c>.</returns>
        public static bool operator !=(BigInteger bi1, BigInteger bi2)
        {
            if (bi1._SpecialValue != bi2.SpecialValue)//Unterschiedliche Spezialwerte
                return true;
            if (bi1.SpecialValue != SpecialValues.None)//Gleiche Spezialwerte, ungleich None //! NaN != NaN
                return true;

            //Einfach entscheidbare Fälle
            if (bi1.IsZero == true && bi2.IsZero == true)//Beide 0
                return false;
            if (bi1.IsZero != bi2.IsZero)//eines 0
                return true;
            if (bi1.IsNegative != bi2.IsNegative)//Unterschiedliche Vorzeichen
                return true;
            if (bi1.data.Length != bi2.data.Length)//selbe Länge der Daten
                return true;

            //Bytes durchgehen
            for (int i = 0; i < bi1.data.Length; ++i)
                if (bi1.data[i] == bi2.data[i])
                    return false;
            return true;//Keine gemeinsamen Bytes gefunden
        }

        /// <summary>
        /// Verschiebt die Datenbits um die angegebenen Stellen nach links. Die rechten Stellen werden mit 0 aufgefüllt.
        /// </summary>
        /// <param name="bi1">Der Wert, deren Daten verschoben werden sollen.</param>
        /// <param name="shiftVal">Die Anzahl der Stellen, um die die Datenbits verschoben werden sollen.</param>
        public static BigInteger operator <<(BigInteger bi1, int shiftVal)
        {
            if (bi1.SpecialValue != SpecialValues.None)
                throw new ArgumentOutOfRangeException(ResourceManager.GetMessage("ArgOutRang_InvalidSpecialValue"));

            BigInteger result = BigInteger.Abs(bi1);//Ergebnis aus Absolutem Betrag vom Parameter erstellen

            int shiftAmount = 32;
            int bufLen = 0;

            for (int count = shiftVal; count > 0; )//Bits in 32er Schritten durchgehen
            {
                bufLen = result.data.Length; //Länge des Array neu ermitteln, falls Zielarray mehr als 32 Bit größer ist
                if (count < shiftAmount)//Solange um 1 uint verschieben, bis nur noch ein Restwert übrig ist, um den dann verschoben wird.
                    shiftAmount = count;

                //Bits verschieben
                ulong carry = 0;
                for (int i = 0; i < bufLen; i++)
                {
                    ulong val = ((ulong)result.data[i]) << shiftAmount;
                    val |= carry;

                    result.data[i] = (uint)(val & 0xFFFFFFFF);
                    carry = val >> 32;
                }

                if (carry != 0)//Wenn ein Überlauf existiert > diesen verschieben
                {
                    uint[] buffer2 = new uint[result.data.Length + 1];
                    Array.Copy(result.data, buffer2, result.data.Length);
                    buffer2[buffer2.Length - 1] = (uint)carry;//Um 1 Element erweitern
                    result.data = buffer2;

                }
                count -= shiftAmount;//32 Bit bzw. Rest abziehen, bis count 0 ist
            }
            if (bi1.IsNegative)//Wenn ursprungswert Negativ, dann ist das Ergebnis auch Negativ
                return -result;
            return result;
        }

        /// <summary>
        /// Verschiebt die Datenbits um die angegebenen Stellen nach rechts. Die rechten Stellen werden mit 0 aufgefüllt.
        /// </summary>
        /// <param name="bi1">Der Wert, deren Daten verschoben werden sollen.</param>
        /// <param name="shiftVal">Die Anzahl der Stellen, um die die Datenbits verschoben werden sollen.</param>
        public static BigInteger operator >>(BigInteger bi1, int shiftVal)
        {
            if (bi1.SpecialValue != SpecialValues.None)
                throw new ArgumentOutOfRangeException(ResourceManager.GetMessage("ArgOutRang_InvalidSpecialValue"));

            if (bi1.IsZero) //? 0 im Datenspeicher > Foplge: immer 0 als Ergebnis
                return bi1;

            BigInteger result = new BigInteger(bi1);
            int len = result.data.Length;

            //Array um Verschiebungsbits verlängern
            uint[] buffer2 = new uint[result.data.Length + (shiftVal / 32) + 1];
            if ((bool)bi1._sign)
                for (int i = 0; i < buffer2.Length; ++i)
                    buffer2[i] = 0x00000000;
            else
                for (int i = 0; i < buffer2.Length; ++i)
                    buffer2[i] = 0xFFFFFFFF;
            Array.Copy(result.data, buffer2, result.data.Length);
            result.data = buffer2;

            int shiftAmount = 32;
            int invShift = 0;
            int bufLen = result.data.Length;

            for (int count = shiftVal; count > 0; )
            {
                if (count < shiftAmount)
                {
                    shiftAmount = count;
                    invShift = 32 - shiftAmount;
                }

                ulong carry = 0;
                for (int i = bufLen - 1; i >= 0; i--)
                {
                    ulong val = ((ulong)result.data[i]) >> shiftAmount;
                    val |= carry;

                    carry = ((ulong)result.data[i]) << invShift;
                    result.data[i] = (uint)(val);
                }

                count -= shiftAmount;
            }

            buffer2 = new uint[len - (shiftVal / 32) /*+ 1*/];

            Array.Copy(result.data, buffer2, buffer2.Length);//Array kürzen
            result.data = buffer2;

            result.RemoveEmptyDataFields();

            if (bi1.IsNegative)
            {
                uint mask = 0x80000000;
                for (int i = 0; i < 32; i++)
                {
                    if ((result.data.Last()/*[result.dataLength - 1]*/ & mask) != 0)
                        break;

                    result.data[result.data.Length - 1] |= mask;
                    mask >>= 1;
                }
            }
            return result;
        }

        /// <summary>
        /// Kehrt alle Bits im Datenpuffer um und tauscht das Vorzeichen. Sollte de Wert 0 sein, wird 0 zurück gegeben.
        /// </summary>
        /// <param name="bi1">Der Wert, deren Bits und Vorzeichen umgekehrt werden sollen.</param>
        public static BigInteger operator ~(BigInteger bi1)
        {
            if (bi1.SpecialValue != SpecialValues.None)
                throw new ArgumentOutOfRangeException(ResourceManager.GetMessage("ArgOutRang_InvalidSpecialValue"));

            if (bi1.IsZero)
                return new BigInteger();
            BigInteger result = new BigInteger(bi1);
            result._data = (from x in result._data
                            select ~x).ToArray();
            result._sign = !(bool)result._sign;
            return result;
        }

        /// <summary>
        /// Setzt ein Bit im Ergebnis auf True, wenn genau ein Bit an der selben Position in <paramref name="bi1"/> oder <paramref name="bi2"/> gesetzt ist.
        /// </summary>
        /// <param name="bi1">Der erste Operand.</param>
        /// <param name="bi2">Der zweite Operand.</param>
        public static BigInteger operator ^(BigInteger bi1, BigInteger bi2)
        {
            if (bi1.SpecialValue != SpecialValues.None)
                throw new ArgumentOutOfRangeException(ResourceManager.GetMessage("ArgOutRang_InvalidSpecialValue"));

            uint[] data = new uint[Math.Max(bi1._data.Length, bi2._data.Length) + 1];//1 mehr, damit die bestimmung des Vorzeichens auch klappt, wenn das Datenarray bis ganz nach vorn mit Daten ohne Vorzeichen gefüllt ist.

            for (int i = 0; i < data.Length; i++)
            {
                uint u1 = bi1.IsNegative ? 0xFFFFFFFF : 0x00000000;
                uint u2 = bi2.IsNegative ? 0xFFFFFFFF : 0x00000000;
                if (i < bi1._data.Length)
                    u1 = bi1._data[i];
                if (i < bi2._data.Length)
                    u2 = bi2._data[i];
                data[i] = u1 ^ u2;
            }
            BigInteger result = new BigInteger() { _data = data, _sign = (data[data.Length - 1] >> 31) == 0 ? true : false };
            result.RemoveEmptyDataFields();
            return result;
        }

        /// <summary>
        /// Setzt ein Bit im Ergebnis auf True, wenn die jeweiligen Bits in <paramref name="bi1"/> und <paramref name="bi2"/> gesetzt sind.
        /// </summary>
        /// <param name="bi1">Der erste Operand.</param>
        /// <param name="bi2">Der zweite Operand.</param>
        public static BigInteger operator &(BigInteger bi1, BigInteger bi2)
        {
            if (bi1.SpecialValue != SpecialValues.None)
                throw new ArgumentOutOfRangeException(ResourceManager.GetMessage("ArgOutRang_InvalidSpecialValue"));

            uint[] data = new uint[Math.Max(bi1._data.Length, bi2._data.Length) + 1];

            for (int i = 0; i < data.Length; i++)
            {
                uint u1 = bi1.IsNegative ? 0xFFFFFFFF : 0x00000000;
                uint u2 = bi2.IsNegative ? 0xFFFFFFFF : 0x00000000;
                if (i < bi1._data.Length)
                    u1 = bi1._data[i];
                if (i < bi2._data.Length)
                    u2 = bi2._data[i];
                data[i] = (uint)(u1 & u2);
            }

            BigInteger result = new BigInteger() { _data = data, _sign = (data[data.Length - 1] >> 31) == 0 ? true : false };
            result.RemoveEmptyDataFields();
            return result;
        }

        /// <summary>
        /// Setzt ein Bit im Ergebnis auf True, wenn mindestens eines der jeweiligen Bits in <paramref name="bi1"/> und <paramref name="bi2"/> gesetzt ist.
        /// </summary>
        /// <param name="bi1">Der erste Operand.</param>
        /// <param name="bi2">Der zweite Operand.</param>
        public static BigInteger operator |(BigInteger bi1, BigInteger bi2)
        {
            if (bi1.SpecialValue != SpecialValues.None)
                throw new ArgumentOutOfRangeException(ResourceManager.GetMessage("ArgOutRang_InvalidSpecialValue"));

            uint[] data = new uint[Math.Max(bi1._data.Length, bi2._data.Length) + 1];

            for (int i = 0; i < data.Length; i++)
            {
                uint u1 = bi1.IsNegative ? 0xFFFFFFFF : 0x00000000;
                uint u2 = bi2.IsNegative ? 0xFFFFFFFF : 0x00000000;
                if (i < bi1._data.Length)
                    u1 = bi1._data[i];
                if (i < bi2._data.Length)
                    u2 = bi2._data[i];
                data[i] = (uint)(u1 | u2);
            }

            BigInteger result = new BigInteger() { _data = data, _sign = (data[data.Length - 1] >> 31) == 0 ? true : false };
            result.RemoveEmptyDataFields();
            return result;
        }



        #endregion

        #endregion

        #region override

        #region Object

        /// <summary>
        /// Gibt die Dezimale Darstellungsweise des gespeicherten Wertes zurück. Dabei wird auf den in DigitSet gespeicherten Zeichensatz zurück gegriffen.
        /// </summary>
        public override string ToString()
        {
            return this.ToString(10,false);
        }

		public string ToString(int v) {
			return this.ToString(10, false);
		}

		/// <summary>
		/// Liefert den kombinierten Hashcode der Instanzrelevanten Member zurück.
		/// </summary>
		public override int GetHashCode()
        {
            if (this.IsZero)
                return 0;
            else
                return this._sign.GetHashCode() ^ this.data.GetItemsHashCode();
        }

        /// <summary>
        /// Vergleicht ein Objekt mit dieser Instanz auf Gleichheit mithilfe des Standartvergleichs.
        /// </summary>
        /// <param name="obj">Das Vergleichsobjekt.</param>
        /// <returns><c>True</c>, wenn die Instanzen übereinstimmen, andernfalls <c>False</c>.</returns>
        public override bool Equals(object obj)
        {
            return obj is BigInteger && (BigInteger)obj == this;
        }

        #endregion

        #endregion

        #region interfaces

        #region IComparable Member

        /// <summary>
        /// Vergleicht ein Objekt mit dieser Instanz auf Gleichheit.
        /// </summary>
        /// <param name="obj">Das zu vergleichende Objekt.</param>
        /// <returns>1 wenn <paramref name="obj"/> keine Instanz von BigInteger ist oder <paramref name="obj"/> 
        /// einen niedrigereren Wert hat als diese Instanz. 0 wenn der Wert der Instanzen gleich ist. -1 wenn 
        /// <paramref name="obj"/> einen höheren Wert hat als diese Instanz.</returns>
        public int CompareTo(object obj)
        {
            if (!(obj is BigInteger))
                return 1;
            return CompareTo((BigInteger)obj);
        }

        #endregion

        #region IComparable<BigInteger> Member

        /// <summary>
        /// Vergleicht ein Objekt mit dieser Instanz auf Gleichheit.
        /// </summary>
        /// <param name="other">Die zu vergleichende Instanz.</param>
        /// <returns>1 wenn <paramref name="other"/> einen niedrigereren Wert hat als diese Instanz. 0 wenn der 
        /// Wert der Instanzen gleich ist. -1 wenn <paramref name="other"/> einen höheren Wert hat als diese Instanz.</returns>
        /// <remarks>Der Größt-Mögliche Wert ist NaN, PositiveInfinity ist technisch bedingt nur der Zweithöchste Wert.</remarks>
        public int CompareTo(BigInteger other)
        {
            //? NaN ist nicht mit anderen Werten vergleichbar. Darum geben alle Relations-Operatoren False beim Vergleich mit NaN zurück.
            //? Da NaN in einer Folge einen Festen Platz benötigt, ordne ich diesen am Ende ein. 
            //? Aufgrund der mathematisch korrekt arbeitenden Operatoren muss dieser Sachverhalt zuvor geprüft werden.
            if (this.SpecialValue == SpecialValues.NaN && other.SpecialValue == SpecialValues.NaN)
                return 0;
            if (this.SpecialValue == SpecialValues.NaN)
                return 1;
            if (other.SpecialValue == SpecialValues.NaN)
                return -1;

            if (other == this)
                return 0;
            if (other < this)
                return 1;
            else
                return -1;
        }

        #endregion

        #region IEquatable<BigInteger> Member

        /// <summary>
        /// Vergleicht ein Objekt mit dieser Instanz auf Wertegleichheit.
        /// </summary>
        /// <param name="other">Die zu vergleichende Instanz.</param>
        /// <returns><c>True</c>, wenn der Wert dieser Instanz mit dem Wert von <paramref name="other"/> übereinstimmt. Andernfalls <c>False</c>.</returns>
        public bool Equals(BigInteger other)
        {
            return this == other;
        }

        #endregion

        #region IFormattable Member

        /// <summary>
        /// Gibt die dezimale Darstellung des gespeicherten Wertes zurück.
        /// </summary>
        /// <param name="format">Wird ignoriert.</param>
        /// <param name="formatProvider">Wird ignoriert.</param>
        /// <returns>Die dezimale Darstellung des gespeicherten Wertes.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return this.ToString(10,false);
        }

        #endregion

        #region IBigNumber<BigInteger> Member

        /// <summary>
        /// Analysiert eine Zeichenfolge und weißt der Instanz den dargestellten Wert zu.
        /// </summary>
        /// <param name="number">Die zu analysierende Zeichenfolge.</param>
        public void ParseString(string number)
        {
            this = BigInteger.Parse(number);
        }

        /// <summary>
        /// Analysiert eine Zeichenfolge und weißt der Instanz den dargestellten Wert zu.
        /// </summary>
        /// <param name="number">Die zu analysierende Zeichenfolge.</param>
        public void ParseMSWord(string number)
        {
            this = BigInteger.Parse(number);
        }

        #endregion

        #region INumber Member

        /// <summary>
        /// Ruft diese Instanz ab.
        /// </summary>
        public INumber Result
        {
            get
            {
                return this;
            }
        }

        private SpecialValues _SpecialValue;
        /// <summary>
        /// Ruft den gespeicherten Spezialwert ab.
        /// </summary>
        public SpecialValues SpecialValue
        {
            get
            {
                return _SpecialValue;
            }
        }


        #endregion

        #region IMathComparison<BigInteger> Member

        /// <summary>
        /// Überprüft, ob ein Wert größer ist, als der Wert dieser Instanz.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> größer ist als der Wert dieser Instanz. Andernfalls <c>False</c>.</returns>
        public bool IsGreater(BigInteger param)
        {
            return param > this;
        }
        /// <summary>
        /// Überprüft, ob ein Wert größer oder gleich dem Wert dieser Instanz ist.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> größer oder gleich dem Wert dieser Instanz ist. Andernfalls <c>False</c>.</returns>
        public bool IsGreaterEqual(BigInteger param)
        {
            return param >= this;
        }
        /// <summary>
        /// Überprüft, ob ein Wert größer ist, als der Wert dieser Instanz.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> kleiner ist als der Wert dieser Instanz. Andernfalls <c>False</c>.</returns>
        public bool IsSmaller(BigInteger param)
        {
            return param < this;
        }
        /// <summary>
        /// Überprüft, ob ein Wert kleiner oder gleich dem Wert dieser Instanz ist.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> kleiner oder gleich dem Wert dieser Instanz ist. Andernfalls <c>False</c>.</returns>
        public bool IsSmallerEqual(BigInteger param)
        {
            return param <= this;
        }

        #endregion

        #region IMathEqualComparison<Matrix> Member

        /// <summary>
        /// Überprüft, ob ein Wert gleich dem Wert dieser Instanz ist.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> und der Wert dieser Instanz gleich sind. Andernfalls <c>False</c>.</returns>
        public bool IsEqual(BigInteger param)
        {
            return param == this;
        }
        /// <summary>
        /// Überprüft, ob ein Wert ungleich dem Wert dieser Instanz ist.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> und der Wert dieser Instanz ungleich sind. Andernfalls <c>False</c>.</returns>
        public bool IsNotEqual(BigInteger param)
        {
            return param != this;
        }

        /// <summary>
        /// Ruft einen Wert ab, der angibt ob der Wert in einem bestimmten Definitionsbereich liegt.
        /// </summary>
        /// <param name="set">Die zu prüfende Definitionsmenge.</param>
        /// <returns><c>True</c>, wenn diese Instanz in der angegebenen Definitionsmenge liegt. ANdernfalls <c>False</c>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#")]
        public bool IsInDomain(DomainSet set)
        {
            switch (set.Set)
            {
                case DomainSets.NaturalWithoutZero:
                    return this > 0;
                case DomainSets.Natural:
                    return this >= 0;
                default:
                    return true;
            }
        }

        #endregion

        #endregion

        #region Hilfsmethoden

        ///// <summary>
        ///// Weißt den öffentlichen Eigenschaften ihren Standartwert zu.
        ///// </summary>
        //[DebuggerStepThrough()]
        //protected void InitValues()
        //{
        //    this.DigitSet = "0123456789ABCDEFGHIOJKLMNOPQRSTUVWXYZ";
        //}

        /// <summary>
        /// Entfernt leere 4er-Byte-Gruppen aus dem Datenarray.
        /// </summary>
        private void RemoveEmptyDataFields()
        {
            if (this.IsZero)
                return;
            int c = 0;
            if (this._sign == true)
            {
                for (int i = this.data.Length - 1; i >= 0; --i)
                    if (this.data[i] == 0x00000000)
                        ++c;
                    else
                        break;
            }
            else
            {
                for (int i = this.data.Length - 1; i >= 0; --i)
                    if (this.data[i] == 0xFFFFFFFF)
                        ++c;
                    else
                        break;
            }
            if (c == 0)
                return;
            uint[] newData = new uint[this.data.Length - c == 0 ? 1 : this._data.Length - c];
            Array.Copy(this.data, 0, newData, 0, newData.Length);
            this.data = newData;
        }

        ///// <summary>
        ///// Löscht leere Felder am Anfang des Datenarrays weg. Vorsicht! Nur für wenige Hilfsmethoden geschrieben.
        ///// </summary>
        //private void RemoveEmptyDataFieldsFirst()
        //{
        //    if (this.IsZero)
        //        return;
        //    int c = 0;
        //    if (this._sign == true)
        //    {
        //        for (int i = 0; i < this.data.Length; ++i)
        //            if (this.data[i] == 0x00000000)
        //                ++c;
        //            else
        //                break;
        //    }
        //    else
        //    {
        //        for (int i = 0; i < this.data.Length; ++i)
        //            if (this.data[i] == 0xFFFFFFFF)
        //                ++c;
        //            else
        //                break;
        //    }
        //    uint[] newData = new uint[this.data.Length - c == 0 ? 1 : this._data.Length - c];
        //    Array.Copy(this.data, c, newData, 0, newData.Length);
        //    this.data = newData;
        //}

        private static void ToStringHelper(BigInteger bi1, ulong radix, ref BigInteger outQuotient, ref BigInteger outRemainder)
        {
            uint[] result = new uint[bi1.data.Length];
            int resultPos = 0;

            outRemainder.data = new uint[bi1.data.Length];
            for (int i = 0; i < bi1.data.Length; i++)
                outRemainder.data[i] = bi1.data[i];
            outRemainder._sign = bi1._sign;

            ulong divisor = radix;
            int pos = outRemainder.data.Length - 1;
            ulong dividend = (ulong)outRemainder.data[pos];

            if (dividend >= divisor)
            {
                ulong quotient = dividend / divisor;
                result[resultPos++] = (uint)quotient;

                outRemainder.data[pos] = (uint)(dividend % divisor);
            }
            pos--;

            while (pos >= 0)
            {
                dividend = ((ulong)outRemainder.data[pos + 1] << 32) + (ulong)outRemainder.data[pos];
                ulong quotient = dividend / divisor;
                result[resultPos++] = (uint)quotient;

                outRemainder.data[pos + 1] = 0;
                outRemainder.data[pos--] = (uint)(dividend % divisor);
            }

            if (resultPos == 0)
            {
                outQuotient.data = new uint[0];
                outQuotient._sign = null;
            }
            else
            {
                outQuotient.data = result.Reverse().ToArray();
                outQuotient._sign = true;
            }
            while (resultPos != outQuotient.data.Length)
            {
                uint[] tmpData = new uint[outQuotient.data.Length - 1];
                Array.Copy(outQuotient.data, 1, tmpData, 0, tmpData.Length);
                outQuotient.data = tmpData;
            }
            outQuotient.RemoveEmptyDataFields/*First*/();
            //    outRemainder.RemoveEmptyDataFieldsFirst();
        }

        ///<remarks>
        ///<paramref name="outQuotient"/> muss die selbe Größe wie <paramref name="bi1"/> haben. 
        ///<paramref name="outQuotient"/> darf höchstens 4 Bytes groß sein.
        ///</remarks>
        private static void DivideByte(ref BigInteger bi1, ref BigInteger bi2, ref  BigInteger outQuotient, ref  BigInteger outRemainder)
        {
			uint[] result = new uint[bi1._data.Length];//Maximale Länge
            int resultPos = 0;

            outRemainder = new BigInteger(bi1);

            ulong divisor = (ulong)bi2.data[0];
            int position = outRemainder._data.Length - 1;
            ulong dividend = (ulong)outRemainder.data[position];

            if (dividend >= divisor)
            {
                ulong quotient = dividend / divisor;
                result[resultPos++] = (uint)quotient;

                outRemainder.data[position] = (uint)(dividend % divisor);
            }
            position--;

            while (position >= 0)
            {
                dividend = ((ulong)outRemainder.data[position + 1] << 32) + (ulong)outRemainder.data[position];
                ulong quotient = dividend / divisor;
                result[resultPos++] = (uint)quotient;

                outRemainder.data[position + 1] = 0;
                outRemainder.data[position--] = (uint)(dividend % divisor);
            }

            int j = 0;
            for (int i = /*outQuotient._data.Length */resultPos - 1; i >= 0; i--, j++)//Ergebnis in die Instanz kopieren
                // if(i<resultPos ) //? => 26.10.2013 18:56
                outQuotient.data[j] = result[i];
            for (; j < outQuotient._data.Length; j++)//Restliche 4er Bytes auf 0 setzen
                outQuotient.data[j] = 0;

            outQuotient._sign = true;//Damit nicht immer 0 heraus kommt

            //! in First geändert
            outQuotient.RemoveEmptyDataFields/*First*/();//die leeren Bytes weg kürzen
        }

        /// <remarks><paramref name="outQuotient"/> muss die selbe Größe wie <paramref name="bi1"/> haben.</remarks>
        private static void DivideBytes(ref BigInteger _bi1, ref BigInteger _bi2, ref BigInteger outQuotient, ref BigInteger outRemainder) {
			BigInteger bi1 = new BigInteger(_bi1);
			BigInteger bi2 = new BigInteger(_bi2);
			uint[] result = new uint[bi1._data.Length];//Maximale Länge

            int remainderLen = bi1._data.Length + 1;
            uint[] remainder = new uint[remainderLen];

            uint mask = 0x80000000;
            uint val = bi2.data[bi2._data.Length - 1];
            int shift = 0, resultPos = 0;

            while (mask != 0 && (val & mask) == 0)
            {
                shift++; mask >>= 1;
            }

            for (int i = 0; i < bi1._data.Length; i++)
                remainder[i] = bi1.data[i];
            remainder = remainder.ShiftLeft(shift);
            bi2 = bi2 << shift;

            int j = remainderLen - bi2._data.Length;
            int position = remainderLen - 1;

            ulong firstDivisorByte = bi2.data[bi2._data.Length - 1];
            ulong secondDivisorByte = bi2.data[bi2._data.Length - 2];

            int divisorLen = bi2._data.Length + 1;
            uint[] dividendPart = new uint[divisorLen];

            while (j > 0)
            {
                ulong dividend = ((ulong)remainder[position] << 32) + (ulong)remainder[position - 1];

                ulong q_hat = dividend / firstDivisorByte;
                ulong r_hat = dividend % firstDivisorByte;

                bool done = false;
                while (!done)
                {
                    done = true;

                    if (q_hat == 0x100000000 ||
                       (q_hat * secondDivisorByte) > ((r_hat << 32) + remainder[position - 2]))
                    {
                        q_hat--;
                        r_hat += firstDivisorByte;

                        if (r_hat < 0x100000000)
                            done = false;
                    }
                }

                for (int h = 0; h < divisorLen; h++)
                    dividendPart[h] = remainder[position - h];

                BigInteger k2 = new BigInteger(dividendPart);
                BigInteger s2 = bi2 * (long)q_hat;

                while (s2 > k2)
                {
                    q_hat--;
                    s2 -= bi2;
                }
                BigInteger y2 = k2 - s2;

                y2.data = y2.data.Extend(bi2._data.Length + 1);
                for (int h = 0; h < divisorLen; h++)
                    remainder[position - h] = y2.data[bi2._data.Length - h];

                result[resultPos++] = (uint)q_hat;

                position--;
                j--;
            }

            outQuotient.data = new uint[resultPos];
            int y = 0;
            for (int x = outQuotient._data.Length - 1; x >= 0/*&&y<resultPos*/; x--, y++)
                outQuotient.data[y] = result[x];

            outQuotient._sign = true;//Damit nachfolgende Methode funktioniert, also nicht einfach 0 erkannt wird wegen IsZero
            //x   outQuotient.RemoveEmptyDataFieldsFirst();
            outQuotient.RemoveEmptyDataFields(); //! Zusätzlich hinzugefügt //? Wieder entkommentiert

            //x outRemainder._data = new uint[remainder.Length]; //! Neuzuweisung
            for (y = 0; y < outRemainder.data.Length; y++)
                outRemainder.data[y] = remainder[y];
            for (; y < outRemainder._data.Length; y++)
                outRemainder.data[y] = 0;

            outRemainder = outRemainder >> shift;
		}

		#endregion

		//! Parse (0x..., Normal, e10, ...radix angeben, ...
		//! exlicit string
		//! explicit double
		//! explicit float

		//! Rundungsregeln!?
		//! Throw DivideByZeroException?

		public static int ToInt32(BigInteger val) {
			if(val == 0) return 0;
			if(val.data.Length == 1) {
				uint i = val.data[0];
				if(i <= int.MaxValue) {
					return (int)i;
				}
			}
			throw new ArgumentNullException("value");
		}

		public static int Log10(BigInteger val) {
			return (Abs(new BigInteger(val))).ToString().Length - 1;
		}

		public static BigInteger THREE_D = 3;
		public static int UP = 1;
		public static BigInteger CubeRoot(BigInteger x) {
			double log = Log10(x);
			log /= 3;
			log = Math.Floor(log);
			BigInteger y = 1;               // Guess
			for(double p = 1; p <= log; p++) {
				y *= 10;
			}
			bool neg = x.IsNegative;
			if(neg) x = -x;
			BigInteger d;               // Last difference of y3 and x
			BigInteger l;               // The limit for optimal guess
			BigInteger changeInY = -1;
			if(x == 0)
				return 0;
			else if(x == 1)
				return 1;
			else if(x == -1)
				return -1;
			else {
				l = 1;// 1E-14 * x;
				y = ((x / (y * y)) + y * 2) / 3;
				d = y * y * y - x;
				if(d.IsNegative) d = -d;
				while(l < d && (changeInY < 0 || changeInY > 1)) {
					BigInteger ny = ((x / (y * y)) + y * 2) / 3;
					changeInY = ny - y;
					y = ny;
					if(changeInY.IsNegative) changeInY *= -1;
					d = y * y * y - x;
					if(d.IsNegative) d = -d;
				}
			}


			return y;
		}
	}
}