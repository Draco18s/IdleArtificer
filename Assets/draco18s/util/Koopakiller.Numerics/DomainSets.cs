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
    /// Stellt die verschiedenen Definitionsbereiche als Enumerationswerte bereit.
    /// </summary>
    public enum DomainSets
    {
        /// <summary>
        /// Der Bereich der Natürlichen Zahlen ohne 0.
        /// </summary>
        NaturalWithoutZero = -4,
        /// <summary>
        /// Der Bereich der Natürlichen.
        /// </summary>
        Natural = -3,
        /// <summary>
        /// Der Bereich der ganzen Zahlen.
        /// </summary>
        Integer = -2,
        /// <summary>
        /// Bereich der rationalen Zahlen.
        /// </summary>
        Rational = -1,
        /// <summary>
        /// Der Bereich der reellen Zahlen.
        /// </summary>
        Real = 0,
        /// <summary>
        /// Bereich der komplexen Zahlen.
        /// </summary>
        Complex = 1,
    }
}
