//! Created by Tom Lambert alias Koopakiller
//! Project started: 2011
//! License: Microsoft Reciprocal License (Ms-RL)
//! Project site: https://numerics.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Koopakiller.Numerics.Resources;

namespace Koopakiller.Numerics
{
    /// <summary>
    /// Stellt einen Ausnahmetyp bereit, der verwendet wird um einen Fehler in einer mathematischen Funktion zu melden.
    /// </summary>
    public class FunctionException : Exception
    {
        /// <summary>
        /// Erstellt eine neue Instanz der FunctionException-Klasse.
        /// </summary>
        [Obsolete("Bad exception handling.")]
        public FunctionException() { }

        /// <summary>
        /// Erstellt eine neue Instanz der FunctionException-Klasse.
        /// </summary>
        /// <param name="message">Der anzugebende Meldung.</param>
        public FunctionException(string message) : base(message) { }

        /// <summary>
        /// Erstellt eine neue Instanz der FunctionException-Klasse.
        /// </summary>
        /// <param name="message">Der anzugebende Meldung.</param>
        /// <param name="innerException">Die Ausnahme, aufgrund der diese Ausnahme ausgelöst wurde.</param>
        public FunctionException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Erstellt eine neue Instanz der FunctionException-Klasse.
        /// </summary>
        /// <param name="fx">Der Name der Funktion.</param>
        /// <param name="message">Ein zusätzlicher Meldungstext der den Fehler näher beschreibt.</param>
        public FunctionException(string fx, string message)
            : base(message)
        {
            this.Function = fx;
        }

        /// <summary>
        /// Erstellt eine neue Instanz der FunctionException-Klasse.
        /// </summary>
        /// <param name="fx">Der Name der Funktion.</param>
        /// <param name="message">Ein zusätzlicher Meldungstext der den Fehler näher beschreibt.</param>
        /// <param name="innerException">Ein Fehlerobjekt, welches zu dem Fehler in der Funktion führte.</param>
        public FunctionException(string fx, string message, Exception innerException)
            : base(message, innerException)
        {
            this.Function = fx;
        }

        /// <summary>
        /// Ruft den Namen der Funktion ab.
        /// </summary>
        public string Function { get; private set; }
    }
}
