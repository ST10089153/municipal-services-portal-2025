using System;

namespace MunicipalServicesReporting
{
    /// <summary>
    /// Represents a local event or announcement
    /// </summary>
    public class Event
    {
        public string EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public string Organizer { get; set; }
        public bool IsAnnouncement { get; set; }

        public Event(string eventId, string title, string description, DateTime eventDate,
                     string category, string location, string organizer, bool isAnnouncement = false)
        {
            EventId = eventId;
            Title = title;
            Description = description;
            EventDate = eventDate;
            Category = category;
            Location = location;
            Organizer = organizer;
            IsAnnouncement = isAnnouncement;
        }

        public override string ToString()
        {
            return $"{Title} - {EventDate:dd MMM yyyy} ({Category})";
        }
    }
}