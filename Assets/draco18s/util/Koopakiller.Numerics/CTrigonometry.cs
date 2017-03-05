//! Created by Tom Lambert alias Koopakiller
//! Project started: 2011
//! License: Microsoft Reciprocal License (Ms-RL)
//! Project site: https://numerics.codeplex.com/license

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Koopakiller.Numerics
//{

//    /// <summary>
//    /// Repräsentiert eine trigonometrische Aufgabe.
//    /// </summary>
//    public struct CTrigonometry : ICalc
//    {
//        /// <summary>
//        /// Erstellt eine neue Instanz der Calculation1-Klasse.
//        /// </summary>
//        /// <param name="c">Der Parameter der Rechnung.</param>
//        /// <param name="type">Der Typ der Rechnung.</param>
//        /// <param name="angleUnit">Die Einheit, in der die Winkel angegeben werden. Bei komplexen Zahlen wird nur Radiant unterstützt.</param>
//        public CTrigonometry(ICalc c, CaluclationTypes type, AngleUnits angleUnit)
//            : this()
//        {
//            this.Param = c;
//            this.Type = type;
//            this.AngleUnit = angleUnit;
//        }

//        /// <summary>
//        /// Ruft den Parameter der Rechnung ab oder legt diesen fest.
//        /// </summary>
//        public ICalc Param { get; set; }
//        /// <summary>
//        /// Ruft den Typ der Rechnung ab.
//        /// </summary>
//        public CaluclationTypes Type { get; set; }
//        /// <summary>
//        /// Ruft die Einheit der Gradangaben ab oder legt diese fest.
//        /// </summary>
//        public AngleUnits AngleUnit { get; set; }
//    }
//}
