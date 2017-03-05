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
    /// Stellt eine Menge, bestehend aus einzelnen Elementen dar.
    /// </summary>
    /// <typeparam name="T">Der Typ, von dem die Elemente der Menge sind.</typeparam>
    public struct ListSet<T> : ISet<ListSet<T>, T>, ICollection<T>, IList<T>, IMathEqualComparison<ListSet<T>> where T : IMathEqualComparison<T>, new()
    {
        /// <summary>
        /// Erstellt eine neue Menge mit den angegebenen Elementen.
        /// </summary>
        /// <param name="items">Die Elemente, welche in der Menge enthalten sein sollen.</param>
        public ListSet(params T[] items)
            : this()
        {
            this.InsertItemsSecure(items);
        }
        /// <summary>
        /// Erstellt eine neue Menge mit den angegebenen Elementen.
        /// </summary>
        /// <param name="acceptDuplicates"><c>True</c>, wenn doppelte Elemente enthalten sein dürfen. Andernfalls <c>False</c>.</param>
        /// <param name="items">Die Elemente, welche in der Menge enthalten sein sollen.</param>
        public ListSet(bool acceptDuplicates, params T[] items)
            : this()
        {
            this.AcceptDuplicates = acceptDuplicates;
            this.InsertItemsSecure(items);
        }

        /// <summary>
        /// Fügt die, dem Konstruktor übergebenen, Elemente hinzu.
        /// </summary>
        private void InsertItemsSecure(T[] items)
        {
            if (items == null)
                return;
            foreach (T item in items)
                this.Add(item);
        }

        private List<T> _Items;
        /// <summary>
        /// Ruft die Elemente der Menge ab oder legt diese fest.
        /// </summary>
        private List<T> Items
        {
            get
            {
                if (this._Items == null)
                    this._Items = new List<T>();
                return this._Items;
            }
            set
            {
                this._Items = value;
            }
        }

        /// <summary>
        /// Ruft die Anzahl der Elemente in der Menge ab.
        /// </summary>
        public int Cardinality
        {
            get
            {
                return this.Items.Count;
            }
        }

        /// <summary>
        /// Ruft ein Element an der angegebenen Position ab oder legt dieses fest.
        /// </summary>
        /// <param name="index">Die Position des Elements, das abgerufen werden soll.</param>
        /// <returns>Das Element, welches sich an der angegebenen Position befindet.</returns>
        public T this[int index]
        {
            get
            {
                return this.Items[index];
            }
            set
            {
                this.Items[index] = value;
            }
        }

        /// <summary>
        /// Bildet die Vereinigungsmenge einer Menge und dieser Instanz.
        /// </summary>
        /// <param name="other">Die zweite Menge.</param>
        /// <returns>Die Vereinigungsmenge aus dieser Instanz und <paramref name="other"/>.</returns>
        public ListSet<T> Union(ListSet<T> other)
        {
            var t = this;
            return new ListSet<T>(this.Items.Where(x => t.DomainSet.IsInSet(x)).Union(other.Items.Where(x => other.DomainSet.IsInSet(x))).ToArray()) { DomainSet = this.DomainSet.Union(other.DomainSet), };
        }

        private bool _AcceptDuplicates;
        /// <summary>
        /// Ruft einen Wert ab, der angibt ob die Liste doppelte Elemente enthalten kann oder nicht oder legt diesen Wert fest.
        /// </summary>
        public bool AcceptDuplicates
        {
            get
            {
                return this._AcceptDuplicates;
            }
            set
            {
                this._AcceptDuplicates = value;
                if (!this._AcceptDuplicates)
                {
                    if (this.Items.ContainsDublicates())
                        throw new ArgumentDoubleException(ResourceManager.GetMessage("ArgDbl_Set"));
                }
            }
        }

        #region Schnittstellen

        #region ICollection<T> Member

        /// <summary>
        /// Fügt ein Element der Menge hinzu.
        /// </summary>
        /// <param name="item">Ein Element, welches noch nicht in der Menge enthalten ist.</param>
        public void Add(T item)
        {
            if (!this._AcceptDuplicates && this.Items.Contains(item))
                throw new ArgumentDoubleException(ResourceManager.GetMessage("ArgDbl_Set"));
            this.Items.Add(item);
        }

        /// <summary>
        /// Löscht alle Elemente aus der Menge.
        /// </summary>
        public void Clear()
        {
            this.Items.Clear();
        }

        /// <summary>
        /// Überprüft ob ein Wert in der Menge enthalten ist.
        /// </summary>
        /// <param name="item">Das zu prüfende Element.</param>
        /// <returns><c>True</c>, wenn das Element in der Menge enthalten ist. ANdernfalls <c>False</c>.</returns>
        public bool Contains(T item)
        {
            return this.Items.Contains(item);
        }

        /// <summary>
        /// Kopiert die Elemente der Menge ab dem angegebenen Index in ein Array des Element-Typs.
        /// </summary>
        /// <param name="array">Das Array, in das die Daten kopiert werden sollen.</param>
        /// <param name="arrayIndex">Der Index, ab dem die Elemente kopiert werden sollen.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.Items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Ruft die ANzahl von Elementen in der Menge ab.
        /// </summary>
        public int Count
        {
            get
            {
                return this.Items.Count;
            }
        }

        /// <summary>
        /// Ruft einen Wert ab, der angibt ob die Liste der Elemente schreibgeschützt ist.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Entfernt ein Element aus der Menge.
        /// </summary>
        /// <param name="item">Das zu entfernende Element.</param>
        /// <returns><c>True</c>, wenn das Element erfolgreich entfernt wurde. Andernfalls <c>False</c>. Sollte das Element nicht enthalten gewesen sein, so wird auch <c>False</c> zurück gegeben.</returns>
        public bool Remove(T item)
        {
            return this.Items.Remove(item);
        }

        #endregion

        #region IEnumerable<T> Member

        /// <summary>
        /// Gibt den Enumerator zurück, der die Elemente der Menge durchläuft.
        /// </summary>
        /// <returns>Ein Enumerator, der die Elemente der Menge durchläuft.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Member

        /// <summary>
        /// Gibt den Enumerator zurück, der die Elemente der Menge durchläuft.
        /// </summary>
        /// <returns>Ein Enumerator, der die Elemente der Menge durchläuft.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion

        #region ISet<SetList<T>,T> Member

        /// <summary>
        /// Überprüft ob ein Wert in der Menge enthalten ist.
        /// </summary>
        /// <param name="element">Das zu prüfende Element.</param>
        /// <returns><c>True</c>, wenn das Element in der Menge enthalten ist. ANdernfalls <c>False</c>.</returns>
        public bool IsInSet(T element)
        {
            return this.Items.Contains(element);
        }

        /// <summary>
        /// Bildet die Schnittmenge einer Menge und dieser Instanz.
        /// </summary>
        /// <param name="other">Die zweite Menge.</param>
        /// <returns>Die Schnittmenge aus dieser Instanz und <paramref name="other"/>.</returns>
        public ListSet<T> Intersect(ListSet<T> other)
        {
            var t = this;
            return new ListSet<T>(this.Items.Where(x => t.DomainSet.IsInSet(x)).Intersect(other.Items.Where(x => other.DomainSet.IsInSet(x))).ToArray()) { DomainSet = this.DomainSet.Intersect(other.DomainSet), };
        }

        /// <summary>
        /// Ruft den Definitionsbereich ab oder legt diesen fest.
        /// </summary>
        public DomainSet DomainSet { get; set; }

        #endregion

        #region IList<T> Member

        /// <summary>
        /// Ruft den Index eines bestimmten Elements ab.
        /// </summary>
        /// <param name="item">Das Element, dessen Index ermittelt werden soll.</param>
        /// <returns>Der Index von <paramref name="item"/> in der Menge.</returns>
        public int IndexOf(T item)
        {
            return this.Items.IndexOf(item);
        }

        /// <summary>
        /// Fügt ein Element in die Menge, an der angegebenen Position, ein.
        /// </summary>
        /// <param name="index">Der Index, wo das Element eingefügt werden soll.</param>
        /// <param name="item">Das einzufügende Element.</param>
        public void Insert(int index, T item)
        {
            if (!this._AcceptDuplicates && this.Items.Contains(item))
                throw new ArgumentDoubleException(ResourceManager.GetMessage("ArgDbl_Set"));
            this.Items.Insert(index, item);
        }

        /// <summary>
        /// Entfernt ein Element aus der Menge, an der angegebenen Position.
        /// </summary>
        /// <param name="index">Die Position des Elements in der Menge.</param>
        public void RemoveAt(int index)
        {
            this.Items.RemoveAt(index);
        }

        #endregion

        #region IMathEqualComparison<SetList<T>> Member

        /// <summary>
        /// Überprüft, ob ein Wert gleich dem Wert dieser Instanz ist.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> und der Wert dieser Instanz gleich sind. Andernfalls <c>False</c>.</returns>
        public bool IsEqual(ListSet<T> param)
        {
            return this == param;
        }

        /// <summary>
        /// Überprüft, ob ein Wert ungleich dem Wert dieser Instanz ist.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> und der Wert dieser Instanz ungleich sind. Andernfalls <c>False</c>.</returns>
        public bool IsNotEqual(ListSet<T> param)
        {
            return this != param;
        }

        /// <summary>
        /// Ruft einen Wert ab, der angibt ob der Wert in einem bestimmten Definitionsbereich liegt.
        /// </summary>
        /// <param name="set">Die zu prüfende Definitionsmenge.</param>
        /// <returns><c>True</c>, wenn diese Instanz in der angegebenen Definitionsmenge liegt. ANdernfalls <c>False</c>.</returns>
        /// <remarks>Der Rückgabewert ist immer <c>False</c>.</remarks>
        public bool IsInDomain(DomainSet domainSet)
        {
            return false;
        }

        #endregion

        #region INumber Member

        /// <summary>
        /// Ruft diese Instanz ab.
        /// </summary>
        INumber INumber.Result
        {
            get
            {
                return this;
            }
        }

        private SpecialValues _SpecialValue;
        /// <summary>
        /// Ruft den gespeicherten Spezialwert ab.
        /// </summary>
        public SpecialValues SpecialValue
        {
            get
            {
                return this._SpecialValue;
            }
        }

        #endregion

        #endregion

        #region operator

        /// <summary>
        /// Überprüft, ob ein Wert gleich einem anderen Wert ist.
        /// </summary>
        /// <param name="sl1">Der erste Wert.</param>
        /// <param name="sl2">Der zweite Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="sl1"/> und <paramref name="sl2"/> Wertgleich sind. Andernfalls <c>False</c>.</returns>
        public static bool operator ==(ListSet<T> sl1, ListSet<T> sl2)
        {
            if (sl1.Count != sl2.Count)
                return false;
            foreach (T item in sl1)
                if (!sl2.Contains(item))
                    return false;
            return true;
        }

        /// <summary>
        /// Überprüft, ob ein Wert ungleich einem anderen Wert ist.
        /// </summary>
        /// <param name="sl1">Der erste Wert.</param>
        /// <param name="sl2">Der zweite Wert.</param>
        /// <returns><c>False</c>, wenn <paramref name="sl1"/> und <paramref name="sl2"/> Wertgleich sind. Andernfalls <c>True</c>.</returns>
        public static bool operator !=(ListSet<T> sl1, ListSet<T> sl2)
        {
            if (sl1.Count == sl2.Count)
                return false;
            foreach (T item in sl1)
                if (sl2.Contains(item))
                    return false;
            return false;
        }

        #endregion

        private static readonly ListSet<T> _NaN = new ListSet<T>() { _SpecialValue = SpecialValues.NaN, };
        /// <summary>
        /// Ruft einen eine Menge ohne Elemente ab, die nicht definiert ist.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static ListSet<T> NaN
        {
            get
            {
                return _NaN;
            }
        }

        /// <summary>
        /// Ruft eine leere Menge ab.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static ListSet<T> Empty
        {
            get
            {
                return _Empty;
            }
        }
        private static ListSet<T> _Empty = new ListSet<T>();

        /// <summary>
        /// Ruft einen Wert ab, der angibt ob es sich bei der Menge um eine leere Menge handelt.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.Items.Count == 0;
            }
        }

        #region override

        /// <summary>
        /// Überprüft ein Objekt mit dieser Instanz auf Wertgleichheit.
        /// </summary>
        /// <param name="obj">Das andere, zu vergleichende, Objekt.</param>
        /// <returns><c>True</c>, wenn <paramref name="obj"/> von Typ <see cref="ListSet{T}"/> ist und die gleichen Elemente aufweist. Andernfalls <c>False</c>.</returns>
        public override bool Equals(object obj)
        {
            return obj is ListSet<T> && (ListSet<T>)obj == this;
        }

        /// <summary>
        /// Gibt den Hashcode der Elemente der Menge zurück.
        /// </summary>
        /// <returns>Der Hashcode der enthaltenen Elemente.</returns>
        public override int GetHashCode()
        {
            return this.Items.GetItemsHashCode();
        }

        /// <summary>
        /// Formatiert eine Zeichenfolge aus dieser Instanz.
        /// </summary>
        /// <returns>Eine Zeichenfolge, welche die Elemente dieser Menge repräsentiert.</returns>
        public override string ToString()
        {
            return "{" + string.Join(" | ", this.Items.Select(x => x.ToString()).ToArray()) + "}";
        }

        #endregion
    }
}
