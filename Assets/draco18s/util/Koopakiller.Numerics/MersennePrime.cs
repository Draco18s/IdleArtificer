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
    /// [Veraltet] Diese Klasse ist in der Lage folgende Mersenne-Primzahlen zu ermitteln:
    /// <list type="System.Long">
    /// <item>3</item>
    /// <item>7</item>
    /// <item>31</item>
    /// <item>127</item>
    /// <item>8191</item>
    /// <item>131071</item>
    /// <item>524287</item>
    /// <item>2147483647</item>
    /// <item>2305843009213693951</item>
    /// </list>
    /// </summary>
    [Obsolete("Use a static list.")]
    public class MersennePrime : Prime
    {
        /// <summary>
        /// Erstellt eine neue Instanz der MersennePrime-Klasse. Die Liste wird nicht automatisch Erweitert und enthält zu Beginn keine Elemente.
        /// </summary>
        public MersennePrime() : base() { }
        /// <summary>
        /// Erstellt eine neue Instanz der MersennePrime-Kasse mit der angegebenen Anzahl an Mersenne-Primzahlen.
        /// </summary>
        /// <param name="count">Die Anzahl zu erzeugender Mersenne-Primzahlen.</param>
        public MersennePrime(int count) : base(count) { }
        /// <summary>
        /// Erstellt eine neue Instanz der MersennePrime-Klasse und setzt die AutoExtend-Eigenschaft.
        /// </summary>
        /// <param name="autoExtend">Bestimmt, ob die Liste automatisch erweitert werden soll, wenn eine Mersenne-Primzahl abgefragt wird, die noch nicht gefunden wurde.</param>
        public MersennePrime(bool autoExtend) : base(autoExtend) { }
        /// <summary>
        /// Erstellt eine neue Instanz der MersennePrime-Klasse mit der angegebenen Anzahl an Mersenne-Primzahlen und dem setzen der AutoExtend-Eigenschaft.
        /// </summary>
        /// <param name="autoExtend">Bestimmt, ob die Liste automatisch erweitert werden soll, wenn eine Mersenne-Primzahl abgefragt wird, die noch nicht gefunden wurde.</param>
        /// <param name="count">Die Anzahl zu erzeugender Mersenne-Primzahlen.</param>
        public MersennePrime(bool autoExtend, int count) : base(autoExtend, count) { }

        /// <summary>
        /// überprüft ob eine Primzahl in die Öffentliche Liste passt.
        /// </summary>
        /// <param name="num">Die zu prüfende Primzahl.</param>
        /// <returns><c>True</c>, wenn die Primzahl eine Mersenne-Primzahl ist, andernfalls <c>False</c>.</returns>
        
        protected override bool OnCheckNumber(BigInteger num)
        {
            double d = Math.Log((ulong)num + 1, 2);
            return d == (ulong)d;
        }
    }
}
