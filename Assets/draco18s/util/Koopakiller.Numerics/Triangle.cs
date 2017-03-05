//! Created by Tom Lambert alias Koopakiller
//! Project started: 2011
//! License: Microsoft Reciprocal License (Ms-RL)
//! Project site: https://numerics.codeplex.com/license

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Koopakiller.Numerics.Resources;

namespace Koopakiller.Numerics
{
    /// <summary>
    /// Stellt ein Dreieck dar.
    /// </summary>
    /// <typeparam name="T">Ein Typ, mit dem die Koordinaten der Eckpunkte dargestellt werden können.</typeparam>
    public class Triangle<T> : INotifyPropertyChanged, IDrawable<T> where T : IPoint<T>, new()
    {
        #region .ctor

        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="Triangle&lt;T&gt;"/>-Klasse.
        /// </summary>
        /// <param name="pA">Die Koordinaten des Eckpunktes A.</param>
        /// <param name="pB">Die Koordinaten des Eckpunktes B.</param>
        /// <param name="pC">Die Koordinaten des Eckpunktes C.</param>
        public Triangle(T pA, T pB, T pC)
        {
            this._PointA = pA;
            this._PointB = pB;
            this._PointC = pC;
        }

        #endregion

        #region Felder

        private T _PointA;
        private T _PointB;
        private T _PointC;
        private AngleUnits _AngleUnit = AngleUnits.Radian;

        #endregion

        #region Eigenschaften

        //Punkte
        /// <summary>
        /// Ruft die Koordinaten des Eckpunktes A ab oder legt diese fest.
        /// </summary>
        public T PointA
        {
            get
            {
                return this._PointA;
            }
            set
            {
                if (value.Equals((T)this._PointB) || value.Equals((T)this._PointC))
                    throw new ArgumentException(ResourceManager.GetMessage("Arg_CantSame", "value", "PointB / PointC"));
                if (!this._PointA.Equals(value))
                {
                    this._PointA = value;
                    OnPropertyChanged("_PointA");
                    OnDependendedPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Ruft die Koordinaten des Eckpunktes B ab oder legt diese fest.
        /// </summary>
        public T PointB
        {
            get
            {
                return this._PointB;
            }
            set
            {
                if (value.Equals((T)this._PointA) || value.Equals((T)this._PointC))
                    throw new ArgumentException(ResourceManager.GetMessage("Arg_CantSame", "value", "PointA / PointC"));
                if (!this._PointB.Equals(value))
                {
                    this._PointB = value;
                    OnPropertyChanged("_PointB");
                    OnDependendedPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Ruft die Koordinaten des Eckpunktes C ab oder legt diese fest.
        /// </summary>
        public T PointC
        {
            get
            {
                return this._PointC;
            }
            set
            {
                if (value.Equals((T)this._PointA) || value.Equals((T)this._PointB))
                    throw new ArgumentException(ResourceManager.GetMessage("Arg_CantSame", "value", "PointA / PointB"));
                if (!this._PointC.Equals(value))
                {
                    this._PointC = value;
                    OnPropertyChanged("_PointC");
                    OnDependendedPropertyChanged();
                }
            }
        }

        //Satz des Pytagoras
        /// <summary>
        /// Ruft die Länge der Seite a ab.
        /// </summary>
        public double SideA
        {
            get
            {
                return this.PointB.Distance(this.PointC);
            }
        }
        /// <summary>
        /// Ruft die Länge der Seite b ab.
        /// </summary>
        public double SideB
        {
            get
            {
                return this.PointC.Distance(this.PointA);
            }
        }
        /// <summary>
        /// Ruft die Länge der Seite c ab.
        /// </summary>
        public double SideC
        {
            get
            {
                return this.PointA.Distance(this.PointB);
            }
        }

        //Cosinussatz
        /// <summary>
        /// Ruft die Größe des Alpha-Winkels in der in <see cref="AngleUnit"/> gespeicherten Größe ab.
        /// </summary>
        public double AngleA
        {
            get
            {
                return Calculator.Acos((this.SideB * this.SideB + this.SideC * this.SideC - this.SideA * this.SideA) / (2 * this.SideB * this.SideC), this.AngleUnit);
            }
        }
        /// <summary>
        /// Ruft die Größe des Beta-Winkels in der in <see cref="AngleUnit"/> gespeicherten Größe ab.
        /// </summary>
        public double AngleB
        {
            get
            {
                return Calculator.Acos((this.SideA * this.SideA + this.SideC * this.SideC - this.SideB * this.SideB) / (2 * this.SideA * this.SideC), this.AngleUnit);
            }
        }
        /// <summary>
        /// Ruft die Größe des Gamma-Winkels in der in <see cref="AngleUnit"/> gespeicherten Größe ab.
        /// </summary>
        public double AngleC
        {
            get
            {
                return Calculator.Acos((this.SideA * this.SideA + this.SideB * this.SideB - this.SideC * this.SideC) / (2 * this.SideA * this.SideB), this.AngleUnit);
            }
        }

        //Satz des Heron
        /// <summary>
        /// Ruft den Flächeninhalt des Dreiecks ab.
        /// </summary>
        public double Area
        {
            get
            {
                double s = this.Perimeter / 2;
                return Calculator.Sqrt(s * (s - this.SideA) * (s - this.SideB) * (s - this.SideC));
            }
        }

        //Satz des Pytagoras
        /// <summary>
        /// Ruft den Umfang des Dreiecks ab.
        /// </summary>
        public double Perimeter
        {
            get
            {
                return this.PointA.Distance(this.PointB)
                     + this.PointB.Distance(this.PointC)
                     + this.PointC.Distance(this.PointA);
            }
        }

        //Höhen
        //A = 0,5 * g * h_g
        /// <summary>
        /// Ruft die Länge der Höhe zur Seite A ab.
        /// </summary>
        public double HeightA
        {
            get
            {
                return this.Area * 2 / this.SideA;
            }
        }
        /// <summary>
        /// Ruft die Länge der Höhe zur Seite b ab.
        /// </summary>
        public double HeightB
        {
            get
            {
                return this.Area * 2 / this.SideB;
            }
        }
        /// <summary>
        /// Ruft die Länge der Höhe zur Seite c ab.
        /// </summary>
        public double HeightC
        {
            get
            {
                return this.Area * 2 / this.SideC;
            }
        }

        //Seitenhalbierende
        /// <summary>
        /// Ruft die Länge der Seitenhalbierenden zur Seite a ab.
        /// </summary>
        public double MedianA
        {
            get
            {
                return Calculator.Sqrt(2 * (this.SideB * this.SideB + this.SideC * this.SideC) - this.SideA * this.SideA) / 2;
            }
        }
        /// <summary>
        /// Ruft die Länge der Seitenhalbierenden zur Seite b ab.
        /// </summary>
        public double MedianB
        {
            get
            {
                return Calculator.Sqrt(2 * (this.SideC * this.SideC + this.SideA * this.SideA) - this.SideB * this.SideB) / 2;
            }
        }
        /// <summary>
        /// Ruft die Länge der Seitenhalbierenden zur Seite c ab.
        /// </summary>
        public double MedianC
        {
            get
            {
                return Calculator.Sqrt(2 * (this.SideA * this.SideA + this.SideB * this.SideB) - this.SideC * this.SideC) / 2;
            }
        }

        //Dreiecksart
        /// <summary>
        /// Ruft einen Wert ab, der angibt ob das Dreieck rechtwinklig ist.
        /// </summary>
        public bool IsRightTriangle
        {
            get
            {
                return (((this.AngleA - 90 < 0.000000001 && this.AngleA - 90 > -0.000000001) || (this.AngleB - 90 < 0.000000001 && this.AngleB - 90 > -0.000000001) || (this.AngleC - 90 < 0.000000001 && this.AngleC - 90 > -0.000000001)) && this.AngleUnit == AngleUnits.Degree)
                    || (((this.AngleA - 100 < 0.000000001 && this.AngleA - 100 > -0.000000001) || (this.AngleB - 100 < 0.000000001 && this.AngleB - 100 > -0.000000001) || (this.AngleC - 100 < 0.000000001 && this.AngleC - 100 > -0.000000001)) && this.AngleUnit == AngleUnits.Gradian)
                    || (((this.AngleA - Math.PI / 2 < 0.000000001 && this.AngleA - Math.PI / 2 > -0.000000001) || (this.AngleB - Math.PI / 2 < 0.000000001 && this.AngleB - Math.PI / 2 > -0.000000001) || (this.AngleC - Math.PI / 2 < 0.000000001 && this.AngleC - Math.PI / 2 > -0.000000001)) && this.AngleUnit == AngleUnits.Radian);
                //? Nachfolgende Version weißt zu viele Ungenauigkeiten auf. Leider.
                //return ((this.AngleA == 90 || this.AngleB == 90 || this.AngleC == 90) && this.AngleUnit == AngleUnits.Degree)
                //    || ((this.AngleA == 100 || this.AngleB == 100 || this.AngleC == 100) && this.AngleUnit == AngleUnits.Gradian)
                //    || ((this.AngleA == Math.PI / 2 || this.AngleB == Math.PI / 2 || this.AngleC == Math.PI / 2) && this.AngleUnit == AngleUnits.Radian);
            }
        }
        /// <summary>
        /// Ruft einen Wert ab, der angibt ob das Dreieck gleichseitig ist.
        /// </summary>
        public bool IsEquilateralTriangle
        {
            get
            {
                return this.SideA == this.SideB && this.SideB == this.SideC;
            }
        }
        /// <summary>
        /// Ruft einen Wert ab, der angibt ob das Dreieck gleichschenklig ist.
        /// </summary>
        public bool IsIsosceles
        {
            get
            {
                return this.SideA == this.SideB || this.SideB == this.SideC || this.SideC == this.SideA;
            }
        }

        /// <summary>
        /// Ruft die Dimension ab, in der das Dreieck dargestellt wird.
        /// </summary>
        public int Dimension
        {
            get
            {
                return this.PointA.Dimension;
            }
        }

        /// <summary>
        /// Ruft den Radius des Inkreises ab.
        /// </summary>
        public double RadiusOfInCircle
        {
            get
            {
                // s·tan(α)·tan(β)·tan(γ) 
                return (this.Perimeter / 2) * Calculator.Tan(this.AngleA, this.AngleUnit) * Calculator.Tan(this.AngleB, this.AngleUnit) * Calculator.Tan(this.AngleC, this.AngleUnit);
            }
        }
        /// <summary>
        /// Ruft den Radius des Umkreises ab.
        /// </summary>
        public double RadiusOfCircumscribedCircle
        {
            get
            {
                return this.SideA / (2 * Calculator.Sin(this.AngleA, this.AngleUnit));
            }
        }

        /// <summary>
        /// Ruft das Zentrum des Inkreises ab.
        /// </summary>
        public T CenterOfInCircle
        {
            get
            {
                double[] d1 = this._PointA.Coordinates;
                double[] d2 = this._PointA.Coordinates;
                double[] d3 = this._PointA.Coordinates;

                double[] result = new double[d1.Length];
                for (int i = 0; i < this.Dimension; ++i)
                {
                    result[i] = (this.SideA * d1[i] + this.SideB * d2[i] + this.SideC * d3[i]) / this.Perimeter;
                }
                T t = new T();
                t.Coordinates = result;
                return t;
            }
        }

        /// <summary>
        /// Ruft das Zentrum des Umkreises ab.
        /// </summary>
        public T CenterOfCircumscribedCircle
        {
            get
            {
                //Baryzentrische Koordinaten Berechnen:
                double u = Calculator.Sin(this.AngleA, this.AngleUnit);
                double v = Calculator.Sin(this.AngleB, this.AngleUnit);
                double w = Calculator.Sin(this.AngleC, this.AngleUnit);

                double[] d1 = this._PointA.Coordinates;
                double[] d2 = this._PointA.Coordinates;
                double[] d3 = this._PointA.Coordinates;

                //In kartesische Koordinaten umrechnen
                double[] result = new double[d1.Length];
                for (int i = 0; i < this.Dimension; ++i)
                {
                    result[i] = (u * d1[i] + v * d2[i] + w * d3[i]) / (u + v + w);
                }
                T t = new T();
                t.Coordinates=result;
                return t;
            }
        }

        //Flächenschwerpunkt
        /// <summary>
        /// Ruft den Flächenschwerpunkt (Schnittpunkt der Seitenhalbierenden) des Dreiecks ab.
        /// </summary>
        public T Centroid
        {
            get
            {
                double[] result = new double[this.PointA.Dimension];
                for(int i=0;i<this.PointA .Coordinates .Length ;++i)
                {
                    result[i] = (1 / 3) * (this.PointA.Coordinates[i] + this.PointB.Coordinates[i] + this.PointC.Coordinates[i]);
                }
                T t = new T();
                t.Coordinates = result;
                return t;
            }
        }

        /// <summary>
        /// Ruft die für Winkel zu verwendende Einheit ab oder legt diese fest.
        /// </summary>
        public AngleUnits AngleUnit
        {
            get
            {
                return this._AngleUnit;
            }
            set
            {
                if (this._AngleUnit != value)
                {
                    this._AngleUnit = value;
                    OnPropertyChanged("AngleUnit");
                    OnPropertyChanged("AngleA");
                    OnPropertyChanged("AngleB");
                    OnPropertyChanged("AngleC");
                }
            }
        }

        #region INotifyPropertyChanged Member

        /// <summary>
        /// Wird aufgerufen, wenn nicht setzbare Eigenschaften, welche von den Eckpunkten abhängig sind, sich ändern könnten.
        /// </summary>
        protected virtual void OnDependendedPropertyChanged()
        {
            OnPropertyChanged("HeightA");
            OnPropertyChanged("HeightB");
            OnPropertyChanged("HeightC");
            OnPropertyChanged("AngleA");
            OnPropertyChanged("AngleB");
            OnPropertyChanged("AngleC");
            OnPropertyChanged("SideA");
            OnPropertyChanged("SideB");
            OnPropertyChanged("SideC");
            OnPropertyChanged("Perimeter");
            OnPropertyChanged("Area");
            OnPropertyChanged("IsRightTriangle");
            OnPropertyChanged("IsEquilateralTriangle");
            OnPropertyChanged("IsIsosceles");
            OnPropertyChanged("RadiusOfInnerCircle");
            OnPropertyChanged("RadiusOfOuterCircle");
            OnPropertyChanged("CenterOfInnerCircle");
            OnPropertyChanged("CenterOfOuterCircle");
        }

        /// <summary>
        /// Wird aufgerufen, wenn sich eine spezifische Eigenschaft ändert.
        /// </summary>
        /// <param name="propertyName">Der Name der geänderten Eigenschaft.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var @event = this.PropertyChanged;
            if (@event != null)
                @event(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Wird ausgelöst, wenn sich ein Eigenschaftswert ändert.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #endregion

        #region Schnittstellen

        #region IDrawable<T> Member

        /// <summary>
        /// Ruft die Eckpunkte des Dreiecks ab.
        /// </summary>
        public IEnumerable<IPoint<T>> Points
        {
            get
            {
                yield return (IPoint<T>)this._PointA;
                yield return (IPoint<T>)this._PointB;
                yield return (IPoint<T>)this._PointC;
                //return new IPoint<T>[] { this._PointA, this._PointB, this._PointC };
            }
        }

        #endregion

        #endregion
    }
}
