//! Created by Tom Lambert alias Koopakiller
//! Project started: 2011
//! License: Microsoft Reciprocal License (Ms-RL)
//! Project site: https://numerics.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using Koopakiller.Numerics.Resources;

namespace Koopakiller.Numerics
{
    /// <summary>
    /// Stellt eine Matrix dar.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1036:OverrideMethodsOnComparableTypes")]
    public struct Matrix : /*IBigNumber,*/ INumber, IFormattable, IComparable, IComparable<Matrix>, IEquatable<Matrix>, IMathEqualComparison<Matrix>
    {
        #region .ctor

        /// <summary>
        /// Erstellt eine neue Instanz der Matrix-Klasse.
        /// </summary>
        /// <param name="xSize">Die Anzahl der Spalten der Matrix.</param>
        /// <param name="ySize">Die Anzahl der Zeilen der Matrix.</param>
        public Matrix(int xSize, int ySize)
            : this()
        {
            if ((xSize <= 1 && ySize <= 1) && (xSize != 0 && ySize != 0))
            {
                throw new ArgumentException(ResourceManager.GetMessage("ArgOutRang_ArrayMinSize2DMatrixEmpty"));
            }
            this.cplx = new Complex[xSize, ySize];
        }

        #endregion

        /// <summary>
        /// Eine leere Matrix mit 0 Zeilen und 0 Spalten.
        /// </summary>
        public static readonly Matrix Empty = new Matrix(0, 0);

        #region override

        /// <summary>
        /// Vergleicht ein Objekt mit dieser Instanz auf Gleichheit.
        /// </summary>
        /// <param name="obj">Das Vergleichsobjekt.</param>
        /// <returns><c>True</c>, wenn <paramref name="obj"/> vom Typ <see cref="Matrix"/> ist und die selben Elemente wie diese Instanz enthält.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Complex) || ((Matrix)obj).SizeX != this.SizeX || ((Matrix)obj).SizeY != this.SizeY)
                return false;
            for (int x = 0; x < ((Matrix)obj).SizeX; ++x)
            {
                for (int y = 0; y < ((Matrix)obj).SizeY; ++y)
                {
                    if (((Matrix)obj)[x, y] != this[x, y])
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Erzeugt einen Hashcode der Matrix aufgrund der enthaltenen Elemente.
        /// </summary>
        /// <remarks>Das Ergebnis ist gleich, wenn die Dimensionen und enthaltenen Elemente gleich sind.</remarks>
        /// <returns>Ein Hashcode, welcher aus den enthaltenen Elementen generiert wurde.</returns>
        public override int GetHashCode()
        {
            int hash = this.SizeX ^ this.SizeY;
            for (int x = 0; x < this.SizeX; ++x)
            {
                for (int y = 0; y < this.SizeY; ++y)
                {
                    hash ^= this[x, y].GetHashCode();
                }
            }
            return base.GetHashCode();
        }

        /// <summary>
        /// Erstellt eine Zeichenfolge, welche den Wert der Matrix in einem Zeile darstellt.
        /// </summary>
        /// <returns>Der Wert der Matrix, dargestellt als Zeichenfolge.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int y = 0; y < this.SizeY; ++y)
            {
                sb.Append("[");
                for (int x = 0; x < this.SizeX; ++x)
                {
                    sb.Append("[");
                    sb.Append(this[x, y].ToString());
                    sb.Append("]");
                }
                sb.Append("]");
            }
            sb.Append("]");
            return sb.ToString();
        }

        #endregion

        [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member")]
        Complex[,] cplx;

        /// <summary>
        /// Ruft die Anzahl der Spalten der Matrix ab.
        /// </summary>
        public int SizeX
        {
            get
            {
                return this.cplx.GetLength(0);
            }
        }
        /// <summary>
        /// Ruft die Anzahl der Zeilen der Matrix ab.
        /// </summary>
        public int SizeY
        {
            get
            {
                return this.cplx.GetLength(1);
            }
        }

        /// <summary>
        /// Erstellt aus der linearen Darstellungsform in einer Zeichenfolge einer Matrix eine neue Instanz der Matrix-Klasse.
        /// </summary>
        /// <param name="num">Die zu konvertierende Zeichenfolge.</param>
        /// <returns>Eine Instanz der Klasse Matrix, welche die Werte der angegebenen Zeichenfolge beinhaltet.</returns>
        public static Matrix Parse(string num)
        {
            return Parse(num, NumberFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Analysiert eine Zeichenfolge und erstellt daraus eine neue Instanz der <see cref="Matrix"/>-Strukur.
        /// </summary>
        /// <param name="num">Die zu analysierende Zeichenfolge.</param>
        /// <param name="provider">Das Format, in dem die einzelnen Stellen dargestellt sind.</param>
        public static Matrix Parse(string num, IFormatProvider provider)
        {
            if (string.IsNullOrEmpty(num))
                throw new ArgumentNullException("num", ResourceManager.GetMessage("ArgNull_ParamCantNull", "num"));

            int count = 0;
            int index = -1;
            List<Complex> line = new List<Complex>();
            List<List<Complex>> lines = new List<List<Complex>>();
            for (int i = 0; i < num.Length; ++i)
            {
                if (num[i] == '[')
                {
                    ++count;
                    index = i + 1;
                }
                else if (num[i] == ']')
                {
                    --count;
                    switch (count)
                    {
                        case 0://Matrix verlassen
                            Matrix m = new Matrix(lines[0].Count, lines.Count);
                            for (int x = 0; x < m.SizeX; ++x)
                            {
                                for (int y = 0; y < m.SizeY; ++y)
                                {
                                    m[x, y] = lines[y][x];
                                }
                            }
                            return m;
                        case 1://Zeile verlassen
                            lines.Add(line);
                            line = new List<Complex>();
                            break;
                        case 2://Zelle verlassen
                            line.Add(Complex.Parse(num.Substring(index, i - index), provider));
                            break;
                    }
                }
            }
            throw new ArgumentException(ResourceManager.GetMessage("Arg_ValIsntMatrix", "num"));
        }

        /// <summary>
        /// Analysiert eine Zeichenfolge, in der eine Matrix im linearen Format von MS Word dargestellt ist.
        /// </summary>
        /// <param name="word">Die zu analysierende Zeichenfolge.</param>
        public static Matrix ParseMSWord(string word)
        {
            return ParseMSWord(word, NumberFormatInfo.CurrentInfo);
        }
        /// <summary>
        /// Analysiert eine Zeichenfolge, in der eine Matrix im linearen Format von MS Word dargestellt ist.
        /// </summary>
        /// <param name="word">Die zu analysierende Zeichenfolge.</param>
        /// <param name="provider">Das Format, in dem die einzelnen Zahlenwerte dargestellt sind.</param>
        public static Matrix ParseMSWord(string word, IFormatProvider provider)
        {
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("word", ResourceManager.GetMessage("ArgNull_ParamCantNull", "word"));

            //(1&2&3@4&5&6@7&8&9)
            string[][] items = (from line in word.Split('@')
                                select line.Split('&')).ToArray();
            Matrix m = new Matrix(items[0].Length, items.Length);
            for (int y = 0; y < items.Length; ++y)
            {
                for (int x = 0; x < items[0].Length; ++x)
                {
                    m[x, y] = Complex.Parse(items[y][x], provider);
                }
            }
            return m;
        }

        /// <summary>
        /// Ruft ein Element der Matrix an der angegebenen Position ab oder legt dieses fest..
        /// </summary>
        /// <param name="xCoordinate">Die X-Koordinate des abzurufenden Elements. Dieser Index ist Nullbasiert.</param>
        /// <param name="yCoordinate">Die Y-Koordinate des abzurufenden Elements. Dieser Index ist Nullbasiert.</param>
        /// <returns>Das Element am angegebenen Index.</returns>
        [SuppressMessage("Microsoft.Design", "CA1023:IndexersShouldNotBeMultidimensional")]
        public Complex this[int xCoordinate, int yCoordinate]
        {
            get
            {
                return this.cplx[xCoordinate, yCoordinate];
            }
            set
            {
                this.cplx[xCoordinate, yCoordinate] = value;
            }
        }

        /// <summary>
        /// Ruft einen Wert ab, der angibt ob die Matrix keinen Inhalt hat.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.SizeX == 0 && this.SizeY == 0;
            }
        }

        /// <summary>
        /// Erzeugt aus einer Matrix einen mehrzeiligen String, welcher die Felder Grafisch darstellt.
        /// </summary>
        /// <returns>Die Matrix, als String dargestellt.</returns>
        public string CreateTextMatrix()
        {
            return CreateTextMatrix(null, NumberFormatInfo.CurrentInfo); //? Wie Double.ToString()
        }

        /// <summary>
        /// Erzeugt aus einer Matrix einen mehrzeiligen String, welcher die Felder Grafisch darstellt. Die Zahlen werden entsprechend der Angaben Formatiert.
        /// </summary>
        /// <param name="format">Eine numerische Formatierungszeichenfolge.</param>
        /// <param name="formatProvider">Ein Objekt, das kulturspezifische Formatierungsinformationen bereitstellt. </param>
        /// <returns>Eine Zeichenfolge, welche dem Wert dieser Instanz entspricht.</returns>
        public string CreateTextMatrix(string format, IFormatProvider formatProvider)
        {//┌│└┐│┘
            string[][] items = new string[this.SizeX][];
            for (int x = 0; x < this.SizeX; ++x)
            {
                string[] cells = new string[this.SizeY];
                for (int y = 0; y < this.SizeY; ++y)
                {
                    cells[y] = this[x, y].ToString(format, formatProvider);
                }
                int n = cells.Max(cell => cell.Length);//Längste Zeile ermitteln
                items[x] = (from cell in cells
                            select cell.PadLeft(n)).ToArray();
            }

            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < this.SizeY; ++y)
            {
                if (y == 0)
                    sb.Append("┌");
                else if (y == this.SizeY - 1)
                    sb.Append("└");
                else
                    sb.Append("│");
                for (int x = 0; x < this.SizeX; ++x)
                {
                    sb.Append(items[x][y]);
                    if (x != this.SizeX - 1)
                        sb.Append(" ");
                }
                if (y == 0)
                    sb.Append("┐\r\n");
                else if (y == this.SizeY - 1)
                    sb.Append("┘\r\n");
                else
                    sb.Append("│");
            }

            return sb.ToString();
        }

        #region operator

        /// <summary>
        /// Addiert 2 Matrizen miteinander. Die Matrizen müssen die selben Dimensionen aufweisen.
        /// </summary>
        /// <param name="m1">Der 1. Summand.</param>
        /// <param name="m2">Der 2. Summand.</param>
        /// <returns>Die Summer beider Matrizen</returns>
        /// <exception cref="System.ArgumentException">Wird geworfen, wenn die Matrizen Unterschiedliche Größen haben.</exception>
        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            if (m1.SizeX != m2.SizeX || m1.SizeY != m2.SizeY)
                throw new ArgumentException(ResourceManager.GetMessage("Arg_MatrixSameSize", "m1", "m2"));
            Matrix m = new Matrix(m1.SizeX, m1.SizeY);
            for (int x = 0; x < m1.SizeX; ++x)
            {
                for (int y = 0; y < m1.SizeY; ++y)
                {
                    m[x, y] = m1[x, y] + m2[x, y];
                }
            }
            return m;
        }

        /// <summary>
        /// Subtrahiert 2 Matrizen voneinander. Die Matrizen müssen die selben Dimensionen aufweisen.
        /// </summary>
        /// <param name="m1">Der Minuend.</param>
        /// <param name="m2">Der Subtrahend.</param>
        /// <returns>Die Differenz beider Matrizen.</returns>
        /// <exception cref="System.ArgumentException">Wird geworfen, wenn die Matrizen Unterschiedliche Größen haben.</exception>
        public static Matrix operator -(Matrix m1, Matrix m2)
        {
            if (m1.SizeX != m2.SizeX || m1.SizeY != m2.SizeY)
                throw new ArgumentException(ResourceManager.GetMessage("Arg_MatrixSameSize", "m1", "m2"));
            Matrix m = new Matrix(m1.SizeX, m1.SizeY);
            for (int x = 0; x < m1.SizeX; ++x)
            {
                for (int y = 0; y < m1.SizeY; ++y)
                {
                    m[x, y] = m1[x, y] - m2[x, y];
                }
            }
            return m;
        }

        /// <summary>
        /// Führt eine Skalarmultiplikation mit einer Matrize aus.
        /// </summary>
        /// <param name="matrix">Die Matrix der Multiplikation.</param>
        /// <param name="number">Die Skalare der Multiplikation.</param>
        /// <returns>Das Produkt der Skalare und der Matrix.</returns>
        public static Matrix operator *(Matrix matrix, Complex number)
        {
            Matrix result = new Matrix(matrix.SizeX, matrix.SizeY);
            for (int x = 0; x < matrix.SizeX; ++x)
            {
                for (int y = 0; y < matrix.SizeY; ++y)
                {
                    result[x, y] = matrix[x, y] * number;
                }
            }
            return result;
        }

        /// <summary>
        /// Führt eine Skalarmultiplikation mit einer Matrize aus.
        /// </summary>
        /// <param name="number">Die Skalare der Multiplikation.</param>
        /// <param name="matrix">Die Matrix der Multiplikation.</param>
        /// <returns>Das Produkt der Skalare und der Matrix.</returns>
        public static Matrix operator *(Complex number, Matrix matrix)
        {
            return matrix * number;
        }

        /// <summary>
        /// Multipliziert 2 Matrizen miteinander.
        /// </summary>
        /// <param name="m1">Der erste Faktor.</param>
        /// <param name="m2">Der zweite Faktor.</param>
        /// <returns>Das Produkt aus <paramref name="m1"/> und <paramref name="m2"/>.</returns>
        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            if (m1.SizeX != m2.SizeY)
                throw new ArgumentException(ResourceManager.GetMessage("ArgOutRang_ParamMustSmaller", "m1.SizeX (" + m1.SizeX + ")", "m2.SizeY (" + m2.SizeY + ")"));

            Matrix m = new Matrix(m1.SizeY, m2.SizeX);
            for (int x = 0; x < m.SizeX; ++x)
            {
                for (int y = 0; y < m.SizeY; ++y)
                {
                    m[x, y] = CalcMultipleCell(m1, m2, x, y);
                }
            }

            return m;
        }

        /// <summary>
        /// Negiert eine Matrix.
        /// </summary>
        /// <param name="matrix">Die zu negierende Matrix.</param>
        /// <returns>Eine Matrix, dessen negierter Wert, <paramref name="matrix"/> entspricht.</returns>
        public static Matrix operator -(Matrix matrix)
        {
            return matrix * -1;
        }

        /// <summary>
        /// Negiert eine Matrix.
        /// </summary>
        /// <param name="matrix">Die zu negierende Matrix.</param>
        /// <returns>Eine Matrix, dessen negierter Wert, <paramref name="matrix"/> entspricht.</returns>
        public static Matrix Negate(Matrix matrix)
        {
            return -matrix;
        }

        /// <summary>
        /// Vergleicht 2 Matrizen auf Gleichheit.
        /// </summary>
        /// <param name="m1">Die erste Matrix.</param>
        /// <param name="m2">Die zweite Matrix.</param>
        /// <returns><c>True</c>, wenn <paramref name="m1"/> un <paramref name="m2"/> den gleichen Wert haben. Andernfalls <c>False</c>.</returns>
        public static bool operator ==(Matrix m1, Matrix m2)
        {
            for (int x = 0; x < m1.SizeX; ++x)
            {
                for (int y = 0; y < m1.SizeY; ++y)
                {
                    if (m1[x, y] != m2[x, y])
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Vergleicht 2 Matrizen auf Ungleichheit.
        /// </summary>
        /// <param name="m1">Die erste Matrix.</param>
        /// <param name="m2">Die zweite Matrix.</param>
        /// <returns><c>True</c>, wenn <paramref name="m1"/> un <paramref name="m2"/> nicht den gleichen Wert haben. Andernfalls <c>False</c>.</returns>
        public static bool operator !=(Matrix m1, Matrix m2)
        {
            for (int x = 0; x < m1.SizeX; ++x)
            {
                for (int y = 0; y < m1.SizeY; ++y)
                {
                    if (m1[x, y] != m2[x, y])
                        return true;
                }
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Addiert 2 Matrizen miteinander. Die Matrizen müssen die selben Dimensionen aufweisen.
        /// </summary>
        /// <param name="m1">Der 1. Summand.</param>
        /// <param name="m2">Der 2. Summand.</param>
        /// <returns>Die Summer beider Matrizen</returns>
        /// <exception cref="System.ArgumentException">Wird geworfen, wenn die Matrizen Unterschiedliche Größen haben.</exception>
        [DebuggerStepThrough()]
        public static Matrix Add(Matrix m1, Matrix m2)
        {
            return m1 + m2;
        }

        /// <summary>
        /// Subtrahiert 2 Matrizen voneinander. Die Matrizen müssen die selben Dimensionen aufweisen.
        /// </summary>
        /// <param name="m1">Der Minuend.</param>
        /// <param name="m2">Der Subtrahend.</param>
        /// <returns>Die Differenz beider Matrizen.</returns>
        /// <exception cref="System.ArgumentException">Wird geworfen, wenn die Matrizen Unterschiedliche Größen haben.</exception>
        [DebuggerStepThrough()]
        public static Matrix Subtract(Matrix m1, Matrix m2)
        {
            return m1 - m2;
        }

        /// <summary>
        /// Multipliziert 2 Matrizen miteinander.
        /// </summary>
        /// <param name="m1">Der erste Faktor.</param>
        /// <param name="m2">Der zweite Faktor.</param>
        /// <returns>Das Produkt aus <paramref name="m1"/> und <paramref name="m2"/>.</returns>
        [DebuggerStepThrough()]
        public static Matrix Multiply(Matrix m1, Matrix m2)
        {
            return m1 * m2;
        }

        /// <summary>
        /// Führt eine Skalarmultiplikation mit einer Matrize aus.
        /// </summary>
        /// <param name="matrix">Die Matrix der Multiplikation.</param>
        /// <param name="number">Die Skalare der Multiplikation.</param>
        /// <returns>Das Produkt der Skalare und der Matrix.</returns>
        [DebuggerStepThrough()]
        public static Matrix Multiply(Matrix matrix, Complex number)
        {
            return matrix * number;
        }

        /// <summary>
        /// Führt eine Skalarmultiplikation mit einer Matrize aus.
        /// </summary>
        /// <param name="number">Die Skalare der Multiplikation.</param>
        /// <param name="matrix">Die Matrix der Multiplikation.</param>
        /// <returns>Das Produkt der Skalare und der Matrix.</returns>
        [DebuggerStepThrough()]
        public static Matrix Multiply(Complex number, Matrix matrix)
        {
            return matrix * number;
        }

        /// <summary>
        /// Erstellt eine Identitätsmatrix der angegebenen Größe.
        /// </summary>
        /// <param name="size">Die Anzahl der Zeilen und der Spalten der zu erzeugenden Identitätsmatrix.</param>
        /// <returns>Eine Identitätsmatrix der angegebenen Größe.</returns>
        /// <exception cref="System.ArgumentException">Wird ausgelöst, wenn size kleiner gleich 0 ist.</exception>
        public static Matrix CreateIdentityMatrix(int size)
        {
            if (size <= 0)
                throw new ArgumentException(ResourceManager.GetMessage("ArgOutRang_ParamMustGreater", "size (" + size + ")", "0"), "size");

            Matrix result = new Matrix(size, size);
            for (int x = 0; x < size; ++x)
            {
                for (int y = 0; y < size; ++y)
                {
                    if (x == y)
                        result[x, y] = 1;
                    else
                        result[x, y] = 0;
                }
            }
            return result;
        }

        /// <summary>
        /// Invertiert eine Matrix.
        /// </summary>
        /// <param name="matrix">Die zu invertierende Matrix.</param>
        /// <returns>Eine Matrix, welche die Inverse zur übergebenen Matrix darstellt.</returns>
        /// <exception cref="System.ArgumentException">Wird ausgelöst, wenn die Determinante der angegebenen Matrix 0 ist.</exception>
        public static Matrix Reciprocal(Matrix matrix)
        {
            if (matrix.Determinant == 0)
                throw new ArgumentException(ResourceManager.GetMessage("Arg_DeterminantCantZero"), "matrix");
            Matrix result = CreateIdentityMatrix(matrix.SizeX);
            for (int x = 0; x < matrix.SizeX; ++x)
            {
                Complex c = matrix[x, x];//1 In der Spalte
                if (!(c.Real == 1 && c.Imaginary == 0))
                {
                    for (int x2 = 0; x2 < matrix.SizeX; ++x2)//Ganze Zeile dividieren
                    {
                        matrix[x2, x] /= c;
                        result[x2, x] /= c;
                    }
                }
                for (int y = 0; y < matrix.SizeY; ++y)// 0en erzeugen
                {
                    if (y != x)// 1 auslassen
                    {
                        c = matrix[x, y];
                        for (int x2 = 0; x2 < matrix.SizeX; ++x2)//Ganze Zeile dividieren
                        {
                            matrix[x2, y] -= c * matrix[x2, x];
                            result[x2, y] -= c * result[x2, x];
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Potenziert eine Matrix.
        /// </summary>
        /// <param name="matrix">Die Basis der Potenz.</param>
        /// <param name="exponent">Der Exponent der Potenz. Dieser muss mindestens -1 betragen.</param>
        /// <returns>Bei Übergabe von -1 die Inverse der Matrix, siehe Reciprocal. Bei übergabe von 0 wird eine Identitätsmatrix 
        /// der größe der übergebenen Matrix zurück gegeben. Bei allen anderen Matritzen wird die entsprechende Potenz zurück gegeben.</returns>
        /// <exception cref="System.ArgumentException">Wird ausgelöst, wenn die Matrix nicht quadratisch ist, oder b nicht größer gleich -1 ist.</exception>
        public static Matrix Pow(Matrix matrix, int exponent)
        {
            if (matrix.SizeX != matrix.SizeY)
                throw new ArgumentException(ResourceManager.GetMessage("Arg_XMustEqualWithY", "matrix.SizeX (" + matrix.SizeX + ")", "matrix.SizeY (" + matrix.SizeY + ")"), "matrix");
            if (exponent < -1)
                throw new ArgumentOutOfRangeException("exponent", ResourceManager.GetMessage("ArgOutRang_ParamMustGreaterEqual", "exponent (" + exponent + ")", "1"));
            if (exponent == -1)
                return Reciprocal(matrix);
            if (exponent == 0)
                return CreateIdentityMatrix(matrix.SizeX);
            Matrix result = matrix;
            while (exponent > 0)
            {
                result *= matrix;
                --exponent;
            }
            return result;
        }

        /// <summary>
        /// Berechnet eine Zelle einer Matrizenmultiplikation.
        /// </summary>
        /// <param name="m1">Die 1. Matrix.</param>
        /// <param name="m2">Die 2. Matrix.</param>
        /// <param name="x">Die Nullbasierte x-Koordinate der zu berechnenden Zelle.</param>
        /// <param name="y">Die Nullbasierte y-Koordinate der zu berechnenden Zelle.</param>
        private static Complex CalcMultipleCell(Matrix m1, Matrix m2, int x, int y)
        {
            Complex c = new Complex();
            for (int i = 0; i < m1.SizeX; ++i)
                c += m1[i, y] * m2[x, i];
            return c;
        }

        /// <summary>
        /// Ruft die Determinante der Matrix ab.
        /// </summary>
        public Complex Determinant
        {
            get
            {
                return CalcDeterminant(this);
            }
        }

        /// <summary>
        /// Berechnet die Determinante einer Matrix.
        /// </summary>
        /// <param name="matrix">Die zu verarbeitende Matrix.</param>
        /// <returns>Die Determinante der übergebenen Matrix.</returns>
        public Complex CalcDeterminant(Matrix matrix)
        {
            if (matrix.SizeX != matrix.SizeY)
                throw new ArgumentOutOfRangeException("matrix",ResourceManager.GetMessage("Arg_XMustEqualWithY", "m.SizeX (" + matrix.SizeX + ")", "m.SizeY (" + matrix.SizeY + ")"));

            switch (matrix.SizeX)
            {
                case 0:
                    throw new ArgumentOutOfRangeException("matrix",ResourceManager.GetMessage("ArgOutRang_ParamMustGreater", "m.SizeX (" + matrix.SizeX + ")", "0"));
                case 1:
                    return matrix[0, 0];
                case 2:
                    return (matrix[0, 0] * matrix[1, 1]) - (matrix[1, 0] * matrix[0, 1]);
                case 3:
                    return (matrix[0, 0] * matrix[1, 1] * matrix[2, 2]) + (matrix[0, 1] * matrix[1, 2] * matrix[2, 0]) + (matrix[0, 2] * matrix[1, 0] * matrix[2, 1])
                         - (matrix[2, 0] * matrix[1, 1] * matrix[0, 2]) - (matrix[2, 1] * matrix[1, 2] * matrix[0, 0]) - (matrix[0, 1] * matrix[1, 0] * matrix[2, 2]);
                default:
                    Complex c = new Complex();
                    for (int i = 0; i < matrix.SizeX; ++i)
                    {
                        Matrix m1 = GetPart(1, matrix.SizeX - 1, 0, SizeY - 1, i);
                        if (i % 2 == 0)
                            c += m1.Determinant * matrix[0, i];
                        else
                            c -= m1.Determinant * matrix[0, i];
                    }
                    return c;
            }
        }

        /// <summary>
        /// Gibt einen Teil der Matrix zurück.
        /// </summary>
        /// <param name="left">Die 1. Spalte der neuen Matrix. (Nullbasiert)</param>
        /// <param name="right">Die letzte Spalte der neuen Matrix. (Nullbasiert)</param>
        /// <param name="top">Die 1. Zeile der neuen Matrix. (Nullbasiert)</param>
        /// <param name="bottom">Die Zeile Spalte der neuen Matrix. (Nullbasiert)</param>
        /// <returns>Die Teilmatrix aus dem angegebenen Bereich.</returns>
        public Matrix GetPart(int left, int right, int top, int bottom)
        {
            return GetPart(left, right, top, bottom, -1);
        }

        /// <summary>
        /// (Privat)Gibt einen Teil der Matrix zurück.
        /// </summary>
        /// <param name="left">Die 1. Spalte der neuen Matrix. (Nullbasiert)</param>
        /// <param name="right">Die letzte Spalte der neuen Matrix. (Nullbasiert)</param>
        /// <param name="top">Die 1. Zeile der neuen Matrix. (Nullbasiert)</param>
        /// <param name="bottom">Die Zeile Spalte der neuen Matrix. (Nullbasiert)</param>
        /// <param name="exceptLine">Die auszulassende Zeile. Wenn keine Zeile ausgelassen werden soll, dann geben Sie -1 an.</param>
        /// <returns>Die Teilmatrix aus dem angegebenen Bereich.</returns>
        private Matrix GetPart(int left, int right, int top, int bottom, int exceptLine)
        {
            Matrix m = new Matrix(right - left + 1, bottom - top + 1 - (exceptLine == -1 ? 0 : 1));
            for (int y = 0; y < bottom - top + (exceptLine == -1 ? 0 : 1); ++y)
            {
                if (y > exceptLine)
                    for (int x = 0; x < right - left + (exceptLine == -1 ? 0 : 1); ++x)
                    {
                        m[x, y - 1] = this[x + left, y + top];
                    }
                else if (y != exceptLine)
                    for (int x = 0; x < right - left + (exceptLine == -1 ? 0 : 1); ++x)
                    {
                        m[x, y] = this[x + left, y + top];
                    }
            }
            return m;
        }

        /// <summary>
        /// Überprüft ob die Matrix Symmentrisch ist.
        /// </summary>
        /// <param name="matrix">Die zu prüfende Matrix.</param>
        /// <returns><c>True</c>, wenn die Matrix symmetrisch ist, andernfalls <c>False</c>.</returns>
        public static bool IsMatrixSymmetric(Matrix matrix)
        {
            for (int x = 0; x < matrix.SizeX; ++x)
            {
                for (int y = 0; y < matrix.SizeY; ++y)
                {
                    if (y <= x)//Nur oben rechts liegendes Dreieck abprüfen
                        break;
                    if (matrix[x, y] != matrix[y, x])
                        return false;
                }
            }
            return true;
        }

        #region Eigenschaften

        /// <summary>
        /// Ruft einen Wert ab, der angibt ob die Matrix symmetrisch ist.
        /// </summary>
        public bool IsSymmetric
        {
            get
            {
                return IsMatrixSymmetric(this);
            }
        }

        #endregion

        #region IFormattable Member

        /// <summary>
        /// Formatiert diese Matrix im Linearen Format unter berücksichtigung des Formats und der Kultur.
        /// </summary>
        /// <param name="format">Das Format für die Anzeige der Zahlen.</param>
        /// <param name="formatProvider">Der FormatPrivider für die Formatierung.</param>
        /// <returns>Eine Lineare Ausgabe der Matrix als Zeichenfoge.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int y = 0; y < this.SizeY; ++y)
            {
                sb.Append("[");
                for (int x = 0; x < this.SizeX; ++x)
                {
                    sb.Append("[");
                    sb.Append(this[x, y].ToString(format, formatProvider));
                    sb.Append("]");
                }
                sb.Append("]");
            }
            sb.Append("]");
            return sb.ToString();
        }

        #endregion

        #region IEquatable<Matrix> Member

        /// <summary>
        /// Vergleicht ein Objekt mit dieser Instanz auf Gleichheit.
        /// </summary>
        /// <param name="other">Das andere Objekt.</param>
        /// <returns><c>True</c>, wenn die Objekte gleich sind, andernfalls <c>False</c>.</returns>
        public bool Equals(Matrix other)
        {
            return this == other;
        }

        #endregion

        #region IComparable Member

        /// <summary>
        /// Vergleicht diese Instanz mit einem anderen Objekt und gibt einen Wert zurück, mit dem der Wert gegenüber dieser Instanz eingeordnet werden kann.
        /// </summary>
        /// <param name="obj">Das andere Objekt, das mit dieser Instanz verglichen werden kann.</param>
        /// <returns>Ein Wert, anhand dem <paramref name="obj"/> in Bezug zu dieser Instanz eingeordnet werden kann.</returns>
        public int CompareTo(object obj)
        {
            if (!(obj is Matrix))
                return 1;
            return this.CompareTo((Matrix)obj);
        }

        #endregion

        #region IComparable<Matrix> Member

        /// <summary>
        /// Vergleicht diese Instanz mit einer anderen Matrix und gibt einen Wert zurück, mit dem der Wert gegenüber dieser Instanz eingeordnet werden kann.
        /// </summary>
        /// <param name="other">Die andere Matrix, die mit dieser Instanz verglichen werden kann.</param>
        /// <returns>Ein Wert, anhand dem <paramref name="other"/> in Bezug zu dieser Instanz eingeordnet werden kann.</returns>
        public int CompareTo(Matrix other)
        {
            if (other.SizeX < this.SizeX)
                return -1;
            if (other.SizeY < this.SizeY)
                return -1;
            
            for(int x=0;x<this.SizeX ;++x)
            for(int y=0;y<this.SizeY ;++y)
            {
                if (this[x, y] < other[x, y])
                    return -1;
                if (this[x, y] > other[x, y])
                    return 1;
            }
            return 0;
        }

        #endregion

        //internal override IBigNumber GetNumber()
        //{
        //    throw new NotImplementedException();
        //}

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

        #region IMathEqualComparison<Matrix> Member

        /// <summary>
        /// Überprüft, ob ein Wert gleich dem Wert dieser Instanz ist.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> und der Wert dieser Instanz gleich sind. Andernfalls <c>False</c>.</returns>
        public bool IsEqual(Matrix param)
        {
            return param == this;
        }
        /// <summary>
        /// Überprüft, ob ein Wert ungleich dem Wert dieser Instanz ist.
        /// </summary>
        /// <param name="param">Der andere Wert.</param>
        /// <returns><c>True</c>, wenn <paramref name="param"/> und der Wert dieser Instanz ungleich sind. Andernfalls <c>False</c>.</returns>
        public bool IsNotEqual(Matrix param)
        {
            return param != this;
        }

        /// <summary>
        /// Ruft einen Wert ab, der angibt ob der Wert in einem bestimmten Definitionsbereich liegt.
        /// </summary>
        /// <param name="domainSet">Die zu prüfende Definitionsmenge.</param>
        /// <returns><c>True</c>, wenn diese Instanz in der angegebenen Definitionsmenge liegt. ANdernfalls <c>False</c>.</returns>
        /// <remarks>Der Rückgabewert ist immer <c>False</c>.</remarks>
        public bool IsInDomain(DomainSet domainSet)
        {
            return false;
        }

        #endregion
    }
}
