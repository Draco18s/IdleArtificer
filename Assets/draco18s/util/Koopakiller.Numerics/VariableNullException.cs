//! Created by Tom Lambert alias Koopakiller
//! Project started: 2011
//! License: Microsoft Reciprocal License (Ms-RL)
//! Project site: https://numerics.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Koopakiller.Numerics
{
    /// <summary>
    /// Wird verwendet, wenn eine Variable keinen Wert hat.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
    public class VariableNullException : Exception
    {
        /// <summary>
        /// Ruft den Namen der Variablen ab.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="Koopakiller.Numerics.VariableNullException"/>-Klasse.
        /// </summary>
        /// <param name="name">Der Name der Variablen.</param>
        /// <param name="message">Eine zusätzliche Meldung zum Fehler.</param>
        /// <param name="innerException">Eine innere Ausnahme, die zu diesem Fehler führte.</param>
        public VariableNullException(string name, string message, Exception innerException)
            : base(message, innerException)
        {
            this.Name = name;
        }
        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="Koopakiller.Numerics.VariableNullException"/>-Klasse.
        /// </summary>
        /// <param name="name">Der Name der Variablen.</param>
        /// <param name="message">Eine zusätzliche Meldung zum Fehler.</param>
        public VariableNullException(string name, string message) : this(name, message, null) { }
        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="Koopakiller.Numerics.VariableNullException"/>-Klasse.
        /// </summary>
        /// <param name="name">Der Name der Variablen.</param>
        public VariableNullException(string name) : this(name, "", null) { }

        /// <summary>
        /// Ruft einen String zum visualisieren der Ausnahme ab.
        /// </summary>
        public override string ToString()
        {
            return "Variable: " + Name + "\n" + base.ToString();
        }
    }
}
