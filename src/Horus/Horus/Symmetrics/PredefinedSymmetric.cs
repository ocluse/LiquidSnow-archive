namespace Thismaker.Horus.Symmetrics
{
    public static class PredefinedSymmetric
    {
        /// <summary>
        /// Creates a random <see cref="Symmetric{TAlg}.Salt"/> every time.
        /// The salt must be saved for the sake of a future operation.
        /// </summary>
        public static Symmetric AesRandom
        {
            get => new SymmetricBuilder()
                .WithAlgorithm(EncryptionAlgorithm.AES)
                .Build();
        }

        /// <summary>
        /// Uses a hardcorded salt. It is unadvisable to use this, though the salt
        /// need not be saved, merely calling the function will suffice.
        /// </summary>
        public static Symmetric AesFixed
        {
            get => new SymmetricBuilder()
                .WithAlgorithm(EncryptionAlgorithm.AES)
                .WithSalt("aselrias38490a32")
                .Build();
        }

        /// <summary>
        /// Creates a random <see cref="Symmetric{TAlg}.Salt"/> every time.
        /// The salt must be saved for the sake of decryption
        /// </summary>
        public static Symmetric RijndaelRandom
        {
            get => new SymmetricBuilder()
                .WithAlgorithm(EncryptionAlgorithm.Rijndael)
                .Build();
        }

        /// <summary>
        /// Uses a hardcorded salt. It is unadvisable to use this, though the salt
        /// need not be saved, merely calling the function will suffice.
        /// </summary>
        public static Symmetric RijndaelFixed
        {
            get => new SymmetricBuilder()
                .WithAlgorithm(EncryptionAlgorithm.Rijndael)
                .WithSalt("aselrias38490a32")
                .Build();
        }
    }
}
