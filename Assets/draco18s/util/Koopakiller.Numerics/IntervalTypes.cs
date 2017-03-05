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
    /// Stellt die Intervallarten bereit.
    /// </summary>
    public enum Types
    {
        /// <summary>
        /// Ein geschlossenes Intervall.
        /// </summary>
        Closed = 0,
        /// <summary>
        /// Ein Beidseitig geöffnetes Intervall.
        /// </summary>
        Opened,
        /// <summary>
        /// Ein Linksseitig offenes Intervall.
        /// </summary>
        LeftOpened,
        /// <summary>
        /// Ein Rechtsseitig offenes Intervall.
        /// </summary>
        RightOpened,
    }
}
