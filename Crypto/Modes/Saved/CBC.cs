﻿using System;
using VTDev.Projects.CEX.Crypto.Ciphers;

namespace VTDev.Projects.CEX.Crypto.Modes
{
    /// <summary>
    /// Implements Cipher Block Chaining (CBC) mode
    /// </summary>
    public class CBC : ICipherMode, IDisposable
    {
        #region Fields
        private IBlockCipher _blockCipher;
        private int _blockSize = 16;
        private byte[] _cbcIv;
        private byte[] _cbcNextIv;
        private bool _isDisposed = false;
        private bool _isEncryption = false;
        #endregion

        #region Properties
        /// <summary>
        /// Unit block size of internal cipher.
        /// </summary>
        public int BlockSize
        {
            get { return _blockCipher.BlockSize; }
        }

        /// <summary>
        /// Underlying Cipher
        /// </summary>
        public IBlockCipher Cipher
        {
            get { return _blockCipher; }
            set { _blockCipher = value; }
        }

        /// <summary>
        /// Used as encryptor, false for decryption. 
        /// </summary>
        public bool IsEncryption
        {
            get { return _isEncryption; }
            private set { _isEncryption = value; }
        }

        /// <summary>
        /// Cipher name
        /// </summary>
        public string Name
        {
            get { return "CBC"; }
        }

        /// <summary>
        /// Intitialization Vector
        /// </summary>
        public byte[] Vector
        {
            get { return _cbcIv; }
            set
            {
                if (value == null)
                    throw new ArgumentOutOfRangeException("Invalid IV! IV can not be null.");
                if (value.Length != this.BlockSize)
                    throw new ArgumentOutOfRangeException("Invalid IV size! Valid size must be equal to cipher blocksize.");

                int vectorSize = value.Length;
                _cbcIv = new byte[vectorSize];
                _cbcNextIv = new byte[vectorSize];

                Buffer.BlockCopy(value, 0, _cbcIv, 0, vectorSize);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initialize the Cipher
        /// </summary>
        /// <param name="Cipher">Underlying encryption algorithm</param>
        public CBC(IBlockCipher Cipher)
        {
            _blockCipher = Cipher;
            _blockSize = this.BlockSize;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the Cipher
        /// </summary>
        /// <param name="Encryption">Cipher is used for encryption, false to decrypt</param>
        /// <param name="KeyParam">KeyParam containing key and vector</param>
        public void Init(bool Encryption, KeyParams KeyParam)
        {
            if (KeyParam.Key == null)
                throw new ArgumentNullException("Key can not be null!");
            if (KeyParam.IV == null)
                throw new ArgumentNullException("IV can not be null!");

            _blockCipher.Init(Encryption, KeyParam);
            _blockSize = this.BlockSize;
            this.Vector = KeyParam.IV;
            this.IsEncryption = Encryption;
        }

        /// <summary>
        /// Transform a block of bytes.
        /// Init must be called before this method can be used.
        /// </summary>
        /// <param name="Input">Bytes to Encrypt/Decrypt</param>
        /// <param name="Output">Encrypted or Decrypted bytes</param>
        public void Transform(byte[] Input, byte[] Output)
        {
            if (this.IsEncryption)
                EncryptBlock(Input, Output);
            else
                DecryptBlock(Input, Output);
        }

        /// <summary>
        /// Transform a block of bytes within an array.
        /// Init must be called before this method can be used.
        /// </summary>
        /// <param name="Input">Bytes to Encrypt</param>
        /// <param name="InOffset">Offset with the Input array</param>
        /// <param name="Output">Encrypted bytes</param>
        /// <param name="OutOffset">Offset with the Output array</param>
        public void Transform(byte[] Input, int InOffset, byte[] Output, int OutOffset)
        {
            if (this.IsEncryption)
                EncryptBlock(Input, InOffset, Output, OutOffset);
            else
                DecryptBlock(Input, InOffset, Output, OutOffset);
        }

        /// <summary>
        /// Decrypt a single block of bytes.
        /// </summary>
        /// <param name="Input">Encrypted bytes</param>
        /// <param name="Output">Decrypted bytes</param>
        public void DecryptBlock(byte[] Input, byte[] Output)
        {
            // copy input to temp iv
            Buffer.BlockCopy(Input, 0, _cbcNextIv, 0, Input.Length);
            // decrypt input
            _blockCipher.DecryptBlock(Input, Output);
            // xor output and iv
            for (int i = 0; i < Input.Length; i++)
                Output[i] ^= _cbcIv[i];

            // copy forward iv
            Buffer.BlockCopy(_cbcNextIv, 0, _cbcIv, 0, _cbcIv.Length);
        }

        /// <summary>
        /// Decrypt a block of bytes within an array.
        /// </summary>
        /// <param name="Input">Encrypted bytes</param>
        /// <param name="InOffset">Offset with the Input array</param>
        /// <param name="Output">Decrypted bytes</param>
        /// <param name="OutOffset">Offset with the Output array</param>
        public void DecryptBlock(byte[] Input, int InOffset, byte[] Output, int OutOffset)
        {
            // copy input to temp iv
            Buffer.BlockCopy(Input, InOffset, _cbcNextIv, 0, _blockSize);
            // decrypt input
            _blockCipher.DecryptBlock(Input, InOffset, Output, OutOffset);
            // xor output and iv
            for (int i = 0; i < _blockSize; i++)
                Output[OutOffset + i] ^= _cbcIv[i];

            // copy forward iv
            Buffer.BlockCopy(_cbcNextIv, 0, _cbcIv, 0, _cbcIv.Length);
        }

        /// <summary>
        /// Encrypt a block of bytes.
        /// </summary>
        /// <param name="Input">Bytes to Encrypt</param>
        /// <param name="Output">Encrypted bytes</param>
        public void EncryptBlock(byte[] Input, byte[] Output)
        {
            // xor iv and input
            for (int i = 0; i < Input.Length; i++)
                _cbcIv[i] ^= Input[i];

            // encrypt iv
            _blockCipher.EncryptBlock(_cbcIv, Output);
            // copy output to iv
            Buffer.BlockCopy(Output, 0, _cbcIv, 0, _cbcIv.Length);
        }

        /// <summary>
        /// Encrypt a block of bytes within an array.
        /// </summary>
        /// <param name="Input">Bytes to Encrypt</param>
        /// <param name="InOffset">Offset with the Input array</param>
        /// <param name="Output">Encrypted bytes</param>
        /// <param name="OutOffset">Offset with the Output array</param>
        public void EncryptBlock(byte[] Input, int InOffset, byte[] Output, int OutOffset)
        {
            // xor iv and input
            for (int i = 0; i < _blockSize; i++)
                _cbcIv[i] ^= Input[InOffset + i];

            // encrypt iv
            _blockCipher.EncryptBlock(_cbcIv, 0, Output, OutOffset);
            // copy output to iv
            Buffer.BlockCopy(Output, OutOffset, _cbcIv, 0, _cbcIv.Length);
        }
        #endregion

        #region IDispose
        /// <summary>
        /// Dispose of this class and the underlying cipher
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool Disposing)
        {
            if (!_isDisposed && Disposing)
            {
                if (_blockCipher != null)
                    _blockCipher.Dispose();

                if (_cbcIv != null)
                {
                    Array.Clear(_cbcIv, 0, _cbcIv.Length);
                    _cbcIv = null;
                }
                if (_cbcNextIv != null)
                {
                    Array.Clear(_cbcNextIv, 0, _cbcNextIv.Length);
                    _cbcNextIv = null;
                }

                _isDisposed = true;
            }
        }
        #endregion
    }
}