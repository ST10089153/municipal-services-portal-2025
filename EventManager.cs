using System;
using System.Collections.Generic;
using System.Linq;

namespace MunicipalServicesReporting
{
    /// <summary>
    /// Manages events using advanced data structures
    /// </summary>
    public class EventManager
    {
        // SortedDictionary for efficient date-based event organization
        private SortedDictionary<DateTime, List<Event>> eventsByDate;

        // Queue for managing recently viewed events (Last 10)
        private Queue<Event> recentlyViewedEvents;

        // HashSet for unique categories
        private HashSet<string> eventCategories;

        // Dictionary for tracking user search patterns
        private Dictionary<string, int> searchPatternTracker;

        // Priority Queue simulation using SortedDictionary for upcoming events
        private SortedDictionary<DateTime, Event> upcomingEventsQueue;

        private const int MAX_RECENT_VIEWS = 10;

        public EventManager()
        {
            eventsByDate = new SortedDictionary<DateTime, List<Event>>();
            recentlyViewedEvents = new Queue<Event>(MAX_RECENT_VIEWS);
            eventCategories = new HashSet<string>();
            searchPatternTracker = new Dictionary<string, int>();
            upcomingEventsQueue = new SortedDictionary<DateTime, Event>();

            InitializeSampleData();
        }

        /// <summary>
        /// Adds sample events to the system
        /// </summary>
        private void InitializeSampleData()
        {
            List<Event> sampleEvents = new List<Event>
            {
                new Event("EVT001", "Community Clean-Up Day", "Join us for a city-wide clean-up initiative",
                    DateTime.Now.AddDays(5), "Community", "Central Park", "Parks Department"),

                new Event("EVT002", "Town Hall Meeting", "Quarterly municipal budget discussion",
                    DateTime.Now.AddDays(10), "Government", "City Hall", "Mayor's Office"),

                new Event("EVT003", "Youth Soccer Tournament", "Annual inter-school soccer competition",
                    DateTime.Now.AddDays(15), "Sports", "Municipal Stadium", "Sports & Recreation"),

                new Event("EVT004", "Blood Drive", "Red Cross blood donation campaign",
                    DateTime.Now.AddDays(7), "Health", "Community Center", "Health Department"),

                new Event("EVT005", "Farmers Market", "Weekly fresh produce and crafts market",
                    DateTime.Now.AddDays(3), "Market", "Main Street", "Economic Development"),

                new Event("EVT006", "Library Book Sale", "Annual fundraising book sale",
                    DateTime.Now.AddDays(12), "Culture", "Public Library", "Library Services"),

                new Event("EVT007", "Water Conservation Workshop", "Learn water-saving techniques",
                    DateTime.Now.AddDays(8), "Environment", "Civic Center", "Water & Sanitation"),

                new Event("EVT008", "Street Festival", "Music, food, and entertainment",
                    DateTime.Now.AddDays(20), "Entertainment", "Downtown District", "Tourism Board"),

                new Event("EVT009", "Business Networking Breakfast", "Connect with local entrepreneurs",
                    DateTime.Now.AddDays(6), "Business", "Chamber of Commerce", "Economic Development"),

                new Event("EVT010", "Fire Safety Seminar", "Home fire prevention and safety tips",
                    DateTime.Now.AddDays(14), "Safety", "Fire Station 3", "Fire Department"),

                new Event("EVT011", "Art Exhibition Opening", "Local artists showcase",
                    DateTime.Now.AddDays(18), "Culture", "Municipal Gallery", "Arts & Culture"),

                new Event("EVT012", "Senior Citizens Lunch", "Monthly social gathering",
                    DateTime.Now.AddDays(4), "Community", "Senior Center", "Social Services"),

                new Event("EVT013", "Traffic Safety Campaign Launch", "New road safety initiative",
                    DateTime.Now.AddDays(9), "Safety", "Traffic Department", "Metro Police"),

                new Event("EVT014", "Youth Coding Workshop", "Introduction to programming for teens",
                    DateTime.Now.AddDays(11), "Education", "Tech Hub", "Education Department"),

                new Event("EVT015", "Municipal Job Fair", "Employment opportunities showcase",
                    DateTime.Now.AddDays(16), "Business", "Convention Center", "HR Department"),

                new Event("EVT016", "Recycling Awareness Day", "Learn about waste management",
                    DateTime.Now.AddDays(13), "Environment", "Eco Park", "Waste Management"),

                new Event("ANN001", "Road Closure Notice", "Main Street closed for repairs Oct 20-25",
                    DateTime.Now.AddDays(2), "Infrastructure", "Main Street", "Roads Department", true),

                new Event("ANN002", "Water Interruption Alert", "Scheduled maintenance in Zone 4",
                    DateTime.Now.AddDays(1), "Utilities", "Zone 4 Suburbs", "Water Services", true),

                new Event("ANN003", "New Parking Regulations", "Updated parking bylaws effective Nov 1",
                    DateTime.Now, "Government", "City-wide", "Traffic Department", true)
            };

            foreach (var evt in sampleEvents)
            {
                AddEvent(evt);
            }
        }

        /// <summary>
        /// Adds an event to the system
        /// </summary>
        public void AddEvent(Event evt)
        {
            // Add to SortedDictionary
            DateTime dateKey = evt.EventDate.Date;
            if (!eventsByDate.ContainsKey(dateKey))
            {
                eventsByDate[dateKey] = new List<Event>();
            }
            eventsByDate[dateKey].Add(evt);

            // Add category to HashSet
            eventCategories.Add(evt.Category);

            // Add to upcoming events queue (if future event)
            if (evt.EventDate >= DateTime.Now && !evt.IsAnnouncement)
            {
                upcomingEventsQueue[evt.EventDate] = evt;
            }
        }

        /// <summary>
        /// Track when user views an event
        /// </summary>
        public void RecordEventView(Event evt)
        {
            if (recentlyViewedEvents.Count >= MAX_RECENT_VIEWS)
            {
                recentlyViewedEvents.Dequeue();
            }
            recentlyViewedEvents.Enqueue(evt);
        }

        /// <summary>
        /// Track user search patterns
        /// </summary>
        public void RecordSearch(string category, DateTime? date)
        {
            string searchKey = category ?? "AllCategories";

            if (searchPatternTracker.ContainsKey(searchKey))
            {
                searchPatternTracker[searchKey]++;
            }
            else
            {
                searchPatternTracker[searchKey] = 1;
            }
        }

        /// <summary>
        /// Get all events
        /// </summary>
        public List<Event> GetAllEvents()
        {
            List<Event> allEvents = new List<Event>();
            foreach (var dateGroup in eventsByDate.Values)
            {
                allEvents.AddRange(dateGroup);
            }
            return allEvents;
        }

        /// <summary>
        /// Search events by category and/or date
        /// </summary>
        public List<Event> SearchEvents(string category = null, DateTime? date = null)
        {
            RecordSearch(category, date);

            List<Event> results = GetAllEvents();

            if (!string.IsNullOrEmpty(category) && category != "All Categories")
            {
                results = results.Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (date.HasValue)
            {
                results = results.Where(e => e.EventDate.Date == date.Value.Date).ToList();
            }

            return results;
        }

        /// <summary>
        /// Get recommended events based on user search patterns
        /// </summary>
        public List<Event> GetRecommendedEvents()
        {
            if (searchPatternTracker.Count == 0)
            {
                // Return upcoming events if no search history
                return upcomingEventsQueue.Values.Take(5).ToList();
            }

            // Find most searched categories
            var topCategories = searchPatternTracker
                .OrderByDescending(kvp => kvp.Value)
                .Take(3)
                .Select(kvp => kvp.Key)
                .Where(k => k != "AllCategories")
                .ToList();

            List<Event> recommended = new List<Event>();

            foreach (var category in topCategories)
            {
                var categoryEvents = GetAllEvents()
                    .Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase)
                                && e.EventDate >= DateTime.Now)
                    .OrderBy(e => e.EventDate)
                    .Take(3);

                recommended.AddRange(categoryEvents);
            }

            return recommended.Distinct().Take(6).ToList();
        }

        /// <summary>
        /// Get all unique categories
        /// </summary>
        public List<string> GetAllCategories()
        {
            return eventCategories.OrderBy(c => c).ToList();
        }

        /// <summary>
        /// Get recently viewed events
        /// </summary>
        public List<Event> GetRecentlyViewedEvents()
        {
            return recentlyViewedEvents.Reverse().ToList();
        }

        /// <summary>
        /// Sort events by specified criteria
        /// </summary>
        public List<Event> SortEvents(List<Event> events, string sortBy)
        {
            switch (sortBy.ToLower())
            {
                case "date":
                    return events.OrderBy(e => e.EventDate).ToList();
                case "category":
                    return events.OrderBy(e => e.Category).ThenBy(e => e.EventDate).ToList();
                case "name":
                    return events.OrderBy(e => e.Title).ToList();
                default:
                    return events;
            }
        }
    }
}