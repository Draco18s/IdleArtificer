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
    /// Stellt ein mathematisches Intervall dar.
    /// </summary>
    /// <typeparam name="T">Der Typ, von dem die Elemente dem Intervall sind.</typeparam>
    public struct Interval<T> : ISet<Interval<T>, T> where T : IMathComparison<T>, new()
    {
        #region .ctor

        /// <summary>
        /// Erstellt ein Offenes Intervall mit dem angegebenen Bereich.
        /// </summary>
        /// <param name="minimum">Die Untergrenze des Intervalls.</param>
        /// <param name="maximum">Die Obergrenze des Intervalls.</param>
        public Interval(T minimum, T maximum) : this(minimum, maximum, Types.Opened) { }
        /// <summary>
        /// Erstellt ein Intervall mit dem angegebenen Bereich.
        /// </summary>
        /// <param name="minimum">Die Untergrenze des Intervalls.</param>
        /// <param name="maximum">Die Obergrenze des Intervalls.</param>
        /// <param name="type">Die Art des Intervalls.</param>
        public Interval(T minimum, T maximum, Types type)
            : this()
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
            this.IntervalType = type;
        }

        #endregion
        
        #region Eigenschaften

        /// <summary>
        /// Ruft die Untergrenze des Intervalls ab oder legt diese fest.
        /// </summary>
        public T Minimum { get; set; }
        /// <summary>
        /// Ruft die Obergrenze des Intervalls ab oder legt diese fest.
        /// </summary>
        public T Maximum { get; set; }

        /// <summary>
        /// Ruft die Intervallart ab oder legt diese fest.
        /// </summary>
        public Types IntervalType { get; set; }

        /// <summary>
        /// Ruft die Menge ab, in der das Intervall definiert ist oder legt dieses fest.
        /// </summary>
        public DomainSet DomainSet { get; set; }

        #endregion

        #region ISet<T> Member

        /// <summary>
        /// Ruft einen Wert ab, der Bestimmt ob das Intervall einen Wert einschließt oder nicht.
        /// </summary>
        /// <param name="element">Das zu prüfende Element.</param>
        /// <returns><c>True</c>, wenn <paramref name="element"/> sich in dem Intervall befindet. Andernfalls <c>False</c>.</returns>
        public bool IsInSet(T element)
        {
            return this.DomainSet.IsInSet(element)
                && ((element.IsGreater(this.Minimum) && element.IsSmaller(this.Maximum))
                 || (element.IsEqual(this.Minimum) && (this.IntervalType == Types.LeftOpened || this.IntervalType == Types.Opened))
                 || (element.IsEqual(this.Maximum) && (this.IntervalType == Types.RightOpened || this.IntervalType == Types.Opened)));
        }

        /// <summary>
        /// Ruft einen Wert ab, der angibt ob die Menge leer ist.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.Minimum.IsGreater(this.Maximum)
                    || (this.Minimum.IsEqual(this.Maximum) && this.IntervalType != Types.Closed);
            }
        }

        /// <summary>
        /// Bildet die Schnittmenge von diesem und einem anderen Intervall.
        /// </summary>
        /// <param name="other">Das zweite Intervall.</param>
        /// <returns>Die Schnittmenge aus dieser Menge und <paramref name="other"/>.</returns>
        public Interval<T> Intersect(Interval<T> other)
        {
            if (this.Maximum.IsSmaller(other.Minimum)//this liegt vor t2
             || this.Minimum.IsGreater(other.Minimum)//this liegt hinter t2
             || (this.Maximum.IsEqual(other.Minimum) && (this.IntervalType == Types.RightOpened || this.IntervalType == Types.Opened || other.IntervalType == Types.LeftOpened || this.IntervalType == Types.Opened))//this liegt vor t2, die Grenzen Schneiden sich aber in einem Wert.
             || (this.Minimum.IsEqual(other.Maximum) && (this.IntervalType == Types.LeftOpened || this.IntervalType == Types.Opened || other.IntervalType == Types.RightOpened || this.IntervalType == Types.Opened)))//this liegt hinter t2, die Grenzen Schneiden sich aber in einem Wert.
                return Interval<T>.Empty;//Leeres Intervall zurück geben.
            return IntersectWithoutSpecial(other);
            }
         public Interval<T> IntersectWithoutSpecial(Interval<T> other)
        {T min, max;
            min = this.Minimum.IsGreater(other.Minimum) ? this.Minimum : other.Minimum;
            max = this.Maximum.IsSmaller(other.Minimum) ? this.Maximum : other.Maximum;
            bool l = false, r = false;//True=geschlossen, False=geöffnet
            l = this.Minimum.IsEqual(other.Minimum) ? (this.IntervalType == Types.Closed || this.IntervalType == Types.RightOpened) && (other.IntervalType == Types.Closed || other.IntervalType == Types.RightOpened) : true;//Linksseitig die gleichen Werte und beide min linksseitig geschlossen?
            r = this.Maximum.IsEqual(other.Maximum) ? (this.IntervalType == Types.Closed || this.IntervalType == Types.LeftOpened) && (other.IntervalType == Types.Closed || other.IntervalType == Types.LeftOpened) : true;//Rechtsseitig die gleichen Werte und beide min. rechtsseitig geschlossen?
            return new Interval<T>(min, max, l && r ? Types.Closed : l ? Types.RightOpened : r ? Types.LeftOpened : Types.Opened) { DomainSet = this.DomainSet.Intersect(other.DomainSet), };
          }

        private static Interval<T> _Empty = new Interval<T>(new T(), new T(), Types.Opened);
        /// <summary>
        /// Ruft ein Intervall ohne Elemente ab.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static Interval<T> Empty
        {
            get
            {
                return _Empty;
            }
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

        /// <summary>
        /// Ruft den gespeicherten Spezialwert ab.
        /// </summary>
        SpecialValues INumber.SpecialValue
        {
            get
            {
                return SpecialValues.None;
            }
        }

        #endregion

        #region operator

        /// <summary>
        /// Überprüft 2 Intervalle auf Gleichheit.
        /// </summary>
        /// <param name="i1">Das erste Intervall.</param>
        /// <param name="i2">Das zweite Intervall.</param>
        /// <returns><c>True</c>, wenn die Intervalle den selben Wertebereich umfassen. Andernfalls <c>False</c>.</returns>
        public static bool operator ==(Interval<T> i1, Interval<T> i2)
        {
            return i1.Minimum.IsEqual(i2.Minimum)
                && i1.Maximum.IsEqual(i2.Maximum)
                && i1.IntervalType == i2.IntervalType
                && i1.DomainSet.Equals(i2.DomainSet);
        }

        /// <summary>
        /// Überprüft 2 Intervalle auf Ungleichheit.
        /// </summary>
        /// <param name="i1">Das erste Intervall.</param>
        /// <param name="i2">Das zweite Intervall.</param>
        /// <returns><c>False</c>, wenn die Intervalle den selben Wertebereich umfassen. Andernfalls <c>True</c>.</returns>
        public static bool operator !=(Interval<T> i1, Interval<T> i2)
        {
            return !(i1 == i2);
        }

        #endregion

        #region override

        /// <summary>
        /// Vergleicht ein Objekt mit diesem Intervall auf Wertgleichheit.
        /// </summary>
        /// <param name="obj">Das andere Objekt.</param>
        /// <returns><c>True</c>, wenn <paramref name="obj"/> vom Typ <see cref="Interval{T}"/> ist und den selben Wertebereich umfasst wie dieses Intervall. Andernfalls <c>False</c>.</returns>
        public override bool Equals(object obj)
        {
            return obj is Interval<T> && (Interval<T>)obj == this;
        }

        /// <summary>
        /// Ermittelt den Wertabhängigen Hashcode des Intervalls.
        /// </summary>
        /// <returns>Der Hashcode des Intervalls.</returns>
        public override int GetHashCode()
        {
            return this.Minimum.GetHashCode() ^ this.Maximum.GetHashCode() ^ this.DomainSet.GetHashCode() ^ this.IntervalType.GetHashCode();
        }

        /// <summary>
        /// Ruft das Intervall als Zeichenfolge ab.
        /// </summary>
        /// <returns>Die Darstellung des Intervalls als Zeichenfolge.</returns>
        public override string ToString()
        {
            string format = "";
            switch (this.IntervalType)
            {
                case Types.Opened:
                    format = "]{0}|{1}["; break;
                case Types.Closed:
                    format = "[{0}|{1}]"; break;
                case Types.LeftOpened:
                    format = "]{0}|{1}]"; break;
                case Types.RightOpened:
                    format = "[{0}|{1}["; break;
            }
            return string.Format(CultureInfo.CurrentCulture, format, this.Minimum, this.Maximum) + (" | " + this.DomainSet.ToString());
        }

        #endregion
    }
}