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
    /// Stellt einen Delegaten bereit mit dem zwei Objekte gleichen Typs verglichen werden.
    /// </summary>
    /// <typeparam name="TType">Der Typ von <paramref name="value1"/> und <paramref name="value2"/>.</typeparam>
    /// <param name="value1">Das erste Objekt.</param>
    /// <param name="value2">Das zweite Objekt.</param>
    /// <returns><c>True</c>, wenn <paramref name="value1"/> und <paramref name="value2"/> übereinstimmen. Andernfalls <c>False</c>.</returns>
    public delegate bool ComparerMethod<TType>(TType value1, TType value2);
}
