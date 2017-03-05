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
    /// Stellt verschiedene Einheiten für Winkel bereit.
    /// </summary>
    public enum AngleUnits
    {
        /// <summary>
        /// Der Winkel ist in Radiant (Bogenmaß) angegeben.
        /// </summary>
        Radian,
        /// <summary>
        /// Der Winkel ist in Grad angegeben.
        /// </summary>
        Degree,
        /// <summary>
        /// Der Winkel ist in Neugrad (Gon) angegeben.
        /// </summary>
        Gradian,
    }
}
