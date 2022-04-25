using System;

namespace Proyecto_Intermodular.api
{
    public class ApiException : Exception
    {
        public ApiException() : base() { }
        public ApiException(string message) : base(message) { }
        protected ApiException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class ItemNotFoundException : ApiException
    {
        public ItemNotFoundException() : base() { }
        public ItemNotFoundException(string message) : base(message) { }
        protected ItemNotFoundException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class WrongCredentialsException : ApiException
    {
        public WrongCredentialsException() : base() { }
        public WrongCredentialsException(string message) : base(message) { }
        protected WrongCredentialsException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class AlreadyInUseException : ApiException
    {
        public AlreadyInUseException() : base() { }
        public AlreadyInUseException(string message) : base(message) { }
        protected AlreadyInUseException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class NotEnoughStockException : ApiException
    {
        public NotEnoughStockException() : base() { }
        public NotEnoughStockException(string message) : base(message) { }
        protected NotEnoughStockException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
