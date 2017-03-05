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
    /// Stellt eine Definitionsmenge bereit.
    /// </summary>
    public struct DomainSet : ISet<DomainSet, DomainSets>
    {
        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="DomainSet"/>-Struktur.
        /// </summary>
        /// <param name="set">Die zu speichernde Definitionsmenge.</param>
        public DomainSet(DomainSets set)
            : this()
        {
            this.Set = set;
        }

        /// <summary>
        /// Ruft die Definitionsmenge ab oder legt diese fest.
        /// </summary>
        public DomainSets Set { get; set; }

        /// <summary>
        /// Überprüft, ob sich ein Element in dieser Menge befindet.
        /// </summary>
        /// <typeparam name="T">Der Typ des Elements, das geprüft werden soll.</typeparam>
        /// <param name="element">Das zu prüfende Element.</param>
        /// <returns><c>True</c>, wenn <paramref name="element"/> sich in dieser Menge befindet. Andernfalls <c>False</c>.</returns>
        public bool IsInSet<T>(T element) where T : IMathEqualComparison<T>
        {
            return element.IsInDomain(this);
        }

        /// <summary>
        /// Ruft die Vereinigungsmenge dieser Menge und einer anderen Definitionsmenge ab.
        /// </summary>
        /// <param name="other">Die andere Definitionsmenge.</param>
        /// <returns>Die Vereinigungsmenge dieser Instanz und <paramref name="other"/>.</returns>
        public DomainSet Union(DomainSet other)
        {
            return new DomainSet((DomainSets)Math.Max((int)this.Set, (int)other.Set));
        }

        #region ISet<DomainSet,DomainSets> Member

        /// <summary>
        /// Ruft einen Wert ab, der angibt ob der Definitionsbereich einen anderen mit einschließt.
        /// </summary>
        /// <param name="element">Der Definitionsbereich, der geprüft werden soll.</param>
        /// <returns><c>True</c>, wenn <paramref name="element"/> in dieser Menge enthalten ist. Andernfalls <c>False</c>.</returns>
        public bool IsInSet(DomainSets element)
        {
            return this.Set >= element;
        }

        /// <summary>
        /// Ruft einen Wert ab, der angibt ob die Menge leer ist.
        /// </summary>
        /// <remarks>Dieser Wert ist immer <c>False</c>.</remarks>
        public bool IsEmpty
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Ruft einen Wert ab, der angibt, ob eine andere Definitionsmenge in dieser Menge enthalten ist.
        /// </summary>
        public DomainSet Intersect(DomainSet other)
        {
            return new DomainSet((DomainSets)Math.Min((int)this.Set, (int)other.Set));
        }

        /// <summary>
        /// Ruft diese Instanz ab oder legt diese fest.
        /// </summary>
        DomainSet ISet<DomainSet, DomainSets>.DomainSet
        {
            get
            {
                return this;
            }
            set
            {
                this = value;
            }
        }

        #endregion

        #region INumber Member

        /// <summary>
        /// Ruft diese Instanz ab.
        /// </summary>
        public INumber Result
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Ruft den gespeicherten Spezialwert ab.
        /// </summary>
        public SpecialValues SpecialValue
        {
            get
            {
                return SpecialValues.None;
            }
        }

        #endregion

        #region operator

        /// <summary>
        /// Vergleicht 2 Definitionsmengen auf Gleichheit.
        /// </summary>
        /// <param name="ds1">Die erste Definitionsmenge.</param>
        /// <param name="ds2">Die zweite Definitionsmenge.</param>
        /// <returns><c>True</c>, wenn <paramref name="ds1"/> und <paramref name="ds2"/> den selben Definitionsbereich beschreiben. Andernfalls <c>False</c>.</returns>
        public static bool operator ==(DomainSet ds1, DomainSet ds2)
        {
            return ds1.Set == ds2.Set;
        }

        /// <summary>
        /// Vergleicht 2 Definitionsmengen auf Ungleichheit.
        /// </summary>
        /// <param name="ds1">Die erste Definitionsmenge.</param>
        /// <param name="ds2">Die zweite Definitionsmenge.</param>
        /// <returns><c>False</c>, wenn <paramref name="ds1"/> und <paramref name="ds2"/> den selben Definitionsbereich beschreiben. Andernfalls <c>True</c>.</returns>
        public static bool operator !=(DomainSet ds1, DomainSet ds2)
        {
            return ds1.Set != ds2.Set;
        }

        #endregion

        #region override

        /// <summary>
        /// Ermittelt den Hashcode dieser Instanz.
        /// </summary>
        /// <returns>Der Hashcode dieser Instanz.</returns>
        public override int GetHashCode()
        {
            return (int)this.Set;
        }

        /// <summary>
        /// Überprüft ein Objekt mit dieser Instanz auf Wertgleichheit.
        /// </summary>
        /// <param name="obj">Das andere, zu vergleichende, Objekt.</param>
        /// <returns><c>True</c>, wenn <paramref name="obj"/> mit dieser Instanz Wertgleich ist. Andernfalls <c>False</c>.</returns>
        public override bool Equals(object obj)
        {
            return obj is DomainSet && obj.GetHashCode() == this.GetHashCode();
        }

        /// <summary>
        /// Ruft das Symbol des verwendeten Zahlenbereichs ab.
        /// </summary>
        /// <returns>Das Symbol des verwendeten Zahlenbereichs.</returns>
        /// <remarks>Der Bereich der natürlichen Zahlen ohne 0 wird mit dem OHNE-Operator ∖ dargestellt. (ℕ∖0)<para/>
        /// Es werden die Unicode-Sonderzeichen mit doppelter Linie verwendet. Der Bereich der Komplexen Zahlen liefert ein großes, lateinisches Standard-C.</remarks>
        public override string ToString()
        {
            //ℙℚℝℤℍℕC∖
            switch (this.Set)
            {
                case DomainSets.NaturalWithoutZero:
                    return "ℕ∖0";
                case DomainSets.Natural:
                    return "ℕ";
                case DomainSets.Integer:
                    return "ℤ";
                case DomainSets.Rational:
                    return "ℝ";
                case DomainSets.Real:
                    return "ℚ";
                case DomainSets.Complex:
                    return "C";
                default:
                    return string.Empty;
            }
        }

        #endregion
    }


}
