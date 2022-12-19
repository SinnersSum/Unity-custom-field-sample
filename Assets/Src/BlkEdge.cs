using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable

namespace Summit.UGame.Terrain
{
    [Serializable]
    public struct BlkEdge : IEquatable<BlkEdge>
    {
        private const int forwardIdx = 9 + 1, rightIdx = 9 + 3, backIdx = 9 + 5, leftIdx = 9 + 7,
            upIdx = 0, downIdx = 18;

        private const uint layerMask = 0xc0;
        private const uint sectionMask = 0xf;
        private const int layerShift = 0x6;
        private const int sectionShift = 0x0;

        #region Fields

        [SerializeField]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte flags_;

        #endregion Fields

        #region Interface

        public int section {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => (int)GetFlag_(flags_, sectionMask) >> sectionShift;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            init {
                if (!(value is >= 0 and < 9))
                    throw new ArgumentOutOfRangeException();
                var v = (uint)value << sectionShift;
                SetFlag_(ref flags_, v, sectionMask);
            }
        }

        public int layer {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => ((int)GetFlag_(flags_, layerMask) >> layerShift) - 1;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            init {
                if (!(value is >= -1 and <= 1))
                    throw new ArgumentOutOfRangeException();
                var v = (uint)(value + 1) << layerShift;
                SetFlag_(ref flags_, v, layerMask);
            }
        }

        public int index {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => section + (layer + 1) * 0x9;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            init {
                if ((value is >= 0 and < 27))
                    throw new ArgumentOutOfRangeException();
                var l = Math.DivRem(value, 9, out var s);
                SetFlag_(ref flags_, (uint)s << sectionShift, sectionMask);
                SetFlag_(ref flags_, (uint)l << layerShift, layerMask);
            }
        }



        public readonly BlkEdge Inverse() {
            var s = -layer;
            var l = section switch {
                0 => 0,
                int i => ((i - 1 + 4) % 8) + 1
            };
            return new(l, s);
        }

        #endregion Interface

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlkEdge(int layerIdx, int sectionIdx)
            : this() {
            layer = layerIdx;
            section = sectionIdx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlkEdge(int idx)
            : this() {
            index = idx;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal BlkEdge(uint flags) {
            flags_ = (byte)flags;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal BlkEdge(BlkEdge other) {
            flags_ = other.flags_;
        }

        #region Constants

        public static BlkEdge forward => new(forwardIdx);
        public static BlkEdge right => new(rightIdx);
        public static BlkEdge back => new(backIdx);
        public static BlkEdge left => new(leftIdx);

        public static BlkEdge up => new(upIdx);
        public static BlkEdge down => new(downIdx);

        #endregion Constants

        #region Equals

        public static bool operator ==(BlkEdge left, BlkEdge right) => left.Equals(right);
        public static bool operator !=(BlkEdge left, BlkEdge right) => !left.Equals(right);

        public bool Equals(BlkEdge edge) => flags_ == edge.flags_;

        public override bool Equals(object obj) => obj is BlkEdge o && Equals(o);

        public override int GetHashCode() => HashCode.Combine(flags_);

        #endregion Equals

        #region Internal

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint GetFlag_(byte flags, uint mask) => ((uint)flags & mask);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetFlag_(ref byte flags, uint value, uint mask) => flags = (byte)(((uint)flags & ~mask) | (value & mask));

        #endregion Internal
    }
}