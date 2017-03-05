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
    /// Wird implementiert, wenn der Typ eine mathematische Menge darstellt.
    /// </summary>
    /// <typeparam name="TType">Der Typ, von dem die Menge ist.</typeparam>
    /// <typeparam name="TElementType">Der Typ, von dem die Elemente der Menge sind.</typeparam>
    public interface ISet<TType, TElementType> : INumber
    {
        /// <summary>
        /// Bestimmt, ob sich ein Element in der Menge befindet.
        /// </summary>
        /// <param name="element">Das zu prüfende Element.</param>
        /// <returns><c>True</c>, wenn <paramref name="element"/> sich in der Menge befindet. Andernfalls <c>False</c>.</returns>
        bool IsInSet(TElementType element);

        /// <summary>
        /// Ruft einen Wert ab, der angibt ob die Menge leer ist.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Bildet die Schnittmenge dieser und einer anderen Menge.
        /// </summary>
        /// <param name="other">Die zweite Menge.</param>
        /// <returns>Die Schnittmenge der Menge und <paramref name="other"/>.</returns>
        TType Intersect(TType other);

        /// <summary>
        /// Ruft die Definitionsmenge der Menge ab oder legt diese fest.
        /// </summary>
        DomainSet DomainSet { get; set; }
    }
}
