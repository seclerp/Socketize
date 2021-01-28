using System;
using MessagePack;
using Socketize.Core.Serialization.Abstractions;

namespace Socketize.Core.Serialization
{
    /// <summary>
    /// MessagePack based DTO serializer.
    /// </summary>
    public class MessagePackDtoSerializer : IDtoSerializer
    {
        /// <inheritdoc />
        public byte[] Serialize(object value)
        {
            return MessagePackSerializer.Serialize(value);
        }

        /// <inheritdoc />
        public object Deserialize(Type valueType, byte[] rawValue)
        {
            return MessagePackSerializer.Deserialize(valueType, rawValue);
        }
    }
}