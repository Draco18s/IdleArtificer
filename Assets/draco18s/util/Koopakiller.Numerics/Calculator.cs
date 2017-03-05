//! Created by Tom Lambert alias Koopakiller
//! Project started: 2011
//! License: Microsoft Reciprocal License (Ms-RL)
//! Project site: https://numerics.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Koopakiller.Numerics.Resources;

//! implementing to feb 2014

namespace Koopakiller.Numerics
{
/// <summary>
/// Stellt verschiedene Operationen für vordefinierte Zahlentypen und Parsing-Operationen bereit. <para/>
/// Diese Klasse ist noch nicht fertig gestellt.
/// </summary>
    public class Calculator
    {
        public string Calculate(string calc)
        {
            if (string.IsNullOrEmpty(calc))
                throw new ArgumentNullException(ResourceManager.GetMessage("ArgNull_ParamEmpty", "calc"));
            calc = calc.ToLower(); // Groß-/Kleinschreibung ignorieren
            calc = calc.SimplifyChars(' '); // Doppelte Leerzeichen entfernen
            calc = calc.Replace(';', '|'); // Semikolons gegen Senkrechte Striche austauschen
            CalculateInternal(ref calc, 0, calc.Length - 1);
            return calc;
        }

        private string CalculateInternal(ref string c, int start, int end)
        {
            bool var = false;//Nachfolgendes Zeichen ist eine Variable
            //bool setVar = false;//Nachfolgender Wert ist an die letzte Variable zu übergeben.
            //char lastVarName = '\0';//Der Name der letzten Variable, darf nur zum setzen dieser verwendet werden.
            //bool? currentVarParts = false;//True, wenn aktueller Wert einen reellen-, False wenn aktueller Wert einen imaginären Teil enthält. null = Noch kein Teil analysiert.
            INumber var1 = null, var2 = null;//Enthält die Werte der aktuellen 2 Variablen
            for (int i = start; i < end; ++i)
            {
                switch (c[i])
                {
                    case '°':
                        var = true;
                        break;
                    default:
                        if (var)
                        {
                            //Variable

                            //Zuweisung?
                            if ((i + 2 < end && c[i + 1] == '=')// °x=
                                || (i + 3 < end && c[i + 1] == ' ' && c[i + 2] == '='))// °x =
                            {
                                //setVar = true;
                                //lastVarName = c[i];

                                ++i;// = bzw. Leerzeichen überspringen
                                if (c[i] == ' ') ++i;// = überspringen wenn Leerzeichen enthalten.
                            }
                            else
                            {
                                //Auswertung, der bisher leeren Variable zuweisen
                                if (var1 == null)
                                    var1 = GetVar(c[i]);
                                else if (var2 == null)
                                    var2 = GetVar(c[i]);
                            }
                            var = false;//Nächstes Zeichen ist kein Variablenname mehr
                        }
                        else
                        {
                            if ((c[i] > '0' && c[i] < '9') || c[i] == '-' || c[i] == '+' || c[i] == 'i')//Wert
                            {
                                //Eigentlicher Wert, eventuell Komplex
                            }
                            else if (c[i] == '(' || c[i] == ')')
                            {
                                //Vektor, Funktionsparameter oder geklammerter Ausdruck
                            }
                            else if (c[i] == '[' || c[i] == ']')
                            {
                                //Intervall oder Matrix
                            }
                            else if (c[i] == '{' || c[i] == '}')
                            {
                                //Menge
                            }
                            //Funktion
                        }
                        break;
                }
            }

            return "";
        }

        /// <summary>
        /// Setzt den Wert einer Variable.
        /// </summary>
        /// <param name="c">Der Name der Variable.</param>
        /// <param name="value">Der Wert der Variable.</param>
        /// <remarks>Wenn <paramref name="value"/> null ist, dann wird der Wert, sofern vorhanden, gelöscht.</remarks>
        void SetVar(char c, INumber value)
        {
            if (this.Vars.ContainsKey(c))
                if (value == null)
                    this.Vars.Remove(c);
                else
                    this.Vars[c] = value;
            else
                this.Vars.Add(c, value);
        }

        INumber GetVar(char c)
        {
            try
            {
                return this.Vars[c];
            }
            catch (Exception ex)
            {
                throw new VariableNullException(c.ToString(), ResourceManager.GetMessage("VarNull_DontExists", c.ToString()), ex);
            }
        }

        private Dictionary<char, INumber> Vars = new Dictionary<char, INumber>();//Liste von Variablen neu initialisieren;

        #region Trigonometrie

        /// <summary>
        /// Ruft die Größe des Vollkreises in der angegebenen Einheit ab.
        /// </summary>
        /// <param name="unit">Die Einheit in der der Vollkreis gemessen werden soll.</param>
        /// <returns>Die Größe des Vollkreises in der Einheit <paramref name="unit"/>.</returns>
        public static double Circle(AngleUnits unit)
        {
            switch (unit)
            {
                case AngleUnits.Degree:
                    return 360;
                case AngleUnits.Radian:
                    return Math.PI;
                default:
                    return 400;
            }
        }

        /// <summary>
        /// Gibt den Sinus des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Sinus von <paramref name="value"/>.</returns>
        public static double Sin(double value)
        {
            return Math.Sin(value);
        }
        /// <summary>
        /// Gibt den Kosinus des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Kosinus von <paramref name="value"/>. </returns>
        public static double Cos(double value)
        {
            return Math.Cos(value);
        }
        /// <summary>
        /// Gibt den Tangens des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Tangens von <paramref name="value"/>. </returns>
        public static double Tan(double value)
        {
            return Sin(value) / Cos(value);
        }
        /// <summary>
        /// Gibt den Kotangens des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Kotangens von <paramref name="value"/>. </returns>
        public static double Cot(double value)
        {
            return 1 / Tan(value);
        }
        /// <summary>
        /// Gibt den Sekans des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Sekans von <paramref name="value"/>. </returns>
        public static double Sec(double value)
        {
            return 1 / Cos(value);
        }
        /// <summary>
        /// Gibt den Kosekans des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Kosekans von <paramref name="value"/>. </returns>
        public static double Csc(double value)
        {
            return 1 / Sin(value);
        }

        /// <summary>
        /// Gibt den Hyperbelsinus des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Hyperbelsinus von <paramref name="value"/>.</returns>
        public static double Sinh(double value)
        {
            return Math.Sinh(value);
        }
        /// <summary>
        /// Gibt den Hyperbelkosinus des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Hyperbelkosinus von <paramref name="value"/>.</returns>
        public static double Cosh(double value)
        {
            return Math.Cosh(value);
        }
        /// <summary>
        /// Gibt den Hyperbeltangens des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Hyperbeltangens von <paramref name="value"/>.</returns>
        public static double Tanh(double value)
        {
            return Sinh(value) / Cosh(value);
        }
        /// <summary>
        /// Gibt den Hyperbelkotangens des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Hyperbelkosinus von <paramref name="value"/>.</returns>
        public static double Coth(double value)
        {
            return Cosh(value) / Sinh(value);
        }
        /// <summary>
        /// Gibt den Hyperbelkosekans des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Hyperbelkosekans von <paramref name="value"/>.</returns>
        public static double Sech(double value)
        {
            return 1 / Cosh(value);
        }
        /// <summary>
        /// Gibt den Hyperbelsekans des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß angegebener Winkel.</param>
        /// <returns>Der Hyperbelsekans von <paramref name="value"/>.</returns>
        public static double Csch(double value)
        {
            return 1 / Sinh(value);
        }

        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelsinus die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Kotangens darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelsinus <paramref name="value"/> ist.</returns>
        public static double Asin(double value)
        {
            return Math.Asin(value);
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelkosinus die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Kotangens darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelkosinus <paramref name="value"/> ist.</returns>
        public static double Acos(double value)
        {
            return Math.Acos(value);
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbeltangens die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Kotangens darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbeltangens <paramref name="value"/> ist.</returns>
        public static double Atan(double value)
        {
            return Math.Atan(value);
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelkotangens die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Kotangens darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelkotangens <paramref name="value"/> ist.</returns>
        public static double Acot(double value)
        {
            return Atan(1 / value);
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelsekans die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Kotangens darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelsekans <paramref name="value"/> ist.</returns>
        public static double Asec(double value)
        {
            return Acos(1 / value);
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelkosekans die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Kotangens darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelkosekans <paramref name="value"/> ist.</returns>
        public static double Acsc(double value)
        {
            return Asin(1 / value);
        }

        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelsinus die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Hyperbelsinus darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelsinus <paramref name="value"/> ist.</returns>
        public static double Asinh(double value)
        {
            return Log(value + Sqrt(value * value + 1));
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelkosinus die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Hyperbelkosinus darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelkosinus <paramref name="value"/> ist.</returns>
        public static double Acosh(double value)
        {
            return Log(value + Sqrt(value * value - 1));
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbeltangens die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Hyperbeltangens darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbeltangens <paramref name="value"/> ist.</returns>
        public static double Atanh(double value)
        {
            return 0.5 * Log((1 + value) / (1 - value));
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelkotangens die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Hyperbelkotangens darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelkotangens <paramref name="value"/> ist.</returns>
        public static double Acoth(double value)
        {
            return 0.5 * Log((value + 1) / (value - 1));
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelsekans die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Hyperbelsekans darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelsekans <paramref name="value"/> ist.</returns>
        public static double Asech(double value)
        {
            return Log((1 + Sqrt(1 - value * value)) / value);
        }
        /// <summary>
        /// Gibt einen Winkel/Wert zurück, dessen Hyperbelkosekans die angegebene Zahl ist.
        /// </summary>
        /// <param name="value">Eine Zahl die einen Hyperbelkosekans darstellt.</param>
        /// <returns>Ein Wert/Winkel dessen Hyperbelkosekans <paramref name="value"/> ist.</returns>
        public static double Acsch(double value)
        {
            return Log((1 + Sqrt(1 + value * value)) / value);
        }

        /// <summary>
        /// Gibt den Sinus des angegebenen Winkels zurück.
        /// </summary>
        /// <param name="value">Ein im Bogenmaß, Gradmaß oder Gon angegebener Winkel/Wert.</param>
        /// <param name="angleUnit">Die Einheit, in der der Winkel angegeben wurde.</param>
        /// <returns>Der Sinus von <paramref name="value"/>. </returns>
        public static double Sin(double value, AngleUnits angleUnit)
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
        public static double Cos(double value, AngleUnits angleUnit)
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
        public static double Tan(double value, AngleUnits angleUnit)
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
        public static double Cot(double value, AngleUnits angleUnit)
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
        public static double Sec(double value, AngleUnits angleUnit)
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
        public static double Csc(double value, AngleUnits angleUnit)
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
        public static double Sinh(double value, AngleUnits angleUnit)
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
        public static double Cosh(double value, AngleUnits angleUnit)
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
        public static double Tanh(double value, AngleUnits angleUnit)
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
        public static double Coth(double value, AngleUnits angleUnit)
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
        public static double Csch(double value, AngleUnits angleUnit)
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
        public static double Sech(double value, AngleUnits angleUnit)
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
        public static double Asin(double value, AngleUnits angleUnit)
        {
            double a = Asin(value);
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
        public static double Acos(double value, AngleUnits angleUnit)
        {
            double a = Acos(value);
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
        public static double Atan(double value, AngleUnits angleUnit)
        {
            double a = Atan(value);
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
        public static double Acot(double value, AngleUnits angleUnit)
        {
            double a = Atan(1 / value);
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
        public static double Asec(double value, AngleUnits angleUnit)
        {
            double a = Acos(1 / value);
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
        public static double Acsc(double value, AngleUnits angleUnit)
        {
            double a = Asin(1 / value);
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
        public static double Asinh(double value, AngleUnits angleUnit)
        {
            double a = Asinh(value);
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
        public static double Acosh(double value, AngleUnits angleUnit)
        {
            double a = Acosh(value);
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
        public static double Atanh(double value, AngleUnits angleUnit)
        {
            double a = Atanh(value);
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
        public static double Acoth(double value, AngleUnits angleUnit)
        {
            double a = Acoth(value);
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
        public static double Asech(double value, AngleUnits angleUnit)
        {
            double a = Asech(value);
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
        public static double Acsch(double value, AngleUnits angleUnit)
        {
            double a = Acsch(value);
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

        /// <summary>
        /// Berechnet die Quadratwurzel von <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Ein Wert dessen Quadratwurzel berechnet werden soll.</param>
        /// <returns>Ein Wert dessen Quadrat <paramref name="value"/> ist.</returns>
        public static double Sqrt(double value)
        {
            return Math.Sqrt(value);
        }
        /// <summary>
        /// Gibt natürlichen Logarithmus (zur Basis e) einer Zahl zurück.
        /// </summary>
        /// <param name="value">Ein Wert dessen natürlicher Logarithmus (zur Basis e) bestimmt werden soll.</param>
        public static double Log(double value)
        {
            return Math.Log(value);
        }

        /// <summary>
        /// Gibt einen Wert zurück der das Vorzeichen einer Zahl angibt
        /// </summary>
        /// <param name="value">Eine Zahl mit Vorzeichen (+ oder -).</param>
        /// <returns>-1 wenn <paramref name="value"/> kleiner als 0 ist, 0 wenn <paramref name="value"/> 0 ist ansonsten 1.</returns>
        public static double Sgn(double value)
        {
            if (value < 0)
                return -1;
            if (value > 0)
                return -1;
            return 0;
        }
    }
}
