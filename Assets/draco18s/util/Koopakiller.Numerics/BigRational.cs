//! Created by Tom Lambert alias Koopakiller
//! Project started: 2011
//! License: Microsoft Reciprocal License (Ms-RL)
//! Project site: https://numerics.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Koopakiller.Numerics.Resources;

namespace Koopakiller.Numerics
{
    /// <summary>
    /// Stellt eine Dezimalzahl mit beliebig hoher Genauigkeit dar.<para/>
    /// Der Zahl muss als Bruch darstellbar sein.
    /// </summary>
    [DebuggerDisplay("{ToDecimalString(10)}")]
    public struct BigRational : INumber, IFormattable, IComparable, IComparable<BigRational>, IEquatable<BigRational>, IMathComparison<BigRational> // : IBigNumber<BigDecimal>
    {
		#region .ctor

		public BigRational(BigRational value):this() {
			Numerator = new BigInteger(value.Numerator);
			Denominator = new BigInteger(value.Denominator);
		}

		/// <summary>
		/// Erstellt eine neue Instanz der <see cref="BigRational"/>-Klasse aus den angegebenen Bruch-Werten.
		/// </summary>
		/// <param name="numerator">Der Zähler des Bruchs.</param>
		/// <param name="denominator">Der Nenner des Bruchs.</param>
		public BigRational(BigInteger numerator, BigInteger denominator)
            : this(0, numerator, denominator) { }

        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="BigRational"/>-Klasse aus den angegebenen Bruch-Werten.
        /// </summary>
        /// <param name="whole">Die Anzahl an ganzen Teilen in dem Bruch.</param>
        /// <param name="numerator">Der Zähler des Bruchs.</param>
        /// <param name="denominator">Der Nenner des Bruchs.</param>
        public BigRational(BigInteger whole, BigInteger numerator, BigInteger denominator)
            : this()
        {
            this.Numerator = numerator + denominator * whole;
            this.Denominator = denominator;
        }

        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="BigRational"/>-Klasse mit dem angegebenen Wert.
        /// </summary>
        /// <param name="value">Der zu speichernde Wert.</param>
        public BigRational(BigInteger value)
            : this()
        {
            this.Numerator = value;
            this.Denominator = 1;
        }

        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="BigRational"/>-Klasse mit dem angegebenen Wert.
        /// </summary>
        /// <param name="value">Der zu speichernde Wert.</param>
        public BigRational(int value)
            : this()
        {
            this.Numerator = value;
            this.Denominator = 1;
        }

		public BigRational(float value) : this() {
			string str = value.ToString();
			int s = str.Length;
			s -= (str.IndexOf('.'));
			double p = 1 / (Math.Pow(10, s));
			Numerator = 1;
			Denominator = 1;
			long bits = BitConverter.DoubleToInt64Bits(value + p);
			bool negative = (bits < 0);
			int exponent = (int)((bits >> 52) & 0x7ffL);
			long mantissa = bits & 0xfffffffffffffL;
			if(exponent == 0) {
				exponent++;
			}
			else {
				mantissa = mantissa | (1L << 52);
			}

			int div = (exponent - 1023);

			if(div >= 0) {
				Numerator = mantissa * BigInteger.Pow(2, div);
				Denominator = BigInteger.Pow(2, 52);
			}
			else {
				div = -div;
				Numerator = mantissa * BigInteger.Pow(2, 0);
				Denominator = BigInteger.Pow(2, 52 + div);
			}

			this = BigRational.Truncate(new BigRational((negative ? -1 : 1) * Numerator, Denominator), (uint)(s - 1));
		}

		/// <summary>
		/// Erstellt eine neue Instanz der <see cref="BigRational"/>-Struktur mit dem angegebenen Wert.
		/// </summary>
		/// <param name="value">Der als Bruch zu speichernde Wert.</param>
		public BigRational(double value) : this() {
			string str = value.ToString();
			int s = str.Length;
			s -= (str.IndexOf('.'));
			double p = 1 / (Math.Pow(10, s));
			Numerator = 1;
			Denominator = 1;
			long bits = BitConverter.DoubleToInt64Bits(value+p);
			bool negative = (bits < 0);
			int exponent = (int)((bits >> 52) & 0x7ffL);
			long mantissa = bits & 0xfffffffffffffL;
			if(exponent == 0) {
				exponent++;
			}
			else {
				mantissa = mantissa | (1L << 52);
			}
			
			int div = (exponent - 1023);
			
			if(div >= 0) {
				Numerator = mantissa * BigInteger.Pow(2, div);
				Denominator = BigInteger.Pow(2, 52);
			}
			else {
				div = -div;
				Numerator = mantissa * BigInteger.Pow(2, 0);
				Denominator = BigInteger.Pow(2, 52+div);
			}

			this = BigRational.Truncate(new BigRational((negative?-1:1) * Numerator, Denominator),(uint)(s-1));
		}
		/// <summary>
		/// Erstellt eine neue Instanz der <see cref="BigRational"/>-Struktur mit dem angegebenen Wert.
		/// </summary>
		/// <param name="value">Der als Bruch zu speichernde Wert.</param>
		public BigRational(decimal value)
            : this()
        {
            this = BigRational.Parse(value.ToString(CultureInfo.CurrentCulture), "0123456789", 10, CultureInfo.CurrentCulture);
        }

        #endregion

        #region Eigenschaften

        /// <summary>
        /// Ruft den Zähler des Bruchs ab oder legt diesen fest.
        /// </summary>
        public BigInteger Numerator { get; set; }
        /// <summary>
        /// Ruft den Nenner des Bruchs ab oder legt diesen fest.
        /// </summary>
        public BigInteger Denominator { get; set; }

        /// <summary>
        /// Ruft einen Wert ab, der angibt ob der Wert keine Zahl ist.
        /// </summary>
        public bool IsNan
        {
            get
            {
                return this.Denominator == 0 || this.Numerator.SpecialValue == SpecialValues.NaN;
            }
        }

        /// <summary>
        /// Ruft einen Wert ab, der angibt ob der Wert dieser Instanz 0 ist.
        /// </summary>
        public bool IsZero
        {
            get
            {
                return this == Zero;
            }
        }

        /// <summary>
        /// Ruft einen Wert ab, der angibt ob der Wert dieser Instanz negativ ist.
        /// </summary>
        public bool IsNegative
        {
            get
            {
                if (this.SpecialValue == SpecialValues.NegativeInfinity)
                    return true;
                if (this.SpecialValue != SpecialValues.None)
                    return false;
                if (this.IsZero)
                    return false;
                if (this.Numerator.IsNegative == this.Denominator.IsNegative)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Ruft einen Wert ab, der angibt ob der Wert dieser Instanz positiv ist.
        /// </summary>
        public bool IsPositive
        {
            get
            {
                if (this.SpecialValue == SpecialValues.PositiveInfinity)
                    return true;
                if (this.SpecialValue != SpecialValues.None)
                    return false;
                if (this.IsZero)
                    return false;
                if (this.Numerator.IsNegative == this.Denominator.IsNegative)
                    return true;
                else
                    return false;
            }
        }

        #endregion

        #region operator

        /// <summary>
        /// Bildet die Summe von zwei Brüchen.
        /// </summary>
        /// <param name="br1">Der erste Summand.</param>
        /// <param name="br2">Der zweite Summand.</param>
        /// <returns>Die Summe aus <paramref name="br1"/> und <paramref name="br2"/>.</returns>
        /// <remarks>Das Ergebnis lässt sich eventuell noch vereinfachen.</remarks>
        public static BigRational operator +(BigRational br1, BigRational br2)
        {
            //if (br1.SpecialValue != SpecialValues.None || br2.SpecialValue != SpecialValues.None)
            switch (br1.SpecialValue)
            {
                case SpecialValues.NaN:
                    return NaN;
                case SpecialValues.PositiveInfinity:
                    if (br2.SpecialValue == SpecialValues.NegativeInfinity)
                        return NaN;
                    //if (br2 == Zero)
                    //    return Zero;
                    return PositiveInfinity;
                case SpecialValues.NegativeInfinity:
                    if (br2.SpecialValue == SpecialValues.PositiveInfinity)
                        return NaN;
                    //if (br2 == Zero)
                    //    return Zero;
                    return NegativeInfinity;
                default:
                    switch (br2.SpecialValue)
                    {
                        case SpecialValues.NaN:
                            return NaN;
                        case SpecialValues.PositiveInfinity:
                            if (br1.SpecialValue == SpecialValues.NegativeInfinity)
                                return NaN;
                            if (br1 == Zero)
                                return Zero;
                            return PositiveInfinity;
                        case SpecialValues.NegativeInfinity:
                            if (br1.SpecialValue == SpecialValues.PositiveInfinity)
                                return NaN;
                            if (br1 == Zero)
                                return Zero;
                            return NegativeInfinity;
                    }
                    break;
            }
            // a/b + c/d = (a*d)/(b*d) + (b*c)/(b*d) = (a*d+b*c)/(b*d)
            return new BigRational(br1.Numerator * br2.Denominator + br2.Numerator * br1.Denominator, br1.Denominator * br2.Denominator);
        }
        /// <summary>
        /// Bildet die Summe aus einem Bruch und einem ganzzahligem Skalar.
        /// </summary>
        /// <param name="br">Der erste Summand (Bruch).</param>
        /// <param name="scalar">Der zweite Summand (Skalar).</param>
        /// <returns>Die Summe aus <paramref name="br"/> und <paramref name="scalar"/>.</returns>
        /// <remarks>Das Ergebnis lässt sich eventuell noch vereinfachen.</remarks>
        public static BigRational operator +(BigRational br, BigInteger scalar)
        {
            //if (br.SpecialValue != SpecialValues.None || scalar.SpecialValue != SpecialValues.None)
            switch (br.SpecialValue)
            {
                case SpecialValues.NaN:
                    return NaN;
                case SpecialValues.PositiveInfinity:
                    if (scalar.SpecialValue == SpecialValues.NegativeInfinity || scalar.SpecialValue == SpecialValues.NaN)
                        return NaN;
                    //if (br2 == Zero)
                    //    return Zero;
                    return PositiveInfinity;
                case SpecialValues.NegativeInfinity:
                    if (scalar.SpecialValue == SpecialValues.PositiveInfinity || scalar.SpecialValue == SpecialValues.NaN)
                        return NaN;
                    //if (br2 == Zero)
                    //    return Zero;
                    return NegativeInfinity;
                default:
                    switch (scalar.SpecialValue)
                    {
                        case SpecialValues.NaN:
                            return NaN;
                        case SpecialValues.PositiveInfinity:
                            //if (br1 == Zero)
                            return PositiveInfinity;
                        //return PositiveInfinity;
                        case SpecialValues.NegativeInfinity:
                            //if (br1 == Zero)
                            return NegativeInfinity;
                        //return NegativeInfinity;
                    }
                    break;
            }
            return new BigRational(br.Numerator + br.Denominator * scalar, br.Denominator);
        }
        /// <summary>
        /// Bildet die Summe aus einem Bruch und einem ganzzahligem Skalar.
        /// </summary>
        /// <param name="scalar">Der erste Summand (Skalar).</param>
        /// <param name="br">Der zweite Summand (Bruch).</param>
        /// <returns>Die Summe aus <paramref name="scalar"/> und <paramref name="br"/>.</returns>
        /// <remarks>Das Ergebnis lässt sich eventuell noch vereinfachen.</remarks>
        public static BigRational operator +(BigInteger scalar, BigRational br)
        {
            return br + scalar;
        }

        /// <summary>
        /// Bildet die Differenz von zwei Brüchen.
        /// </summary>
        /// <param name="br1">Der Minuend.</param>
        /// <param name="br2">Der Subtrahend.</param>
        /// <returns>Die Differenz aus <paramref name="br1"/> und <paramref name="br2"/>.</returns>
        /// <remarks>Das Ergebnis lässt sich eventuell noch vereinfachen.</remarks>
        public static BigRational operator -(BigRational br1, BigRational br2)
        {
            //if (br1.SpecialValue != SpecialValues.None || br2.SpecialValue != SpecialValues.None)
            switch (br1.SpecialValue)
            {
                case SpecialValues.NaN:
                    return NaN;
                case SpecialValues.PositiveInfinity:
                    if (br2.SpecialValue == SpecialValues.PositiveInfinity || br2.SpecialValue == SpecialValues.NaN)
                        return NaN;
                    //if (br2 == Zero)
                    //    return Zero;
                    return PositiveInfinity;
                case SpecialValues.NegativeInfinity:
                    if (br2.SpecialValue == SpecialValues.NegativeInfinity || br2.SpecialValue == SpecialValues.NaN)
                        return NaN;
                    //if (br2 == Zero)
                    //    return Zero;
                    return NegativeInfinity;
                default:
                    switch (br2.SpecialValue)
                    {
                        case SpecialValues.NaN:
                            return NaN;
                        case SpecialValues.PositiveInfinity:
                            //if (br1 == Zero)
                            return NegativeInfinity;
                        //return PositiveInfinity;
                        case SpecialValues.NegativeInfinity:
                            //if (br1 == Zero)
                            return PositiveInfinity;
                        //return NegativeInfinity;
                    }
                    break;
            }
            return new BigRational(br1.Numerator * br2.Denominator - br2.Numerator * br1.Denominator, br1.Denominator * br2.Denominator);
        }
        /// <summary>
        /// Bildet die Differenz von einem Bruch und einem Skalar.
        /// </summary>
        /// <param name="br">Der Minuend (Bruch).</param>
        /// <param name="scalar">Der Subtrahend (Skalar).</param>
        /// <returns>Die Differenz aus <paramref name="br"/> und <paramref name="scalar"/>.</returns>
        /// <remarks>Das Ergebnis lässt sich eventuell noch vereinfachen.</remarks>
        public static BigRational operator -(BigRational br, BigInteger scalar)
        {
            //  if (br.SpecialValue != SpecialValues.None || scalar.SpecialValue != SpecialValues.None)
            switch (br.SpecialValue)
            {
                case SpecialValues.NaN:
                    return NaN;
                case SpecialValues.PositiveInfinity:
                    if (scalar.SpecialValue == SpecialValues.PositiveInfinity || scalar.SpecialValue == SpecialValues.NaN)
                        return NaN;
                    //if (br2 == Zero)
                    //    return Zero;
                    return PositiveInfinity;
                case SpecialValues.NegativeInfinity:
                    if (scalar.SpecialValue == SpecialValues.NegativeInfinity || scalar.SpecialValue == SpecialValues.NaN)
                        return NaN;
                    //if (br2 == Zero)
                    //    return Zero;
                    return NegativeInfinity;
                default:
                    switch (scalar.SpecialValue)
                    {
                        case SpecialValues.NaN:
                            return NaN;
                        case SpecialValues.PositiveInfinity:
                            //if (br1 == Zero)
                            return NegativeInfinity;
                        //return PositiveInfinity;
                        case SpecialValues.NegativeInfinity:
                            //if (br1 == Zero)
                            return PositiveInfinity;
                        //return NegativeInfinity;
                    }
                    break;
            }

            return new BigRational(br.Numerator - br.Denominator * scalar, br.Denominator);
        }

		public static BigInteger ToBigInt(BigRational m) {
			return (m.Numerator / m.Denominator);
		}

		/// <summary>
		/// Bildet die Differenz von einem Bruch und einem Skalar.
		/// </summary>
		/// <param name="scalar">Der Minuend (Skalar).</param>
		/// <param name="rational">Der Subtrahend (Bruch).</param>
		/// <returns>Die Differenz aus <paramref name="scalar"/> und <paramref name="rational"/>.</returns>
		/// <remarks>Das Ergebnis lässt sich eventuell noch vereinfachen.</remarks>
		public static BigRational operator -(BigInteger scalar, BigRational rational)
        {
            //if (scalar.SpecialValue != SpecialValues.None || br1.SpecialValue != SpecialValues.None)
            switch (scalar.SpecialValue)
            {
                case SpecialValues.NaN:
                    return NaN;
                case SpecialValues.PositiveInfinity:
                    if (rational.SpecialValue == SpecialValues.PositiveInfinity || rational.SpecialValue == SpecialValues.NaN)
                        return NaN;
                    //if (br2 == Zero)
                    //    return Zero;
                    return PositiveInfinity;
                case SpecialValues.NegativeInfinity:
                    if (rational.SpecialValue == SpecialValues.NegativeInfinity || rational.SpecialValue == SpecialValues.NaN)
                        return NaN;
                    //if (br2 == Zero)
                    //    return Zero;
                    return NegativeInfinity;
                default:
                    switch (rational.SpecialValue)
                    {
                        case SpecialValues.NaN:
                            return NaN;
                        case SpecialValues.PositiveInfinity:
                            //if (br1 == Zero)
                            return NegativeInfinity;
                        //return PositiveInfinity;
                        case SpecialValues.NegativeInfinity:
                            //if (br1 == Zero)
                            return PositiveInfinity;
                        //return NegativeInfinity;
                    }
                    break;
            }
            return new BigRational(rational.Denominator * scalar - rational.Numerator, rational.Denominator);
        }

        /// <summary>
        /// Negiert einen Wert.
        /// </summary>
        /// <param name="br">Der zu negierende Wert.</param>
        /// <returns>Der negierte Wert von <paramref name="br"/>.</returns>
        /// <remarks>Das Ergebnis lässt sich eventuell noch vereinfachen.</remarks>
        public static BigRational operator -(BigRational br)
        {
            switch (br.SpecialValue)
            {
                case SpecialValues.NegativeInfinity:
                    return PositiveInfinity;
                case SpecialValues.PositiveInfinity:
                    return NegativeInfinity;
                case SpecialValues.NaN:
                    return NaN;
            }
            return new BigRational(-br.Numerator, br.Denominator);
        }

        /// <summary>
        /// Berechnet das Produkt von zwei Brüchen.
        /// </summary>
        /// <param name="br1">Der erste Faktor.</param>
        /// <param name="br2">Der zweite Faktor.</param>
        /// <returns>Das Produkt aus <paramref name="br1"/> und <paramref name="br2"/>.</returns>
        /// <remarks>Das Ergebnis lässt sich eventuell noch vereinfachen.</remarks>
        public static BigRational operator *(BigRational br1, BigRational br2)
        {
            //if (br1.SpecialValue != SpecialValues.None || br2.SpecialValue != SpecialValues.None)
            switch (br1.SpecialValue)
            {
                case SpecialValues.NaN:
                    return NaN;
                case SpecialValues.PositiveInfinity:
                    if (br2.SpecialValue == SpecialValues.NaN)
                        return NaN;
                    if (br2 == Zero)
                        return Zero;
                    if (br2 < Zero)
                        return NegativeInfinity;
                    else
                        return PositiveInfinity;
                case SpecialValues.NegativeInfinity:
                    if (br2.SpecialValue == SpecialValues.NaN)
                        return NaN;
                    if (br2 == Zero)
                        return Zero;
                    if (br2 < Zero)
                        return PositiveInfinity;
                    else
                        return NegativeInfinity;
                default:
                    switch (br2.SpecialValue)
                    {
                        case SpecialValues.NaN:
                            return NaN;
                        case SpecialValues.PositiveInfinity:
                            if (br1 == Zero)
                                return Zero;
                            if (br1 < Zero)
                                return NegativeInfinity;
                            else
                                return PositiveInfinity;
                        case SpecialValues.NegativeInfinity:
                            if (br1 == Zero)
                                return Zero;
                            if (br1 < Zero)
                                return PositiveInfinity;
                            else
                                return NegativeInfinity;
                    }
                    break;
            }
            return new BigRational(br1.Numerator * br2.Numerator, br1.Denominator * br2.Denominator);
        }
        /// <summary>
        /// Multipliziert einen Bruch mit einem ganzzahligem Skalar.
        /// </summary>
        /// <param name="br">Der erste Faktor (Bruch).</param>
        /// <param name="scalar">Der zweite Faktor (Skalar).</param>
        /// <returns>Das Produkt aus <paramref name="br"/> und <paramref name="scalar"/>.</returns>
        /// <remarks>Das Ergebnis lässt sich eventuell noch vereinfachen.</remarks>
        public static BigRational operator *(BigRational br, BigInteger scalar)
        {
            if (br.SpecialValue != SpecialValues.None || scalar.SpecialValue != SpecialValues.None)
                switch (br.SpecialValue)
                {
                    case SpecialValues.NaN:
                        return NaN;
                    case SpecialValues.PositiveInfinity:
                        if (scalar.SpecialValue == SpecialValues.NaN)
                            return NaN;
                        if (scalar == Zero)
                            return Zero;
                        if (scalar < Zero)
                            return NegativeInfinity;
                        else
                            return PositiveInfinity;
                    case SpecialValues.NegativeInfinity:
                        if (scalar.SpecialValue == SpecialValues.NaN)
                            return NaN;
                        if (scalar == Zero)
                            return Zero;
                        if (scalar < Zero)
                            return PositiveInfinity;
                        else
                            return NegativeInfinity;
                    default:
                        switch (scalar.SpecialValue)
                        {
                            case SpecialValues.NaN:
                                return NaN;
                            case SpecialValues.PositiveInfinity:
                                if (br.SpecialValue == SpecialValues.NaN)
                                    return NaN;
                                if (br == Zero)
                                    return Zero;
                                if (br < Zero)
                                    return NegativeInfinity;
                                else
                                    return PositiveInfinity;
                            case SpecialValues.NegativeInfinity:
                                if (br.SpecialValue == SpecialValues.NaN)
                                    return NaN;
                                if (br == Zero)
                                    return Zero;
                                if (br < Zero)
                                    return PositiveInfinity;
                                else
                                    return NegativeInfinity;
                        }
                        break;
                }
            return new BigRational(br.Numerator * scalar, br.Denominator);
        }
        /// <summary>
        /// Multipliziert einen Bruch mit einem ganzzahligem Skalar.
        /// </summary>
        /// <param name="scalar">Der erste Faktor (Skalar).</param>
        /// <param name="br">Der zweite Faktor (Bruch).</param>
        /// <returns>Das Produkt aus <paramref name="br"/> und <paramref name="scalar"/>.</returns>
        /// <remarks>Das Ergebnis lässt sich eventuell noch vereinfachen.</remarks>
        public static BigRational operator *(BigInteger scalar, BigRational br)
        {
            return new BigRational(scalar * br.Numerator, br.Denominator);
        }

        /// <summary>
        /// Dividiert zwei Brüche durch einander.
        /// </summary>
        /// <param name="br1">Der Dividend.</param>
        /// <param name="br2">Der Divisor.</param>
        /// <returns>Der Quotient aus <paramref name="br1"/> und <paramref name="br2"/>.</returns>
        /// <remarks>Das Ergebnis lässt sich eventuell noch vereinfachen.</remarks>
        public static BigRational operator /(BigRational br1, BigRational br2)
        {
            //if (br1.SpecialValue != SpecialValues.None || br2.SpecialValue != SpecialValues.None)
            switch (br1.SpecialValue)
            {
                case SpecialValues.NaN:
                    return NaN;
                case SpecialValues.PositiveInfinity:
                    if (br2.SpecialValue == SpecialValues.None)
                    {
                        if (br2 == Zero)
                            return NaN;
                        if (br2 < Zero)
                            return NegativeInfinity;
                        else
                            return PositiveInfinity;
                    }
                    else
                        return NaN;
                case SpecialValues.NegativeInfinity:
                    if (br2.SpecialValue == SpecialValues.None)
                    {
                        if (br2 == Zero)
                            return NaN;
                        if (br2 < Zero)
                            return PositiveInfinity;
                        else
                            return NegativeInfinity;
                    }
                    else
                        return NaN;
                default:
                    switch (br2.SpecialValue)
                    {
                        case SpecialValues.NaN:
                            return NaN;
                        case SpecialValues.PositiveInfinity: //  x / unendlich = 0
                        case SpecialValues.NegativeInfinity:
                            return Zero;
                    }
                    break;
            }

            if (br2.IsZero)
                return NaN;

            return new BigRational(br1.Numerator * br2.Denominator, br1.Denominator * br2.Numerator);
        }
        /// <summary>
        /// Dividiert einen Bruch durch einen ganzzahligen Skalar.
        /// </summary>
        /// <param name="rational">Der Dividend (Bruch).</param>
        /// <param name="value">Der Divisor (Skalar).</param>
        /// <returns>Der Quotient aus <paramref name="rational"/> und <paramref name="value"/>.</returns>
        /// <remarks>Das Ergebnis lässt sich eventuell noch vereinfachen.</remarks>
        public static BigRational operator /(BigRational rational, BigInteger value)
        {
            //  if (br.SpecialValue != SpecialValues.None || value.SpecialValue != SpecialValues.None)
            switch (rational.SpecialValue)
            {
                case SpecialValues.NaN:
                    return NaN;
                case SpecialValues.PositiveInfinity:
                    if (value.SpecialValue == SpecialValues.None)
                    {
                        if (value == Zero)
                            return NaN;
                        if (value < Zero)
                            return NegativeInfinity;
                        else
                            return PositiveInfinity;
                    }
                    else
                        return NaN;
                case SpecialValues.NegativeInfinity:
                    if (value.SpecialValue == SpecialValues.None)
                    {
                        if (value == Zero)
                            return NaN;
                        if (value < Zero)
                            return PositiveInfinity;
                        else
                            return NegativeInfinity;
                    }
                    else
                        return NaN;
                default:
                    switch (value.SpecialValue)
                    {
                        case SpecialValues.NaN:
                            return NaN;
                        case SpecialValues.PositiveInfinity: //  x / unendlich = 0
                        case SpecialValues.NegativeInfinity:
                            return Zero;
                    }
                    break;
            }
            if (value.IsZero)
                return NaN;
            return new BigRational(rational.Numerator, rational.Denominator * value);
        }
        /// <summary>
        /// Dividiert einen ganzzahligen Wert durch einen Bruch.
        /// </summary>
        /// <param name="value">Der Divisor (Skalar).</param>
        /// <param name="rational">Der Dividend (Bruch).</param>
        /// <returns>Der Quotient aus <paramref name="value"/> und <paramref name="rational"/>.</returns>
        /// <remarks>Das Ergebnis lässt sich eventuell noch vereinfachen.</remarks>
        public static BigRational operator /(BigInteger value, BigRational rational)
        {
            //   if (value.SpecialValue != SpecialValues.None || br.SpecialValue != SpecialValues.None)
            switch (value.SpecialValue)
            {
                case SpecialValues.NaN:
                    return NaN;
                case SpecialValues.PositiveInfinity:
                    if (rational.SpecialValue == SpecialValues.None)
                    {
                        if (rational == Zero)
                            return NaN;
                        if (rational < Zero)
                            return NegativeInfinity;
                        else
                            return PositiveInfinity;
                    }
                    else
                        return NaN;
                case SpecialValues.NegativeInfinity:
                    if (rational.SpecialValue == SpecialValues.None)
                    {
                        if (rational == Zero)
                            return NaN;
                        if (rational < Zero)
                            return PositiveInfinity;
                        else
                            return NegativeInfinity;
                    }
                    else
                        return NaN;
                default:
                    switch (rational.SpecialValue)
                    {
                        case SpecialValues.NaN:
                            return NaN;
                        case SpecialValues.PositiveInfinity: //  x / unendlich = 0
                        case SpecialValues.NegativeInfinity:
                            return Zero;
                    }
                    break;
            }
            if (rational.IsZero)
                return NaN;
            return new BigRational(value * rational.Denominator, rational.Numerator);
        }

        /// <summary>
        /// Berechnet den Rest einer Division.
        /// </summary>
        /// <param name="br1">Der Dividend.</param>
        /// <param name="br2">Der Divisor.</param>
        /// <returns>Der Rest aus der Division von <paramref name="br1"/> durch <paramref name="br2"/>.</returns>
        public static BigRational operator %(BigRational br1, BigRational br2)
        {
            // a/b % c/d  == (a*d % b*c)/b*d
            return new BigRational((br1.Numerator * br2.Denominator) % (br1.Denominator * br2.Numerator), (br1.Denominator * br2.Denominator));
        }

        /// <summary>
        /// Erhöht den Wert des Bruchs um 1.
        /// </summary>
        /// <param name="value">Der zu erhöhende Wert.</param>
        /// <returns>Der Wert von <paramref name="value"/> erhöht um 1.</returns>
        /// <remarks>Das Ergebnis lässt sich eventuell noch vereinfachen.</remarks>
        public static BigRational operator ++(BigRational value)
        {
            if (value.SpecialValue != SpecialValues.None)
                return value;
            return new BigRational(value.Numerator + value.Denominator, value.Denominator);
        }
        /// <summary>
        /// Erniedrigt den Wert des Bruchs um 1.
        /// </summary>
        /// <param name="value">Der zu erhöhende Wert.</param>
        /// <returns>Der Wert von <paramref name="value"/> erniedrigt um 1.</returns>
        /// <remarks>Das Ergebnis lässt sich eventuell noch vereinfachen.</remarks>
        public static BigRational operator --(BigRational value)
        {
            if (value.SpecialValue != SpecialValues.None)
                return value;
            return new BigRational(value.Numerator - value.Denominator, value.Denominator);
        }

        /// <summary>
        /// Vergleicht zwei Brüche mit einander.
        /// </summary>
        /// <param name="br1">Der erste Bruch.</param>
        /// <param name="br2">Der zweite Bruch.</param>
        /// <returns><c>True</c>, wenn <paramref name="br1"/> und <paramref name="br2"/> im Quotienten übereinstimmen. Andernfalls <c>False</c>.</returns>
        public static bool operator ==(BigRational br1, BigRational br2)
        {
            if (br1.SpecialValue != SpecialValues.None)
                return false;
            if (br1.Denominator == br2.Denominator)
                return br1.Numerator == br2.Numerator;
            return br1.Numerator * br2.Denominator == br2.Numerator * br1.Denominator;
            //return br1.Numerator == br2.Numerator && br1.Denominator == br2.Denominator;
        }
        /// <summary>
        /// Vergleicht zwei Brüche mit einander.
        /// </summary>
        /// <param name="br1">Der erste Bruch.</param>
        /// <param name="br2">Der zweite Bruch.</param>
        /// <returns><c>False</c>, wenn <paramref name="br1"/> und <paramref name="br2"/> im Quotienten übereinstimmen. Andernfalls <c>True</c>.</returns>
        public static bool operator !=(BigRational br1, BigRational br2)
        {
            if (br1.SpecialValue != SpecialValues.None)
                return true;
            return !(br1 == br2);
        }

        /// <summary>
        /// Vergleicht die Größe der Quotienten von zwei Brüchen.
        /// </summary>
        /// <param name="br1">Der erste Vergleichswert.</param>
        /// <param name="br2">Der zweite Vergleichswert.</param>
        /// <returns><c>True</c>, wenn <paramref name="br1"/> kleiner ist als <paramref name="br2"/>. Andernfalls <c>False</c>.</returns>
        public static bool operator <(BigRational br1, BigRational br2)
        {
            if (br1.SpecialValue == SpecialValues.NaN || br2.SpecialValue == SpecialValues.NaN)
                return false;
            if (br1.SpecialValue == SpecialValues.NegativeInfinity && br2.SpecialValue != SpecialValues.NegativeInfinity)
                return true;
            if (br1.SpecialValue != SpecialValues.PositiveInfinity && br2.SpecialValue == SpecialValues.PositiveInfinity)
                return true;
            if (br1.SpecialValue != SpecialValues.None || br2.SpecialValue != SpecialValues.None)
                return false;

            if (br1.Denominator == br2.Denominator)
                return br1.Numerator < br2.Numerator;
            return br1.Numerator * br2.Denominator < br2.Numerator * br1.Denominator;
        }
        /// <summary>
        /// Vergleicht die Größe der Quotienten von zwei Brüchen.
        /// </summary>
        /// <param name="br1">Der erste Vergleichswert.</param>
        /// <param name="br2">Der zweite Vergleichswert.</param>
        /// <returns><c>True</c>, wenn <paramref name="br1"/> größer ist als <paramref name="br2"/>. Andernfalls <c>False</c>.</returns>
        public static bool operator >(BigRational br1, BigRational br2)
        {
            if (br1.SpecialValue == SpecialValues.NaN || br2.SpecialValue == SpecialValues.NaN)
                return false;
            if (br2.SpecialValue == SpecialValues.NegativeInfinity && br1.SpecialValue != SpecialValues.NegativeInfinity)
                return true;
            if (br2.SpecialValue != SpecialValues.PositiveInfinity && br1.SpecialValue == SpecialValues.PositiveInfinity)
                return true;
            if (br1.SpecialValue != SpecialValues.None || br2.SpecialValue != SpecialValues.None)
                return false;

            if (br1.Denominator == br2.Denominator)
                return br1.Numerator > br2.Numerator;
            return br1.Numerator * br2.Denominator > br2.Numerator * br1.Denominator;
        }
        /// <summary>
        /// Vergleicht die Größe der Quotienten von zwei Brüchen.
        /// </summary>
        /// <param name="br1">Der erste Vergleichswert.</param>
        /// <param name="br2">Der zweite Vergleichswert.</param>
        /// <returns><c>True</c>, wenn <paramref name="br1"/> kleiner oder gleich <paramref name="br2"/> ist. Andernfalls <c>False</c>.</returns>
        public static bool operator <=(BigRational br1, BigRational br2)
        {
            if (br1.Denominator == br2.Denominator)
                return br1.Numerator <= br2.Numerator;
            return br1.Numerator * br2.Denominator <= br2.Numerator * br1.Denominator;
        }
        /// <summary>
        /// Vergleicht die Größe der Quotienten von zwei Brüchen.
        /// </summary>
        /// <param name="br1">Der erste Vergleichswert.</param>
        /// <param name="br2">Der zweite Vergleichswert.</param>
        /// <returns><c>True</c>, wenn <paramref name="br1"/> größer oder gleich <paramref name="br2"/> ist. Andernfalls <c>False</c>.</returns>
        public static bool operator >=(BigRational br1, BigRational br2)
        {
            if (br1.Denominator == br2.Denominator)
                return br1.Numerator >= br2.Numerator;
            return br1.Numerator * br2.Denominator >= br2.Numerator * br1.Denominator;
        }

        /// <summary>
        /// Schneidet die Nachkommastelle(n) ab und gibt eine Instanz der <see cref="BigInteger"/>-Struktur zurück.
        /// </summary>
        /// <param name="value">Der zu konvertierende Wert.</param>
        /// <returns>Der Wert von <paramref name="value"/> ohne Nachkommastellen.</returns>
        public static explicit operator BigInteger(BigRational value)
        {
            switch (value.SpecialValue)
            {
                case SpecialValues.NegativeInfinity:
                    return BigInteger.NegativeInfinity;
                case SpecialValues.PositiveInfinity:
                    return BigInteger.PositiveInfinity;
                case SpecialValues.NaN:
                    return BigInteger.NaN;
            }
            return value.Numerator / value.Denominator;
        }

        /// <summary>
        /// Konvertiert den Bruch in eine Gleitkommazahl mit doppelter Genauigkeit.<para/>
        /// Diese Methode ist noch sehr ineffektiv!
        /// </summary>
        /// <param name="value">Der zu konvertierende Bruch.</param>
        /// <returns>Eine Gleitkommazahl mit doppelter Genauigkeit mit dem Wert von <paramref name="value"/>.</returns>
        public static explicit operator Double(BigRational value)
        {
            return double.Parse(value.ToDecimalString(13,CultureInfo.CurrentCulture ),CultureInfo.CurrentCulture );
        }

        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="BigRational"/>-Struktur aus dem angegebenen Wert.
        /// </summary>
        /// <param name="value">Der zu speichernde Wert.</param>
        /// <returns>Der Wert von <paramref name="value"/>, abgespeichert in einer Instanz der <see cref="BigRational"/>-Struktur.</returns>
        public static implicit operator BigRational(BigInteger value)
        {
            return new BigRational(value);
		}
		/// <summary>
		/// Erstellt eine neue Instanz der <see cref="BigRational"/>-Struktur aus dem angegebenen Wert.
		/// </summary>
		/// <param name="value">Der zu speichernde Wert.</param>
		/// <returns>Der Wert von <paramref name="value"/>, abgespeichert in einer Instanz der <see cref="BigRational"/>-Struktur.</returns>
		public static implicit operator BigRational(float value) {
			return new BigRational(value);
		}
		/// <summary>
		/// Erstellt eine neue Instanz der <see cref="BigRational"/>-Struktur aus dem angegebenen Wert.
		/// </summary>
		/// <param name="value">Der zu speichernde Wert.</param>
		/// <returns>Der Wert von <paramref name="value"/>, abgespeichert in einer Instanz der <see cref="BigRational"/>-Struktur.</returns>
		public static implicit operator BigRational(double value)
        {
            return new BigRational(value);
        }
        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="BigRational"/>-Struktur aus dem angegebenen Wert.
        /// </summary>
        /// <param name="value">Der zu speichernde Wert.</param>
        /// <returns>Der Wert von <paramref name="value"/>, abgespeichert in einer Instanz der <see cref="BigRational"/>-Struktur.</returns>
        public static implicit operator BigRational(decimal value)
        {
            return new BigRational(value);
        }
        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="BigRational"/>-Struktur aus dem angegebenen Wert.
        /// </summary>
        /// <param name="value">Der zu speichernde Wert.</param>
        /// <returns>Der Wert von <paramref name="value"/>, abgespeichert in einer Instanz der <see cref="BigRational"/>-Struktur.</returns>
        public static implicit operator BigRational(int value)
        {
            return new BigRational(value);
        }

        #endregion

        #region Methoden

        /// <summary>
        /// Vereinfacht diesen Bruch.
        /// </summary>
        public void Simplify()
        {
            if (this.Numerator.SpecialValue != SpecialValues.None || this.Numerator == 0)
            {
                this.Denominator = 1;
                return;
            }
            var gcd = BigInteger.GreatestCommonDivisor(this.Numerator, this.Denominator);
            this.Numerator = this.Numerator / gcd;
            this.Denominator = this.Denominator / gcd;
        }

        /// <summary>
        /// Negiert diesen Bruch.
        /// </summary>
        /// <param name="simplify">Übergeben Sie <c>True</c>, wenn der Rückgabewert gekürzt werden soll. Andernfalls <c>False</c>.</param>
        public void Negate(bool simplify = true)
        {
            this.Numerator = -this.Numerator;
            if (simplify)
                this.Simplify();
        }

        /// <summary>
        /// Wandelt den Bruch in eine Dezimalzahl mit der angegebenen Genauigkeit um.
        /// </summary>
        /// <param name="maxDecimalPlaces">Die Maximalzahl an Dezimalstellen.</param>
        /// <returns>Den Wert des Bruchs in der Dezimalschreibweiße mit der angegebenen Maximalzahl an Stellen nach dem Komma.</returns>
        
        public string ToDecimalString(uint maxDecimalPlaces)
        {
            return this.ToDecimalString(maxDecimalPlaces, CultureInfo.CurrentCulture);
        }
        /// <summary>
        /// Wandelt den Bruch in eine Dezimalzahl mit der angegebenen Genauigkeit um.
        /// </summary>
        /// <param name="maxDecimalPlaces">Die Maximalzahl an Dezimalstellen.</param>
        /// <param name="culture">Die Kultur, unter der die Umwandlung statt finden soll.</param>
        /// <returns>Den Wert des Bruchs in der Dezimalschreibweiße mit der angegebenen Maximalzahl an Stellen nach dem Komma.</returns>
        
        public string ToDecimalString(uint maxDecimalPlaces, CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException("culture",ResourceManager.GetMessage("ArgNull_Param", "culture"));
            switch (this.SpecialValue)
            {
                case SpecialValues.NaN:
                    return culture.NumberFormat.NaNSymbol;
                case SpecialValues.PositiveInfinity:
                    return culture.NumberFormat.PositiveInfinitySymbol;
                case SpecialValues.NegativeInfinity:
                    return culture.NumberFormat.NegativeInfinitySymbol;
                default:
                    if (this.Denominator.IsZero)
                        return culture.NumberFormat.NaNSymbol;
                    break;
            }

			//? Notizen siehe 09.11.2013 00:34
			var rational = BigRational.Truncate(this,maxDecimalPlaces);
            BigInteger e = 0;
            int y = GetToDecimalStringE(rational.Denominator, maxDecimalPlaces, out e);
            if (y == 0)
                return rational.Numerator.ToString();
            string num = (rational.Numerator * e).ToString();
			
			//UnityEngine.Debug.Log(rational.Numerator + " / " + rational.Denominator);
			while(y > num.Length) {
				num = "0" + num;
			}
            return (num.Length == y ? "0" : num.Substring(0, num.Length - y))
                + (y == 0 ? "" : culture.NumberFormat.NumberDecimalSeparator
                               + num.Substring(num.Length - y, y));
        }

        private static int GetToDecimalStringE(BigInteger denominator, uint maxDecimalPlaces, out BigInteger e)
        {
            int result = 0;
            BigInteger num = 1;
            while (result < maxDecimalPlaces)
            {
                if (num % denominator == 0)
                {
                    e = num / denominator;
                    return result;
                }

                num *= 10;
                ++result;
            }
            e = num / denominator;
            return result;
        }

        #endregion

        #region Methoden (static)

        /// <summary>
        /// Vereinfacht einen Bruch.
        /// </summary>
        /// <param name="br">Der zu vereinfachende Bruch.</param>
        /// <returns>Die vereinfachte Version von <paramref name="br"/>.</returns>
        public static BigRational Simplify(BigRational br)
        {
            br.Simplify();
            return br;
        }

        #region + - * / % neg

        /// <summary>
        /// Bildet die Summe von zwei Brüchen.
        /// </summary>
        /// <param name="br1">Der erste Summand.</param>
        /// <param name="br2">Der zweite Summand.</param>
        /// <param name="simplify">Geben Sie <c>True</c> an, wenn das Ergebnis gekürzt werden soll. Andernfalls <c>False</c>.<para />Dieser Parameter ist Optional. Der Standardwert ist <c>True</c>.</param>
        /// <returns>Die Summe aus <paramref name="br1"/> und <paramref name="br2"/>.</returns>
        public static BigRational Add(BigRational br1, BigRational br2, bool simplify = true)
        {
            if (simplify)
                return BigRational.Simplify(br1 + br2);
            else
                return br1 + br2;
        }
        /// <summary>
        /// Bildet die Summe aus einem Bruch und einem ganzzahligem Skalar.
        /// </summary>
        /// <param name="rational">Der erste Summand (Bruch).</param>
        /// <param name="scalar">Der zweite Summand (Skalar).</param>
        /// <param name="simplify">Geben Sie <c>True</c> an, wenn das Ergebnis gekürzt werden soll. Andernfalls <c>False</c>.<para />Dieser Parameter ist Optional. Der Standardwert ist <c>True</c>.</param>
        /// <returns>Die Summe aus <paramref name="rational"/> und <paramref name="scalar"/>.</returns>
        public static BigRational Add(BigRational rational, BigInteger scalar, bool simplify = true)
        {
            if (simplify)
                return BigRational.Simplify(rational + scalar);
            else
                return rational + scalar;
        }
        /// <summary>
        /// Bildet die Summe aus einem Bruch und einem ganzzahligem Skalar.
        /// </summary>
        /// <param name="scalar">Der erste Summand (Skalar).</param>
        /// <param name="rational">Der zweite Summand (Bruch).</param>
        /// <param name="simplify">Geben Sie <c>True</c> an, wenn das Ergebnis gekürzt werden soll. Andernfalls <c>False</c>.<para />Dieser Parameter ist Optional. Der Standardwert ist <c>True</c>.</param>
        /// <returns>Die Summe aus <paramref name="scalar"/> und <paramref name="rational"/>.</returns>
        public static BigRational Add(BigInteger scalar, BigRational rational, bool simplify = true)
        {
            if (simplify)
                return BigRational.Simplify(scalar + rational);
            else
                return scalar + rational;
        }

        /// <summary>
        /// Bildet die Differenz von zwei Brüchen.
        /// </summary>
        /// <param name="br1">Der Minuend.</param>
        /// <param name="br2">Der Subtrahend.</param>
        /// <param name="simplify">Geben Sie <c>True</c> an, wenn das Ergebnis gekürzt werden soll. Andernfalls <c>False</c>.<para />Dieser Parameter ist Optional. Der Standardwert ist <c>True</c>.</param>
        /// <returns>Die Differenz aus <paramref name="br1"/> und <paramref name="br2"/>.</returns>
        public static BigRational Subtract(BigRational br1, BigRational br2, bool simplify = true)
        {
            if (simplify)
                return BigRational.Simplify(br1 - br2);
            else
                return br1 - br2;
        }
        /// <summary>
        /// Bildet die Differenz von einem Bruch und einem Skalar.
        /// </summary>
        /// <param name="rational">Der Minuend (Bruch).</param>
        /// <param name="scalar">Der Subtrahend (Skalar).</param>
        /// <param name="simplify">Geben Sie <c>True</c> an, wenn das Ergebnis gekürzt werden soll. Andernfalls <c>False</c>.<para />Dieser Parameter ist Optional. Der Standardwert ist <c>True</c>.</param>
        /// <returns>Die Differenz aus <paramref name="rational"/> und <paramref name="scalar"/>.</returns>
        public static BigRational Subtract(BigRational rational, BigInteger scalar, bool simplify = true)
        {
            if (simplify)
                return BigRational.Simplify(rational - scalar);
            else
                return rational - scalar;
        }
        /// <summary>
        /// Bildet die Differenz von einem Bruch und einem Skalar.
        /// </summary>
        /// <param name="scalar">Der Minuend (Skalar).</param>
        /// <param name="rational">Der Subtrahend (Bruch).</param>
        /// <param name="simplify">Geben Sie <c>True</c> an, wenn das Ergebnis gekürzt werden soll. Andernfalls <c>False</c>.<para />Dieser Parameter ist Optional. Der Standardwert ist <c>True</c>.</param>
        /// <returns>Die Differenz aus <paramref name="scalar"/> und <paramref name="rational"/>.</returns>
        public static BigRational Subtract(BigInteger scalar, BigRational rational, bool simplify = true)
        {
            if (simplify)
                return BigRational.Simplify(scalar - rational);
            else
                return scalar - rational;
        }

        /// <summary>
        /// Negiert einen Wert.
        /// </summary>
        /// <param name="value">Der zu negierende Wert.</param>
        /// <param name="simplify">Geben Sie <c>True</c> an, wenn das Ergebnis gekürzt werden soll. Andernfalls <c>False</c>.<para />Dieser Parameter ist Optional. Der Standardwert ist <c>True</c>.</param>
        /// <returns>Der negierte Wert von <paramref name="value"/>.</returns>  
        public static BigRational Negate(BigRational value, bool simplify = true)
        {
            if (simplify)
                return BigRational.Simplify(-value);
            else
                return -value;
        }

        /// <summary>
        /// Erhöht den Wert des Bruchs um 1.
        /// </summary>
        /// <param name="value">Der zu erhöhende Wert.</param>
        /// <returns>Der Wert von <paramref name="value"/> erhöht um 1.</returns>
        /// <remarks>Das Ergebnis lässt sich eventuell noch vereinfachen.</remarks>
        public static BigRational Increment(BigRational value)
        {
            return ++value;
        }
        /// <summary>
        /// Erniedrigt den Wert des Bruchs um 1.
        /// </summary>
        /// <param name="value">Der zu erhöhende Wert.</param>
        /// <returns>Der Wert von <paramref name="value"/> erniedrigt um 1.</returns>
        /// <remarks>Das Ergebnis lässt sich eventuell noch vereinfachen.</remarks>
        public static BigRational Decrement(BigRational value)
        {
            return --value;
        }

        /// <summary>
        /// Berechnet das Produkt von zwei Brüchen.
        /// </summary>
        /// <param name="br1">Der erste Faktor.</param>
        /// <param name="br2">Der zweite Faktor.</param>
        /// <param name="simplify">Geben Sie <c>True</c> an, wenn das Ergebnis gekürzt werden soll. Andernfalls <c>False</c>.<para />Dieser Parameter ist Optional. Der Standardwert ist <c>True</c>.</param>
        /// <returns>Das Produkt aus <paramref name="br1"/> und <paramref name="br2"/>.</returns>
        public static BigRational Multiply(BigRational br1, BigRational br2, bool simplify = true)
        {
            if (simplify)
                return BigRational.Simplify(br1 * br2);
            else
                return br1 * br2;
        }
        /// <summary>
        /// Multipliziert einen Bruch mit einem ganzzahligem Skalar.
        /// </summary>
        /// <param name="rational">Der erste Faktor (Bruch).</param>
        /// <param name="scalar">Der zweite Faktor (Skalar).</param>
        /// <param name="simplify">Geben Sie <c>True</c> an, wenn das Ergebnis gekürzt werden soll. Andernfalls <c>False</c>.<para />Dieser Parameter ist Optional. Der Standardwert ist <c>True</c>.</param>
        /// <returns>Das Produkt aus <paramref name="rational"/> und <paramref name="scalar"/>.</returns>
        public static BigRational Multiply(BigRational rational, BigInteger scalar, bool simplify = true)
        {
            if (simplify)
                return BigRational.Simplify(rational * scalar);
            else
                return rational * scalar;
        }
        /// <summary>
        /// Multipliziert einen Bruch mit einem ganzzahligem Skalar.
        /// </summary>
        /// <param name="scalar">Der erste Faktor (Skalar).</param>
        /// <param name="rational">Der zweite Faktor (Bruch).</param>
        /// <param name="simplify">Geben Sie <c>True</c> an, wenn das Ergebnis gekürzt werden soll. Andernfalls <c>False</c>.<para />Dieser Parameter ist Optional. Der Standardwert ist <c>True</c>.</param>
        /// <returns>Das Produkt aus <paramref name="rational"/> und <paramref name="scalar"/>.</returns>
        public static BigRational Multiply(BigInteger scalar, BigRational rational, bool simplify = true)
        {
            if (simplify)
                return BigRational.Simplify(scalar * rational);
            else
                return scalar * rational;
        }

        /// <summary>
        /// Dividiert zwei Brüche durch einander.
        /// </summary>
        /// <param name="br1">Der Dividend.</param>
        /// <param name="br2">Der Divisor.</param>
        /// <param name="simplify">Geben Sie <c>True</c> an, wenn das Ergebnis gekürzt werden soll. Andernfalls <c>False</c>.<para />Dieser Parameter ist Optional. Der Standardwert ist <c>True</c>.</param>
        /// <returns>Der Quotient aus <paramref name="br1"/> und <paramref name="br2"/>.</returns>
        public static BigRational Divide(BigRational br1, BigRational br2, bool simplify = true)
        {
            if (simplify)
                return BigRational.Simplify(br1 / br2);
            else
                return br1 / br2;
        }
        /// <summary>
        /// Dividiert einen Bruch durch einen ganzzahligen Skalar.
        /// </summary>
        /// <param name="rational">Der Dividend (Bruch).</param>
        /// <param name="scalar">Der Divisor (Skalar).</param>
        /// <param name="simplify">Geben Sie <c>True</c> an, wenn das Ergebnis gekürzt werden soll. Andernfalls <c>False</c>.<para />Dieser Parameter ist Optional. Der Standardwert ist <c>True</c>.</param>
        /// <returns>Der Quotient aus <paramref name="rational"/> und <paramref name="scalar"/>.</returns>
        public static BigRational Divide(BigRational rational, BigInteger scalar, bool simplify = true)
        {
            if (simplify)
                return BigRational.Simplify(rational / scalar);
            else
                return rational / scalar;
        }
        /// <summary>
        /// Dividiert einen ganzzahligen Wert durch einen Bruch.
        /// </summary>
        /// <param name="scalar">Der Divisor (Skalar).</param>
        /// <param name="rational">Der Dividend (Bruch).</param>
        /// <param name="simplify">Geben Sie <c>True</c> an, wenn das Ergebnis gekürzt werden soll. Andernfalls <c>False</c>.<para />Dieser Parameter ist Optional. Der Standardwert ist <c>True</c>.</param>
        /// <returns>Der Quotient aus <paramref name="rational"/> und <paramref name="rational"/>.</returns>
        public static BigRational Divide(BigInteger scalar, BigRational rational, bool simplify = true)
        {
            if (simplify)
                return BigRational.Simplify(scalar / rational);
            else
                return scalar / rational;
        }

        /// <summary>
        /// Berechnet den Rest einer Division.
        /// </summary>
        /// <param name="br1">Der Dividend.</param>
        /// <param name="br2">Der Divisor.</param>
        /// <param name="simplify">Geben Sie <c>True</c> an, wenn das Ergebnis gekürzt werden soll. Andernfalls <c>False</c>.<para />Dieser Parameter ist Optional. Der Standardwert ist <c>True</c>.</param>
        /// <returns>Der Rest aus der Division von <paramref name="br1"/> durch <paramref name="br2"/>.</returns>
        public static BigRational Mod(BigRational br1, BigRational br2, bool simplify = true)
        {
            if (simplify)
                return BigRational.Simplify(br1 % br2);
            else
                return br1 % br2;
        }

        #endregion

        /// <summary>
        /// Berechnet den Absoluten Wert eines Bruchs.
        /// </summary>
        /// <param name="value">Ein Bruch, dessen absoluter Wert berechnet werden soll.</param>
        /// <param name="simplify">Geben Sie <c>True</c> an, wenn das Ergebnis gekürzt werden soll. Andernfalls <c>False</c>.<para />Dieser Parameter ist Optional. Der Standardwert ist <c>True</c>.</param>
        /// <returns>Der Absolute Wert von <paramref name="value"/>.</returns>
        public static BigRational Abs(BigRational value, bool simplify = true)
        {
            if (value.SpecialValue == SpecialValues.None)
            {
                if (simplify)
                    value.Simplify();
                if (value.IsNegative)
                    return -value;
                else
                    return value;
            }
            else
            {
                switch (value.SpecialValue)
                {
                    case SpecialValues.PositiveInfinity:
                    //    return NegativeInfinity;
                    case SpecialValues.NegativeInfinity:
                        return PositiveInfinity;
                    default:
                        return NaN;
                }
            }
        }

        /// <summary>
        /// BErechnet den Signierten Wert von einem Bruch.
        /// </summary>
        /// <param name="value">Der Bruch von dem der signierte Wert ermittelt werden soll.</param>
        /// <returns>Der signierte Wert von <paramref name="value"/>.</returns>
        public static BigRational Sgn(BigRational value)
        {
            if (value.IsNegative)
                return new BigRational(new BigInteger(-1));
            else if (value.IsPositive)
                return new BigRational(new BigInteger(1));
            else if (value.IsZero)
                return new BigRational(new BigInteger(0));
            else
                return NaN;
        }

        /// <summary>
        /// Bildet die Reziproke des angegebenen Bruchs.
        /// </summary>
        /// <param name="value">Der Bruch dessen Reziproke gebildet werden soll.</param>
        /// <param name="simplify">Geben Sie <c>True</c> an, wenn das Ergebnis gekürzt werden soll. Andernfalls <c>False</c>.<para />Dieser Parameter ist Optional. Der Standardwert ist <c>True</c>.</param>
        /// <returns>Die Reziproke von <paramref name="value"/>.</returns>
        public static BigRational Reciprocal(BigRational value, bool simplify = true)
        {
            if (simplify)
                return BigRational.Simplify(new BigRational(value.Denominator, value.Numerator));
            else
                return new BigRational(value.Denominator, value.Numerator);
        }

        /// <summary>
        /// Potenziert einen Bruch mit einem ganzzahligem Exponenten.
        /// </summary>
        /// <param name="baseValue">Die Basis der zu berechnenden Potenz. (Der Bruch)</param>
        /// <param name="exponent">Der Exponent der zu berechnenden Potenz. (Die ganze Zahl)</param>
        /// <param name="simplify">Geben Sie <c>True</c> an, wenn das Ergebnis gekürzt werden soll. Andernfalls <c>False</c>.<para />Dieser Parameter ist Optional. Der Standardwert ist <c>True</c>.</param>
        /// <returns>Die Potenz aus <paramref name="baseValue"/> und <paramref name="exponent"/>.</returns>
        public static BigRational Pow(BigRational baseValue, BigInteger exponent, bool simplify = false)
        {
            if (exponent == 0)
            {
                // 0^0 -> 1
                // n^0 -> 1
                return new BigRational(new BigInteger(1));
            }
            else if (exponent.IsNegative)
            {
                if (baseValue == BigRational.Zero)
                {
                    throw new ArgumentException("cannot raise zero to a negative power", "baseValue");
                }
                // n^(-e) -> (1/n)^e
                baseValue = BigRational.Reciprocal(baseValue, simplify);
                exponent = BigInteger.Negate(exponent);
            }

            BigRational r = 1;
			BigRational a = baseValue;
			while (exponent > _One)
            {
				if(exponent % _Two == _One) {
					r *= a;
				}
				a *= a;
				exponent = exponent >> 1;
			}
			r *= a;
			if(simplify)
				r.Simplify();

			return r;
        }

		public static BigRational Truncate(BigRational value, uint precision, bool simplify = false) {
			BigInteger m = new BigInteger(1, precision);
			BigRational result = new BigRational((value.Numerator * m)/value.Denominator, m);
			if(simplify)
				result.Simplify();
			return result;
		}

		/// <summary>
		/// Berechnet PI mit der angegebenen Anzahl an Nachkommastellen.
		/// </summary>
		/// <param name="numberOfDigitsRequired">Die Anzahl an Nachkommastellen.</param>
		/// <param name="simplify">Geben Sie <c>True</c> an, wenn das Ergebnis gekürzt werden soll. Andernfalls <c>False</c>.<para />Dieser Parameter ist Optional. Der Standardwert ist <c>True</c>.</param>
		/// <returns>PI mit <paramref name="numberOfDigitsRequired"/> Nachkommastellen.</returns>

		public static BigRational CalculatePi(uint numberOfDigitsRequired, bool simplify = true)
        {
            if (numberOfDigitsRequired == 0)//Keine Nachkommastellen
                return 3;

            //Quelle: http://kashfarooq.wordpress.com/tag/pi/
            numberOfDigitsRequired += 8; //  To be safe, compute 8 extra digits, to be dropped at end. The 8 is arbitrary

            //var a = BigInteger.Multiply(InverseTan(5, numberOfDigitsRequired), new BigInteger(16)); //16 x arctan(1/5)
            //var b = BigInteger.Multiply(InverseTan(239, numberOfDigitsRequired), new BigInteger(4)); //4 x arctan(1/239)
            //var c1 = a - b;
            //var c2 = b - a;

            var result = new BigRational(BigInteger.Divide(BigInteger.Subtract(
                    BigInteger.Multiply(CalculatePi_InverseTan(5, numberOfDigitsRequired), new BigInteger(16)), //16 x arctan(1/5)
                    BigInteger.Multiply(CalculatePi_InverseTan(239, numberOfDigitsRequired), new BigInteger(4)) //4 x arctan(1/239)
                ), new BigInteger(100000000)), BigInteger.Pow(10, numberOfDigitsRequired - 8));
            if (simplify)
                result.Simplify();
            return result;
        }

        private static BigInteger CalculatePi_InverseTan(int denominator, uint numberOfDigitsRequired)
        {
            int demonimatorSquared = denominator * denominator;

            int degreeNeeded = CalculatePi_GetDegreeOfPrecisionNeeded(demonimatorSquared, numberOfDigitsRequired);

            BigInteger tenToNumberPowerOfDigitsRequired = BigInteger.Pow(10, numberOfDigitsRequired);

            int c = 2 * degreeNeeded + 1;
            BigInteger s = BigInteger.Divide(tenToNumberPowerOfDigitsRequired, new BigInteger(c)); // s = (10^N)/c
            for (int i = 0; i < degreeNeeded; i++)
            {
                c = c - 2;
                var temp1 = BigInteger.Divide(tenToNumberPowerOfDigitsRequired, new BigInteger(c));
                var temp2 = BigInteger.Divide(s, new BigInteger(demonimatorSquared));
                s = BigInteger.Subtract(temp1, temp2);
            }
            //x    Console.WriteLine("Number of iterations=" + degreeNeeded);

            // return s/denominator, which is integer part of 10^numberOfDigitsRequired times arctan(1/k)
            return BigInteger.Divide(s, new BigInteger(denominator));

        }

        private static int CalculatePi_GetDegreeOfPrecisionNeeded(int demonimatorSquared, uint numberOfDigitsRequired)
        {
            //the degree of the Taylor polynomial needed to achieve numberOfDigitsRequired
            //digit accuracy of arctan(1/denominator).
            int degreeNeeded = 0;

            while ((Math.Log(2 * degreeNeeded + 3) + (degreeNeeded + 1) * Math.Log10(demonimatorSquared))
                                                    <= numberOfDigitsRequired * Math.Log(10))
            {
                degreeNeeded++;
            }
            return degreeNeeded;
        }

        /// <summary>
        /// Berechnet den Sinus von einem Wert mit Hilfe einer Taylorreihe.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <param name="iterations">Die Anzahl an Gliedern in der Taylorreihe.</param>
        /// <param name="simplify">Geben Sie <c>True</c> an, wenn das Ergebnis gekürzt werden soll. Andernfalls <c>False</c>.<para />Dieser Parameter ist Optional. Der Standardwert ist <c>True</c>.</param>
        /// <returns>Der Sinus von <paramref name="value"/>.</returns>
        public static BigRational TaylorSin(BigRational value, int iterations, bool simplify = true)
        {
            iterations *= 2;
            BigRational result = 0;
            BigRational num = value;
            BigInteger den = 1;
            bool b = false;
            for (int i = 1; i <= iterations; i += 2)
            {
                if (b)
                    result -= Divide(num, den, false);
                else
                    result += Divide(num, den, false);
                b = !b;
                den *= (i + 1) * (i + 2);
                num *= value * value;
                //  if(simplify )
                //      num.Simplify();
            }
            if (simplify)
                result.Simplify();
            return result;
        }

        #region Hilfsmethoden - CalculatePi

        ///// <summary>
        ///// Hilfsmethode von CalculatePi.
        ///// </summary>
        //public static BigInteger InverseTan(int denominator, int numberOfDigitsRequired)
        //{
        //    int demonimatorSquared = denominator * denominator;

        //    int degreeNeeded = GetDegreeOfPrecisionNeeded(demonimatorSquared, numberOfDigitsRequired);

        //    BigInteger tenToNumberPowerOfDigitsRequired = BigInteger.Pow(10, numberOfDigitsRequired);

        //    int c = 2 * degreeNeeded + 1;
        //    BigInteger s = tenToNumberPowerOfDigitsRequired / new BigInteger(c); // s = (10^N)/c
        //    for (int i = 0; i < degreeNeeded; i++)
        //    {
        //        c -= 2;
        //        var temp1 = tenToNumberPowerOfDigitsRequired / new BigInteger(c);
        //        var temp2 = s / new BigInteger(demonimatorSquared);
        //        var n = temp1 - temp2;
        //        s = (tenToNumberPowerOfDigitsRequired / new BigInteger(c)) - (s / new BigInteger(demonimatorSquared));
        //    }

        //    // return s/denominator, which is integer part of 10^numberOfDigitsRequired times arctan(1/k)
        //    return s / new BigInteger(denominator);
        //}

        ///// <summary>
        ///// Hilfsmethode von CalculatePi.
        ///// </summary>
        //private static int GetDegreeOfPrecisionNeeded(int demonimatorSquared, int numberOfDigitsRequired)
        //{
        //    int degreeNeeded = 0;
        //    double logDemonimatorSquared = Math.Log10(demonimatorSquared);
        //    double log10 = Math.Log(10);
        //    while ((Math.Log(2 * degreeNeeded + 3) + (degreeNeeded + 1) * logDemonimatorSquared) <= numberOfDigitsRequired * log10)
        //        degreeNeeded++;

        //    return degreeNeeded;
        //}

        #endregion

        #region Parse


        /// <summary>
        /// Erstellt eine neue <see cref="BigRational"/>-Instanz aus der angegebenen Zeichenfolge.
        /// </summary>
        /// <remarks>Es können entweder der Zähler und der Nenner, durch ein '/' getrennt oder eine Dezimalzahl, mit dem Dezimaltrennzeichen der angegebenen Kultur angegeben werden.</remarks>
        /// <param name="value">Die zu analysierende Zeichenfolge.</param>
        /// <param name="digits">Die zu verwendenden Ziffern.</param>
        /// <param name="radix">Die Basis der übergebenen Zahl(en).</param>
        /// <param name="culture">Die Kultur, unter der die Umwandlung statt finden soll.</param>
        /// <returns>Eine <see cref="BigRational"/>-Instanz mit dem Wert von <paramref name="value"/>.</returns>
        public static BigRational Parse(string value, string digits, int radix, CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException(ResourceManager.GetMessage("ArgNull_Param", "culture"));
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(ResourceManager.GetMessage("ArgNull_ParamEmpty", "value"));
            if (value == culture.NumberFormat.NegativeInfinitySymbol)
                return BigRational.NegativeInfinity;
            if (value == culture.NumberFormat.PositiveInfinitySymbol)
                return BigRational.PositiveInfinity;
            if (value == culture.NumberFormat.NaNSymbol)
                return BigRational.NaN;
            switch (value.ToLower())
            {
                case "∞":
                case "+∞":
                case "infinity":
                case "+infinity":
                    return BigRational.PositiveInfinity;
                case "-∞":
                case "-infinity":
                    return BigRational.NegativeInfinity;
                case "nan":
                    return BigRational.NaN;
            }

            if (value.Contains("/"))
            {
                string[] parts = value.Replace(" ", "").Split('/');
                return ParseDecimal(parts, digits, radix);
            }
            else
            {
                string[] parts = value.Replace(culture.NumberFormat.NumberGroupSeparator, "")
                                      .Split(new string[] { culture.NumberFormat.NumberDecimalSeparator, }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                    parts = new string[] { parts[0], "", };
                return ParseDecimal(parts, digits, radix);
            }
        }

        private static BigRational ParseDecimal(string[] parts, string digits, int radix)
        {
            return new BigRational(BigInteger.Parse(parts[0] + parts[1], digits, radix), BigInteger.Parse(digits[1] + string.Empty.PadLeft(parts[1].Length, digits[0]), digits, radix));
        }
		//      private static BigRational ParseRational(string[] parts, string digits, int radix)
		//      {
		//          return new BigRational(BigInteger.Parse(parts[0], digits, radix), BigInteger.Parse(parts[1], digits, radix));
		//      }

		#endregion

		#endregion

		#region Konstanten
#pragma warning disable 0414
		private static readonly BigRational _NaN = new BigRational(BigInteger.NaN);
        private static readonly BigRational _NegativeInfinity = new BigRational(BigInteger.NegativeInfinity);
        private static readonly BigRational _PositiveInfinity = new BigRational(BigInteger.PositiveInfinity);
        private static readonly BigRational _Zero = new BigRational(new BigInteger(0));
		private static readonly BigRational _One = new BigRational(new BigInteger(1));
		private static readonly BigRational _NegOne = new BigRational(new BigInteger(-1));
		private static readonly BigRational _Two = new BigRational(new BigInteger(2));
		private static readonly BigRational _Nineteen = new BigRational(new BigInteger(19));
#pragma warning restore 0414
		/// <summary>
		/// Stellt eine unveränderbare <see cref="BigRational"/>-Instanz dar, deren Wert keine Zahl ist.
		/// </summary>
		public static BigRational NaN
        {
            get
            {
                return _NaN;
            }
        }
        /// <summary>
        /// Stellt eine unveränderbare <see cref="BigRational"/>-Instanz dar, deren Wert negativ Unendlich ist.
        /// </summary>
        public static BigRational NegativeInfinity
        {
            get
            {
                return _NegativeInfinity;
            }
        }
        ///<summary>
        /// Stellt eine unveränderbare <see cref="BigRational"/>-Instanz dar, deren Wert positiv Unendlich ist.
        /// </summary>
        public static BigRational PositiveInfinity
        {
            get
            {
                return _PositiveInfinity;
            }
        }
        ///<summary>
        /// Stellt eine unveränderbare <see cref="BigRational"/>-Instanz dar, deren Wert 0 ist.
        /// </summary>
        public static BigRational Zero
        {
            get
            {
                return _Zero;
            }
        }

        #endregion

        #region Schnittstellen

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

        /// <summary>
        /// Ruft den Typ des gespeicherten Werts ab.
        /// </summary>
        public SpecialValues SpecialValue
        {
            get
            {
                if (this.Denominator.IsZero)
                    return SpecialValues.NaN;
                return this.Numerator.SpecialValue;
            }
        }

        #endregion

        #region IFormattable Member

        /// <summary>
        /// Ruft den gespeicherten Wert als Bruch ab.
        /// </summary>
        /// <param name="format">Das für die Formatierung zu verwendende Format.</param>
        /// <param name="formatProvider">Stellt Kulturspezifische Informationen für die Formatierungen bereit.</param>
        /// <returns>Eine Zeichenfolge, die den abgespeicherten Wert darstellt.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
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
                    if (this.Denominator.IsZero)
                        return "NaN";
                    else
                        return string.Format(formatProvider, "{0} / {1}", this.Numerator.ToString(format, formatProvider), this.Denominator.ToString(format, formatProvider));
            }
        }

        #endregion

        #region IComparable Member

        /// <summary>
        /// Vergleicht ein Objekt mit dieser Instanz und gibt einen Wert zurück, mit dem man das Objekt und den Bruch der Größe nach Ordnen kann.
        /// </summary>
        /// <param name="obj">Das zu vergleichende Objekt.</param>
        /// <returns><c>-1</c> wenn <paramref name="obj"/> vor dieser Instanz kommt, <c>0</c> wenn <paramref name="obj"/> den 
        /// gleichen Wert wie diese Instanz hat oder <c>1</c> wenn <paramref name="obj"/> hinter dieser Instanz kommt.<para/>
        /// Wenn <paramref name="obj"/> nicht vom Typ <see cref="BigRational"/> ist, dann wird <c>1</c> zurück gegeben.</returns>
        public int CompareTo(object obj)
        {
            if (obj is BigRational)
                return CompareTo((BigRational)obj);
            else
                return 1;
        }

        #endregion

        #region IComparable<BigRational> Member

        /// <summary>
        /// Vergleicht einen Bruch mit dieser Instanz und gibt einen Wert zurück, mit dem man die die beiden Brüche der größe nach Ordnen kann.
        /// </summary>
        /// <param name="other">Der zu vergleichende Bruch.</param>
        /// <returns><c>-1</c> wenn <paramref name="other"/> vor dieser Instanz kommt, <c>0</c> wenn <paramref name="other"/> den 
        /// gleichen Wert wie diese Instanz hat oder <c>1</c> wenn <paramref name="other"/> hinter dieser Instanz kommt.</returns>
        public int CompareTo(BigRational other)
        {
            if (this < other)
                return -1;
            if (this > other)
                return 1;
            if (this == other)
                return 0;
            if (this.SpecialValue != SpecialValues.NaN)
                return 1;
            return other.SpecialValue != SpecialValues.NaN ? -1 : 0;
        }

        #endregion

        #region IEquatable<BigRational> Member

        /// <summary>
        /// Vergleicht eine Instanz der <see cref="BigRational"/>-Struktur mit dieser Instanz.
        /// </summary>
        /// <param name="other">Die zu vergleichende Instanz.</param>
        /// <returns><c>True</c>, wenn der Quotient dieser Instanz mit dem Quotienten von <paramref name="other"/> übereinstimmt. Andernfals <c>False</c>.</returns>
        public bool Equals(BigRational other)
        {
            return this == other;
        }

        #endregion

        #region IMathComparison<BigRational> Member

        /// <summary>
        /// Überprüft, ob ein Wert größer ist, als der Wert dieser Instanz.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> größer ist als der Wert dieser Instanz. Andernfalls <c>False</c>.</returns>
        public bool IsGreater(BigRational param)
        {
            return this > param;
        }

        /// <summary>
        /// Überprüft, ob ein Wert größer oder gleich dem Wert dieser Instanz ist.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> größer oder gleich dem Wert dieser Instanz ist. Andernfalls <c>False</c>.</returns>
        public bool IsGreaterEqual(BigRational param)
        {
            return this >= param;
        }

        /// <summary>
        /// Überprüft, ob ein Wert größer ist, als der Wert dieser Instanz.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> kleiner ist als der Wert dieser Instanz. Andernfalls <c>False</c>.</returns>
        public bool IsSmaller(BigRational param)
        {
            return this < param;
        }

        /// <summary>
        /// Überprüft, ob ein Wert kleiner oder gleich dem Wert dieser Instanz ist.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> kleiner oder gleich dem Wert dieser Instanz ist. Andernfalls <c>False</c>.</returns>
        public bool IsSmallerEqual(BigRational param)
        {
            return this <= param;
        }

        #endregion

        #region IMathEqualComparison<BigRational> Member

        /// <summary>
        /// Vergleicht ein Objekt mit dieser Instanz auf Gleichheit.
        /// </summary>
        /// <param name="param">Das zu vergleichende Objekt.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> vom Typ <see cref="BigRational"/> ist und sich der Quotient von <paramref name="param"/> nicht von dieser Instanz unterscheidet. Andernfalls <c>False</c>.</returns>
        public bool IsEqual(BigRational param)
        {
            return this == param;
        }

        /// <summary>
        /// Vergleicht ein Objekt mit dieser Instanz auf Gleichheit.
        /// </summary>
        /// <param name="param">Das zu vergleichende Objekt.</param>
        /// <returns><c>False</c>, wenn <paramref name="param"/> vom Typ <see cref="BigRational"/> ist und sich der Quotient von <paramref name="param"/> nicht von dieser Instanz unterscheidet. Andernfalls <c>True</c>.</returns>
        public bool IsNotEqual(BigRational param)
        {
            return this != param;
        }

        /// <summary>
        /// Überprüft ob sich der Wert in der angegebenen Menge befindet.
        /// </summary>
        /// <param name="domainSet">Die zu prüfende Menge.</param>
        /// <returns><c>True</c>, wenn <paramref name="domainSet"/> den Wert dieser Instanz enthält.</returns>
        public bool IsInDomain(DomainSet domainSet)
        {
            return (int)domainSet.Set >= (int)DomainSets.Rational;
        }

        #endregion

        #endregion

        #region override

        /// <summary>
        /// Ermittelt den Hashcode dieser Instanz.
        /// </summary>
        /// <returns>Der Hashcode dieser Instanz.</returns>
        public override int GetHashCode()
        {
            return this.Numerator.GetHashCode() ^ this.Denominator.GetHashCode();
        }

        /// <summary>
        /// Vergleicht ein Objekt mit dieser Instanz auf Gleichheit.
        /// </summary>
        /// <param name="obj">Das zu vergleichende Objekt.</param>
        /// <returns><c>True</c>, wenn <paramref name="obj"/> vom Typ <see cref="BigRational"/> ist und sich der Quotient von <paramref name="obj"/> nicht von dieser Instanz unterscheidet. Andernfalls <c>False</c>.</returns>
        public override bool Equals(object obj)
        {
            return obj is BigRational && this == (BigRational)obj;
        }

        /// <summary>
        /// Ruft den gespeicherten Wert als Bruch ab.
        /// </summary>
        /// <returns>Eine Zeichenfolge, die den abgespeicherten Wert darstellt.</returns>
        public override string ToString()
        {
            return this.ToString("", CultureInfo.CurrentCulture);
        }

        #endregion
    }
}
