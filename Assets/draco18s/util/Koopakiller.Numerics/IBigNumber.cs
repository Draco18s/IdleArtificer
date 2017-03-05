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
    /// Muss implementiert werden, wenn es sich bei dem Typ um eine Zahl handelt.
    /// </summary>
    public interface IBigNumber<T>
    {
        /// <summary>
        /// Analysiert eine Zeichenfolge und weißt der Instanz den dargestellten Wert zu.
        /// </summary>
        /// <param name="number">Die zu analysierende Zeichenfolge.</param>
        void ParseString(string number);
        /// <summary>
        /// Analysiert eine Zeichenfolge, welche im linearen Format einer Formel von Microsoft Word dargestellt ist, und weißt der Instanz den dargestellten Wert zu.
        /// </summary>
        /// <param name="number">Die zu analysierende Zeichenfolge.</param>
        void ParseMSWord(string number);
    }
}
