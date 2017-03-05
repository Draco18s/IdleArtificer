//! Created by Tom Lambert alias Koopakiller
//! Project started: 2011
//! License: Microsoft Reciprocal License (Ms-RL)
//! Project site: https://numerics.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Koopakiller.Numerics.Resources;

namespace Koopakiller.Numerics
{
    /// <summary>
    /// Stellt eine komplexe Zahl dar.
    /// </summary>
    public struct Complex : /*IBigNumber,*/INumber, IFormattable, IComparable, IComparable<Complex>, IEquatable<Complex>, IMathComparison<Complex>
    {
        #region .ctor

        /// <summary>
        /// Erstellt eine neue Instanz der Complex-Struktur.
        /// </summary>
        /// <param name="real">Der reale Anteil des Wertes.</param>
        public Complex(double real) : this(real, 0D) { }
        /// <summary>
        /// Erstellt eine neue Instanz der Complex-Struktur.
        /// </summary>
        /// <param name="real">Der reale Anteil des Wertes.</param>
        /// <param name="img">Der imaginäre Anteil des Wertes.</param>
        public Complex(double real, double img) : this(real, img, AngleUnits.Radian) { }
        /// <summary>
        /// Erstellt eine neue Instanz der Complex-Struktur.
        /// </summary>
        /// <param name="real">Der reale Anteil des Wertes.</param>
        /// <param name="img">Der imaginäre Anteil des Wertes.</param>
        /// <param name="angleUnit">Die Einheit, in der Winkelangaben gemacht werden.</param>
        public Complex(double real, double img, AngleUnits angleUnit)
            : this()
        {
            this._Real = real;
            this._Imaginary = img;
            this._AngleUnit = angleUnit;
        }

        #endregion

        #region Felder

        private double _Real;
        private double _Imaginary;
        private AngleUnits _AngleUnit;

        #endregion

        #region Eigenschaften

        /// <summary>
        /// Ruft den Reellen Anteil des Wertes ab.
        /// </summary>
        public double Real
        {
            get
            {
                return this._Real;
            }
        }
        /// <summary>
        /// Ruft den Imaginären Anteil des Wertes ab.
        /// </summary>
        public double Imaginary
        {
            get
            {
                return this._Imaginary;
            }
        }
        /// <summary>
        /// Ruft die Magnitude des Wertes ab.
        /// </summary>
        public double Magnitude
        {
            get
            {
                return Complex.Abs(this);
            }
        }
        /// <summary>
        /// Ruft die Phase des Wertes ab.
        /// </summary>
        public double Phase
        {
            get
            {
                return Math.Atan2(this._Imaginary, this._Real);
            }
        }
        /// <summary>
        /// Ruft die Einheit ab, in der Winkel angegeben werden.
        /// </summary>
        public AngleUnits AngleUnit
        {
            get
            {
                return _AngleUnit;
            }
        }

        #endregion

        #region Mathematische Methoden

        /// <summary>
        /// Berechnet den Absolutwert einer komplexen Zahl.
        /// </summary>
        /// <param name="value">Der Wert, dessen Absolutwert berechnet werden sollen.</param>
        /// <returns>Der Absolutwert von <paramref name="value"/>.</returns>
        public static double Abs(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return double.NaN;
            if (double.IsInfinity(value._Real) || double.IsInfinity(value._Imaginary))
                return double.PositiveInfinity;

            double absReal = Math.Abs(value._Real);
            double absImg = Math.Abs(value._Imaginary);
            double tmp = 0;
            if (absReal > absImg)
            {
                tmp = absImg / absReal;
                return (absReal * Math.Sqrt(1.0 + (tmp * tmp)));
            }
            if (absImg == 0.0)
            {
                return absReal;
            }
            tmp = absReal / absImg;
            return (absImg * Math.Sqrt(1.0 + (tmp * tmp)));
        }

        /// <summary>
        /// Erstellt einen neuen Complex-Wert aus Polarkoordinaten.
        /// </summary>
        /// <param name="magnitude">Die Magnitude der Polarkoordinaten.</param>
        /// <param name="phase">Die Phase der Polarkoordinaten.</param>
        /// <returns>Ein neuer Complex-Wert, berechnet aus <paramref name="magnitude"/> und <paramref name="phase"/>.</returns>
        public static Complex FromPolarCoordinates(double magnitude, double phase)
        {
            return new Complex(magnitude * Math.Cos(phase), magnitude * Math.Sin(phase));
        }

        private static Complex Scale(Complex value, double factor)
        {
            return new Complex(factor * value._Real, factor * value._Imaginary);
        }

        /// <summary>
        /// Berechnet die Reziproke einer Zahl.
        /// </summary>
        /// <param name="value">Die Zahl.</param>
        /// <returns>Die Reziproke der übergebenen Zahl.</returns>
        public static Complex Reciprocal(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            if ((value._Real == 0.0) && (value._Imaginary == 0.0))
            {
                return _Zero;
            }
            return (_One / value);
        }

        #region Trigonometrie

        /// <summary>
        /// Gibt den Sinus des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Sinus von <paramref name="value"/>.</returns>
        public static Complex Sin(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return new Complex(Math.Sin(value._Real) * Math.Cosh(value._Imaginary), Math.Cos(value._Real) * Math.Sinh(value._Imaginary));
        }
        /// <summary>
        /// Gibt den Kosinus des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Kosinus von <paramref name="value"/>. </returns>
        public static Complex Cos(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return new Complex(Math.Cos(value._Real) * Math.Cosh(value._Imaginary), -(Math.Sin(value._Real) * Math.Sinh(value._Imaginary)));
        }
        /// <summary>
        /// Gibt den Tangens des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Tangens von <paramref name="value"/>. </returns>
        public static Complex Tan(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return Sin(value) / Cos(value);
        }
        /// <summary>
        /// Gibt den Kotangens des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Kotangens von <paramref name="value"/>. </returns>
        public static Complex Cot(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return 1 / Tan(value);
        }
        /// <summary>
        /// Gibt den Sekans des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Sekans von <paramref name="value"/>. </returns>
        public static Complex Sec(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return 1 / Cos(value);
        }
        /// <summary>
        /// Gibt den Kosekans des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Kosekans von <paramref name="value"/>. </returns>
        public static Complex Csc(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return 1 / Sin(value);
        }

        /// <summary>
        /// Gibt den Hyperbelsinus des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Hyperbelsinus von <paramref name="value"/>.</returns>
        public static Complex Sinh(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return new Complex(Math.Sinh(value._Real) * Math.Cos(value._Imaginary), Math.Cosh(value._Real) * Math.Sin(value._Imaginary));
        }
        /// <summary>
        /// Gibt den Hyperbelkosinus des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Hyperbelkosinus von <paramref name="value"/>.</returns>
        public static Complex Cosh(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return new Complex(Math.Cosh(value._Real) * Math.Cos(value._Imaginary), Math.Sinh(value._Real) * Math.Sin(value._Imaginary));
        }
        /// <summary>
        /// Gibt den Hyperbeltangens des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Hyperbeltangens von <paramref name="value"/>.</returns>
        public static Complex Tanh(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return Sinh(value) / Cosh(value);
        }
        /// <summary>
        /// Gibt den Hyperbelkotangens des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Hyperbelkosinus von <paramref name="value"/>.</returns>
        public static Complex Coth(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return Cosh(value) / Sinh(value);
        }
        /// <summary>
        /// Gibt den Hyperbelkosekans des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Hyperbelkosekans von <paramref name="value"/>.</returns>
        public static Complex Sech(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return 1 / Cosh(value);
        }
        /// <summary>
        /// Gibt den Hyperbelsekans des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Hyperbelsekans von <paramref name="value"/>.</returns>
        public static Complex Csch(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return 1 / Sinh(value);
        }

        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelsinus die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Kotangens darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelsinus <paramref name="value"/> ist.</returns>
        public static Complex Asin(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return -_ImaginaryOne * Ln((_ImaginaryOne * value) + Sqrt(_One - (value * value)));
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelkosinus die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Kotangens darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelkosinus <paramref name="value"/> ist.</returns>
        public static Complex Acos(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return -_ImaginaryOne * Ln(value + (_ImaginaryOne * Sqrt(_One - (value * value))));
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbeltangens die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Kotangens darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbeltangens <paramref name="value"/> ist.</returns>
        public static Complex Atan(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            Complex complex = new Complex(2.0, 0.0);
            return (_ImaginaryOne / complex) * (Ln(_One - (_ImaginaryOne * value)) - Ln(_One + (_ImaginaryOne * value)));
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelkotangens die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Kotangens darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelkotangens <paramref name="value"/> ist.</returns>
        public static Complex Acot(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return Atan(1 / value);
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelsekans die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Kotangens darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelsekans <paramref name="value"/> ist.</returns>
        public static Complex Asec(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return Acos(1 / value);
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelkosekans die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Kotangens darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelkosekans <paramref name="value"/> ist.</returns>
        public static Complex Acsc(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return Asin(1 / value);
        }

        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelsinus die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Hyperbelsinus darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelsinus <paramref name="value"/> ist.</returns>
        public static Complex Asinh(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return Ln(value + Sqrt(value * value + 1));
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelkosinus die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Hyperbelkosinus darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelkosinus <paramref name="value"/> ist.</returns>
        public static Complex Acosh(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return Ln(value + Sqrt(value * value - 1));
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbeltangens die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Hyperbeltangens darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbeltangens <paramref name="value"/> ist.</returns>
        public static Complex Atanh(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return 0.5 * Ln((1 + value) / (1 - value));
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelkotangens die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Hyperbelkotangens darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelkotangens <paramref name="value"/> ist.</returns>
        public static Complex Acoth(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return 0.5 * Ln((value + 1) / (value - 1));
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelsekans die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Hyperbelsekans darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelsekans <paramref name="value"/> ist.</returns>
        public static Complex Asech(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return Ln((1 + Sqrt(1 - value * value)) / value);
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelkosekans die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Hyperbelkosekans darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelkosekans <paramref name="value"/> ist.</returns>
        public static Complex Acsch(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return Ln((1 + Sqrt(1 + value * value)) / value);
        }

        /// <summary>
        /// Gibt den Sinus des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß, Gradmaß oder Gon angegebener Winkel/Wert.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Der Sinus von <paramref name="value"/>. </returns>
        public static Complex Sin(Complex value, AngleUnits angleUnit)
        {
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return Sin(value / 180 * Math.PI);
                case AngleUnits.Gradian:
                    return Sin(value / 200 * Math.PI);
                case AngleUnits.Radian:
                default:
                    return Sin(value);
            }
        }
        /// <summary>
        /// Gibt den Kosinus des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß, Gradmaß oder Gon angegebener Winkel/Wert.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Der Kosinus von <paramref name="value"/>. </returns>
        public static Complex Cos(Complex value, AngleUnits angleUnit)
        {
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return Cos(value / 180 * Math.PI);
                case AngleUnits.Gradian:
                    return Cos(value / 200 * Math.PI);
                case AngleUnits.Radian:
                default:
                    return Cos(value);
            }
        }
        /// <summary>
        /// Gibt den Tangens des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß, Gradmaß oder Gon angegebener Winkel/Wert.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Der Tangens von <paramref name="value"/>. </returns>
        public static Complex Tan(Complex value, AngleUnits angleUnit)
        {
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return Tan(value / 180 * Math.PI);
                case AngleUnits.Gradian:
                    return Tan(value / 200 * Math.PI);
                case AngleUnits.Radian:
                default:
                    return Tan(value);
            }
        }
        /// <summary>
        /// Gibt den Kotangens des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß, Gradmaß oder Gon angegebener Winkel/Wert.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Der Kotangens von <paramref name="value"/>. </returns>
        public static Complex Cot(Complex value, AngleUnits angleUnit)
        {
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return Cot(value / 180 * Math.PI);
                case AngleUnits.Gradian:
                    return Cot(value / 200 * Math.PI);
                case AngleUnits.Radian:
                default:
                    return Cot(value);
            }
        }
        /// <summary>
        /// Gibt den Sekans des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß, Gradmaß oder Gon angegebener Winkel/Wert.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Der Sekans von <paramref name="value"/>. </returns>
        public static Complex Sec(Complex value, AngleUnits angleUnit)
        {
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return Sec(value / 180 * Math.PI);
                case AngleUnits.Gradian:
                    return Sec(value / 200 * Math.PI);
                case AngleUnits.Radian:
                default:
                    return Sec(value);
            }
        }
        /// <summary>
        /// Gibt den Kosekans des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß, Gradmaß oder Gon angegebener Winkel/Wert.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Der Kosekans von <paramref name="value"/>. </returns>
        public static Complex Csc(Complex value, AngleUnits angleUnit)
        {
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return Csc(value / 180 * Math.PI);
                case AngleUnits.Gradian:
                    return Csc(value / 200 * Math.PI);
                case AngleUnits.Radian:
                default:
                    return Csc(value);
            }
        }

        /// <summary>
        /// Gibt den Hyperbelsinus des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß, Gradmaß oder Gon angegebener Winkel/Wert.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Der Hyperbelsinus von <paramref name="value"/>.</returns>
        public static Complex Sinh(Complex value, AngleUnits angleUnit)
        {
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return Sinh(value / 180 * Math.PI);
                case AngleUnits.Gradian:
                    return Sinh(value / 200 * Math.PI);
                case AngleUnits.Radian:
                default:
                    return Sinh(value);
            }
        }
        /// <summary>
        /// Gibt den Hyperbelkosinus des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß, Gradmaß oder Gon angegebener Winkel/Wert.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Der Hyperbelkosinus von <paramref name="value"/>.</returns>
        public static Complex Cosh(Complex value, AngleUnits angleUnit)
        {
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return Cosh(value / 180 * Math.PI);
                case AngleUnits.Gradian:
                    return Cosh(value / 200 * Math.PI);
                case AngleUnits.Radian:
                default:
                    return Cosh(value);
            }
        }
        /// <summary>
        /// Gibt den Hyperbeltangens des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß, Gradmaß oder Gon angegebener Winkel/Wert.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Der Hyperbeltangens von <paramref name="value"/>.</returns>
        public static Complex Tanh(Complex value, AngleUnits angleUnit)
        {
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return Tanh(value / 180 * Math.PI);
                case AngleUnits.Gradian:
                    return Tanh(value / 200 * Math.PI);
                case AngleUnits.Radian:
                default:
                    return Tanh(value);
            }
        }
        /// <summary>
        /// Gibt den Hyperbelkotangens des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß, Gradmaß oder Gon angegebener Winkel/Wert.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Der Hyperbelkotangens von <paramref name="value"/>.</returns>
        public static Complex Coth(Complex value, AngleUnits angleUnit)
        {
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return Coth(value / 180 * Math.PI);
                case AngleUnits.Gradian:
                    return Coth(value / 200 * Math.PI);
                case AngleUnits.Radian:
                default:
                    return Coth(value);
            }
        }
        /// <summary>
        /// Gibt den Hyperbelsekans des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß, Gradmaß oder Gon angegebener Winkel/Wert.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Der Hyperbelsekans von <paramref name="value"/>.</returns>
        public static Complex Csch(Complex value, AngleUnits angleUnit)
        {
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return Csch(value / 180 * Math.PI);
                case AngleUnits.Gradian:
                    return Csch(value / 200 * Math.PI);
                case AngleUnits.Radian:
                default:
                    return Csch(value);
            }
        }
        /// <summary>
        /// Gibt den Hyperbelsekans des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß, Gradmaß oder Gon angegebener Winkel/Wert.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Der Hyperbelsekans von <paramref name="value"/>.</returns>
        public static Complex Sech(Complex value, AngleUnits angleUnit)
        {
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return Sech(value / 180 * Math.PI);
                case AngleUnits.Gradian:
                    return Sech(value / 200 * Math.PI);
                case AngleUnits.Radian:
                default:
                    return Sech(value);
            }
        }

        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Sinus die angegebene Zahl
        /// </summary>
        /// <param name="value">Eine Zahl die einen Sinus darstellt.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Ein Wert/Winkel dessen Sinus <paramref name="value"/> ist.</returns>
        public static Complex Asin(Complex value, AngleUnits angleUnit)
        {
            Complex a = Asin(value);
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return a / Math.PI * 180;
                case AngleUnits.Gradian:
                    return a / Math.PI * 200;
                case AngleUnits.Radian:
                default:
                    return a;
            }
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Kosinus die angegebene Zahl
        /// </summary>
        /// <param name="value">Eine Zahl die einen Kosinus darstellt.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Ein Wert/Winkel dessen Kosinus <paramref name="value"/> ist.</returns>
        public static Complex Acos(Complex value, AngleUnits angleUnit)
        {
            Complex a = Acos(value);
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return a / Math.PI * 180;
                case AngleUnits.Gradian:
                    return a / Math.PI * 200;
                case AngleUnits.Radian:
                default:
                    return a;
            }
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Tangens die angegebene Zahl
        /// </summary>
        /// <param name="value">Eine Zahl die einen Tangens darstellt.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Ein Wert/Winkel dessen Tangens <paramref name="value"/> ist.</returns>
        public static Complex Atan(Complex value, AngleUnits angleUnit)
        {
            Complex a = Atan(value);
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return a / Math.PI * 180;
                case AngleUnits.Gradian:
                    return a / Math.PI * 200;
                case AngleUnits.Radian:
                default:
                    return a;
            }
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Kotangens die angegebene Zahl
        /// </summary>
        /// <param name="value">Eine Zahl die einen Kotangens darstellt.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Ein Wert/Winkel dessen Kotangens <paramref name="value"/> ist.</returns>
        public static Complex Acot(Complex value, AngleUnits angleUnit)
        {
            Complex a = Atan(1 / value);
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return a / Math.PI * 180;
                case AngleUnits.Gradian:
                    return a / Math.PI * 200;
                case AngleUnits.Radian:
                default:
                    return a;
            }
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Sekans die angegebene Zahl
        /// </summary>
        /// <param name="value">Eine Zahl die einen Sekans darstellt.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Ein Wert/Winkel dessen Sekans <paramref name="value"/> ist.</returns>
        public static Complex Asec(Complex value, AngleUnits angleUnit)
        {
            Complex a = Acos(1 / value);
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return a / Math.PI * 180;
                case AngleUnits.Gradian:
                    return a / Math.PI * 200;
                case AngleUnits.Radian:
                default:
                    return a;
            }
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Kosekans die angegebene Zahl
        /// </summary>
        /// <param name="value">Eine Zahl die einen Kosekans darstellt.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Ein Wert/Winkel dessen Kosekans <paramref name="value"/> ist.</returns>
        public static Complex Acsc(Complex value, AngleUnits angleUnit)
        {
            Complex a = Asin(1 / value);
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return a / Math.PI * 180;
                case AngleUnits.Gradian:
                    return a / Math.PI * 200;
                case AngleUnits.Radian:
                default:
                    return a;
            }
        }

        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelsinus die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Hyperbelsinus darstellt.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelsinus <paramref name="value"/> ist.</returns>
        public static Complex Asinh(Complex value, AngleUnits angleUnit)
        {
            Complex a = Asinh(value);
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return a / Math.PI * 180;
                case AngleUnits.Gradian:
                    return a / Math.PI * 200;
                case AngleUnits.Radian:
                default:
                    return a;
            }
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelkosinus die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Hyperbelkosinus darstellt.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelkosinus <paramref name="value"/> ist.</returns>
        public static Complex Acosh(Complex value, AngleUnits angleUnit)
        {
            Complex a = Acosh(value);
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return a / Math.PI * 180;
                case AngleUnits.Gradian:
                    return a / Math.PI * 200;
                case AngleUnits.Radian:
                default:
                    return a;
            }
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbeltangens die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Hyperbeltangens darstellt.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbeltangens <paramref name="value"/> ist.</returns>
        public static Complex Atanh(Complex value, AngleUnits angleUnit)
        {
            Complex a = Atanh(value);
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return a / Math.PI * 180;
                case AngleUnits.Gradian:
                    return a / Math.PI * 200;
                case AngleUnits.Radian:
                default:
                    return a;
            }
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelkotangens die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Hyperbelkotangens darstellt.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelkotangens <paramref name="value"/> ist.</returns>
        public static Complex Acoth(Complex value, AngleUnits angleUnit)
        {
            Complex a = Acoth(value);
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return a / Math.PI * 180;
                case AngleUnits.Gradian:
                    return a / Math.PI * 200;
                case AngleUnits.Radian:
                default:
                    return a;
            }
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelsekans die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Hyperbelsekans darstellt.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelsekans <paramref name="value"/> ist.</returns>
        public static Complex Asech(Complex value, AngleUnits angleUnit)
        {
            Complex a = Asech(value);
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return a / Math.PI * 180;
                case AngleUnits.Gradian:
                    return a / Math.PI * 200;
                case AngleUnits.Radian:
                default:
                    return a;
            }
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelkosekans die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Hyperbelkosekans darstellt.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelkosekans <paramref name="value"/> ist.</returns>
        public static Complex Acsch(Complex value, AngleUnits angleUnit)
        {
            Complex a = Acsch(value);
            switch (angleUnit)
            {
                case AngleUnits.Degree:
                    return a / Math.PI * 180;
                case AngleUnits.Gradian:
                    return a / Math.PI * 200;
                case AngleUnits.Radian:
                default:
                    return a;
            }
        }

        #endregion

        #region Funktionen 3. Grades

        /// <summary>
        /// Berechnet die Potenz zweier Complex-Werte.
        /// </summary>
        /// <param name="value">Die Basis.</param>
        /// <param name="power">Der Exponent.</param>
        /// <returns>Die Potenz aus <paramref name="value"/> und <paramref name="power"/>.</returns>
        public static Complex Pow(Complex value, Complex power)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;

            if (power == _Zero)
                return _One;
            if (value == _Zero)
                return _Zero;

            double d = Abs(value);
            double val1 = Math.Atan2(value._Imaginary, value._Real);
            double val2 = (power._Real * val1) + (power._Imaginary * Math.Log(d));
            double val3 = Math.Pow(d, power._Real) * Math.Pow(Math.E, -power._Imaginary * val1);
            return new Complex(val3 * Math.Cos(val2), val3 * Math.Sin(val2));
        }

        /// <summary>
        /// Berechnet die Quadratwurzel eines Wertes.
        /// </summary>
        /// <param name="value">Der Wert, dessen Quadratwurzel gezogen werden soll.</param>
        /// <returns>Ein Wert, dessen Quadrat <paramref name="value"/> entspricht.</returns>
        public static Complex Sqrt(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return FromPolarCoordinates(Math.Sqrt(value.Magnitude), value.Phase / 2.0);
        }

        /// <summary>
        /// Berechnet die n. Wurzel eines Wertes.
        /// </summary>
        /// <param name="value">Der Radizent.</param>
        /// <param name="exponent">Der Wurzelexponent.</param>
        /// <returns>Die Wurzel aus <paramref name="exponent"/> und <paramref name="value"/>.</returns>
        public static Complex Root(Complex value, Complex exponent)
        {
            return Pow(exponent, 1 / value);
        }

        /// <summary>
        /// Berechnet den natürlichen Logarithmus eines Wertes.
        /// </summary>
        /// <param name="value">Der Wert, dessen Logarithmus ermittelt werden soll.</param>
        /// <returns>Ein Wert, welcher der natürliche Logarithmus von <paramref name="value"/> ist.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ln")]
        public static Complex Ln(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN)
                return NaN;
            return new Complex(Math.Log(Abs(value)), Math.Atan2(value._Imaginary, value._Real));
        }

        /// <summary>
        /// Berechnet den Logarithmus eines Wertes zu einer bestimmten Basis.
        /// </summary>
        /// <param name="value">Der Wert, dessen Logarithmus ermittelt werden soll.</param>
        /// <param name="baseValue">Die Basis des Logarithmus.</param>
        /// <returns>Ein Wert, welcher der Logarithmus von <paramref name="value"/> zur Basis <paramref name="baseValue"/> ist.</returns>
        public static Complex Log(Complex value, double baseValue)
        {
            return Ln(value) / Ln(baseValue);
        }

        /// <summary>
        /// Berechnet den Logarithmus eines Wertes zur Basis 10.
        /// </summary>
        /// <param name="value">Der Wert, dessen Logarithmus ermittelt werden soll.</param>
        /// <returns>Ein Wert, welcher der Logarithmus von <paramref name="value"/> zur Basis 10 ist.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Lg")]
        public static Complex Lg(Complex value)
        {
            return Scale(Ln(value), 0.43429448190325);
        }

        #endregion

        #region Operator-Funktionen

        /// <summary>
        /// Addiert 2 Instanzen der Complex-Struktur.
        /// </summary>
        /// <param name="n1">Der 1. Summand.</param>
        /// <param name="n2">Der 2. Summand.</param>
        /// <returns>Die Summe aus <paramref name="n1"/> und <paramref name="n2"/>.</returns>
        public static Complex Add(Complex n1, Complex n2)
        {
            return n1 + n2;
        }

        /// <summary>
        /// Subtrahiert 2 Werte voneinander.
        /// </summary>
        /// <param name="n1">Der Minuend.</param>
        /// <param name="n2">Der Subtrahend.</param>
        /// <returns>Die Differenz aus <paramref name="n1"/> und <paramref name="n2"/>.</returns>
        public static Complex Subtract(Complex n1, Complex n2)
        {
            return n1 - n2;
        }

        /// <summary>
        /// Multipliziert 2 Werte miteinander.
        /// </summary>
        /// <param name="n1">Der erste Faktor.</param>
        /// <param name="n2">Der zweite Faktor.</param>
        /// <returns>Das Produkt aus <paramref name="n1"/> und <paramref name="n2"/>.</returns>
        public static Complex Multiply(Complex n1, Complex n2)
        {
            return n1 * n2;
        }

        /// <summary>
        /// Dividiert 2 Werte durcheinander.
        /// </summary>
        /// <param name="n1">Der Dividend.</param>
        /// <param name="n2">Der Divisor.</param>
        /// <returns>Der Quotient von <paramref name="n1"/> und <paramref name="n2"/>.</returns>
        public static Complex Divide(Complex n1, Complex n2)
        {
            return n1 / n2;
        }

        /// <summary>
        /// Negiert einen Wert.
        /// </summary>
        /// <param name="number">Der zu negierende Wert.</param>
        /// <returns></returns>
        public static Complex Negate(Complex number)
        {
            return -number;
        }

        #endregion

        #region Operatoren

        /// <summary>
        /// Addiert 2 Instanzen der Complex-Struktur.
        /// </summary>
        /// <param name="n1">Der 1. Summand.</param>
        /// <param name="n2">Der 2. Summand.</param>
        /// <returns>Die Summe aus <paramref name="n1"/> und <paramref name="n2"/>.</returns>
        public static Complex operator +(Complex n1, Complex n2)
        {
            if (n1.SpecialValue == SpecialValues.NaN||n2.SpecialValue == SpecialValues.NaN)
                return NaN;
            return new Complex(n1._Real + n2._Real, n1._Imaginary + n2._Imaginary);
        }

        /// <summary>
        /// Subtrahiert 2 Werte voneinander.
        /// </summary>
        /// <param name="n1">Der Minuend.</param>
        /// <param name="n2">Der Subtrahend.</param>
        /// <returns>Die Differenz aus <paramref name="n1"/> und <paramref name="n2"/>.</returns>
        public static Complex operator -(Complex n1, Complex n2)
        {
            if (n1.SpecialValue == SpecialValues.NaN || n2.SpecialValue == SpecialValues.NaN)
                return NaN;
            return new Complex(n1._Real - n2._Real, n1._Imaginary - n2._Imaginary);
        }

        /// <summary>
        /// Dividiert 2 Werte durcheinander.
        /// </summary>
        /// <param name="n1">Der Dividend.</param>
        /// <param name="n2">Der Divisor.</param>
        /// <returns>Der Quotient von <paramref name="n1"/> und <paramref name="n2"/>.</returns>
        public static Complex operator /(Complex n1, Complex n2)
        {
            if (n1.SpecialValue == SpecialValues.NaN || n2.SpecialValue == SpecialValues.NaN)
                return NaN;
            if (n2.Real == _Zero)
                return NaN;
            double tmp = 0;
            if (Math.Abs(n2._Imaginary) < Math.Abs(n2._Real))
            {
                tmp = n2._Imaginary / n2._Real;
                return new Complex((n1._Real + (n1._Imaginary * tmp)) / (n2._Real + (n2._Imaginary * tmp)), (n1._Imaginary - (n1._Real * tmp)) / (n2._Real + (n2._Imaginary * tmp)));
            }
            tmp = n2._Real / n2._Imaginary;
            return new Complex((n1._Imaginary + (n1._Real * tmp)) / (n2._Imaginary + (n2._Real * tmp)), (-n1._Real + (n1._Imaginary * tmp)) / (n2._Imaginary + (n2._Real * tmp)));
        }

        /// <summary>
        /// Multipliziert 2 Werte miteinander.
        /// </summary>
        /// <param name="n1">Der erste Faktor.</param>
        /// <param name="n2">Der zweite Faktor.</param>
        /// <returns>Das Produkt aus <paramref name="n1"/> und <paramref name="n2"/>.</returns>
        public static Complex operator *(Complex n1, Complex n2)
        {
            if (n1.SpecialValue == SpecialValues.NaN || n2.SpecialValue == SpecialValues.NaN)
                return NaN;
            double real = (n1._Real * n2._Real) - (n1._Imaginary * n2._Imaginary);
            return new Complex(real, (n1._Imaginary * n2._Real) + (n1._Real * n2._Imaginary));
        }

        /// <summary>
        /// Negiert einen Wert.
        /// </summary>
        /// <param name="value">Der zu negierende Wert.</param>
        /// <returns></returns>
        public static Complex operator -(Complex value)
        {
            if (value.SpecialValue == SpecialValues.NaN )
                return NaN;
            return new Complex(-value._Real, -value._Imaginary);
        }

        /// <summary>
        /// Vergleicht 2 Werte miteinander auf Gleichheit.
        /// </summary>
        /// <param name="n1">Der erste Wert.</param>
        /// <param name="n2">Der zweite Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="n1"/> und <paramref name="n2"/> die gleichen Werte darstellen. Andernfalls <c>False</c>.</returns>
        public static bool operator ==(Complex n1, Complex n2)
        {
            if (n1.SpecialValue == SpecialValues.NaN || n2.SpecialValue == SpecialValues.NaN)
                return false;
            return ((n1._Real == n2._Real) && (n1._Imaginary == n2._Imaginary));
        }

        /// <summary>
        /// Vergleicht 2 Werte miteinander auf Ungleichheit.
        /// </summary>
        /// <param name="n1">Der erste Wert.</param>
        /// <param name="n2">Der zweite Wert.</param>
        /// <returns><c>False</c>, wenn <paramref name="n1"/> und <paramref name="n2"/> die gleichen Werte darstellen. Andernfalls <c>True</c>.</returns>
        public static bool operator !=(Complex n1, Complex n2)
        {
            if (n1.SpecialValue == SpecialValues.NaN || n2.SpecialValue == SpecialValues.NaN)
                return true;
            if (n1._Real == n2._Real)
            {
                return !(n1._Imaginary == n2._Imaginary);
            }
            return true;
        }

        /// <summary>
        /// Überprüft, ob eine komplexe Zahl kleiner ist als eine andere.
        /// </summary>
        /// <param name="c1">Die eventuell kleinere, komplexe Zahl. (Linker Operant)</param>
        /// <param name="c2">Die eventuell größere, komplexe Zahl. (Rechter Operant)</param>
        /// <returns><c>True</c>, wenn <paramref name="c1"/> kleiner ist als <paramref name="c2"/>. Andernfalls <c>False</c>.</returns>
        public static bool operator <(Complex c1, Complex c2)
        {
            if (c1.SpecialValue == SpecialValues.NaN || c2.SpecialValue == SpecialValues.NaN)
                return false;
            return Complex.Abs(c1) < Complex.Abs(c2);
        }

        /// <summary>
        /// Überprüft, ob eine komplexe Zahl größer ist als eine andere.
        /// </summary>
        /// <param name="c1">Die eventuell größere, komplexe Zahl. (Linker Operant)</param>
        /// <param name="c2">Die eventuell kleinere, komplexe Zahl. (Rechter Operant)</param>
        /// <returns><c>True</c>, wenn <paramref name="c1"/> größer ist als <paramref name="c2"/>. Andernfalls <c>False</c>.</returns>
        public static bool operator >(Complex c1, Complex c2)
        {
            if (c1.SpecialValue == SpecialValues.NaN || c2.SpecialValue == SpecialValues.NaN)
                return false;
            return Complex.Abs(c1) > Complex.Abs(c2);
        }

        /// <summary>
        /// Überprüft, ob eine komplexe Zahl kleiner oder gleich ist, wie eine andere.
        /// </summary>
        /// <param name="c1">Die eventuell kleinere, komplexe Zahl. (Linker Operant)</param>
        /// <param name="c2">Die eventuell größere, komplexe Zahl. (Rechter Operant)</param>
        /// <returns><c>True</c>, wenn <paramref name="c1"/> kleiner oder gleich ist wie <paramref name="c2"/>. Andernfalls <c>False</c>.</returns>
        public static bool operator <=(Complex c1, Complex c2)
        {
            return c1 == c2 || c1 < c2;
        }

        /// <summary>
        /// Überprüft, ob eine komplexe Zahl größer oder gleich ist, wie eine andere.
        /// </summary>
        /// <param name="c1">Die eventuell größere, komplexe Zahl. (Linker Operant)</param>
        /// <param name="c2">Die eventuell kleinere, komplexe Zahl. (Rechter Operant)</param>
        /// <returns><c>True</c>, wenn <paramref name="c1"/> größer oder gleich ist wie <paramref name="c2"/>. Andernfalls <c>False</c>.</returns>
        public static bool operator >=(Complex c1, Complex c2)
        {
            return c1 == c2 || c1 > c2;
        }

        #endregion

        #endregion

        #region Statische Felder

        /// <summary>
        /// Ruft ein Objekt ab, der den reellen Wert 1 hat.
        /// </summary>
        public static Complex One
        {
            get
            {
                return _One;
            }
        }
        private static readonly Complex _One = new Complex(1D, 0D);

        /// <summary>
        /// Ruft ein Objekt ab, der den Wert 0 hat.
        /// </summary>
        public static Complex Zero
        {
            get
            {
                return _Zero;
            }
        }
        private static readonly Complex _Zero = new Complex(0D, 0D);

        /// <summary>
        /// Ruft ein Objekt ab, der den imaginären Wert 1 hat.
        /// </summary>
        public static Complex ImaginaryOne
        {
            get
            {
                return _ImaginaryOne;
            }
        }
        private static readonly Complex _ImaginaryOne = new Complex(0D, 1D);

        /// <summary>
        /// Ruft einen konstanten Wert ab, der keine Zahl ist.
        /// </summary>
        public static Complex NaN
        {
            get
            {
                return _NaN;
            }
        }
        private static readonly Complex _NaN = new Complex(0D, 0D) { _SpecialValue = SpecialValues.NaN, };

        #endregion

        #region casts

        /// <summary>
        /// Erstellt einen neuen Complex-Wert aus dem angegebenen Wert.
        /// </summary>
        /// <param name="value">Der Wert, aus dem ein Complex-Wert gebildet werden soll.</param>
        /// <returns>Eine neue Complex-Instanz, mit dem selben Wert wie <paramref name="value"/>.</returns>
        public static explicit operator Complex(decimal value)
        {
            return new Complex((double)value, 0D);
        }

        /// <summary>
        /// Erstellt einen neuen Complex-Wert aus dem angegebenen Wert.
        /// </summary>
        /// <param name="value">Der Wert, aus dem ein Complex-Wert gebildet werden soll.</param>
        /// <returns>Eine neue Complex-Instanz, mit dem selben Wert wie <paramref name="value"/>.</returns>
        public static implicit operator Complex(int value)
        {
            return new Complex(value, 0D);
        }

        /// <summary>
        /// Erstellt einen neuen Complex-Wert aus dem angegebenen Wert.
        /// </summary>
        /// <param name="value">Der Wert, aus dem ein Complex-Wert gebildet werden soll.</param>
        /// <returns>Eine neue Complex-Instanz, mit dem selben Wert wie <paramref name="value"/>.</returns>
        public static implicit operator Complex(double value)
        {
            return new Complex(value, 0D) { _SpecialValue = double.IsNaN(value) ? SpecialValues.NaN : SpecialValues.None, };
        }

        #endregion

        #region override

        /// <summary>
        /// Gibt die komplexe Zahl als Zeichenfolge zurück.
        /// </summary>
        public override string ToString()
        {
            if (this.Imaginary == 0)
                return this.Real.ToString(CultureInfo.InvariantCulture);
            else if (this.Real == 0)
                return string.Format(CultureInfo.InvariantCulture, "{0}i", this.Imaginary);
            else
                return string.Format(CultureInfo.InvariantCulture, "{0}{1}i", this.Real, this.Imaginary < 0 ? this.Imaginary.ToString(CultureInfo.InvariantCulture) : "+" + this.Imaginary.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Ermittelt den Hashcode aus dem reellen und imaginären Anteil der Zahl.
        /// </summary>
        public override int GetHashCode()
        {
            return this._Real.GetHashCode() ^ this._Imaginary.GetHashCode();
        }

        /// <summary>
        /// Vergleicht ein Objekt mit dieser Instanz auf Gleichheit.
        /// </summary>
        /// <param name="obj">Ein Objekt, das verglichen werden soll.</param>
        /// <returns><c>True</c>, wenn <paramref name="obj"/> vom Typ Complex ist und den gleichen Wert repräsentiert. Andernfalls <c>False</c>.</returns>
        public override bool Equals(object obj)
        {
            return ((obj is Complex) && (this == ((Complex)obj)));
        }

        //#region BigNumber

        ///// <summary>
        ///// Gibt diese Instanz zurück.
        ///// </summary>
        //internal override IBigNumber GetNumber()
        //{
        //    return this;
        //}

        //#endregion

        #endregion

        /// <summary>
        /// Konvertiert eine Zeichenfolge in eine Instanz der Klasse Number unter der angegebenen Kultur.
        /// </summary>
        /// <param name="num">Die zu konvertierende Zeichenfolge.</param>
        /// <param name="provider">Ein Objekt, das kulturspezifische Formatierungsinformationen über <c>num</c> bereitstellt. </param>
        /// <returns>Eine Instanz der Klasse Number, welche den Wert der übergebenen Zeichenfolge aufweist.</returns>
        public static Complex Parse(string num, IFormatProvider provider)
        {
            if (string.IsNullOrEmpty(num))
                throw new ArgumentNullException("num", ResourceManager.GetMessage("ArgNull_ParamCantNull", "num"));

            string[] s = num.Split(new char[] { '-', '+' }, StringSplitOptions.RemoveEmptyEntries);
            if (s.Length == 1)
            {
                if (s[0].Contains("i"))
                {
                    if (num.Contains("°") || num.Contains("gon") || num.Contains("rad"))
                        throw new ArgumentException(ResourceManager.GetMessage("Arg_CompleyOnlyAllowRad"), "num");
                    return new Complex(0, num.StartsWith("-", StringComparison.OrdinalIgnoreCase) ? -double.Parse(s[0].TrimEnd('i'), provider) : double.Parse(s[0].TrimEnd('i'), provider));
                }
                else
                {
                    AngleUnits unit = AngleUnits.Radian;
                    if (num.Contains("°"))
                        unit = AngleUnits.Degree;
                    if (num.Contains("gon"))
                        unit = AngleUnits.Gradian;
                    s[0] = s[0].Replace("°", "").Replace("gon", "").Replace("rad", "");
                    return new Complex(num.StartsWith("-", StringComparison.OrdinalIgnoreCase) ? -double.Parse(s[0].TrimEnd('i'), provider) : double.Parse(s[0].TrimEnd('i'), provider), 0, unit);
                }
            }
            else
            {
                if (num.Contains("°") || num.Contains("gon") || num.Contains("rad"))
                    throw new ArgumentException(ResourceManager.GetMessage("Arg_CompleyOnlyAllowRad"), "num");

                double a = 0, b = 0;
                a = double.Parse(s[0].TrimEnd('i'), provider);
                b = double.Parse(s[1].TrimEnd('i'), provider);
                if (num.StartsWith("-", StringComparison.OrdinalIgnoreCase))
                    a *= -1;
                if (num.TrimStart('-').Contains("-"))
                    b *= -1;

                if (s[0].Contains("i"))
                    return new Complex(b, a);
                else
                    return new Complex(a, b);
            }
        }

        /// <summary>
        /// Konvertiert eine Zeichenfolge in eine Instanz der Klasse Number unter der aktuellen Kultur.
        /// </summary>
        /// <param name="num">Die zu konvertierende Zeichenfolge.</param>
        /// <returns>Eine Instanz der Klasse Number, welche den Wert der übergebenen Zeichenfolge aufweist.</returns>
        public static Complex Parse(string num)
        {
            return Parse(num, NumberFormatInfo.CurrentInfo);
        }

        #region interfaces

        #region IFormattable Member

        /// <summary>
        /// Gibt die Zahl, unter beachtung der angaben, als Zeichenfolge zurück.
        /// </summary>
        /// <param name="format">Eine numerische Formatierungszeichenfolge.</param>
        /// <param name="formatProvider">Ein Objekt, das kulturspezifische Formatierungsinformationen bereitstellt. </param>
        /// <returns>Eine Zeichenfolge, welche dem Wert dieser Instanz entspricht.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (this.Imaginary == 0)
                return this.Real.ToString(formatProvider);
            else if (this.Real == 0)
                return string.Format(formatProvider, "{0}i", this.Imaginary.ToString(format, formatProvider));
            else
                return string.Format(formatProvider, "{0}{1}i", this.Real.ToString(format, formatProvider), this.Imaginary < 0 ? this.Imaginary.ToString(format, formatProvider) : "+" + this.Imaginary.ToString(format, formatProvider));
        }

        #endregion

        #region IEquatable<Number> Member

        /// <summary>
        /// Vergleicht ein anderes Objekt mit dieser Instanz.
        /// </summary>
        /// <param name="other">Das Vergleichsobjekt.</param>
        /// <returns><c>True</c>, wenn <paramref name="other"/> vom Typ <see cref="Complex"/> ist und den selben Wert enthältst. Andernfalls <c>False</c>.</returns>
        public bool Equals(Complex other)
        {
            return other == this;
        }

        #endregion

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

        #region IComparable<BigComplex> Member

        /// <summary>
        /// Vergleicht ein Objekt mit dieser Instanz auf Gleichheit.
        /// </summary>
        /// <param name="other">Die zu vergleichende Instanz.</param>
        /// <returns>1 wenn <paramref name="other"/> einen niedrigereren Wert hat als diese Instanz. 0 wenn der 
        /// Wert der Instanzen gleich ist. -1 wenn <paramref name="other"/> einen höheren Wert hat als diese Instanz.</returns>
        public int CompareTo(Complex other)
        {
            if (other == this)
                return 0;
            if (other < this)
                return 1;
            else
                return -1;
        }

        #endregion

        #region IMathComparison<Complex> Member

        /// <summary>
        /// Überprüft, ob ein Wert größer ist, als der Wert dieser Instanz.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> größer ist als der Wert dieser Instanz. Andernfalls <c>False</c>.</returns>
        public bool IsGreater(Complex param)
        {
            return param > this;
        }
        /// <summary>
        /// Überprüft, ob ein Wert größer oder gleich dem Wert dieser Instanz ist.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> größer oder gleich dem Wert dieser Instanz ist. Andernfalls <c>False</c>.</returns>
        public bool IsGreaterEqual(Complex param)
        {
            return param >= this;
        }
        /// <summary>
        /// Überprüft, ob ein Wert größer ist, als der Wert dieser Instanz.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> kleiner ist als der Wert dieser Instanz. Andernfalls <c>False</c>.</returns>
        public bool IsSmaller(Complex param)
        {
            return param < this;
        }
        /// <summary>
        /// Überprüft, ob ein Wert kleiner oder gleich dem Wert dieser Instanz ist.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> kleiner oder gleich dem Wert dieser Instanz ist. Andernfalls <c>False</c>.</returns>
        public bool IsSmallerEqual(Complex param)
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
        public bool IsEqual(Complex param)
        {
            return param == this;
        }
        /// <summary>
        /// Überprüft, ob ein Wert ungleich dem Wert dieser Instanz ist.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> und der Wert dieser Instanz ungleich sind. Andernfalls <c>False</c>.</returns>
        public bool IsNotEqual(Complex param)
        {
            return param != this;
        }

        /// <summary>
        /// Ruft einen Wert ab, der angibt ob der Wert in einem bestimmten Definitionsbereich liegt.
        /// </summary>
        /// <param name="domainSet">Die zu prüfende Definitionsmenge.</param>
        /// <returns><c>True</c>, wenn diese Instanz in der angegebenen Definitionsmenge liegt. ANdernfalls <c>False</c>.</returns>
        public bool IsInDomain(DomainSet domainSet)
        {
            switch (domainSet.Set)
            {
                case DomainSets.Complex:
                    return true;
                default:
                    return false;
            }
        }

        #endregion

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
        /// Ruft einen Wert ab, der angibt ob der abgespeicherte Wert speziell ist.
        /// </summary>
        public SpecialValues SpecialValue
        {
            get
            {
                return this._SpecialValue;
            }
        }

        #endregion
    }
}