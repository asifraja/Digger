#define ItineraryIdAsInt 

using System;

namespace easyjet.Types
{
#if ItineraryIdAsLong
    public class ItineraryId : IDColumn<long>
    {
        public ItineraryId(long Identity) : base(Identity)
        {
        }
    }
#else
    public class ItineraryId : IDColumn<int>
    {
        public ItineraryId(int Identity) : base(Identity)
        {
        }
    }
#endif

    public class IDColumn<T> : IEjIdentityField<T> where T : struct
    {
        public IDColumn(T Identity)
        {
            Value = Identity;
        }
        public char CompareTo(T that)
        {
            throw new NotImplementedException();
        }
        public static implicit operator T(IDColumn<T> Identity)
        {
            return Identity.Value;
        }
        public static implicit operator IDColumn<T>(T Identity)
        { return new IDColumn<T>(Identity); }

        public T Value { get; set; }
    }

    public interface IEjIdentityField<T> where T : struct
    {
        char CompareTo(T that);
        T Value { set; get; }
    }
}
