# Municipal Services Application – Part 2

##  Overview
The **Municipal Services Application** is a comprehensive Windows Forms desktop system designed to enhance citizen engagement with local government services in South Africa.  
**Part 2** focuses on the **Local Events and Announcements** module, featuring data-driven insights, intuitive navigation, and professional UI design.

---

##  Key Highlights
- **19+ Pre-loaded Events** covering diverse community activities  
- **Advanced Data Structures:** SortedDictionaries, Queues, HashSets, Dictionaries  
- **Intelligent Recommendation System** based on user search behavior  
- **Professional UI Design** with dark sidebars and color-coded categories  
- **Real-time Statistics** and feedback panels  
- **Seamless Navigation** between modules  

---

##  Features

###  Part 1 (Previously Implemented)
**Report Issues Module**
- Submit municipal service issues with location, category, and description  
- Attach photos and documents  
- Track issue progress with engagement feedback  
- Participation points system  
- Desktop notifications  

###  Part 2 (Current Implementation)
**Local Events & Announcements**
- Browse 19+ community events and announcements  
- Advanced search by category and/or date  
- Sort by Date, Category, or Name  
- Personalized Recommendations based on user activity  
- Color-coded Categories for organization  
- Detailed event information modals  
- Real-time statistics dashboard  

###  Part 3 (Upcoming)
**Service Request Status Tracking** *(Coming Soon)*  

---

##  Technical Implementation

| Component | Details |
|------------|----------|
| **Language** | C# |
| **Framework** | .NET Framework 4.7.2 |
| **UI Framework** | Windows Forms |
| **Architecture** | Object-Oriented Programming (OOP) |
| **Design Pattern** | Model–View separation |

---

##  Installation & Setup

###  Prerequisites
- **.NET Framework:** 4.7.2 or higher  
- **IDE:** Visual Studio 2017 or later (recommended)

###  Step-by-Step Installation

#### Option 1: Visual Studio
1. **Clone or Download Repository**  
   ```bash
   https://github.com/VCPTA/bca3-prog7312-part-2-submission-ST10089153.git

2. **Open Solution**
   - Double-click `MunicipalServicesReporting.sln`

3. **Restore NuGet Packages** (if any)
   - Right-click Solution → Restore NuGet Packages

4. **Build Solution**
   - Build → Rebuild Solution (or press `Ctrl+Shift+B`)

5. **Run Application**
   - Press `F5` or click "Start"

#### Option 2: Command Line

1. **Navigate to Project Directory**
   ```bash
   cd MunicipalServicesReporting
   ```

2. **Build Project**
   ```bash
   msbuild MunicipalServicesReporting.sln /p:Configuration=Release
   ```

3. **Run Executable**
   ```bash
   bin\Release\MunicipalServicesReporting.exe
   ```

### Troubleshooting Installation

**Issue:** Build errors related to .NET Framework version  
**Solution:** Install .NET Framework 4.7.2 Developer Pack  
**Download:** https://dotnet.microsoft.com/download/dotnet-framework/net472

**Issue:** Missing references  
**Solution:** Clean and rebuild solution  
- Build → Clean Solution → Rebuild Solution

## User Guide

### Main Menu

Upon launching, you'll see three service options:

1. **Report Issues** - Submit municipal service problems
2. **Local Events & Announcements** - Browse community events (Active)
3. **Service Request Status** - Track submissions (Coming in Part 3)

### Using Local Events & Announcements

#### Viewing Events

1. Click "Local Events & Announcements" from main menu
2. All 19 events display automatically in the list
3. **Statistics Panel** at top shows:
   - Total Events count
   - Announcements count
   - Events this week
   - Number of categories

**Special Indicators:**
- 🔴 **Red Background + Bold Text** =  Important Announcement
- 🟡 **Colored Background** =  Event happening this week
- ⚪ **White Background** =  Future event

#### Search Functionality

**Search by Category:**
1. Select category from dropdown (e.g., "Sports")
2. Click "🔍 Search"
3. View filtered results

**Search by Date:**
1. Check "Filter by Date" checkbox
2. Select date using date picker
3. Click "🔍 Search"

**Combined Search:**
1. Select both category AND date
2. Click "🔍 Search"
3. View events matching both criteria

**Clear Filters:**
- Click "Clear" button to reset all filters and show all events

#### Sorting Events

Use the "Sort By" dropdown to organize events:
- **Date** - Chronological order (default)
- **Category** - Alphabetically by category, then by date
- **Name** - Alphabetically by event title

Results update automatically upon selection.

#### Getting Recommendations

1. Search for events in different categories (try 3-5 searches)
2. Click " Recommendations" button
3. System analyzes your search history
4. Displays 6 personalized event suggestions
5. Recommendation panel appears showing algorithm insights

**How Recommendations Work:**
- Tracks which categories you search most
- Suggests upcoming events from your favorite categories
- Updates dynamically as you search more

#### Viewing Event Details

1. Double-click any event in the list
2. Detailed modal window opens showing:
   - Full date (e.g., "Monday, 15 January 2025")
   - Category
   - Location
   - Organizer
   - Complete description
3. Click "Close" to return to event list

#### Navigation

- **← Back to Menu** button (bottom left) returns to main menu
- All data and filter states are preserved during navigation

## Project Structure

```
MunicipalServicesReporting/
│
├── MunicipalServicesReporting.sln     # Solution file
│
├── MunicipalServicesReporting/        # Main project folder
│   │
│   ├── Program.cs                     # Application entry point
│   │   ├── Program class              # Main() method
│   │   ├── Issue class                # Issue data model
│   │   ├── IssueRepository class      # Data persistence
│   │   ├── MainMenuForm class         # Main navigation menu
│   │   └── ReportIssuesForm class     # Report issues feature
│   │
│   ├── Event.cs                       # Event data model
│   ├── EventManager.cs                # Event management & data structures
│   ├── LocalEventsForm.cs             # Events & announcements UI
│   │
│   ├── App.config                     # Application configuration
│   ├── README.md                      # This file
│   │
│   └── Properties/
│       ├── AssemblyInfo.cs            # Assembly metadata
│       ├── Resources.resx             # Application resources
│       └── Settings.settings          # User settings
│
└── Documentation/
    ├── IMPLEMENTATION_GUIDE.md        # Setup instructions
    ├── DataStructures_Reference.md    # Technical documentation
    └── TROUBLESHOOTING.md             # Common issues & fixes
```

## Key Files Description

| File | Purpose | Lines of Code |
|------|---------|---------------|
| `Program.cs` | Application entry, main menu, report issues | ~600 |
| `Event.cs` | Event data model class | ~35 |
| `EventManager.cs` | Data structures & business logic | ~250 |
| `LocalEventsForm.cs` | Events UI & user interactions | ~700 |

## Demonstration

**Video Demonstration:** [Click here to watch the application walkthrough](https://youtu.be/oSRtPhYFKbc?si=f5vHwKycou-_iIM4)



## Contributing

This project is part of an academic submission. For any questions or suggestions, please contact the repository owner.

## License

This project is submitted as part of academic coursework at Varsity College.

## References

Microsoft Corporation (2023) *Windows Forms documentation*. Available at: https://docs.microsoft.com/en-us/dotnet/desktop/winforms/ (Accessed: 13 October 2025).

Microsoft Corporation (2023) *C# programming guide*. Available at: https://docs.microsoft.com/en-us/dotnet/csharp/ (Accessed: 13 October 2025).

Microsoft Corporation (2022) *.NET Framework 4.7.2 documentation*. Available at: https://docs.microsoft.com/en-us/dotnet/framework/ (Accessed: 13 October 2025).

Troelsen, A. and Japikse, P. (2021) *Pro C# 9 with .NET 5: foundational principles and practices in programming*. 10th edn. Berkeley: Apress.

Liberty, J. and MacDonald, B. (2020) *Learning C# 3.0: master the fundamentals of C# 3.0*. Sebastopol: O'Reilly Media.

Skeet, J. (2019) *C# in depth*. 4th edn. Shelter Island: Manning Publications.

Stellman, A. and Greene, J. (2020) *Head first C#: a learner's guide to real-world programming with C# and .NET Core*. 4th edn. Sebastopol: O'Reilly Media.

Albahari, J. and Albahari, B. (2022) *C# 10 in a nutshell: the definitive reference*. Sebastopol: O'Reilly Media.

Price, M.J. (2021) *C# 9 and .NET 5 - modern cross-platform development*. 5th edn. Birmingham: Packt Publishing.

---

---

**Made with ❤️ for South African Communities**

