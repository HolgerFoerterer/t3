﻿using System;
using System.Runtime.InteropServices;
using SharpDX;

namespace T3.Core.DataTypes
{
    public abstract class StructuredList
    {
        public StructuredList(Type type)
        {
            Type = type;
        }

        public Type Type { get; }
        public abstract object Elements { get; }
        public abstract int ElementSizeInBytes { get; }
        public abstract int TotalSizeInBytes { get; }
        public abstract void WriteToStream(DataStream stream);
    }

    public class StructuredList<T> : StructuredList where T : struct
    {
        public StructuredList(int count) : base(typeof(T))
        {
            TypedElements = new T[count];
        }

        public T[] TypedElements { get; }
        public override object Elements => TypedElements;
        public override int ElementSizeInBytes => Marshal.SizeOf<T>();
        public override int TotalSizeInBytes => TypedElements.Length * ElementSizeInBytes;

        public override void WriteToStream(DataStream stream)
        {
            stream.WriteRange(TypedElements);
        }
    }
}