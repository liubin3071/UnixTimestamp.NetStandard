using System;
using System.Globalization;

namespace UnixTimestamp.NetStandard
{
    public class Timestamp
    {
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public DateTime UtcTime { get; }

        public DateTime LocalTime => UtcTime.ToLocalTime();

        public Timestamp(DateTime dateTime)
        {
            switch (dateTime.Kind)
            {
                case DateTimeKind.Local:
                    UtcTime = dateTime.ToUniversalTime();
                    break;
                case DateTimeKind.Utc:
                    UtcTime = dateTime;
                    break;
                case DateTimeKind.Unspecified:
                    //未知时区
                    throw new ArgumentException($"{nameof(dateTime)}.Kind is DateTimeKind.Unspecified,Unkown TimeZone.");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Timestamp(DateTime dateTime, TimeZoneInfo sourceTimeZone)
        {
            if (dateTime.Kind == DateTimeKind.Local && !TimeZoneInfo.Local.Equals(sourceTimeZone) ||
                 dateTime.Kind == DateTimeKind.Utc && !TimeZoneInfo.Utc.Equals(sourceTimeZone))
            {
                //时区冲突
                throw new ArgumentException($"{nameof(dateTime)}.Kind:{dateTime.Kind};{nameof(sourceTimeZone)}:{sourceTimeZone.DisplayName};Local TimeZone:{TimeZoneInfo.Local.DisplayName}.");
            }
            UtcTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, sourceTimeZone);
        }

        public static string Now(int digitsAfterSeconds = 0)
        {
            return new Timestamp(DateTime.UtcNow).ToString(digitsAfterSeconds);
        }



        public static bool TryParse(string value, out Timestamp result)
        {
            return TryParse(value, 0, out result);
        }

        public static bool TryParse(string value, int digitsAfterSeconds, out Timestamp result)
        {
            if (long.TryParse(value, out var timestamp))
            {
                return TryParse(timestamp, digitsAfterSeconds, out result);
            }

            result = null;
            return false;
        }

        public static bool TryParse(long value, out Timestamp result)
        {
            return TryParse(value, 0, out result);
        }

        public static bool TryParse(long value, int digitsAfterSeconds, out Timestamp result)
        {
            var milliSeconds = value * Math.Pow(10, 3 - digitsAfterSeconds);
            //253402300800 时间戳== 10000/1/1 0:0:0 == DateTime.MaxValue + 1s
            if (milliSeconds >= 253402300800000) //毫秒
            {
                result = null;
                return false;
            }

            var time = Epoch.AddMilliseconds(milliSeconds);
            result = new Timestamp(time);
            return true;
        }

        public static Timestamp FromDateTime(DateTime dateTime, TimeZoneInfo sourceTimeZone)
        {
            return new Timestamp(dateTime, sourceTimeZone);
        }
        

        #region OverLoad ==

        public override bool Equals(object obj)
        {
            var other = obj as Timestamp;
            return other != null && Equals(other);
        }

        public bool Equals(Timestamp other)
        {
            return other.UtcTime == UtcTime;
        }

        public override int GetHashCode()
        {
            return UtcTime.GetHashCode();
        }

        public static bool operator ==(Timestamp a, Timestamp b)
        {
            if ((a as object) == null) return (b as object) == null;
            return a.Equals(b);
        }

        public static bool operator !=(Timestamp a, Timestamp b)
        {
            return !(a == b);
        }

        #endregion

        public long ToValue(int digitsAfterSeconds = 0)
        {
            var ticks = UtcTime.Ticks - Epoch.Ticks;
            return ticks / (int)Math.Pow(10, 7 - digitsAfterSeconds);
        }

        public override string ToString()
        {
            return ToString(0);
        }


        public string ToString(int digitsAfterSeconds)
        {
            return ToValue(digitsAfterSeconds).ToString(CultureInfo.InvariantCulture);
        }
    }
}
