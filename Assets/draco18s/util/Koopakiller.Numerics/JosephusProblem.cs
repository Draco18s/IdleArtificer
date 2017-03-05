//! Created by Tom Lambert alias Koopakiller
//! Project started: 2011
//! License: Microsoft Reciprocal License (Ms-RL)
//! Project site: https://numerics.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Koopakiller.Numerics.Resources;

namespace Koopakiller.Numerics
{
    /// <summary>
    /// Stellt Methoden zum lösen des Josephus Problems bereit.
    /// </summary>
    public static class JosephusProblem
    {
        /// <summary>
        /// Löst das Josephus Problem mit spezifischen Werten auf. Die Lösung ist nicht nullbasiert.
        /// </summary>
        /// <param name="elementCount">Die Anzahl von Elementen im Kreis.</param>
        /// <param name="stride">Die Schrittzahl, alle derer ein Element aus dem Kreis entfernt wird.</param>
        /// <returns>Die Position, auf welche aufgelöst wurde. Diese ist nicht nullbasiert!</returns>
        public static int Solve(int elementCount, int stride)
        {
            int pos = 1;
            _CheckValues(elementCount, stride, ref pos);

            if (elementCount == 1 && pos == 1)
                return 1;
            if (stride == 1 && pos == 1)
                return elementCount;

            return _Solve(elementCount, stride, pos);
        }

        /// <summary>
        /// Löst das Josephus Problem mit spezifischen Werten auf. Die Lösung ist nicht nullbasiert.
        /// </summary>
        /// <param name="elementCount">Die Anzahl von Elementen im Kreis.</param>
        /// <param name="stride">Die Schrittzahl, alle derer ein Element aus dem Kreis entfernt wird.</param>
        /// <param name="startPosition">Beschreibt das Element, welches beim ersten Abzählen die 1 ist. Dies ist im Normalfall das 1.</param>
        /// <returns>Die Position, auf welche aufgelöst wurde. Diese ist nicht nullbasiert!</returns>
        public static int Solve(int elementCount, int stride, int startPosition)
        {
            _CheckValues(elementCount, stride, ref startPosition);

            if (elementCount == 1 && startPosition == 1)
                return 1;
            if (stride == 1 && startPosition == 1)
                return elementCount;

            return _Solve(elementCount, stride, startPosition);
        }

        /// <summary>
        /// Wählt durch lösen des Josephus Problems ein Element aus einem Array aus.
        /// </summary>
        /// <typeparam name="T">Der Typ des Arrays und der Rückgabe.</typeparam>
        /// <param name="elements">Das Array, welches die Elemente enthält.</param>
        /// <param name="stride">Die Schrittzahl, alle derer ein Element aus dem Kreis (dem Array) entfernt wird.</param>
        /// <returns>Das Element, aus dem Array, welches nach der Lösung des Josephus Problems übrig bleibt.</returns>
        public static T Solve<T>(IList<T> elements, int stride)
        {
            if (elements == null)
                throw new ArgumentNullException("elements", ResourceManager.GetMessage("ArgNull_ArrayNull", "elements"));
            if(elements.Count ==0)
                throw new ArgumentOutOfRangeException("elements", ResourceManager.GetMessage("ArgOutRang_ArrayMinSize", "elements","1"));
            if (elements.Count == 1)
                return elements[0];

            int pos = 1;
            return elements[Solve(elements.Count, stride, pos) - 1];
        }

        /// <summary>
        /// Wählt durch lösen des Josephus Problems ein Element aus einem Array aus.
        /// </summary>
        /// <typeparam name="T">Der Typ des Arrays und der Rückgabe.</typeparam>
        /// <param name="elements">Das Array, welches die Elemente enthält.</param>
        /// <param name="stride">Die Schrittzahl, alle derer ein Element aus dem Kreis (dem Array) entfernt wird.</param>
        /// <param name="startPosition">Beschreibt das Element, welches beim ersten Abzählen die 1 ist. Dies ist im Normalfall das 1.</param>
        /// <returns>Das Element, aus dem Array, welches nach der Lösung des Josephus Problems übrig bleibt.</returns>
        public static T Solve<T>(IList<T> elements, int stride, int startPosition)
        {
            if (elements == null)
                throw new ArgumentNullException("elements", ResourceManager.GetMessage("ArgNull_ArrayNull", "elements"));
            if(elements.Count ==0)
                throw new ArgumentOutOfRangeException("elements", ResourceManager.GetMessage("ArgOutRang_ArrayMinSize", "elements","1"));
            if (elements.Count == 1)
                return elements[0];
            return elements[Solve(elements.Count, stride, startPosition) - 1];
        }

        #region Hilfsmethoden

        /// <summary>
        /// Löst das Josephus Problem. Die Parameter sind die selben wie in der Public-Version.
        /// </summary>
        private static int _Solve(int elementCount, int stride, int startPosition)
        {
            if (elementCount == 1)
                return 1;

            int newSp = (startPosition + stride - 2) % elementCount + 1;

            int survivor = _Solve(elementCount - 1, stride, newSp);
            if (survivor < newSp)
            {
                return survivor;
            }
            else
                return survivor + 1;

        }

        /// <summary>
        /// Erhöhrt <paramref name="startPosition"/> solange um <paramref name="stride"/> bis <paramref name="startPosition"/> größer als 0 ist.
        /// </summary>
        /// <param name="startPosition">Die Startposition zum lösen.</param>
        /// <param name="stride">Die Schrittzahl, alle derer ein Element aus dem Kreis (dem Array) entfernt wird.</param>
        private static int _MakePositionPositive(int startPosition, int stride)
        {
            while (startPosition <= 0)
            {
                startPosition += stride;
            }
            return startPosition;
        }

        /// <summary>
        /// Überprüft die Parameter auf Falscheingabe.
        /// </summary>
        /// <returns><c>True</c>, wenn alles korrekt übergeben wurde. Andernfalls <c>False</c> oder es wird eine Exception geworfen.</returns>
        private static bool _CheckValues(int elementCount, int stride, ref int startPosition)
        {
            if (elementCount < 1)
                throw new ArgumentOutOfRangeException("n", ResourceManager.GetMessage("ArgOutRang_ParamMustGreaterEqual", "n", "1"));
            if (stride < 1)
                throw new ArgumentOutOfRangeException("s", ResourceManager.GetMessage("ArgOutRang_ParamMustGreaterEqual", "s", "1"));

            if (startPosition <= 0)//Startposition in einen Bereich größer 0 bringen, um die Indizes anzugleichen
                startPosition = _MakePositionPositive(startPosition, stride);

            return true;
        }

        #endregion
    }
}