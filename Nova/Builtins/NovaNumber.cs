using System;
using System.Collections.Generic;
using System.Text;
using Nova.Runtime;

namespace Nova.Builtins
{
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    [NovaExport("Number")]
    public class NovaNumber {
        public NovaNumber()
        {
            _internal = default(int);
            _myType = typeof (int);
        }

        public NovaNumber(object val)
        {
            _internal = val;
            _myType = val.GetType();
        }

        private object _internal { get; set; }

        private Type _myType { get; set; }

        public override int GetHashCode()
        {
            return _internal.GetHashCode();
        }

        public override string ToString()
        {
            return _internal.ToString();
        }

        private static List<Type> _numberTypes = new List<Type>
        {
            typeof (bool),
            typeof (byte),
            typeof (sbyte),
            typeof (short),
            typeof (ushort),
            typeof (int),
            typeof (uint),
            typeof (long),
            typeof (ulong),
            typeof (float),
            typeof (double),
            typeof (decimal)
        };

        public override bool Equals(object obj)
        {
            if (obj is NovaNumber)
            {
                return RuntimeOperations.Convert(((NovaNumber) obj)._internal, _myType) ==
                       RuntimeOperations.Convert(_internal, _myType);
            }
            if (_numberTypes.Contains(obj.GetType()))
            {
                return RuntimeOperations.Convert(obj, _myType) == RuntimeOperations.Convert(_internal, _myType);
            }
            return false;
        }

        public static bool IsConvertable(object o)
        {
            return o != null && _numberTypes.Contains(o.GetType());
        }

        public static dynamic Convert(NovaNumber number)
        {
            return RuntimeOperations.Convert(number._internal, number._myType);
        }

        public static implicit operator bool(NovaNumber n)
        {
            return (bool)n._internal;
        }

        public static implicit operator NovaNumber(bool n)
        {
            return new NovaNumber(n);
        }

        public static implicit operator byte(NovaNumber n)
        {
            return (byte)n._internal;
        }

        public static implicit operator NovaNumber(byte n)
        {
            return new NovaNumber(n);
        }

        public static implicit operator sbyte(NovaNumber n)
        {
            return (sbyte)n._internal;
        }

        public static implicit operator NovaNumber(sbyte n)
        {
            return new NovaNumber(n);
        }

        public static implicit operator short(NovaNumber n)
        {
            return (short)n._internal;
        }

        public static implicit operator NovaNumber(short n)
        {
            return new NovaNumber(n);
        }

        public static implicit operator ushort(NovaNumber n)
        {
            return (ushort)n._internal;
        }

        public static implicit operator NovaNumber(ushort n)
        {
            return new NovaNumber(n);
        }

        public static implicit operator int(NovaNumber n)
        {
            return (int)n._internal;
        }

        public static implicit operator NovaNumber(int n)
        {
            return new NovaNumber(n);
        }

        public static implicit operator uint(NovaNumber n)
        {
            return (uint)n._internal;
        }

        public static implicit operator NovaNumber(uint n)
        {
            return new NovaNumber(n);
        }

        public static implicit operator long(NovaNumber n)
        {
            return (long)n._internal;
        }

        public static implicit operator NovaNumber(long n)
        {
            return new NovaNumber(n);
        }

        public static implicit operator ulong(NovaNumber n)
        {
            return (ulong)n._internal;
        }

        public static implicit operator NovaNumber(ulong n)
        {
            return new NovaNumber(n);
        }

        public static implicit operator float(NovaNumber n)
        {
            return (float)n._internal;
        }

        public static implicit operator NovaNumber(float n)
        {
            return new NovaNumber(n);
        }

        public static implicit operator double(NovaNumber n)
        {
            return (double)n._internal;
        }

        public static implicit operator NovaNumber(double n)
        {
            return new NovaNumber(n);
        }

        public static implicit operator decimal(NovaNumber n)
        {
            return (decimal)n._internal;
        }

        public static implicit operator NovaNumber(decimal n)
        {
            return new NovaNumber(n);
        }

    }
}
