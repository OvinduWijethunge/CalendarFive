using System;
using System.Collections.Generic;

namespace CalendarFive.Models
{
    public partial class Content
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string EventId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Description { get; set; }
    }
}
