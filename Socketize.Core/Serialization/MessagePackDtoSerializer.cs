using System;
using MessagePack;
using Socketize.Core.Serialization.Abstractions;

namespace Socketize.Core.Serialization
{
    public class MessagePackDtoSerializer : IDtoSerializer
    {
        public byte[] Serialize(object value)
        {
            return MessagePackSerializer.Serialize(value);
        }

        public object Deserialize(Type valueType, byte[] rawValue)
        {
            return MessagePackSerializer.Deserialize(valueType, rawValue);
        }
    }
}