using System;

namespace Socketize.Core.Serialization.Abstractions
{
    public interface IDtoSerializer
    {
        byte[] Serialize(object value);

        object Deserialize(Type valueType, byte[] rawValue);
    }
}