using System;

namespace VrtNuDownloader.Core.Models
{
    public struct UnixTimeStamp
    {
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static bool TryParse(string s, out UnixTimeStamp result)
        {
            double d;
            bool success = double.TryParse(s, out d);
            result = new UnixTimeStamp(d);
            return success;
        }

        public static UnixTimeStamp Now
        {
            get { return new UnixTimeStamp(DateTime.Now.ToUniversalTime()); }
        }

        private double _seconds;
        public double Seconds { get => _seconds; set => _seconds = Math.Round(value, 0); }

        public DateTime ToDateTime()
        {
            return UnixTimeStamp.Epoch.AddSeconds(this.Seconds).ToLocalTime();
        }

        public override string ToString()
        {
            return this.Seconds.ToString();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is UnixTimeStamp))
            {
                return false;
            }

            var stamp = (UnixTimeStamp)obj;
            return Seconds == stamp.Seconds;
        }

        public override int GetHashCode() => -1609761766 + Seconds.GetHashCode();

        public static bool operator ==(UnixTimeStamp a, UnixTimeStamp b) => a.Seconds == b.Seconds;
        public static bool operator !=(UnixTimeStamp a, UnixTimeStamp b) => a.Seconds != b.Seconds;

        public static bool operator >(UnixTimeStamp a, UnixTimeStamp b) => a.Seconds > b.Seconds;
        public static bool operator <(UnixTimeStamp a, UnixTimeStamp b) => a.Seconds < b.Seconds;

        public static bool operator >=(UnixTimeStamp a, UnixTimeStamp b) => a.Seconds >= b.Seconds;
        public static bool operator <=(UnixTimeStamp a, UnixTimeStamp b) => a.Seconds <= b.Seconds;


        public UnixTimeStamp(DateTime? datetime)
        {
            _seconds = datetime == null ? 0 : Math.Round((datetime.Value.ToUniversalTime() - UnixTimeStamp.Epoch).TotalSeconds, 0);
        }

        public UnixTimeStamp(double? seconds)
        {
            if (seconds >= 100000000000000) seconds = seconds/ 1000;
            if (seconds >= 100000000000) seconds = seconds / 1000;

            _seconds = Math.Round(seconds ?? 0, 0);
        }
    }
}
