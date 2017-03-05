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
    /// Möglichkeiten für die Grenzen einer Definitionsmenge.
    /// </summary>
    public enum DomainFunctionLimits
    {
        /// <summary>
        /// Der angegebene Wert ist nicht in der Definitionsmenge.
        /// </summary>        
        Exclusive = 0,
        /// <summary>
        /// Die Grenze wird ignoriert. Jeder größere bzw. kleinerer Wert ist erlaubt.
        /// </summary>
        None,
        /// <summary>
        /// Der angegebene Wert ist noch in der Definitionsmenge.
        /// </summary>
        Inclusive,
    }
}
