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
    /// Stellt einen Delegaten bereit mit dem der zu verwendende Hashcode eines Objekts ermittelt wird.
    /// </summary>
    /// <typeparam name="TType">Der Typ von <paramref name="value"/>.</typeparam>
    /// <param name="value">Das Objekt dessen Hashcode ermittelt werden soll.</param>
    /// <returns>Der Hashcode von <paramref name="value"/>.</returns>
    public delegate int GetHashCodeMethod<TType>(TType value);
}
