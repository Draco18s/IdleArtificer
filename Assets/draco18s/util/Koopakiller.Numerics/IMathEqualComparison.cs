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
    /// Wird implementiert, wenn ein Typ einen Mathematischen Größenvergleich durch ein Gleichheits- (=) oder Ungleichheitszeichen (≠) zulässt.
    /// </summary>
    /// <typeparam name="T">Der Typ, der einen mathematischen Größenvergleich zulässt.</typeparam>
    public interface IMathEqualComparison<T>
    {
        /// <summary>
        /// Überprüft, ob ein Wert gleich dem Wert dieser Instanz ist.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> und der Wert dieser Instanz gleich sind. Andernfalls <c>False</c>.</returns>
        bool IsEqual(T param);
        /// <summary>
        /// Überprüft, ob ein Wert ungleich dem Wert dieser Instanz ist.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> und der Wert dieser Instanz ungleich sind. Andernfalls <c>False</c>.</returns>
        bool IsNotEqual(T param);

        /// <summary>
        /// Überprüft ob sich das Element in einer Definitionsmenge befindet.
        /// </summary>
        /// <param name="domainSet">Die Definitionsmenge in der geprüft werden soll.</param>
        /// <returns><c>True</c>, wenn das Element sich in <paramref name="domainSet"/> befindet. Andernfalls <c>False</c>.</returns>
        bool IsInDomain(DomainSet domainSet);
    }
}
