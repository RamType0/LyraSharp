using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LyraSharp
{
    [StructLayout(LayoutKind.Sequential, Pack = 256 / 8, Size = 256 / 8)]
    public struct BitSet256 : IEquatable<BitSet256>
    {
        ulong a, b, c, d;
        public BitSet256(int value) : this((ulong)value) { }
        public BitSet256(ulong value) : this()
        {
            a = value;
        }
        public BitSet256(ReadOnlySpan<char> value) : this()
        {
            if (value.Length >= 256)
            {
                throw new ArgumentException();
            }


            for (int i = 0; i < value.Length; i++)
            {
                var bit = value[^(i + 1)] switch
                {
                    '0' => false,
                    '1' => true,
                    _ => throw new ArgumentException(),
                };
                this[i] = bit;
            }
        }

        public bool this[int index]
        {
            readonly get
            {
                ExtractByteAndBitIndex(index, out var byteIndex, out var bitIndex);
                var mask = 1 << bitIndex;
                return (MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(this), 1))[byteIndex] & mask) != 0;
            }
            set
            {
                ExtractByteAndBitIndex(index, out var byteIndex, out var bitIndex);
                ref var targetByte = ref MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref this, 1))[byteIndex];
                var mask = (1 << bitIndex);
                if (value)
                {
                    targetByte |= (byte)mask;
                }
                else
                {
                    targetByte &= (byte)~mask;
                }
            }
        }

        public override string ToString()
        {
            Span<char> buffer = stackalloc char[256];
            for (int i = 0; i < 256; i++)
            {
                var bit = this[255 - i] ? 1 : 0;
                bit.TryFormat(buffer[i..], out _);
            }
            return buffer.ToString();
        }

        public readonly int ToInt32() => Unsafe.As<BitSet256, int>(ref Unsafe.AsRef(this));

        readonly void ExtractByteAndBitIndex(int globalBitIndex, out int byteIndex, out int bitIndex)
        {
            if (256 <= (uint)globalBitIndex)
            {
                Throw();
            }

            byteIndex = globalBitIndex >> 3;
            bitIndex = globalBitIndex & 7;
            static void Throw()
            {
                throw new IndexOutOfRangeException();
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is BitSet256 set && Equals(set);
        }

        public bool Equals(BitSet256 other)
        {
            return a == other.a &&
                   b == other.b &&
                   c == other.c &&
                   d == other.d;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(a, b, c, d);
        }

        public static unsafe BitSet256 operator &(in BitSet256 left, in BitSet256 right)
        {
            if (sizeof(Vector<ulong>) <= sizeof(BitSet256))
            {
                var bits = new BitSet256();
                ref var result = ref Unsafe.As<BitSet256, Vector<ulong>>(ref bits);
                ref var x = ref Unsafe.As<BitSet256, Vector<ulong>>(ref Unsafe.AsRef(left));
                ref var y = ref Unsafe.As<BitSet256, Vector<ulong>>(ref Unsafe.AsRef(right));
                for (int i = 0; i < sizeof(BitSet256) / sizeof(Vector<ulong>); i++)
                {
                    Unsafe.Add(ref result, i) = Vector.BitwiseAnd(Unsafe.Add(ref x, i), Unsafe.Add(ref y, i));
                }
                return bits;
            }
            else
            {
                Vector<ulong> x, y;
                Unsafe.Write(&x, left);
                Unsafe.Write(&y, right);
                var result = Vector.BitwiseAnd(x, y);
                return Unsafe.As<Vector<ulong>, BitSet256>(ref result);
            }
        }
        public static unsafe BitSet256 operator |(in BitSet256 left, in BitSet256 right)
        {
            if (sizeof(Vector<ulong>) <= sizeof(BitSet256))
            {
                var bits = new BitSet256();
                ref var result = ref Unsafe.As<BitSet256, Vector<ulong>>(ref bits);
                ref var x = ref Unsafe.As<BitSet256, Vector<ulong>>(ref Unsafe.AsRef(left));
                ref var y = ref Unsafe.As<BitSet256, Vector<ulong>>(ref Unsafe.AsRef(right));
                for (int i = 0; i < sizeof(BitSet256) / sizeof(Vector<ulong>); i++)
                {
                    Unsafe.Add(ref result, i) = Vector.BitwiseOr(Unsafe.Add(ref x, i), Unsafe.Add(ref y, i));
                }
                return bits;
            }
            else
            {
                Vector<ulong> x, y;
                Unsafe.Write(&x, left);
                Unsafe.Write(&y, right);
                var result = Vector.BitwiseOr(x, y);
                return Unsafe.As<Vector<ulong>, BitSet256>(ref result);
            }
        }
        public static unsafe BitSet256 operator ^(in BitSet256 left, in BitSet256 right)
        {
            if (sizeof(Vector<ulong>) <= sizeof(BitSet256))
            {
                var bits = new BitSet256();
                ref var result = ref Unsafe.As<BitSet256, Vector<ulong>>(ref bits);
                ref var x = ref Unsafe.As<BitSet256, Vector<ulong>>(ref Unsafe.AsRef(left));
                ref var y = ref Unsafe.As<BitSet256, Vector<ulong>>(ref Unsafe.AsRef(right));
                for (int i = 0; i < sizeof(BitSet256) / sizeof(Vector<ulong>); i++)
                {
                    Unsafe.Add(ref result, i) = Vector.Xor(Unsafe.Add(ref x, i), Unsafe.Add(ref y, i));
                }
                return bits;
            }
            else
            {
                Vector<ulong> x, y;
                Unsafe.Write(&x, left);
                Unsafe.Write(&y, right);
                var result = Vector.Xor(x, y);
                return Unsafe.As<Vector<ulong>, BitSet256>(ref result);
            }
        }
        public static unsafe BitSet256 operator <<(in BitSet256 left, int right)
        {
            if (right <= -256 | 256 <= right)
            {
                return new BitSet256();
            }

            var x = MemoryMarshal.Cast<BitSet256, ulong>(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(left), 1));

            var ret = new BitSet256();
            var result = MemoryMarshal.Cast<BitSet256, ulong>(MemoryMarshal.CreateSpan(ref ret, 1));

            var indexOffset = (int)MathF.Floor(-right / 64f);
            var localShifts = (64 - (right & 63)) & 63;

            for (int i = 0; i < result.Length; i++)
            {
                var sourceIndex1 = i + indexOffset;
                var source1 = (uint)sourceIndex1 < 4 ? x[sourceIndex1] : 0;
                var sourceIndex2 = sourceIndex1 + 1;
                var source2 = (uint)sourceIndex2 < 4 ? x[sourceIndex2] : 0;
                result[i] = (source1 >> localShifts) | (source2 << (64 - localShifts));
            }

            return ret;
        }
        public static unsafe BitSet256 operator >>(in BitSet256 left, int right) => left << -right;

        public static bool operator ==(BitSet256 left, BitSet256 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BitSet256 left, BitSet256 right)
        {
            return !(left == right);
        }
    }

}
