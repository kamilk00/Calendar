using System;

namespace Calendar
{

    public class Event
    {

        public int ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Notification { get; set; }

    }

}
