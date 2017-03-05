//! Created by Tom Lambert alias Koopakiller
//! Project started: 2011
//! License: Microsoft Reciprocal License (Ms-RL)
//! Project site: https://numerics.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq;
using Koopakiller.Numerics.Resources;

namespace Koopakiller.Numerics
{
    /// <summary>
    /// Stellt ein Polygon im 2D-Raum dar.
    /// </summary>
    public struct Polygon2D : IDrawable<Point2D>
    {
        /// <summary>
        /// Erstellt eine neue Instanz der Polygon2D-Klasse.
        /// </summary>
        /// <param name="points">Die Eckpunkte, welche das Polygon aufweißt.</param>
        public Polygon2D(params Point2D[] points)
            : this()
        {
            this.Points = new List<Point2D>(points);
        }
        /// <summary>
        /// Erstellt eine neue Instanz der Polygon2D-Klasse.
        /// </summary>
        /// <param name="count">Die Anzahl von Eckpunkten in dem regelmäßigen Polygon.</param>
        /// <param name="size">Der Radius des Polygons vom Zentrum zu den Eckpunkten.</param>
        /// <param name="degree">Die Anzahl Grad, um die das Polygon gedreht werden soll.</param>
        /// <param name="center">Der Mittelpunkt des Polygons.</param>
        public Polygon2D(int count, Point2D center, double size, double degree)
            : this()
        {
            if (count < 3)
                throw new ArgumentOutOfRangeException("count", ResourceManager.GetMessage("ArgOutRang_ParamMustGreaterEqual", "count", "3"));
            if (degree < 0 || degree >= 360)
                throw new ArgumentOutOfRangeException("count", ResourceManager.GetMessage("ArgOutRang_ParamMustInRangeWithoutRight", "degree", "0", "360"));
            if (size < 1)
                throw new ArgumentOutOfRangeException("count", ResourceManager.GetMessage("ArgOutRang_ParamMustGreaterEqual", "size", "0"));

            while (degree >= 360 / count)
                degree -= 360 / count;

            List<Point2D> points = new List<Point2D>();

            for (int i = 0; i < count; ++i)
            {
                // Point2D p1 = new Point2D(), p2 = new Point2D();
                double d = 360d / (double)count * i + degree;

                d += 180 / count;
                if (d >= 360)
                    d -= 360;

                points.Add(GetRegularPoint(center, d, size));
            }
            this.Points = points;
        }
        
        #region Hilfsmethoden

        /// <summary>
        /// Gibt einen Punkt eines regelmäßigen Polygons zurück.
        /// </summary>
        /// <param name="center">Das Zentrum des Polygons</param>
        /// <param name="d">Der Punkt auf der Kreisbahn, zwischen einschließlich 0° und 360°.</param>
        /// <param name="io">Der Radius der Kreisbahn</param>
        private static Point2D GetRegularPoint(Point2D center, double d, double io)
        {
            Point2D p = new Point2D();

            if (d == 0)
                p = new Point2D(center.X, (float)(center.Y - io));
            else if (d < 90)
                p = new Point2D(center.X + (float)(Math.Cos((90 - d) / 180 * Math.PI) * io),
                                center.Y - (float)(Math.Sin((90 - d) / 180 * Math.PI) * io));
            else if (d == 90)
                p = new Point2D((float)(center.X + io), center.Y);
            else if (d < 180)
                p = new Point2D(center.X + (float)(Math.Sin((180 - d) / 180 * Math.PI) * io),
                                center.Y + (float)(Math.Cos((180 - d) / 180 * Math.PI) * io));
            else if (d == 180)
                p = new Point2D(center.X, center.Y + io);
            else if (d < 270)
                p = new Point2D(center.X - (float)(Math.Cos((270 - d) / 180 * Math.PI) * io),
                                center.Y + (float)(Math.Sin((270 - d) / 180 * Math.PI) * io));
            else if (d == 270)
                p = new Point2D((float)(center.X - io), center.Y);
            else if (d < 360)
                p = new Point2D(center.X - (float)(Math.Sin((360 - d) / 180 * Math.PI) * io),
                                center.Y - (float)(Math.Cos((360 - d) / 180 * Math.PI) * io));

            return p;
        }

        #endregion

//        /// <summary>
//        /// Ruft die Fläche des Polygons ab.
//        /// </summary>
//        public double Area
//        {
//            get
//            {
//#warning Vervollständigen!
//                throw new NotImplementedException();
//            }
//        }

        /// <summary>
        /// Ruft die Liste an Eckpunkten ab bzw. legt diese fest.
        /// </summary>
        public List<Point2D> Points { get;private  set; }

        /// <summary>
        /// Stellt die Modi für Treffertests dar.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public enum TouchTestMode
        {
            /// <summary>
            /// Ein Punkt, welcher auf dem Rahmen liegt wird als Treffer gewertet.
            /// </summary>
            IncludeBorder,
            /// <summary>
            /// Ein Punkt, welche auf dem Border liegt wird nicht als Treffer gewertet.
            /// </summary>
            WithoutBorder,
        }

        /// <summary>
        /// Überprüft ob ein Punkt in dem Polygon liegt.
        /// </summary>
        /// <param name="point">Der zu prüfende Punkt.</param>
        /// <param name="mode">Der Modus, wie die Rahmenlinie behandelt werden soll.</param>
        /// <returns><c>True</c>, wenn der Punkt im Polygon liegt, andernfalls <c>False</c>.</returns>
        public bool IsPointInPolygon(Point2D point, TouchTestMode mode)
        {
            if (mode == TouchTestMode.IncludeBorder && IsPointAffected(point))
                return true;
            if (mode == TouchTestMode.WithoutBorder && IsPointAffected(point))
                return false;
            bool c = false;
            for (int i = 0, j = this.Points.Count - 1; i < this.Points.Count; j = i++)
            {
                if (((this.Points[i].Y > point.Y) != (this.Points[j].Y > point.Y))
                  && (point.X < (this.Points[j].X - this.Points[i].X) * (point.Y - this.Points[i].Y) / (this.Points[j].Y - this.Points[i].Y) + this.Points[i].X))
                    c = !c;
            }
            return c;
        }

        /// <summary>
        /// Überprüft ob ein Punkt auf der Rahmenlinie liegt.
        /// </summary>
        /// <param name="point">Der zu prüfende Punkt.</param>
        /// <returns><c>True</c>, wenn der Punkt auf der Rahmenlinie des Polygons liegt, andernfalls <c>False</c>.</returns>
        public bool IsPointAffected(Point2D point)
        {
            if (this.Points.Contains(point))
                return true;

            for (int i = 0; i < this.Points.Count; ++i)
            {
                if (point.IsPointOnLineSegment(this.Points[i], this.Points[i + 1 == this.Points.Count ? 0 : i + 1]))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Ruft den Umfang des Polygons ab.
        /// </summary>
        public double Perimeter
        {
            get
            {
                double u = 0;
                for (int i = 0; i < this.Points.Count; ++i)
                {
                    //Abstand 2er Punkte über Satz des Pytagoras berechnen
                    u += Math.Sqrt(Math.Abs(this.Points[i].X - this.Points[i + 1 == this.Points.Count ? 0 : i + 1].X)
                                 + Math.Abs(this.Points[i].Y - this.Points[i + 1 == this.Points.Count ? 0 : i + 1].Y));
                }
                return u;
            }
        }

        /// <summary>
        /// Spiegelt das Polygon an der X-Achse.
        /// </summary>
        public void FlipX()
        {
            this.Points = Points.Select(point => new Point2D(-point.X, point.Y)).ToList();
        }
        /// <summary>
        /// Spiegelt das Polygon an der Y-Achse.
        /// </summary>
        public void FlipY()
        {
            this.Points = Points.Select(point => new Point2D(point.X, -point.Y)).ToList();
        }
        /// <summary>
        /// Spiegelt das Polygon an der X- und an der Y-Achse.
        /// </summary>
        public void FlipXY()
        {
            this.Points = Points.Select(point => new Point2D(-point.X, -point.Y)).ToList();
        }

        #region override

        /// <summary>
        /// Nur zu Debug-Zwecken.<para/>
        /// Liefert die Anzahl von Punkten im Polygon zurück.
        /// </summary>
        public override string ToString()
        {
            return "Points: " + this.Points.Count;
        }

        /// <summary>
        /// Ruft den gemeinsamen Hashcode der Punkte ab.
        /// </summary>
        /// <returns>Der gemeinsame Hashcode der Punkte des Polygons.</returns>
        public override int GetHashCode()
        {
            return this.Points.GetItemsHashCode();
        }

        /// <summary>
        /// Vergleicht ein Objekt mit dieser Instanz auf Gleichheit.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is Polygon2D && this.GetHashCode() == obj.GetHashCode();
        }

        #endregion

        /// <summary>
        /// Vergleicht 2 Polygone auf Gleichheit miteinander.
        /// </summary>
        /// <param name="p1">Das erste Vergleichspolygon.</param>
        /// <param name="p2">Das zweite Vergleichspolygon.</param>
        /// <returns><c>True</c>, wenn <paramref name="p1"/> und <paramref name="p2"/> die selben Punkte aufweisen. Andernfalls <c>False</c>.</returns>
        public static bool operator ==(Polygon2D p1, Polygon2D p2)
        {
            return p1.GetHashCode() == p2.GetHashCode();
        }

        /// <summary>
        /// Vergleicht 2 Polygone auf Ungleichheit miteinander.
        /// </summary>
        /// <param name="p1">Das erste Vergleichspolygon.</param>
        /// <param name="p2">Das zweite Vergleichspolygon.</param>
        /// <returns><c>True</c>, wenn <paramref name="p1"/> und <paramref name="p2"/> nicht die selben Punkte aufweisen. Andernfalls <c>False</c>.</returns>
        public static bool operator !=(Polygon2D p1, Polygon2D p2)
        {
            return p1.GetHashCode() != p2.GetHashCode();
        }

        #region IDrawable<Point2D> Member

        IEnumerable<IPoint<Point2D>> IDrawable<Point2D>.Points
        {
            get
            {
                return this.Points.Select(x => (IPoint<Point2D>)x).ToArray();
            }
        }

        #endregion
    }
}
