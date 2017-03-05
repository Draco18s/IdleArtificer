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
    /// Stellt die verschiedenen Modi bereit, mit denen Überprüft werden kann, ob 2 Punkte im selben Bereich liegen.
    /// </summary>
   public enum PointInRangeMatchOptions
    {
        /// <summary>
        /// Der Bereich um den ersten Punkt ist quadratisch bzw. Würfelförmig.
        /// </summary>
        Sqare,
        /// <summary>
        /// Der Bereich um den ersten Punkt ist Kreis bzw. Kugelförmig.
        /// </summary>
        Circle,
    }
}
