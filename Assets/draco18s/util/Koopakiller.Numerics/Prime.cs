//! Created by Tom Lambert alias Koopakiller
//! Project started: 2011
//! License: Microsoft Reciprocal License (Ms-RL)
//! Project site: https://numerics.codeplex.com/license

//! Created by Tom Lambert alias Koopakiller
//! Project started: 2011
//! License: Microsoft Reciprocal License (Ms-RL)
//! Project site: https://numerics.codeplex.com/license

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Koopakiller.Numerics.Resources;

namespace Koopakiller.Numerics
{
    /// <summary>
    /// Stellt Berechnungsalgorithmen für Primzahlen bereit.
    /// </summary>
    public class Prime : IEnumerable<BigInteger>
    {
        /// <summary>
        /// Erstellt eine neue Instanz der Prime-Klasse. Die Liste wird nicht automatisch Erweitert und enthält zu Beginn keine Elemente.
        /// </summary>
        public Prime() : this(false, 0) { }
        /// <summary>
        /// Erstellt eine neue Instanz der Prime-Kasse mit der angegebenen Anzahl an Primzahlen.
        /// </summary>
        /// <param name="count">Die Anzahl zu erzeugender Primzahlen.</param>
        public Prime(int count) : this(false, count) { }
        /// <summary>
        /// Erstellt eine neue Instanz der Prime-Klasse und setzt die AutoExtend-Eigenschaft.
        /// </summary>
        /// <param name="autoExtend">Bestimmt, ob die Liste automatisch erweitert werden soll, wenn eine Primzahl abgefragt wird, die noch nicht gefunden wurde.</param>
        public Prime(bool autoExtend) : this(autoExtend, 0) { }
        /// <summary>
        /// Erstellt eine neue Instanz der Prime-Klasse mit der angegebenen Anzahl an Primzahlen und dem setzen der AutoExtend-Eigenschaft.
        /// </summary>
        /// <param name="autoExtend">Bestimmt, ob die Liste automatisch erweitert werden soll, wenn eine Primzahl abgefragt wird, die noch nicht gefunden wurde.</param>
        /// <param name="count">Die Anzahl zu erzeugender Primzahlen.</param>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Prime(bool autoExtend, int count)
        {
            InitList(count);
            this.AutoExtend = autoExtend;
        }

        /// <summary>
        /// Initialisiert die Liste mit der angegebenen Anzahl an öffentlich zugänglichen Primzahlen.
        /// </summary>
        /// <param name="count">Die Anzahl von öffentlichen Primzahlen, welche generiert werden sollen.</param>
        protected void InitList(int count)
        {
            this._Primes = new List<BigInteger>();
            this._PublicPrimes = new List<BigInteger>();
            ExtendList(count);
        }

        /// <summary>
        /// Überprüft ob eine Zahl eine Primzahl ist.
        /// </summary>
        /// <param name="num">Die zu prüfende Zahl.</param>
        
        public static bool IsPrime(BigInteger num)
        {
            if (num < 2) return false;
            if (num == 2)  return true;
            if (num % 2 == 0) return false;
            for (BigInteger i = 3; i < num; i += 2)
            {
                if (num % i == 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Überprüft ob eine Zahl eine Primzahl in der Liste ist oder nicht. Ist die Liste zu klein, wird diese erweitert.
        /// </summary>
        /// <param name="num">Die zu prüfende Zahl.</param>
        
        public bool IsPrimeExtendList(ulong num)
        {
            if (this._PublicPrimes.Contains(num))
                return true;
            while (this._PublicPrimes.Last() < num)
            {
              this.  ExtendList(this._PublicPrimes.Count+1);
                if (this._PublicPrimes[this._PublicPrimes.Count-1] == num)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Erweitert die Liste von Primzahlen auf <paramref name="to"/> Zahlen.
        /// </summary>
        /// <param name="to">Die Anzahl der Primzahlen, welche in der Liste stehen sollen. Wenn die Anzahl größer als <paramref name="to"/> ist, wird das ignoriert.</param>
        public void ExtendList(int to)
        {
            if (to == 0)
                return;
            if (to < 0)
                throw new ArgumentOutOfRangeException("to", ResourceManager.GetMessage("ArgOutRang_ParamMustGreaterEqual", "to (" + to + ")", "0"));

            //  int currentCount = this._PublicPrimes.Count;

            if (/*to >= 1 &&*/ this._Primes.Count == 0)
            {
                this._Primes.Add(2);
                if (this.OnCheckNumber(2))
                    this._PublicPrimes.Add(2);
            }
            if (to >= 2 && this._Primes.Count == 1)
            {
                this._Primes.Add(3L);
                if (this.OnCheckNumber(3L))
                    this._PublicPrimes.Add(3);
            }

            for (BigInteger i = this._Primes.Last() + 2; this._PublicPrimes.Count/* - currentCount */< to; i += 2)
            {
                bool ok = true;
                for (int x = 0; x < this._Primes.Count; ++x)
                {
                    if ((double)i % (double)this._Primes[x] == 0)
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                {
                    this._Primes.Add(i);
                    if (this.OnCheckNumber(i))
                        this._PublicPrimes.Add(i);
                }
            }
        }

        /// <summary>
        /// Überprüft ob eine Primzahl in die Liste der öffentlichen Primzahlen aufgenommen werden soll. 
        /// </summary>
        /// <param name="num">Die zu prüfende Primzahl.</param>
        /// <returns><c>True</c>, wenn die Primzahl öffentlich sein soll, andernfalls <c>False</c>.</returns>
        
        protected virtual bool OnCheckNumber(BigInteger num)
        {
            return true;
        }

        /// <summary>
        /// Ruft einen Wert, der angibt ob die Liste automatisch erweitert werden soll, wenn eine Primzahl abgefragt wird, die noch nicht gefunden wurde, oder legt diesen fest.
        /// </summary>
        public bool AutoExtend { get; set; }

        /// <summary>
        /// Die Liste aller geneierten Primzahlen.
        /// </summary>
        protected List<BigInteger> _Primes { get;private set; }
        /// <summary>
        /// Die Liste aller öffentlich zugänglichen Primzahlen.
        /// </summary>
        
        protected List<BigInteger> _PublicPrimes { get;private set; }

        /// <summary>
        /// Ruft die Primzahl an der angegebenen Position ab. Der Index ist Nullbasiert.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        
        public BigInteger this[int index]
        {
            get
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException("index", ResourceManager.GetMessage("ArgOutRang_ParamMustGreaterEqual", "index (" + index + ")", "0"));
                if (index >= this._PublicPrimes.Count)
                {
                    if (this.AutoExtend)
                        ExtendList(index + 1);
                    else
                        throw new ArgumentOutOfRangeException("index", ResourceManager.GetMessage("ArgOutRang_PrimeSetUpAutoExtendTrue"));
                }
                return this._PublicPrimes[index];
            }
        }

        #region IEnumerable<BigInteger> Member

        /// <summary>
        /// Ruft einen Enumerator für die gefundenen Primzahlen ab.
        /// </summary>
        /// <returns>Eine Enuemrator, mit dessen Hilfe die bisher gefundenen Primzahlen durchiteriert werdne können.</returns>
        public IEnumerator<BigInteger> GetEnumerator()
        {
            return this._Primes.GetEnumerator();
        }

        #endregion

        #region IEnumerable Member

        /// <summary>
        /// Ruft einen Enumerator für die gefundenen Primzahlen ab.
        /// </summary>
        /// <returns>Eine Enuemrator, mit dessen Hilfe die bisher gefundenen Primzahlen durchiteriert werdne können.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._Primes.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Ruft die Anzahl an gefundenen Primzahlen ab oder legt die Größe fest. Wenn die festgelegte Größe kleiner als die der derzeitigen Liste ist, wird das Ignoriert.
        /// </summary>
        public int Count
        {
            get
            {
                return this._Primes.Count;
            }
            set
            {
                this.ExtendList(value);
            }
        }
    }
}
