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
    /// Die Ausnahme, die ausgelöst wird, wenn 2 Argumente (oder bestimmte Eigenschaften) unterschiedliche Werte aufweisen aber gleich sein sollten.
    /// </summary>
    public class ArgumentDifferentException : ArgumentException
    {
        #region .ctor

        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="ArgumentDifferentException"/>-Klasse.
        /// </summary>
        public ArgumentDifferentException() : base() { }
        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="ArgumentDifferentException"/>-Klasse unter Angabe einer benutzerdefinierten Nachricht.
        /// </summary>
        /// <param name="message">Eine benutzerdefinierte Nachricht zum Fehler.</param>
        public ArgumentDifferentException(string message) : base(message) { }
        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="ArgumentDifferentException"/>-Klasse unter Angabe einer benutzerdefinierten Nachricht sowie den Namen der Fehlerhaften Argumente.
        /// </summary>
        /// <param name="message">Eine benutzerdefinierte Nachricht zum Fehler.</param>
        /// <param name="parameter1">Der Name des ersten Arguments.</param>
        /// <param name="parameter2">Der Name des zweiten Arguments.</param>
        public ArgumentDifferentException(string message, string parameter1, string parameter2)
            : base(message, parameter1 + " / " + parameter2)
        {
            this.Parameter1 = parameter1;
            this.Parameter2 = parameter2;
        }
        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="ArgumentDifferentException"/>-Klasse unter Angabe einer benutzerdefinierten Nachricht sowie einer inneren Ausnahme.
        /// </summary>
        /// <param name="message">Eine benutzerdefinierte Nachricht zum Fehler.</param>
        /// <param name="innerException">Eine innere Ausnahme, aufgrund dessen die Argumente falsch sind.</param>
        public ArgumentDifferentException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="ArgumentDifferentException"/>-Klasse unter Angabe einer benutzerdefinierten Nachricht sowie den Namen der Fehlerhaften Argumente sowie deren Werte.
        /// </summary>
        /// <param name="message">Eine benutzerdefinierte Nachricht zum Fehler.</param>
        /// <param name="parameter1">Der Name des ersten Arguments.</param>
        /// <param name="parameter2">Der Name des zweiten Arguments.</param>
        /// <param name="value1">Der Wert des ersten Arguments.</param>
        /// <param name="value2">Der Wert des zweiten Arguments.</param>
        public ArgumentDifferentException(string message, string parameter1, string parameter2, object value1, object value2)
            : base(message, parameter1 + " / " + parameter2)
        {
            this.Parameter1 = parameter1;
            this.Parameter2 = parameter2;
            this.Value1 = value1;
            this.Value2 = value2;
        }

        #endregion

        #region Eigenschaften

        /// <summary>
        /// Ruft den Namen des ersten Arguments ab.
        /// </summary>
        public string Parameter1{get;private set;}
        /// <summary>
        /// Ruft den Namen des zweiten Arguments ab.
        /// </summary>
        public string Parameter2 { get; private set; }

        /// <summary>
        /// Ruft den Wert des ersten Arguments ab.
        /// </summary>
        public object Value1 { get; private set; }
        /// <summary>
        /// Ruft den Wert des zweiten Arguments ab.
        /// </summary>
        public object Value2{get;private set;}

        #endregion
    }
}
