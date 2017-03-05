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
   /// Wird verwendet, wenn ein Typ nicht doppelt vorkommen darf.
   /// </summary>
   public class ArgumentDoubleException:Exception 
    {
        #region .ctor

        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="ArgumentDoubleException"/>-Klasse.
        /// </summary>
        public ArgumentDoubleException() : base() { }
        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="ArgumentDoubleException"/>-Klasse unter Angabe einer benutzerdefinierten Nachricht.
        /// </summary>
        /// <param name="message">Eine benutzerdefinierte Nachricht zum Fehler.</param>
        public ArgumentDoubleException(string message) : base(message) { }
        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="ArgumentDoubleException"/>-Klasse unter Angabe einer benutzerdefinierten Nachricht sowie einer inneren Ausnahme.
        /// </summary>
        /// <param name="message">Eine benutzerdefinierte Nachricht zum Fehler.</param>
        /// <param name="innerException">Eine innere Ausnahme, aufgrund dessen die Argumente falsch sind.</param>
        public ArgumentDoubleException(string message, Exception innerException) : base(message, innerException) { }

        #endregion
    }
}
