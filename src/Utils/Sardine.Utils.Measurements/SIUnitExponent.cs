namespace Sardine.Utils.Measurements
{
    public sealed class UnitExponent : IEquatable<UnitExponent>
    {
        public int Meter
        {
            get => unitData[0];
            init => unitData[0] = value;
        }
        public int Second
        {
            get => unitData[1];
            init => unitData[1] = value;
        }

        public int Kelvin
        {
            get => unitData[2];
            init => unitData[2] = value;
        }
        public int Candela
        {
            get => unitData[3];
            init => unitData[3] = value;
        }
        public int Ampere
        {
            get => unitData[4];
            init => unitData[4] = value;
        }
        public int Kilogram
        {
            get => unitData[5];
            init => unitData[5] = value;
        }
        public int Mole
        {
            get => unitData[6];
            init => unitData[6] = value;
        }

        private readonly int[] unitData = new int[7];
        private static readonly string[] unitSymbols = ["m", "s", "IndependentUnit", "cd", "A", "kg", "mol"];

        public override string ToString()
        {
            string[] symbols = unitData.Where((x) => x != 0).OrderByDescending(x => x).Select((x, i) => $"{unitSymbols[i]}{(x != 1 ? x : "")}").ToArray();

            return string.Join('.', symbols);
        }

        public bool Equals(UnitExponent? other)
        {
            if (other is not null)
            {
                for (int i = 0; i < 7; i++)
                {
                    if (unitData[i] != other.unitData[i])
                    {
                        return false;
                    }
                }
                return true;
            }

            return false;
        }

        public bool CanConvertTo(UnitExponent other, out int signal)
        {
            signal = 0;

            for (int i = 0; i < 7; i++)
            {
                if (unitData[i] == other.unitData[i])
                {
                    signal++;
                    continue;
                }
                if (unitData[i] == -other.unitData[i])
                {
                    signal--;
                    continue;
                }
                return false;
            }
            if (Math.Abs(signal) != 7)
            {
                return false;
            }

            signal = Math.Sign(signal);
            return true;
        }

        public static bool operator ==(UnitExponent first, UnitExponent second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(UnitExponent first, UnitExponent second)
        {
            return !first.Equals(second);
        }

        public UnitExponent() { }
        public UnitExponent(IEnumerable<int> unitValues)
        {
            int[] data = unitValues.ToArray();
            if (data.Length != 7)
            {
                throw new ArgumentException("Unexpected size of unit array.", nameof(unitValues));
            }

            unitData = data;
        }

        public static UnitExponent operator +(UnitExponent first, UnitExponent second)
        {
            int[] newUnits = new int[7];
            for (int i = 0; i < newUnits.Length; i++)
            {
                newUnits[i] = first.unitData[i] + second.unitData[i];
            }

            return new UnitExponent(newUnits);
        }

        public static UnitExponent operator -(UnitExponent first, UnitExponent second)
        {
            int[] newUnits = new int[7];
            for (int i = 0; i < newUnits.Length; i++)
            {
                newUnits[i] = first.unitData[i] - second.unitData[i];
            }

            return new UnitExponent(newUnits);
        }

        public override bool Equals(object? obj) => Equals(obj as UnitExponent);

        public override int GetHashCode() => unitData.GetHashCode();
    }
}