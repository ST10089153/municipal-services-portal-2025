using System;
using System.Collections.Generic;
namespace MunicipalServicesReporting
{
    /// <summary>
    /// Represents a service request with tracking capabilities
    /// </summary>
    public class ServiceRequest
    {
        public string RequestId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime SubmittedDate { get; set; }
        public RequestStatus Status { get; set; }
        public Priority PriorityLevel { get; set; }
        public string AssignedDepartment { get; set; }
        public DateTime? EstimatedCompletionDate { get; set; }
        public DateTime? ActualCompletionDate { get; set; }
        public List<string> Updates { get; set; }
        public List<string> Dependencies { get; set; } // For graph relationships
        public int CitizenImpactScore { get; set; } // Number of affected citizens
        public ServiceRequest()
        {
            Updates = new List<string>();
            Dependencies = new List<string>();
        }
        public ServiceRequest(string requestId, string title, string category,
        string description, string location, DateTime submittedDate,
        RequestStatus status, Priority priority, string department)
        {
            RequestId = requestId;
            Title = title;
            Category = category;
            Description = description;
            Location = location;
            SubmittedDate = submittedDate;
            Status = status;
            PriorityLevel = priority;
            AssignedDepartment = department;
            Updates = new List<string>();
            Dependencies = new List<string>();
        }
        public override string ToString()
        {
            return $"{RequestId} - {Title} [{Status}]";
        }
    }
    public enum RequestStatus
    {
        Submitted,
        UnderReview,
        InProgress,
        OnHold,
        Completed,
        Rejected
    }
    public enum Priority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }
}