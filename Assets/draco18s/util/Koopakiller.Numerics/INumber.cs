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
    /// Wird von "Zahlen-Wert"-Typen implementiert.
    /// </summary>
    public interface INumber
    {
        /// <summary>
        /// Ruft den Wert der Zahl ab.
        /// </summary>
        INumber Result { get; }

        /// <summary>
        /// Ruft einen Wert ab, der angibt welchen Spezialwert der "Zahlen-Wert"-Typ aufweist.
        /// </summary>
        SpecialValues SpecialValue { get; }
    }
}
