using System;
using System.Collections.Generic;

/*
    @File TimeUtills.cs
    @author Elias Vahlberg 
    @Date 2021-07
*/
namespace Utills
{
    public class TimeUtills
    {
        public static TimeUtills Instance { get { if (TimeUtills.instance == null) TimeUtills.instance = new TimeUtills(); return instance; } }
        private static TimeUtills instance;

        private Dictionary<int, DateTime> measurements;
        private int dictCounter;
        public TimeUtills()
        { measurements = new Dictionary<int, DateTime>(); }
        public int startMeasurement()
        {
            dictCounter++;
            measurements.Add(dictCounter, DateTime.Now);
            return dictCounter;
        }
        public long stopMeasurementMillis(int measurementID)
        {
            long delta = -1;
            DateTime prev;
            if (measurements.TryGetValue(measurementID, out prev))
            {
                delta = (long)((TimeSpan)(DateTime.Now - prev)).TotalMilliseconds;
                measurements.Remove(measurementID);
            }
            return delta;
        }
        public float stopMeasurementSeconds(int measurementID)
        {
            float delta = -1;
            DateTime prev;
            if (measurements.TryGetValue(measurementID, out prev))
            {
                delta = (float)((TimeSpan)(DateTime.Now - prev)).TotalSeconds;
            }
            return delta;
        }
    }
}