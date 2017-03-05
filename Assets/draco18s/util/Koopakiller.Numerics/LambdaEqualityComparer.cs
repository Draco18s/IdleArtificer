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
    /// Vergleicht zwei Objekte gleichen Typs anhand von zwei Methoden.
    /// </summary>
    /// <typeparam name="T">Der Typ der zu vergleichenden Objekte.</typeparam>
   public class LambdaEqualityComparer<T> : EqualityComparer<T>
    {
        #region .ctor

        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="LambdaEqualityComparer{T}"/>-Klasse.
        /// </summary>
        /// <param name="comparer">Die zu verwendende Vergleichsmethode.</param>
        /// <param name="hasher">Die zu verwendende Methode zum ermitteln des Hashcodes.</param>
        public LambdaEqualityComparer(ComparerMethod<T> comparer, GetHashCodeMethod<T> hasher)
        {
            this.Comparer = comparer;
            this.Hasher = hasher;
        }

        #endregion

        #region Eigenschaften

        /// <summary>
        /// Ruft die Methode zum vergleichen von zwei Objekten ab oder legt diese fest.
        /// </summary>
        public ComparerMethod<T> Comparer { get; set; }
        /// <summary>
        /// Ruft die Methode zum ermitteln des Hashcodes ab oder legt diese fest.
        /// </summary>
        public GetHashCodeMethod<T> Hasher { get; set; }

        #endregion

        #region Methoden

        /// <summary>
        /// Vergleicht 2 Objekte anhand der <see cref="Comparer"/>-Methode.
        /// </summary>
        /// <param name="x">Das erste Objekt.</param>
        /// <param name="y">Das zweite Objekt.</param>
        /// <returns></returns>
        public override bool Equals(T x, T y)
        {
            return this.Comparer(x, y);
        }

        /// <summary>
        /// Ermittelt den Hashcode eines Objekts anhand der <see cref="Hasher"/>-Methode.
        /// </summary>
        /// <param name="obj">Das Objekt dessen Hashcode ermittelt werden soll.</param>
        /// <returns>Der über <see cref="Hasher"/> ermittelte Hashcode von <paramref name="obj"/>.</returns>
        public override int GetHashCode(T obj)
        {
            return this.Hasher(obj);
        }

        /// <summary>
        /// Ruft den Namen der Klasse in Geschweiften Klammern ab.
        /// </summary>
        /// <returns>Der Name der Klasse in geschweiften Klammern.</returns>
        public override string ToString()
        {
            return "{LambdaEqualityComparer}";
        }

        #endregion
    }
}
