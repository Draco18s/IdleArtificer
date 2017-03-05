//! Created by Tom Lambert alias Koopakiller
//! Project started: 2011
//! License: Microsoft Reciprocal License (Ms-RL)
//! Project site: https://numerics.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Koopakiller.Numerics.Resources;

namespace Koopakiller.Numerics
{
    /// <summary>
    /// Stellt einen Punkt auf einer Fläche dar.
    /// </summary>
    public struct Point2D : IPoint<Point2D>
    {
        /// <summary>
        /// Erstellt eine neue Instanz der Point2D-Struktur.
        /// </summary>
        /// <param name="x">Die X-Koordinate des Punktes.</param>
        /// <param name="y">Die Y-Koordinate des Punktes.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y")]
        public Point2D(double x, double y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        #region Eigenschaften

        /// <summary>
        /// Ruft die X-Koordinate des Punktes im Zweidimensionalen karthesischen Koordinatensystem ab oder legt diese fest.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "X")]
        public double X { get; set; }
        /// <summary>
        /// Ruft die Y-Koordinate des Punktes im Zweidimensionalen karthesischen Koordinatensystem ab oder legt diese fest.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Y")]
        public double Y { get; set; }

        /// <summary>
        /// Ruft die Einheit ab in der der Winkel der Polarkoordinate angegeben ist ab oder legt diese fest.
        /// </summary>
        public AngleUnits AngleUnit { get; set; }

        #region Polarkoordinaten

        /// <summary>
        /// Ruft den Radius des Punktes in Polarkoordinaten ab oder legt diese fest.
        /// </summary>
        /// <remarks>Die X- und Y-Koordinaten dienen als Ausgangswerte und passen sich bei einer Zuweisung an.</remarks>
        public double PolarRadius
        {
            get
            {
                return Calculator.Sqrt(this.X * this.X + this.Y * this.Y);
            }
            set
            {
                this.X = value * Calculator.Cos(this.PolarAnglePhi, this.AngleUnit);
                this.Y = value * Calculator.Sin(this.PolarAnglePhi, this.AngleUnit);
            }
        }

        /// <summary>
        /// Ruft den Winkel ϕ des Punktes in Polarkoordinaten ab oder legt diese fest.
        /// </summary>
        /// <remarks>Die X- und Y-Koordinaten dienen als Ausgangswerte und passen sich bei einer Zuweisung an.</remarks>
        public double PolarAnglePhi
        {
            get
            {
                if (this.X < 0)
                {
                    return Calculator.Atan(this.Y / this.X, this.AngleUnit) + Calculator.Circle(this.AngleUnit) / 2;
                }
                else if (this.X == 0)
                {
                    if (this.Y < 0)
                        return 3 * Calculator.Circle(this.AngleUnit) / 4;
                    else if (this.Y == 0)
                        return 0;
                    else
                        return Calculator.Circle(this.AngleUnit) / 4;
                }
                else
                {
                    if (this.Y < 0)
                        return Calculator.Atan(this.Y / this.X, this.AngleUnit) + Calculator.Circle(this.AngleUnit);
                    else
                        return Calculator.Atan(this.Y / this.X, this.AngleUnit);
                }
            }
            set
            {
                this.X = this.PolarRadius * Calculator.Cos(value, this.AngleUnit);
                this.Y = this.PolarRadius * Calculator.Sin(value, this.AngleUnit);
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Bestimmt ob der Punkt auf einer Geraden liegt.
        /// </summary>
        /// <param name="p1">Ein Punkt der ebenfalls auf der Geraden liegt und nicht die selben Koordinaten wie <paramref name="p2"/> oder dieser Punkt hat. </param>
        /// <param name="p2">Ein Punkt der ebenfalls auf der Geraden liegt und nicht die selben Koordinaten wie <paramref name="p1"/> oder dieser Punkt hat.</param>
        /// <returns><c>True</c>, wenn der Punkt auf der selben Geraden wie die anderen beiden Punkte liegen. Andernfalls <c>False</c>.</returns>
        public bool IsPointOnLine(Point2D p1, Point2D p2)
        {
            if (p1.X == p2.X && p1.Y == p2.Y)
                throw new ArgumentException(ResourceManager.GetMessage("Arg_XCantEqualWithY", "p1", "p2"), "p2");
            //(x - x1) / (x2 - x1) = (y - y1) / (y2 - y1) = (z - z1) / (z2 - z1)
            return (this.X - p1.X) / (p2.X - p1.X) == (this.Y - p1.Y) / (p2.Y - p1.Y);
        }

        /// <summary>
        /// Bestimmt ob der Punkt auf einer Linie zwischen einschließlich zweier Punkte liegt.
        /// </summary>
        /// <param name="p1">Der erste Punkt am Ende der Linie. Dieser muss sich von <paramref name="p2"/> unterscheiden.</param>
        /// <param name="p2">Der zweite Punkt am anderen Ende der Linie. Dieser muss sich von <paramref name="p1"/> unterscheiden.</param>
        /// <returns><c>True</c>, wenn der Punkt die gleichen Koordinaten wie <paramref name="p1"/> oder <paramref name="p2"/> hat bzw. auf der geraden Linie dazwischen liegt.</returns>
        public bool IsPointOnLineSegment(Point2D p1, Point2D p2)
        {
            return IsPointOnLine(p1, p2) && Math.Min(p1.X, p2.X) < this.X && Math.Max(p1.X, p2.X) > this.X && Math.Min(p1.Y, p2.Y) < this.Y && Math.Max(p1.Y, p2.Y) > this.Y;
        }

        #region override

        /// <summary>
        /// Gibt Die Koordinaten in Mathematischer Notation zurück.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}|{1})", this.X, this.Y);
        }

        /// <summary>
        /// Gibt den Kombinierten Hashcode der Koordinaten zurück.
        /// </summary>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode();
        }

        /// <summary>
        /// Vergleicht ein Objekt mit dieser Instanz auf Gleichheit.
        /// </summary>
        /// <param name="obj">Ein Vergleichsobjekt.</param>
        /// <returns><c>True</c>, wenn <paramref name="obj"/> vom Typ Point2D ist und die Koordinaten mit dieser Instanz übereinstimmen. Andernfalls <c>False</c>.</returns>
        public override bool Equals(object obj)
        {
            return obj is Point2D && ((Point2D)obj).X == this.X && ((Point2D)obj).Y == this.Y;
        }

        #endregion

        #region operator

        /// <summary>
        /// Vergleicht die Koordinaten von 2 Punkten auf Gleichheit.
        /// </summary>
        /// <param name="p1">Der 1. Vergleichspunkt.</param>
        /// <param name="p2">Der 2. Vergleichspunkt.</param>
        /// <returns><c>True</c>, wenn die Koordinaten beider Punkte gleich sind. Andernfall <c>False</c>.</returns>
        public static bool operator ==(Point2D p1, Point2D p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        /// <summary>
        /// Vergleicht die Koordinaten von 2 Punkten auf Ungleichheit.
        /// </summary>
        /// <param name="p1">Der 1. Vergleichspunkt.</param>
        /// <param name="p2">Der 2. Vergleichspunkt.</param>
        /// <returns><c>False</c>, wenn die Koordinaten beider Punkte gleich sind. Andernfalls <c>True</c>.</returns>
        public static bool operator !=(Point2D p1, Point2D p2)
        {
            return p1.X != p2.X || p1.Y != p2.Y;
        }

        /// <summary>
        /// Addiert einen Vektor zu einem Punkt hinzu.
        /// </summary>
        /// <param name="point">Der Punkt.</param>
        /// <param name="vector">Der Vektor, der zu <paramref name="point"/> dazu addiert werden soll.</param>
        public static Point2D operator +(Point2D point, Vector vector)
        {
            if (vector.Dimension != 2)
                throw new ArgumentDifferentException(ResourceManager.GetMessage("ArgDiff_Dimension", "point", "vector"), "vector", "point", point.Dimension, vector.Dimension);
            return new Point2D(point.X + vector[0], point.Y + vector[1]);
        }

        /// <summary>
        /// Subtrahiert einen Vektor von einem Punkt weg.
        /// </summary>
        /// <param name="point">Der Punkt.</param>
        /// <param name="vector">Der Vektor, der von <paramref name="point"/> abgezogen werden soll.</param>
        public static Point2D operator -(Point2D point, Vector vector)
        {
            if (vector.Dimension != 2)
                throw new ArgumentDifferentException(ResourceManager.GetMessage("ArgDiff_Dimension", "point", "vector"), "vector", "point", point.Dimension, vector.Dimension);
            return new Point2D(point.X - vector[0], point.Y - vector[1]);
        }

        /// <summary>
        /// Berechnet den Abstand zwischen 2 Punkten als Vektor.
        /// </summary>
        /// <param name="point1">Der 1. Punkt.</param>
        /// <param name="point2">Der 2. Punkt.</param>
        /// <returns>Der Abstand zwischen <paramref name="point1"/> und <paramref name="point2"/> als Vektor.</returns>
        public static Vector operator -(Point2D point1, Point2D point2)
        {
            return new Vector(point2.X - point1.X, point2.Y - point1.Y);
        }

        #endregion

        /// <summary>
        /// Subtrahiert einen Vektor von einem Punkt weg.
        /// </summary>
        /// <param name="point">Der Punkt.</param>
        /// <param name="vector">Der Vektor, der von <paramref name="point"/> abgezogen werden soll.</param>
        public static Point3D Add(Point3D point, Vector vector)
        {
            return point + vector;
        }

        /// <summary>
        /// Subtrahiert einen Vektor von einem Punkt weg.
        /// </summary>
        /// <param name="point">Der Punkt.</param>
        /// <param name="vector">Der Vektor, der von <paramref name="point"/> abgezogen werden soll.</param>
        public static Point3D Subtract(Point3D point, Vector vector)
        {
            return point - vector;
        }

        /// <summary>
        /// Berechnet den Abstand zwischen 2 Punkten als Vektor.
        /// </summary>
        /// <param name="point1">Der 1. Punkt.</param>
        /// <param name="point2">Der 2. Punkt.</param>
        /// <returns>Der Abstand zwischen <paramref name="point1"/> und <paramref name="point2"/> als Vektor.</returns>
        public static Vector Subtract(Point3D point1, Point3D point2)
        {
            return point1 - point2;
        }

        #region IPoint<Point2D> Member

        /// <summary>
        /// Berechnet die Distanz zwischen dieser Instanz und einem 2. Punkt.
        /// </summary>
        /// <param name="p2">Der 2. Punkt.</param>
        /// <returns>Die Distanz zwischen dem in dieser Instanz dargestellten Punkt und <paramref name="p2"/>.</returns>
        public double Distance(Point2D p2)
        {
            return Calculator.Sqrt((this.X - p2.X) * (this.X - p2.X) + (this.Y - p2.Y) * (this.Y - p2.Y));
        }

        /// <summary>
        /// Ruft die Dimension ab, in der dieser Punkt verwendet werden kann. (2)
        /// </summary>
        public int Dimension
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// Ruft die Koordinaten des Punktes ab oder legt diese fest.
        /// </summary>
        public double[] Coordinates
        {
            get
            {
                return new double[] { this.X, this.Y };
            }
            set
            {
                if (value == null) throw new ArgumentNullException(ResourceManager.GetMessage("ArgNull_Param", "value"));
                if (value.Length != 2)
                    throw new ArgumentOutOfRangeException(ResourceManager.GetMessage("ArgOutRang_ArraySize", "value", "2"));
                this.X = value[0];
                this.Y = value[1];
            }
        }

        /// <summary>
        /// Erzeugt einen Vektor aus des Koordinaten des Punktes.
        /// </summary>
        public Vector ToVector()
        {
            return new Vector(this.Coordinates);
        }

        /// <summary>
        /// Bestimmt, ob sich ein Punkt in der näher zu diesem Punkt befindet.
        /// </summary>
        /// <param name="point2">Der zweite, zu prüfende Punkt.</param>
        /// <param name="accuracy">Der höchste Abstand zwischen den 2 Punkten.</param>
        /// <param name="rangeType">Die Art, wie die Distanz berechnet werden soll.</param>
        /// <returns><c>True</c>, wenn sich <paramref name="point2"/> in der Nähe von diesem Punkt befindet. Andernfalls <c>False</c>.</returns>
        public bool IsInRangeOf(Point2D point2, double accuracy, PointInRangeMatchOptions rangeType)
        {
            switch (rangeType)
            {
                case PointInRangeMatchOptions.Sqare:
                    return this.X - accuracy <= point2.X && this.X + accuracy >= point2.X
                        && this.Y - accuracy <= point2.Y && this.Y + accuracy >= point2.Y;
                case PointInRangeMatchOptions.Circle:
                    return this.Distance(point2) <= accuracy;
                default:
                    throw new ArgumentOutOfRangeException(ResourceManager.GetMessage("ArgOutRang_WrongEnumValue"));
            }
        }

        #endregion
    }
}
