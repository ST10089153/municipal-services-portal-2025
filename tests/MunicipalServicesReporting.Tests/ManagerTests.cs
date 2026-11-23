using System;
using System.Linq;
using MunicipalServicesReporting;
using Xunit;

namespace MunicipalServicesReporting.Tests
{
    public class ServiceRequestManagerTests
    {
        private readonly ServiceRequestManager manager = new ServiceRequestManager();

        [Fact]
        public void LoadsSampleRequests()
        {
            Assert.Equal(15, manager.TotalRequests);
        }

        [Fact]
        public void SearchById_UsesBstLookup()
        {
            Assert.Equal("Traffic Signal Malfunction", manager.SearchById("SR007").Title);
            Assert.Null(manager.SearchById("SR999"));
        }

        [Fact]
        public void GetRequestsByPriority_ReturnsOnlyThatPriority()
        {
            var critical = manager.GetRequestsByPriority(Priority.Critical);
            Assert.Equal(new[] { "SR003", "SR007", "SR009", "SR012" }, critical.Select(r => r.RequestId).OrderBy(id => id));
        }

        [Fact]
        public void GetRequestsInDateRange_MatchesLinearFilter()
        {
            var from = DateTime.Now.AddDays(-7);
            var to = DateTime.Now;

            var expected = manager.GetAllRequests()
                .Where(r => r.SubmittedDate.Date >= from.Date && r.SubmittedDate.Date <= to.Date)
                .Select(r => r.RequestId).OrderBy(id => id);

            Assert.Equal(expected, manager.GetRequestsInDateRange(from, to).Select(r => r.RequestId).OrderBy(id => id));
        }

        [Fact]
        public void GetNextPriorityRequest_ReturnsACriticalOpenRequestFirst()
        {
            var next = manager.GetNextPriorityRequest();
            Assert.Equal(Priority.Critical, next.PriorityLevel);
            Assert.NotEqual(RequestStatus.Completed, next.Status);
        }

        [Fact]
        public void GetRelatedRequests_FollowsDependencyChain()
        {
            // SR009 (sewer) -> SR003 (water leak) -> SR001 (pothole) -> SR005 (sidewalk)
            var ids = manager.GetRelatedRequests("SR009").Select(r => r.RequestId);
            Assert.Equal(new[] { "SR009", "SR003", "SR001", "SR005" }, ids);
        }

        [Fact]
        public void SearchRequests_CombinesFilters()
        {
            var results = manager.SearchRequests("Parks & Recreation", RequestStatus.Completed);
            Assert.Equal(new[] { "SR004", "SR013" }, results.Select(r => r.RequestId).OrderBy(id => id));
        }
    }

    public class EventManagerTests
    {
        [Fact]
        public void SearchEvents_FiltersByCategory()
        {
            var results = new EventManager().SearchEvents("Safety");
            Assert.Equal(2, results.Count);
            Assert.All(results, e => Assert.Equal("Safety", e.Category));
        }

        [Fact]
        public void Recommendations_WithNoHistory_AreUpcomingEventsInDateOrder()
        {
            var recs = new EventManager().GetRecommendedEvents();
            Assert.Equal(5, recs.Count);
            Assert.Equal(recs.OrderBy(e => e.EventDate), recs);
            Assert.DoesNotContain(recs, e => e.IsAnnouncement);
        }

        [Fact]
        public void Recommendations_IgnoreAllCategorySearchesWhenRanking()
        {
            var manager = new EventManager();
            // "All categories" is searched most, but it must not push a real category out of the top 3
            for (int i = 0; i < 5; i++) manager.SearchEvents(null);
            manager.SearchEvents("Safety");
            manager.SearchEvents("Culture");
            manager.SearchEvents("Business");

            var categories = manager.GetRecommendedEvents().Select(e => e.Category).Distinct().OrderBy(c => c);
            Assert.Equal(new[] { "Business", "Culture", "Safety" }, categories);
        }
    }
}
