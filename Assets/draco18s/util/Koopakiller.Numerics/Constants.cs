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
    /// Stellt eine Reihe von Konstanten mit einer doppelten Geneuigkeit bereit.
    /// </summary>
   public class Constants:IConstant<double>
    {
        #region IConstant<double> Member

        /// <summary>
        /// Ruft die Kreiszahl π mit doppelter Genauigkeit ab.
        /// </summary>
        public double PI
        {
            get
            {
                return Math.PI;
            }
        }

        /// <summary>
        /// Ruft die Basis des natürlichen Logarithmus e mit doppelter Genauigkeit ab.
        /// </summary>
        public double E
        {
            get
            {
                return Math.E;
            }
        }

        #endregion
    }
}
