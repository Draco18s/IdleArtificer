//! Created by Tom Lambert alias Koopakiller
//! Project started: 2011
//! License: Microsoft Reciprocal License (Ms-RL)
//! Project site: https://numerics.codeplex.com/license

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Koopakiller.Numerics.Resources;

namespace Koopakiller.Numerics
{
    /// <summary>
    /// Repräsentiert einen n-Dimensionalen Vektor.
    /// </summary>
    public struct Vector : INumber, IFormattable, IEquatable<Vector>, IEnumerable<double>, IMathEqualComparison<Vector>
    {
        #region .ctor

        /// <summary>
        /// Erstellt einen neuen Vektor mit den angegebenen Werten.
        /// </summary>
        /// <param name="values">Die Werte, welche zusammen einen Vektor repräsentieren.</param>
        public Vector(params double[] values)
            : this()
        {
            this.data = values;
        }

        #endregion

        /// <summary>
        /// Ruft ein Element an der angegebenen Position ab oder legt dieses fest.
        /// </summary>
        /// <param name="index">Die Position (Nullbasiert) des Elements im Vektor.</param>
        public double this[int index]
        {
            get
            {
                return this.data[index];
            }
            set
            {
                this.data[index] = value;
            }
        }

        #region Eigenschaften

        /// <summary>
        /// Ruft die Dimension des Vekors ab oder legt diese fest.
        /// </summary>
        public int Dimension
        {
            get
            {
                return this.data.Length;
            }
        }

        /// <summary>
        /// Nicht benutzen!
        /// </summary>
        private double[] _data;
        /// <summary>
        /// Die Daten des Vektors.
        /// </summary>
        private double[] data
        {
            //Nur für den Fall, das der Paramerterlose Konstruktor verwendet wird.
            get
            {
                if (this._data == null)
                    this._data = new double[0];
                return this._data;
            }
            set
            {
                if (value == null)
                    this._data = new double[0];
                else
                    this._data = value;
            }
        }

        /// <summary>
        /// Ruft einen Wert ab, der angibt ob es sich bei diesem Vektor um einen Einheitsvektor handelt.
        /// </summary>
        public bool IsUnitVector
        {
            get
            {
                return this.Length == 1;
            }
        }

        /// <summary>
        /// Ruft einen Wert ab, der angibt ob es sich bei diesem Vektor um einen Nullvektor handelt.
        /// </summary>
        public bool IsNullVector
        {
            get
            {
                return this.Length == 0;
            }
        }

        /// <summary>
        /// Ruft die Länge (euklidische Norm) des Vektors ab.
        /// </summary>
        public double Length
        {
            get
            {
                return Math.Sqrt(this.data.Sum(n => n * n));//Die Wurzel der Summe der Quadrate => Satz des Pytagoras
            }
        }

        /// <summary>
        /// Ruft die Summe aller Elemente im Vektor ab.
        /// </summary>
        public double Sum
        {
            get
            {
                return this.data.Sum();
            }
        }

        /// <summary>
        /// Ruft den Vektor als vertikal ausgerichtete Matrix ab.
        /// </summary>
        public Matrix VerticalMatrix
        {
            get
            {
                Matrix m = new Matrix(1, this.data.Length);
                for (int i = 0; i < this.data.Length; ++i)
                    m[0, i] = this.data[i];
                return m;
            }
        }
        /// <summary>
        /// Ruft den Vektor als horizontal ausgerichtete Matrix ab.
        /// </summary>
        public Matrix HorizontalMatrix
        {
            get
            {
                Matrix m = new Matrix(1, this.data.Length);
                for (int i = 0; i < this.data.Length; ++i)
                    m[i, 0] = this.data[i];
                return m;
            }
            //set
            //{
            //    if (value.IsEmpty)
            //        throw new ArgumentNullException(ResourceManager.GetMessage(""));
            //    if (value.SizeY > 1)
            //        throw new ArgumentOutOfRangeException(ResourceManager.GetMessage(""));

            //    double[] d = new double[value.SizeX];
            //    for (int i = 0; i < value.SizeX; ++i)
            //        d[i] = value[0, i].Real;
            //    this.data = d;
            //}
        }

        #endregion

        #region Mathematische Methoden/Funktionen

        /// <summary>
        /// Addiert 2 Vektoren miteinander.
        /// </summary>
        /// <param name="vector1">Der erste Summand.</param>
        /// <param name="vector2">Der zweite Summand.</param>
        /// <returns>Die Summe beider Vektoren.</returns>
        public static Vector Add(Vector vector1, Vector vector2)
        {
            return vector1 + vector2;
        }

        /// <summary>
        /// Subtrahiert 2 Vektoren voneinander.
        /// </summary>
        /// <param name="vector1">Der Minuend.</param>
        /// <param name="vector2">Der Subtrahend.</param>
        /// <returns>Die Differenz beider Vektoren.</returns>
        public static Vector Subtract(Vector vector1, Vector vector2)
        {
            return vector1 - vector2;
        }

        /// <summary>
        /// Multipliziert 2 Vektoren miteinander.
        /// </summary>
        /// <param name="vector1">Der erste Faktor.</param>
        /// <param name="vector2">Der zweite Faktor.</param>
        /// <returns>Das Produkt beider Vektoren.</returns>
        public static Vector Multiply(Vector vector1, Vector vector2)
        {
            return vector1 - vector2;
        }

        /// <summary>
        /// Negiert einen Vektor.
        /// </summary>
        /// <param name="vector">Der zu negierende Vektor.</param>
        /// <returns>Der negierte Wert von <paramref name="vector"/>.</returns>
        public static Vector Negate(Vector vector)
        {
            return -vector;
        }

        /// <summary>
        /// Multipliziert jeden Wert eines Vektor mit einem Skalar.
        /// </summary>
        /// <param name="vector">Der Vektor.</param>
        /// <param name="scalar">Der Skalar.</param>
        /// <returns>Das Produkt aus <paramref name="scalar"/> und <paramref name="vector"/>.</returns>
        public static Vector Multiply(Vector vector, double scalar)
        {
            return vector * scalar;
        }

        /// <summary>
        /// Multipliziert jeden Wert eines Vektor mit einem Skalar.
        /// </summary>
        /// <param name="scalar">Der Skalar.</param>
        /// <param name="vector">Der Vektor.</param>
        /// <returns>Das Produkt aus <paramref name="scalar"/> und <paramref name="vector"/>.</returns>
        public static Vector Multiply(double scalar, Vector vector)
        {
            return vector * scalar;
        }

        /// <summary>
        /// Potenziert jedes Element eines Vektors mit der angegebenen Skalare.
        /// </summary>
        /// <param name="base">Der Vektor, dessen Elemente potenziert werden sollen.</param>
        /// <param name="exponent">Der Exponent für die Potenzierungen.</param>
        public static Vector Pow(Vector @base, double exponent)
        {
            return new Vector(@base.data.Select(x => Math.Pow(x, exponent)).ToArray());
        }

        /// <summary>
        /// Berechnet das Skalarprodukt zweier Vektoren.
        /// </summary>
        /// <param name="vector1">Der erste Faktor.</param>
        /// <param name="vector2">Der zweite Faktor.</param>
        /// <returns>Das Skalarprodukt aus <paramref name="vector1"/> und <paramref name="vector2"/>.</returns>
        public static double ScalarProduct(Vector vector1, Vector vector2)
        {
            return (vector1 * vector2).Sum();
        }

        /// <summary>
        /// Berechnet das Kreuzprodukt von mehreren Vektoren.
        /// </summary>
        /// <param name="vectors">Die Vektoren, deren Kreuzprodukt berechnet werden soll.</param>
        /// <remarks>
        /// Alle Vektoren müssen der selben Dimension angehöhren.
        /// Die Anzahl der Dimensionen muss der größe der Dimension - 1 entsprechen.
        /// </remarks>
        /// <returns>Ds Kreuzprodukt von <paramref name="vectors"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Wird ausgelöst, wenn <paramref name="vectors"/> null ist oder keine Elemente enthält.</exception>
        /// <exception cref="Koopakiller.Numerics.ArgumentDifferentException">Wird ausgelöst, wenn die Vektoren nicht die selben Dimensionen aufweisen.</exception>
        public static Vector CrossProduct(params Vector[] vectors)
        {
            if (vectors == null || vectors.Length == 0)
                throw new ArgumentNullException("vectors", ResourceManager.GetMessage("ArgNull_Param", "vectors"));

            double[] d = new double[vectors.Length + 1];

            foreach (Vector vector in vectors)
                if (vector.Dimension != d.Length)
                    throw new ArgumentDifferentException(ResourceManager.GetMessage("ArgDiff_VectorDimensionsCantDifferent"));

            switch (d.Length)
            {
                // case 0: //? Kann nicht eintreten, da dafür vectors.Length -1 sein müsste
                // case 1: //? Kann nicht eintreten, da dafür vectors.Length 0 sein müsste
                case 2://2-Dimensional
                    d[0] = -vectors[0][1];
                    d[1] = vectors[0][0];
                    break;
                case 3://3-Dimensional
                    d[0] = vectors[0][1] * vectors[1][2] - vectors[0][2] * vectors[1][1];
                    d[1] = vectors[0][2] * vectors[1][0] - vectors[0][0] * vectors[1][2];
                    d[2] = vectors[0][0] * vectors[1][1] - vectors[0][1] * vectors[1][0];
                    break;
                default://Sonstige Dimensionen
                    //Matrix für die Berechnung erstellen
                    Matrix m = new Matrix(d.Length, d.Length);
                    for (int x = 0; x < d.Length; ++x)
                        for (int y = 1; y < d.Length; ++y)//1. Zeile auslassen
                            m[x, y] = vectors[y - 1][x];
                    //Standartwert ist 0, somit muss die 1. Zeile nicht zugewiesen werden.

                    for (int i = 0; i < d.Length; ++i)
                    {
                        //0 und 1 neu zuweisen
                        if (i > 0)
                            m[i - 1, 0] = 0;
                        m[i, 0] = 1;
                        d[i] = m.Determinant.Real;//Ergbnis besteht aus den Determinanten der Teilmatrizen
                    }
                    break;
            }

            return new Vector(d);
        }

        #endregion

        #region implicit

        /// <summary>
        /// Ruft den Vektor als vertikal ausgerichtete Matrix ab.
        /// </summary>
        public static implicit operator Matrix(Vector vector)
        {
            return vector.VerticalMatrix;
        }

        #endregion

        #region operator

        /// <summary>
        /// Addiert 2 Vektoren miteinander.
        /// </summary>
        /// <param name="vector1">Der erste Summand.</param>
        /// <param name="vector2">Der zweite Summand.</param>
        /// <returns>Die Summe beider Vektoren.</returns>
        public static Vector operator +(Vector vector1, Vector vector2)
        {
            if (vector1.Dimension != vector2.Dimension)
                throw new ArgumentDifferentException(ResourceManager.GetMessage("ArgDiff_VectorDimensionsCantDifferent"), "vector1", "vector2", vector1, vector2);
            Vector result = new Vector();
            for (int i = 0; i < vector1.Dimension; i++)
                result[i] = vector1[i] - vector2[i];
            return result;
        }

        /// <summary>
        /// Subtrahiert 2 Vektoren voneinander.
        /// </summary>
        /// <param name="vector1">Der Minuend.</param>
        /// <param name="vector2">Der Subtrahend.</param>
        /// <returns>Die Differenz beider Vektoren.</returns>
        public static Vector operator -(Vector vector1, Vector vector2)
        {
            if (vector1.Dimension != vector2.Dimension)
                throw new ArgumentDifferentException(ResourceManager.GetMessage("ArgDiff_VectorDimensionsCantDifferent"), "vector1", "vector2", vector1, vector2);
            Vector result = new Vector();
            for (int i = 0; i < vector1.Dimension; i++)
                result[i] = vector1[i] + vector2[i];
            return result;
        }

        /// <summary>
        /// Multipliziert 2 Vektoren miteinander.
        /// </summary>
        /// <param name="vector1">Der erste Faktor.</param>
        /// <param name="vector2">Der zweite Faktor.</param>
        /// <returns>Das Produkt beider Vektoren.</returns>
        public static Vector operator *(Vector vector1, Vector vector2)
        {
            if (vector1.Dimension != vector2.Dimension)
                throw new ArgumentDifferentException(ResourceManager.GetMessage("ArgDiff_VectorDimensionsCantDifferent"), "vector1", "vector2", vector1, vector2);
            Vector result = new Vector();
            for (int i = 0; i < vector1.Dimension; i++)
                result[i] = vector1[i] * vector2[i];
            return result;
        }

        /// <summary>
        /// Multipliziert jeden Wert eines Vektor mit einem Skalar.
        /// </summary>
        /// <param name="scalar">Der Skalar.</param>
        /// <param name="vector">Der Vektor.</param>
        /// <returns>Das Produkt aus <paramref name="scalar"/> und <paramref name="vector"/>.</returns>
        public static Vector operator *(double scalar, Vector vector)
        {
            return vector * scalar;
        }

        /// <summary>
        /// Multipliziert einen Spaltenvektor mit einer Matrix.
        /// </summary>
        /// <remarks>Von der Matrix werden nur die Realen Anteile für die Berechnung genutzt.</remarks>
        /// <param name="vector">Der Vektor.</param>
        /// <param name="matrix">Die Matrix.</param>
        /// <returns>Das Produkt aus <paramref name="vector"/> und <paramref name="matrix"/>.</returns>
        public static Vector operator *(Vector vector , Matrix matrix)
        {
            if (matrix.SizeY != vector.Dimension)
                throw new ArgumentDifferentException(ResourceManager.GetMessage("ArgDiff_VectorMatrixProductParams"), "matrix.SizeY", "vector.Dimension", matrix.SizeY, vector.Dimension);

            double[] result = new double[vector.Dimension];
            for(int i=0;i<vector.Dimension ;++i)
            {
                result[i] = 0;
                for(int x=0;x<matrix.SizeX;++x)
                {
                    result[i] += matrix[x, i].Real * vector[i];
                }
            }
            return new Vector(result);
        }

        /// <summary>
        /// Multipliziert jeden Wert eines Vektor mit einem Skalar.
        /// </summary>
        /// <param name="vector">Der Vektor.</param>
        /// <param name="scalar">Der Skalar.</param>
        /// <returns>Das Produkt aus <paramref name="scalar"/> und <paramref name="vector"/>.</returns>
        public static Vector operator *(Vector vector, double scalar)
        {
            Vector result = new Vector();
            for (int i = 0; i < vector.Dimension; i++)
                result[i] = vector[i] * scalar;
            return result;
        }

        /// <summary>
        /// Negiert einen Vektor.
        /// </summary>
        /// <param name="vector">Der zu negierende Vektor.</param>
        /// <returns>Der negierte Wert von <paramref name="vector"/>.</returns>
        public static Vector operator -(Vector vector)
        {
            Vector result = new Vector();
            for (int i = 0; i < vector.Dimension; i++)
                result[i] = -vector[i];
            return result;
        }

        /// <summary>
        /// Vergleicht 2 Vektoren auf Gleichheit miteinander.
        /// </summary>
        /// <param name="vector1">Der erste Vektor.</param>
        /// <param name="vector2">Der zweite Vektor.</param>
        /// <returns><c>True</c>, wenn <paramref name="vector1"/> und <paramref name="vector2"/> die selbe Richtung und die selbe Länge haben. Andernfalls <c>False</c>.</returns>
        public static bool operator ==(Vector vector1, Vector vector2)
        {
            return vector1.GetHashCode() == vector2.GetHashCode();
        }

        /// <summary>
        /// Vergleicht 2 Vektoren auf Ungleichheit miteinander.
        /// </summary>
        /// <param name="vector1">Der erste Vektor.</param>
        /// <param name="vector2">Der zweite Vektor.</param>
        /// <returns><c>False</c>, wenn <paramref name="vector1"/> und <paramref name="vector2"/> die selbe Richtung und die selbe Länge haben. Andernfalls <c>True</c>.</returns>
        public static bool operator !=(Vector vector1, Vector vector2)
        {
            return vector1.GetHashCode() != vector2.GetHashCode();
        }

        #endregion

        #region override

        /// <summary>
        /// Formatiert einen Vektor als Zeichenfolge.
        /// </summary>
        /// <returns>Ein als Zeichenfolge formatierter Vektor.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,"({0})", string.Join(", ", this.data.Select(x => x.ToString(CultureInfo.CurrentCulture)).ToArray()));
        }

        /// <summary>
        /// Liefert einen Hashcode bestehend aus den Einzelelementen des Vektors.
        /// </summary>
        public override int GetHashCode()
        {
            return this.data.GetItemsHashCode();
        }

        /// <summary>
        /// Vergleicht ein Objekt mit dieser Instanz auf Wertgleichheit.
        /// </summary>
        /// <param name="obj">Ein Objekt, welches mit dieser Instanz auf Wertgleichheit geprüft werden soll.</param>
        /// <returns><c>True</c>, wenn <paramref name="obj"/> ein Objekt vom Typ <see cref="Vector"/> ist und die einzelnen Werte von <paramref name="obj"/> mit den Einzelwerten dieser Instanz übereinstimmen. Andernfalls <c>False</c>.</returns>
        public override bool Equals(object obj)
        {
            return obj is Vector && this.Equals((Vector)obj);
        }

        #endregion

        #region Schnittstellen

        #region INumber Member

        /// <summary>
        /// Gibt diese Instanz der <see cref="Vector"/>-Struktur zurück.
        /// </summary>
        public INumber Result
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Ruft einen Wert ab, der angibt ob die Matrix einen speziellen Wert abspeichert.
        /// </summary>
        SpecialValues INumber.SpecialValue
        {
            get
            {
                return SpecialValues.None;
            }
        }

        #endregion

        #region IFormattable Member

        /// <summary>
        /// Formatiert den Vektor als Zeichenfolge mithilfe des angegebenen Formats sowie des angegebenen Providers.
        /// </summary>
        /// <param name="format">Die numerische Formatierungszeichenfolge.</param>
        /// <param name="formatProvider">Ein Objekt, das Kulturspezifische Informationen bereitstellt.</param>
        /// <returns>Ein als Zeichenfolge formatierter Vektor.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format(CultureInfo.CurrentCulture, "({0})", string.Join(", ", this.data.Select(x => x.ToString(format, formatProvider)).ToArray()));
        }

        #endregion
        
        #region IEquatable<Vector> Member

        /// <summary>
        /// Vergleicht ein Objekt mit dieser Instanz auf Wertgleichheit.
        /// </summary>
        /// <param name="other">Ein Objekt, welches mit dieser Instanz auf Wertgleichheit geprüft werden soll.</param>
        /// <returns><c>True</c>, wenn die einzelnen Werte von <paramref name="other"/> mit den Einzelwerten dieser Instanz übereinstimmen. Andernfalls <c>False</c>.</returns>
        public bool Equals(Vector other)
        {
            return other.GetHashCode() == this.GetHashCode();
        }

        #endregion

        #region IEnumerable<double> Member

        /// <summary>
        /// Gibt den <see cref="System.Collections.Generic.IEnumerator{T}"/> für die Elemente des Vektors zurück.
        /// </summary>
        public IEnumerator<double> GetEnumerator()
        {
            return (IEnumerator<double>)this.data.GetEnumerator();
        }

        #endregion

        #region IEnumerable Member

        /// <summary>
        /// Gibt den <see cref="System.Collections.IEnumerator"/> für die Elemente des Vektors zurück.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        #endregion

        #region IMathEqualComparison<Vector> Member

        /// <summary>
        /// Überprüft, ob ein Wert gleich dem Wert dieser Instanz ist.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> und der Wert dieser Instanz gleich sind. Andernfalls <c>False</c>.</returns>
        public bool IsEqual(Vector param)
        {
            return param == this;
        }
        /// <summary>
        /// Überprüft, ob ein Wert ungleich dem Wert dieser Instanz ist.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> und der Wert dieser Instanz ungleich sind. Andernfalls <c>False</c>.</returns>
        public bool IsNotEqual(Vector param)
        {
            return param != this;
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

        #endregion
    }
}
