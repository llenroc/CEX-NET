﻿#region Directives
using System;
using System.Security.Cryptography;
using VTDev.Libraries.CEXEngine.Crypto.Enumeration;
using VTDev.Libraries.CEXEngine.CryptoException;
#endregion

#region License Information
// The MIT License (MIT)
// 
// Copyright (c) 2016 vtdev.com
// This file is part of the CEX Cryptographic library.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// 
// Implementation Details:
// An implementation of a Cryptographically Secure Pseudo Random Number Generator (RCSP). 
// Uses the <a href="http://msdn.microsoft.com/en-us/library/system.security.cryptography.rngcryptoserviceprovider.aspx">RNGCryptoServiceProvider</a> class to produce pseudo random output.
// Written by John Underhill, January 6, 2014
// contact: develop@vtdev.com
#endregion

namespace VTDev.Libraries.CEXEngine.Crypto.Prng
{
    /// <summary>
    /// An implementation of a Cryptographically Secure PRNG using the RNGCryptoServiceProvider class
    /// </summary>
    /// 
    /// <example>
    /// <code>
    /// int x;
    /// using (IRandom rnd = new CSPPrng())
    ///     x = rnd.Next();
    /// </code>
    /// </example>
    /// 
    /// <remarks>
    /// <description>Guiding Publications::</description>
    /// <list type="number">
    /// <item><description>Microsoft <a href="http://msdn.microsoft.com/en-us/library/system.security.cryptography.rngcryptoserviceprovider.aspx">RNGCryptoServiceProvider</a>: class documentation.</description></item>
    /// <item><description>NIST <a href="http://csrc.nist.gov/publications/drafts/800-90/draft-sp800-90b.pdf">SP800-90B</a>: Recommendation for the Entropy Sources Used for Random Bit Generation.</description></item>
    /// <item><description>NIST <a href="http://csrc.nist.gov/publications/fips/fips140-2/fips1402.pdf">Fips 140-2</a>: Security Requirments For Cryptographic Modules.</description></item>
    /// <item><description>RFC <a href="http://www.ietf.org/rfc/rfc4086.txt">4086</a>: Randomness Requirements for Security.</description></item>
    /// </list> 
    /// </remarks>
    public sealed class CSPPrng : IRandom
    {
        #region Constants
        private const string ALG_NAME = "CSPPrng";
        #endregion

        #region Fields
        private bool m_isDisposed = false;
        private RNGCryptoServiceProvider m_rngCrypto;
        #endregion

        #region Properties
        /// <summary>
        /// Get: The prngs type name
        /// </summary>
        public Prngs Enumeral
        {
            get { return Prngs.CSPPrng; }
        }

        /// <summary>
        /// Algorithm name
        /// </summary>
        public string Name
        {
            get { return ALG_NAME; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initialize the class
        /// </summary>
        /// 
        /// <exception cref="CryptoRandomException">Thrown if RNGCryptoServiceProvider initialization failed</exception>
        public CSPPrng()
        {
            try
            {
                m_rngCrypto = new RNGCryptoServiceProvider();
            }
            catch (Exception ex)
            {
                if (m_rngCrypto == null)
                    throw new CryptoRandomException("CSPPrng:Ctor", "RNGCrypto could not be initialized!", ex);
            }
        }

        /// <summary>
        /// Finalize objects
        /// </summary>
        ~CSPPrng()
        {
            Dispose(false);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Fill an array with pseudo random bytes
        /// </summary>
        /// 
        /// <param name="Size">Size of requested byte array</param>
        /// 
        /// <returns>Random byte array</returns>
        public byte[] GetBytes(int Size)
        {
            byte[] data = new byte[Size];

            m_rngCrypto.GetBytes(data);

            return data;
        }

        /// <summary>
        /// Fill an array with pseudo random bytes
        /// </summary>
        /// 
        /// <param name="Output">Array to fill with random bytes</param>
        public void GetBytes(byte[] Output)
        {
            m_rngCrypto.GetBytes(Output);
        }

        /// <summary>
        /// Get a pseudo random 32bit integer
        /// </summary>
        /// 
        /// <returns>Random int</returns>
        public int Next()
        {
            return BitConverter.ToInt32(GetBytes(4), 0);
        }

        /// <summary>
        /// Get a ranged pseudo random 32bit integer
        /// </summary>
        /// 
        /// <param name="Maximum">Maximum value</param>
        /// 
        /// <returns>Random int</returns>
        public int Next(int Maximum)
        {
            int max = (Int32.MaxValue - (Int32.MaxValue % Maximum));
            int x;
            int ret;

            do
            {
                x = Next();
                ret = x % Maximum;
            }
            while (x >= max || ret < 0);

            return ret;
        }

        /// <summary>
        /// Get a ranged pseudo random 32bit integer
        /// </summary>
        /// 
        /// <param name="Minimum">Minimum value</param>
        /// <param name="Maximum">Maximum value</param>
        /// 
        /// <returns>Random int</returns>
        public int Next(int Minimum, int Maximum)
        {
            int range = (Maximum - Minimum + 1);
            int max = (Int32.MaxValue - (Int32.MaxValue % range));
            int x;
            int ret;

            do
            {
                x = Next();
                ret = x % range;
            }
            while (x >= max || ret < 0);

            return Minimum + ret;
        }

        /// <summary>
        /// Get a pseudo random 32bit integer
        /// </summary>
        /// 
        /// <returns>Random int</returns>
        public long NextLong()
        {
            return BitConverter.ToInt64(GetBytes(8), 0);
        }

        /// <summary>
        /// Get a ranged pseudo random 64bit integer
        /// </summary>
        /// 
        /// <param name="Maximum">Maximum value</param>
        /// 
        /// <returns>Random long</returns>
        public long NextLong(long Maximum)
        {
            long max = (Int64.MaxValue - (Int64.MaxValue % Maximum));
            long x;
            long ret;

            do
            {
                x = NextLong();
                ret = x % Maximum;
            }
            while (x >= max || ret < 0);

            return ret;
        }

        /// <summary>
        /// Get a ranged pseudo random 64bit integer
        /// </summary>
        /// 
        /// <param name="Minimum">Minimum value</param>
        /// <param name="Maximum">Maximum value</param>
        /// 
        /// <returns>Random long</returns>
        public long NextLong(long Minimum, long Maximum)
        {
            long range = (Maximum - Minimum + 1);
            long max = (Int64.MaxValue - (Int64.MaxValue % range));
            long x;
            long ret;

            do
            {
                x = NextLong();
                ret = x % range;
            }
            while (x >= max || ret < 0);

            return Minimum + ret;
        }

        /// <summary>
        /// Reset the RNGCryptoServiceProvider instance
        /// </summary>
        public void Reset()
        {
            if (m_rngCrypto != null)
            {
                m_rngCrypto.Dispose();
                m_rngCrypto = null;
            }

            m_rngCrypto = new RNGCryptoServiceProvider();
        }
        #endregion

        #region IDispose
        /// <summary>
        /// Dispose of this class
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool Disposing)
        {
            if (!m_isDisposed && Disposing)
            {
                try
                {
                    if (m_rngCrypto != null)
                    {
                        m_rngCrypto.Dispose();
                        m_rngCrypto = null;
                    }
                }
                catch { }

                m_isDisposed = true;
            }
        }
        #endregion
    }
}
