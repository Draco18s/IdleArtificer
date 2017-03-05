//! Created by Tom Lambert alias Koopakiller
//! Project started: 2011
//! License: Microsoft Reciprocal License (Ms-RL)
//! Project site: https://numerics.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using Koopakiller.Numerics.Resources;

namespace Koopakiller.Numerics
{
    /// <summary>
    /// Stellt einen Punkt im 3-Dimensionalen Raum dar.
    /// </summary>
    public struct Point3D : IPoint<Point3D>
    {
        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="Point3D"/>-Struktur.
        /// </summary>
        /// <param name="x">Die X-Koordinate im dreidimensionalen, kartesischen Koordinatensystem.</param>
        /// <param name="y">Die Y-Koordinate im dreidimensionalen, kartesischen Koordinatensystem.</param>
        /// <param name="z">Die Z-Koordinate im dreidimensionalen, kartesischen Koordinatensystem.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x"), SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y"), SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "z")]
        public Point3D(double x, double y, double z)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        #region Eigenschaften

        /// <summary>
        /// Ruft die X-Koordinate des Punktes im dreidimensionalen, kartesischem Koordinatensystem ab oder legt diese fest.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "X")]
        public double X { get; set; }
        /// <summary>
        /// Ruft die Y-Koordinate des Punktes im dreidimensionalen, kartesischem Koordinatensystem ab oder legt diese fest.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Y")]
        public double Y { get; set; }
        /// <summary>
        /// Ruft die Z-Koordinate des Punktes im dreidimensionalen, kartesischem Koordinatensystem ab oder legt diese fest.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Z")]
        public double Z { get; set; }

        /// <summary>
        /// Ruft die Einheit ab in der der Winkel der Polarkoordinate angegeben ist ab oder legt diese fest.
        /// </summary>
        public AngleUnits AngleUnit { get; set; }

        #region Kugelkoordinaten

        /// <summary>
        /// Ruft den Radius des Punktes in Polarkoordinaten ab oder legt diese fest.
        /// </summary>
        /// <remarks>Die X-, Y- und Z-Koordinaten dienen als Ausgangswerte und passen sich bei einer Zuweisung an.</remarks>
        public double PolarRadius
        {
            get
            {
                return Calculator.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
            }
            set
            {
                this.X = value * Calculator.Sin(this.PolarAngleTheta, this.AngleUnit) * Calculator.Cos(this.PolarAnglePhi, this.AngleUnit);
                this.Y = value * Calculator.Sin(this.PolarAngleTheta, this.AngleUnit) * Calculator.Sin(this.PolarAnglePhi, this.AngleUnit);
                this.Z = value * Calculator.Cos(this.PolarAngleTheta, this.AngleUnit);
            }
        }

        /// <summary>
        /// Ruft den Winkel ϕ des Punktes in Polarkoordinaten ab oder legt diese fest.
        /// </summary>
        /// <remarks>Die X-, Y- und Z-Koordinaten dienen als Ausgangswerte und passen sich bei einer Zuweisung an.</remarks>
        public double PolarAnglePhi
        {
            get
            {
                return Calculator.Atan(this.Y / this.X);
            }
            set
            {
                this.X = this.PolarRadius * Calculator.Sin(this.PolarAngleTheta, this.AngleUnit) * Calculator.Cos(value, this.AngleUnit);
                this.Y = this.PolarRadius * Calculator.Sin(this.PolarAngleTheta, this.AngleUnit) * Calculator.Sin(value, this.AngleUnit);
                this.Z = this.PolarRadius * Calculator.Cos(this.PolarAngleTheta, this.AngleUnit);
            }
        }

        /// <summary>
        /// Ruft den Winkel θ des Punktes in Polarkoordinaten ab oder legt diese fest.
        /// </summary>
        /// <remarks>Die X-, Y- und Z-Koordinaten dienen als Ausgangswerte und passen sich bei einer Zuweisung an.</remarks>
        public double PolarAngleTheta
        {
            get
            {
                return Calculator.Acos(this.Z / this.PolarRadius);
            }
            set
            {
                this.X = this.PolarRadius * Calculator.Sin(value, this.AngleUnit) * Calculator.Cos(this.PolarAnglePhi, this.AngleUnit);
                this.Y = this.PolarRadius * Calculator.Sin(value, this.AngleUnit) * Calculator.Sin(this.PolarAnglePhi, this.AngleUnit);
                this.Z = this.PolarRadius * Calculator.Cos(value, this.AngleUnit);
            }
        }

        #endregion

        #region Zylinderkoordinaten

        /// <summary>
        /// Ruft den Winkel ϕ des Punktes im Zylindrischen Koordinatensystem ab oder legt diesen fest.
        /// </summary>
        public double CylindricalAnglePhi
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
                this.X = this.CylindricalRadius * Calculator.Cos(value, this.AngleUnit);
                this.Y = this.CylindricalRadius * Calculator.Sin(value, this.AngleUnit);
                // Z = Z
            }
        }

        /// <summary>
        /// Ruft den Radius (Entfernung zur Z-Achse) des Punktes im Zylindrischen Koordinatensystem ab oder legt diesen fest.
        /// </summary>
        public double CylindricalRadius
        {
            get
            {
                return Calculator.Sqrt(this.X * this.X + this.Y * this.Y);
            }
            set
            {
                this.X = value * Calculator.Cos(this.CylindricalAnglePhi, this.AngleUnit);
                this.Y = value * Calculator.Sin(this.CylindricalAnglePhi, this.AngleUnit);
                // Z = Z
            }
        }

        #endregion

        #endregion

        #region override

        /// <summary>
        /// Gibt Die Koordinaten in Mathematischer Notation zurück.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}|{1}|{2})", this.X, this.Y, this.Z);
        }

        /// <summary>
        /// Gibt den Kombinierten Hashcode der Koordinaten zurück.
        /// </summary>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Y.GetHashCode();
        }

        /// <summary>
        /// Vergleicht ein Objekt mit dieser Instanz auf Gleichheit.
        /// </summary>
        /// <param name="obj">Ein Vergleichsobjekt.</param>
        /// <returns><c>True</c>, wenn <paramref name="obj"/> vom Typ <see cref="Point3D"/> ist und die Koordinaten mit dieser Instanz übereinstimmen. Andernfalls <c>False</c>.</returns>
        public override bool Equals(object obj)
        {
            return obj is Point3D && ((Point3D)obj).X == this.X && ((Point3D)obj).Y == this.Y && ((Point3D)obj).Z == this.Z;
        }

        #endregion

        #region operator

        /// <summary>
        /// Vergleicht die Koordinaten von 2 Punkten auf Gleichheit.
        /// </summary>
        /// <param name="p1">Der 1. Vergleichspunkt.</param>
        /// <param name="p2">Der 2. Vergleichspunkt.</param>
        /// <returns><c>True</c>, wenn die Koordinaten beider Punkte gleich sind. Andernfalls <c>False</c>.</returns>
        public static bool operator ==(Point3D p1, Point3D p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y || p1.Z == p2.Z;
        }

        /// <summary>
        /// Vergleicht die Koordinaten von 2 Punkten auf Ungleichheit.
        /// </summary>
        /// <param name="p1">Der 1. Vergleichspunkt.</param>
        /// <param name="p2">Der 2. Vergleichspunkt.</param>
        /// <returns><c>False</c>, wenn die Koordinaten beider Punkte gleich sind. Andernfalls <c>True</c>.</returns>
        public static bool operator !=(Point3D p1, Point3D p2)
        {
            return p1.X != p2.X || p1.Y != p2.Y || p1.Z != p2.Z;
        }

        /// <summary>
        /// Addiert einen Vektor zu einem Punkt hinzu.
        /// </summary>
        /// <param name="point">Der Punkt.</param>
        /// <param name="vector">Der Vektor, der zu <paramref name="point"/> dazu addiert werden soll.</param>
        public static Point3D operator +(Point3D point, Vector vector)
        {
            if (vector.Dimension != 3)
                throw new ArgumentDifferentException(ResourceManager.GetMessage("ArgDiff_Dimension", "point", "vector"), "vector", "point", point.Dimension, vector.Dimension);
            return new Point3D(point.X + vector[0], point.Y + vector[1], point.Z + vector[2]);
        }

        /// <summary>
        /// Subtrahiert einen Vektor von einem Punkt weg.
        /// </summary>
        /// <param name="point">Der Punkt.</param>
        /// <param name="vector">Der Vektor, der von <paramref name="point"/> abgezogen werden soll.</param>
        public static Point3D operator -(Point3D point, Vector vector)
        {
            if (vector.Dimension != 3)
                throw new ArgumentDifferentException(ResourceManager.GetMessage("ArgDiff_Dimension", "point", "vector"), "vector", "point", point.Dimension, vector.Dimension);
            return new Point3D(point.X - vector[0], point.Y - vector[1], point.Z - vector[2]);
        }

        /// <summary>
        /// Berechnet den Abstand zwischen 2 Punkten als Vektor.
        /// </summary>
        /// <param name="point1">Der 1. Punkt.</param>
        /// <param name="point2">Der 2. Punkt.</param>
        /// <returns>Der Abstand zwischen <paramref name="point1"/> und <paramref name="point2"/> als Vektor.</returns>
        public static Vector operator -(Point3D point1, Point3D point2)
        {
            return new Vector(point2.X - point1.X, point2.Y - point1.Y, point2.Z - point1.Z);
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

        #region IPoint<Point3D> Member

        /// <summary>
        /// Berechnet die Distanz zwischen dieser Instanz und einem 2. Punkt.
        /// </summary>
        /// <param name="p2">Der 2. Punkt.</param>
        /// <returns>Die Distanz zwischen dem in dieser Instanz dargestellten Punkt und <paramref name="p2"/>.</returns>
        public double Distance(Point3D p2)
        {
            return Calculator.Sqrt((this.X - p2.X) * (this.X - p2.X) + (this.Y - p2.Y) * (this.Y - p2.Y) + (this.Z - p2.Z) * (this.Z - p2.Z));
        }

        /// <summary>
        /// Ruft die Dimension ab, in der dieser Punkt verwendet werden kann. (3)
        /// </summary>
        public int Dimension
        {
            get
            {
                return 3;
            }
        }

        /// <summary>
        /// Ruft die Koordinaten des Punktes als Array ab oder legt diese fest.
        /// </summary>
        public double[] Coordinates
        {
            get
            {
                return new double[] { this.X, this.Y, this.Z };
            }
            set
            {
                if (value == null) throw new ArgumentNullException(ResourceManager.GetMessage("ArgNull_Param", "value"));
                if (value.Length != 3)
                    throw new ArgumentOutOfRangeException(ResourceManager.GetMessage("ArgOutRang_ArraySize", "value", "3"));
                this.X = value[0];
                this.Y = value[1];
                this.Z = value[2];
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
        public bool IsInRangeOf(Point3D point2, double accuracy, PointInRangeMatchOptions rangeType)
        {
            switch (rangeType)
            {
                case PointInRangeMatchOptions.Sqare:
                    return this.X - accuracy <= point2.X && this.X + accuracy >= point2.X
                        && this.Y - accuracy <= point2.Y && this.Y + accuracy >= point2.Y
                        && this.Z - accuracy <= point2.Z && this.Z + accuracy >= point2.Z;
                case PointInRangeMatchOptions.Circle:
                    return this.Distance(point2) <= accuracy;
                default:
                    throw new ArgumentOutOfRangeException(ResourceManager.GetMessage("ArgOutRang_WrongEnumValue"));
            }
        }

        #endregion
    }
}
