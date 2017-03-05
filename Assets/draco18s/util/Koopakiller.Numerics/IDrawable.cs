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
    /// Wird implementiert, wenn das Objekt gezeichnet werden kann.
    /// </summary>
    public interface IDrawable<T> where T: IPoint<T>
    {
        /// <summary>
        /// Ruft die Punkte des zu zeichnenden Polygons ab.
        /// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
IEnumerable<       IPoint<T>> Points { get; }
    }
}
