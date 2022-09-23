using System;
using System.Reflection;

namespace Thismaker.Esna
{
    /// <summary>
    /// Contains settings used by an <see cref="IContainerHandler{TModel, TStorage}"/>
    /// </summary>
    /// <typeparam name="TModel">The usage form of the data</typeparam>
    /// <typeparam name="TStorage">The storage form of the data</typeparam>
    public class ContainerSettings<TModel, TStorage> 
    {
        private string _idPropertyName;
        private string _pkPropertyName;
        private PropertyInfo _idProperty;
        private PropertyInfo _pkProperty;
        private Func<TModel, ConvertArgs, TStorage> _convertToStorage;
        private Func<TStorage, TModel> _convertToModel;

        /// <summary>
        /// Gets or sets the function that is called to convert the <typeparamref name="TModel"/> to the <typeparamref name="TStorage"/>.
        /// </summary>
        /// <remarks>
        /// When no function is provided and it is null, the <typeparamref name="TModel"/> and <typeparamref name="TStorage"/> are considered to be the same.
        /// This means that attempting to convert the model to the storage will return the same object.
        /// </remarks>
        public Func<TModel, ConvertArgs, TStorage> ConvertToStorage
        {
            get
            {
                if (_convertToStorage == null)
                {
                    return (o,e) => (TStorage)(object)o;
                }
                return _convertToStorage;
            }
            set => _convertToStorage = value;
        }

        /// <summary>
        /// Gets or sets the function that is called to convert the <typeparamref name="TStorage"/> to the <typeparamref name="TModel"/>
        /// </summary>
        /// <remarks>
        /// When no function is provided and the property is null, the <typeparamref name="TModel"/> and <typeparamref name="TStorage"/> are considered to be the same.
        /// This means that attempting to convert the storage to the model will return the same object
        /// </remarks>
        public Func<TStorage, TModel> ConvertToModel
        {
            get
            {
                if (_convertToModel == null)
                {
                    return (storage) => (TModel)(object)storage;
                }
                return _convertToModel;
            }
            set => _convertToModel = value;
        }

        /// <summary>
        /// Gets or sets the name of the ID property in the <typeparamref name="TModel"/>.
        /// </summary>
        /// <remarks>
        /// This is used to obtain the ID from the model and cannot be null, attempting to set a null throws a <see cref="ArgumentNullException"/>.
        /// </remarks>
        public string IdPropertyName
        {
            get { return _idPropertyName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(IdPropertyName));
                }

                _idPropertyName = value;
                _idProperty = typeof(TModel).GetProperty(value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the Partition key property in the <typeparamref name="TStorage"/>
        /// </summary>
        /// <remarks>
        /// This is used to obtain the Partition Key from the storage object. When the value is null, the Partition Key obtained via <see cref="GetPartitionKey(TStorage)"/>will always be null.
        /// This means that the <see cref="IContainerHandler{TModel, TStorage}"/> handles the Partition Key the same as the ID of the object.
        /// </remarks>
        public string PkPropertyName
        {
            get => _pkPropertyName;
            set
            {
                _pkPropertyName = value;
                if (string.IsNullOrWhiteSpace(value))
                {
                    _pkProperty = null;
                }
                else
                {
                    _pkProperty = typeof(TStorage).GetProperty(value);
                }
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="ContainerSettings{TModel, TStorage}"/> using the provided values.
        /// </summary>
        /// <param name="idPropertyName">The property name of the ID in the <typeparamref name="TModel"/>.</param>
        /// <param name="pkPropertyName">The property name of the partition key in the <typeparamref name="TStorage"/>.</param>
        public ContainerSettings(string idPropertyName = "Id", string pkPropertyName = null)
        {
            IdPropertyName = idPropertyName;
            PkPropertyName = pkPropertyName;
        }

        /// <summary>
        /// Obtains the partition key value from the storage.
        /// </summary>
        /// <param name="storage">The object to obtain the partition key from</param>
        /// <returns>The partition key obtained, or null if <see cref="PkPropertyName"/> is not set or null.</returns>
        public object GetPartitionKey(TStorage storage)
        {
            if (_pkProperty == null)
            {
                return null;
            }
            return _pkProperty.GetValue(storage);
        }

        /// <summary>
        /// Obtains the ID from the model.
        /// </summary>
        /// <param name="model">The object whose ID is to be obtained</param>
        /// <returns>The ID obtained from the object.</returns>
        public string GetId(TModel model)
        {
            return (string)_idProperty.GetValue(model);
        }
    }
}