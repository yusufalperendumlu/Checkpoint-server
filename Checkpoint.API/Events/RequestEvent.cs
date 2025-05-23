﻿namespace Checkpoint.API.Events
{
    public class RequestEvent
    {
        public string Url { get; set; }
        public bool RequestStatus { get; set; }
        public int StatusCode { get; set; }
        public long ResponseTimeMs { get; set; }
        public DateTime TimeStamp { get; set; }
        public int? IndividualId { get; set; }
        public int? TeamId { get; set; }
    }
}
