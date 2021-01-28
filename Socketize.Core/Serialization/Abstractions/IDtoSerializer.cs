using System;

namespace Socketize.Core.Serialization.Abstractions
{
    /// <summary>
    /// Abstraction that represents serializer used to serialize and deserialize DTO objects.
    /// </summary>
    public interface IDtoSerializer
    {
        /// <summary>
        /// Returns array of bytes that represents given value.
        /// </summary>
        /// <param name="value">Value object to be serialized.</param>
        /// <returns>Array of bytes that represents given value.</returns>
        byte[] Serialize(object value);

        /// <summary>
        /// Returns object of given type deserialized from byte array.
        /// </summary>
        /// <param name="valueType">Type of deserialized object.</param>
        /// <param name="rawValue">Array of bytes that represents.</param>
        /// <returns>Object of given type deserialized from byte array.</returns>
        object Deserialize(Type valueType, byte[] rawValue);
    }
}