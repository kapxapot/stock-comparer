using System;
using System.Collections.Generic;

namespace StockComparer.Models
{
    public class Period
    {
        private DateTime _start;
        private DateTime _end;

        public Period(DateTime start, DateTime end)
        {
            if (start > end)
            {
                throw new ArgumentException("The start must be less or equal to the end.");
            }

            _start = start;
            _end = end;
        }

        public DateTime Start => _start;

        public DateTime End => _end;

        public static Period LastWeek
        {
            get
            {
                var end = DateTime.UtcNow.Date.AddDays(-1);
                var start = end.AddDays(-6);

                return new Period(start, end);
            }
        }

        public IEnumerable<DateTime> StockDays
        {
            get
            {
                var days = new List<DateTime>();
                var day = _start;

                do
                {
                    if (day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday)
                    {
                        days.Add(day);
                    }

                    day = day.AddDays(1);
                }
                while (day <= _end);

                return days;
            }
        }
    }
}
