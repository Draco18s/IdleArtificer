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
    /// Bildet die Basis für eine Konstanten Klasse mit einer bestimmten Genauigkeit.
    /// </summary>
    /// <typeparam name="T">Der Typ von dem die einzelnen Konstanten sind.</typeparam>
   public interface IConstant<T>
    {
        /// <summary>
        /// Ruft die Kreiszahl Pi in der jeweiligen Genauigkeit ab.
        /// </summary>
        T PI { get; }
        /// <summary>
        /// Ruft die Basis des natürlichen Logarithmus e ab in der jeweiligen Genauigkeit ab.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "E")]
        T E { get; }
    }
}
