using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace NumeralSystems.Net.Interface
{
    public interface IArray<TSize, TElement> where TSize: struct, IComparable, IComparable<TSize>, IEquatable<TSize>, IFormattable 
    {
        TElement this[TSize index] { get; set; }
        TSize Length { get; }
        void Resize(TSize newSize);
        void CopyTo(IArray<TSize, TElement> destination, TSize destinationIndex);
        void CopyTo(IArray<TSize, TElement> destination, TSize destinationIndex, TSize sourceIndex, TSize length);
        void CopyTo(TElement[] destination, TSize destinationIndex);
        void CopyTo(TElement[] destination, TSize destinationIndex, TSize sourceIndex, TSize length);
        void CopyFrom(IArray<TSize, TElement> source, TSize sourceIndex);
        void CopyFrom(IArray<TSize, TElement> source, TSize sourceIndex, TSize destinationIndex, TSize length);
        void CopyFrom(TElement[] source, TSize sourceIndex);
        void CopyFrom(TElement[] source, TSize sourceIndex, TSize destinationIndex, TSize length);
        
        IEnumerable<TElement> ToEnumerable();
    }

}