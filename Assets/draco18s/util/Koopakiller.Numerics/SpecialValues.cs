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
    /// Stellt die speziellen Werte für "Zahlen-Wert"-Typen bereit.
    /// </summary>
    public enum SpecialValues
    {
        /// <summary>
        /// Es handelt sich um einen normalen Wert.
        /// </summary>
        None = 0,
        /// <summary>
        /// Es ist keine Zahl.
        /// </summary>
        NaN,
        /// <summary>
        /// Es ist positiv Unendlich.
        /// </summary>
        PositiveInfinity,
        /// <summary>
        /// Es ist negativ Unendlich.
        /// </summary>
        NegativeInfinity,
    }
}
