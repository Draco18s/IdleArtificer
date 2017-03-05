//! Created by Tom Lambert alias Koopakiller
//! Project started: 2011
//! License: Microsoft Reciprocal License (Ms-RL)
//! Project site: https://numerics.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Koopakiller.Numerics
{
    /// <summary>
    /// Muss von allen Typen implementiert werden die einen Punkt repräsentieren.
    /// </summary>
    public interface IPoint<T>
    {
        /// <summary>
        /// Berechnet die Distanz zwischen dieser Instanz und einem 2. Punkt.
        /// </summary>
        /// <param name="p2">Der 2. Punkt.</param>
        /// <returns>Die Distanz zwischen dem in dieser Instanz dargestellten Punkt und <paramref name="p2"/>.</returns>
        double Distance(T p2);

        /// <summary>
        /// Ruft die Dimension ab, in der der Punkt dargestellt werden kann.
        /// </summary>
        int Dimension { get; }

        /// <summary>
        /// Ruft die Koordinaten des Punktes als Array ab oder legt die Punkte fest.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        double[] Coordinates { get; set; }

        /// <summary>
        /// Überprüft, ob 2 Punkte im Selben Bereich liegen.
        /// </summary>
        /// <param name="point2">Der zweite, zu vergleichende Punkt.</param>
        /// <param name="accuracy">Die Größtmögliche Abweichung.</param>
        /// <param name="rangeType">Die Art des Bereichs.</param>
        /// <returns><c>True</c>, wenn der Punkt höchstens um <paramref name="accuracy"/> von dieser Instanz entfernt ist. Andernfalls <c>False</c>.</returns>
        bool IsInRangeOf(T point2, double accuracy, PointInRangeMatchOptions rangeType);

        /// <summary>
        /// Erzeugt einen Vector aus dem Punkt.
        /// </summary>
        /// <returns>Ein neuer Vector, dessen Werte den Koordinaten des Punktes entsprechen.</returns>
        Vector ToVector();
    }
}
