using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnixTimestamp.NetStandard;

// ReSharper disable once CheckNamespace
namespace UnixTimestampTest
{
    [TestClass]
    public class UnixTimestampTests
    {
        [TestMethod]
        public void Eq_Test()
        {
            Timestamp a = null;
            Timestamp b = null;
            Assert.IsTrue(a == null);
            Assert.IsTrue(null == a);
            Assert.IsTrue(a == b);

            var time = DateTime.Now;
            var t1 = new Timestamp(time);
            var t2 = new Timestamp(time);
            var t3 = new Timestamp(DateTime.MinValue, TimeZoneInfo.Utc);

            Assert.IsTrue(t1 == t2);
            Assert.IsFalse(t1 == t3);
        }

        [TestMethod]
        public void Convert_DateTime_Timestamp_Test()
        {
            var datetime = new DateTime(1970, 1, 1, 0, 0, 1, 0, DateTimeKind.Utc);
            var timestamp = new Timestamp(datetime);
            Assert.AreEqual(timestamp.ToValue(), 1);

            datetime = new DateTime(1970, 1, 1, 0, 0, 1, 0, DateTimeKind.Local);
            timestamp = new Timestamp(datetime);
            Assert.AreEqual(timestamp.ToValue(), 1 - TimeZoneInfo.Local.BaseUtcOffset.TotalSeconds);

            Assert.ThrowsException<ArgumentException>(() =>
            {
                datetime = new DateTime(1970, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified);
                var t = new Timestamp(datetime);
            });

            Assert.ThrowsException<ArgumentException>(() =>
            {
                datetime = new DateTime(1970, 1, 1, 0, 0, 1, 0, DateTimeKind.Utc);
                var t = new Timestamp(datetime, TimeZoneInfo.Local);
            });
        }
        [TestMethod]
        public void Convert_Long_Timestamp_Test()
        {
            //1s
            if (Timestamp.TryParse(1, 0, out var result))
            {
                Assert.AreEqual(result.UtcTime, new DateTime(1970, 1, 1, 0, 0, 1, 0, DateTimeKind.Utc));
            }
            else
            {
                Assert.Fail();
            }
            //1ms
            if (Timestamp.TryParse(1, 3, out var result1))
            {
                Assert.AreEqual(result1.UtcTime, new DateTime(1970, 1, 1, 0, 0, 0, 1, DateTimeKind.Utc));
            }
            else
            {
                Assert.Fail();
            }

            if (Timestamp.TryParse(936868149, 0, out var result2))
            {
                Assert.AreEqual(result2.UtcTime, new DateTime(1999, 9, 9, 9, 9, 9, DateTimeKind.Utc));
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void Convert_Timestamp_Long_Test()
        {
            var datetime = new DateTime(1970, 1, 1, 0, 0, 1, 0, DateTimeKind.Utc);
            var timestamp = new Timestamp(datetime);
            var seconds = timestamp.ToValue();
            Assert.AreEqual(seconds, 1);
        }

        [TestMethod]
        public void Convert_string_Timestamp_Test()
        {
            //1s
            if (Timestamp.TryParse("1", 0, out var result))
            {
                Assert.AreEqual(result.UtcTime, new DateTime(1970, 1, 1, 0, 0, 1, 0, DateTimeKind.Utc));
            }
            else
            {
                Assert.Fail();
            }
            //1ms
            if (Timestamp.TryParse("1", 3, out var result1))
            {
                Assert.AreEqual(result1.UtcTime, new DateTime(1970, 1, 1, 0, 0, 0, 1, DateTimeKind.Utc));
            }
            else
            {
                Assert.Fail();
            }

            if (Timestamp.TryParse("936868149", 0, out var result2))
            {
                Assert.AreEqual(result2.UtcTime, new DateTime(1999, 9, 9, 9, 9, 9, DateTimeKind.Utc));
            }
            else
            {
                Assert.Fail();
            }
        }


        [TestMethod]
        public void Timestamp_ToString_Test()
        {
            var datetime = new DateTime(1970, 1, 1, 0, 0, 1, 234, DateTimeKind.Utc);
            var timestamp = new Timestamp(datetime);

            Assert.AreEqual(timestamp.ToString(), "1");
            Assert.AreEqual(timestamp.ToString(3), "1234");

            datetime = new DateTime(1999, 9, 9, 9, 9, 9, DateTimeKind.Utc);
            timestamp = new Timestamp(datetime);
            Assert.AreEqual(timestamp.ToString(), "936868149");
            Assert.AreEqual(timestamp.ToString(3), "936868149000");
        }

        [TestMethod]
        public void Timestamp_ToValue_Test()
        {
            var datetime = new DateTime(1970, 1, 1, 0, 0, 1, 234, DateTimeKind.Utc);
            var timestamp = new Timestamp(datetime);

            Assert.AreEqual(timestamp.ToValue(), 1);
            Assert.AreEqual(timestamp.ToValue(3), 1234);

            datetime = new DateTime(1999, 9, 9, 9, 9, 9, DateTimeKind.Utc);
            timestamp = new Timestamp(datetime);
            Assert.AreEqual(timestamp.ToValue(), 936868149);
            Assert.AreEqual(timestamp.ToValue(3), 936868149000);
        }
    }
}
