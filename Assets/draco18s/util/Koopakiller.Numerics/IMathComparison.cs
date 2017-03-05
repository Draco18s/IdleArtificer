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
    /// Wird implementiert, wenn ein Typ einen Mathematischen Größenvergleich durch Relationszeichen zulässt.
    /// </summary>
    /// <typeparam name="T">Der Typ, der einen mathematischen Größenvergleich zulässt.</typeparam>
    public interface IMathComparison<T> : IMathEqualComparison<T>
    {
        /// <summary>
        /// Überprüft, ob ein Wert größer ist, als der Wert dieser Instanz.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> größer ist als der Wert dieser Instanz. Andernfalls <c>False</c>.</returns>
        bool IsGreater(T param);
        /// <summary>
        /// Überprüft, ob ein Wert größer oder gleich dem Wert dieser Instanz ist.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> größer oder gleich dem Wert dieser Instanz ist. Andernfalls <c>False</c>.</returns>
        bool IsGreaterEqual(T param);
        /// <summary>
        /// Überprüft, ob ein Wert größer ist, als der Wert dieser Instanz.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> kleiner ist als der Wert dieser Instanz. Andernfalls <c>False</c>.</returns>
        bool IsSmaller(T param);
        /// <summary>
        /// Überprüft, ob ein Wert kleiner oder gleich dem Wert dieser Instanz ist.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> kleiner oder gleich dem Wert dieser Instanz ist. Andernfalls <c>False</c>.</returns>
        bool IsSmallerEqual(T param);
    }
}
