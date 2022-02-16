using System;

namespace Thismaker.Esna
{
    /// <summary>
    /// Exception thrown when trying to read or update an item that does not exist in an <see cref="IContainerHandler{TModel, TStorage}"/>.
    /// </summary>
    [Serializable]
    public class ResourceNotFoundException : Exception
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ResourceNotFoundException"/>.
        /// </summary>
        public ResourceNotFoundException() { }
    }

    /// <summary>
    /// Exception thrown when trying to create an item in an <see cref="IContainerHandler{TModel, TStorage}"/> while an item with the same ID already exists.
    /// </summary>
    [Serializable]
    public class ResourceConflictException : Exception
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ResourceConflictException"/>.
        /// </summary>
        public ResourceConflictException() { }
    }
}
