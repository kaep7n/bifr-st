namespace Bifröst
{
    public static class Metrics
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "by design")]
        public static class Bus
        {
            public const string ReceivedEvents = "bifröst_bus_received_events_count";
            public const string ProcessedEvents = "bifröst_bus_processed_events_count";
            public const string IsRunning = "bifröst_bus_is_running";
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "by design")]
        public static class Subscription
        {
            public const string ReceivedEvents = "bifröst_subscribtion_received_events_count";
            public const string ProcessedEvents = "bifröst_subscribtion_processed_events_count";
            public const string IsEnabled = "bifröst_subscribtion_is_enabled";
        }
    }
}
