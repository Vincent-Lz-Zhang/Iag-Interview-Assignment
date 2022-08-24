using System;
using System.Collections.Generic;
using System.Net;

namespace VehicleSummary.UnitTests.Helpers
{
    public static class RandomValuesGenerator
    {
        public static HttpStatusCode GetRandomErrorStatusCode(List<int> excludes = null)
        {
            excludes ??= new List<int>();
            Predicate<int> filter = delegate (int item) { return item > 299 && !excludes.Contains(item); }; // by default, Flurl throws exception for non-2xx status
            int[] values = Array.FindAll((int[])Enum.GetValues(typeof(HttpStatusCode)), filter);
            Random random = new();
            return (HttpStatusCode)values.GetValue(random.Next(values.Length));
        }
    }
}
