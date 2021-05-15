using System;
using System.Text;

namespace Thismaker.Horus.Symmetrics
{
    public class SymmetricBuilder
    {
        private int _keySize=256;
        private int _blockSize = 128;
        private string _salt;
        private int _iterations = 1000;
        private EncryptionAlgorithm _algorithm = EncryptionAlgorithm.AES;

        /// <summary>
        /// Provides the algorithm to be used. Otherwise it will be <see cref="EncryptionAlgorithm.AES"/>
        /// </summary>
        /// <param name="algorithm">The algorthm</param>
        public SymmetricBuilder WithAlgorithm(EncryptionAlgorithm algorithm)
        {
            _algorithm = algorithm;
            return this;
        }

        /// <summary>
        /// If provided, will set the size of the key to be usd. Otherwise 256.
        /// </summary>
        /// <param name="size"></param>
        public SymmetricBuilder WithKeySize(int size)
        {
            _keySize = size;
            return this;
        }

        /// <summary>
        /// If provided, will set the blocks, otherwise 128.
        /// </summary>
        /// <param name="size">Size of each block</param>
        public SymmetricBuilder WithBlockSize(int size)
        {
            _blockSize = size;
            return this;
        }

        /// <summary>
        /// If provided, will set the salt to be used
        /// when deriving password bytes.
        /// Otherwise, a random ASCII salt will be created.
        /// </summary>
        /// <param name="salt">The salt to be used. Should be 16 characters long</param>
        public SymmetricBuilder WithSalt(string salt)
        {
            _salt = salt;
            return this;
        }

        /// <summary>
        /// If provided, will set the number of times the password deriver will work.
        /// Otherwise it will be run 1000 times.
        /// </summary>
        /// <param name="count"></param>
        public SymmetricBuilder WithIterations(int count)
        {
            _iterations = count;
            return this;
        }

        /// <summary>
        /// Builds a <see cref="Symmetric"/> with the set configuration.
        /// </summary>
        public Symmetric Build() 
        {
            if (string.IsNullOrEmpty(_salt))
            {
                var alpha = GetASCIICharacters();

                var builder = new StringBuilder();

                for(int i = 0; i < 16; i++)
                {
                    var index= Horus.Random(0, alpha.Length);
                    builder.Append(alpha[index]);
                }

                _salt = builder.ToString();
            }

            return new Symmetric
            {
                BlockSize = _blockSize,
                Algorithm = _algorithm,
                Iterations = _iterations,
                KeySize = _keySize,
                Salt = _salt
            };
        }

        protected static string GetASCIICharacters()
        {
            StringBuilder alpha = new StringBuilder();
            for (int i = 0x20; i <= 0x7e; i++)
            {
                char c = Convert.ToChar(i);
                alpha.Append(c);
            }

            return alpha.ToString();
        }
    }
}
