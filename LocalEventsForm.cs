using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MunicipalServicesReporting
{
    public partial class LocalEventsForm : Form
    {
        private EventManager eventManager;
        private Panel headerPanel;
        private Panel filterPanel;
        private Panel statsPanel;
        private Panel recommendationPanel;
        private ComboBox cmbCategory;
        private DateTimePicker dtpEventDate;
        private ComboBox cmbSortBy;
        private Button btnSearch;
        private Button btnClear;
        private Button btnShowRecommended;
        private CheckBox chkFilterByDate;
        private ListView lvEvents;
        private Label lblRecommendations;
        private Label lblEventCount;
        private Label lblUpcomingCount;
        private Button btnBack;

        public LocalEventsForm()
        {
            InitializeComponent();
            eventManager = new EventManager();
            SetupUI();
            LoadAllEvents();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Local Events & Announcements";
            this.Size = new Size(1400, 920);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(236, 240, 245);
            this.Font = new Font("Segoe UI", 9);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ResumeLayout(false);
        }

        private void SetupUI()
        {
            // Header
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.White
            };
            headerPanel.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                {
                    e.Graphics.DrawLine(pen, 0, headerPanel.Height - 1, headerPanel.Width, headerPanel.Height - 1);
                }
            };

            Label lblIcon = new Label
            {
                Text = "📅",
                Font = new Font("Segoe UI", 38),
                ForeColor = Color.FromArgb(59, 130, 246),
                Location = new Point(1270, 40),  // Moved to right
                AutoSize = true
            };
            headerPanel.Controls.Add(lblIcon);

            Label lblTitle = new Label
            {
                Text = "Local Events & Announcements",
                UseMnemonic = false, // show "&" literally instead of as a keyboard-shortcut marker
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                AutoSize = true
            };
            lblTitle.Location = new Point(1270 - lblTitle.PreferredWidth - 55, 30);
            headerPanel.Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Using SortedDictionary, Queue, HashSet & Dictionary for smart recommendations",
                UseMnemonic = false, // show "&" literally instead of as a keyboard-shortcut marker
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = true
            };
            lblSubtitle.Location = new Point(1270 - lblSubtitle.PreferredWidth - 55, 75);
            headerPanel.Controls.Add(lblSubtitle);

            this.Controls.Add(headerPanel);

            // Stats Panel
            statsPanel = new Panel
            {
                Location = new Point(30, 140),
                Size = new Size(1340, 100),
                BackColor = Color.White
            };
            statsPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var path = GetRoundedRect(new Rectangle(0, 0, statsPanel.Width, statsPanel.Height), 12))
                {
                    using (var brush = new SolidBrush(Color.White))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                    using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            CreateStatCard("🎉", "Total Events", "0", Color.FromArgb(59, 130, 246), new Point(40, 20));
            CreateStatCard("📢", "Announcements", "0", Color.FromArgb(239, 68, 68), new Point(380, 20));
            CreateStatCard("⏰", "This Week", "0", Color.FromArgb(168, 85, 247), new Point(720, 20));
            CreateStatCard("🏷", "Categories", "0", Color.FromArgb(34, 197, 94), new Point(1060, 20));

            this.Controls.Add(statsPanel);

            // Filter Panel
            filterPanel = new Panel
            {
                Location = new Point(30, 260),
                Size = new Size(1340, 140),
                BackColor = Color.White
            };
            filterPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var path = GetRoundedRect(new Rectangle(0, 0, filterPanel.Width, filterPanel.Height), 12))
                {
                    using (var brush = new SolidBrush(Color.White))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                    using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            Label lblFilterTitle = new Label
            {
                Text = "🔍 Search & Filter (SortedDictionary for date-based retrieval)",
                UseMnemonic = false, // show "&" literally instead of as a keyboard-shortcut marker
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(25, 20),
                AutoSize = true
            };
            filterPanel.Controls.Add(lblFilterTitle);

            Label lblCat = new Label
            {
                Text = "Category (HashSet)",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(30, 60),
                AutoSize = true
            };
            filterPanel.Controls.Add(lblCat);

            cmbCategory = new ComboBox
            {
                Location = new Point(30, 82),
                Size = new Size(250, 32),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat
            };
            cmbCategory.Items.Add("All Categories");
            cmbCategory.SelectedIndex = 0;
            filterPanel.Controls.Add(cmbCategory);

            chkFilterByDate = new CheckBox
            {
                Text = "Filter by Date (SortedDictionary)",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(310, 60),
                AutoSize = true
            };
            filterPanel.Controls.Add(chkFilterByDate);

            dtpEventDate = new DateTimePicker
            {
                Location = new Point(310, 82),
                Size = new Size(250, 32),
                Font = new Font("Segoe UI", 10),
                Enabled = false
            };
            chkFilterByDate.CheckedChanged += (s, e) => dtpEventDate.Enabled = chkFilterByDate.Checked;
            filterPanel.Controls.Add(dtpEventDate);

            Label lblSort = new Label
            {
                Text = "Sort By",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(590, 60),
                AutoSize = true
            };
            filterPanel.Controls.Add(lblSort);

            cmbSortBy = new ComboBox
            {
                Location = new Point(590, 82),
                Size = new Size(200, 32),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat
            };
            cmbSortBy.Items.AddRange(new string[] { "Date", "Category", "Name" });
            cmbSortBy.SelectedIndex = 0;
            cmbSortBy.SelectedIndexChanged += CmbSortBy_SelectedIndexChanged;
            filterPanel.Controls.Add(cmbSortBy);

            btnSearch = CreateStyledButton("🔍 Search", new Point(850, 82), 150, Color.FromArgb(34, 197, 94));
            btnSearch.Click += BtnSearch_Click;
            filterPanel.Controls.Add(btnSearch);

            btnClear = CreateStyledButton("Clear", new Point(1020, 82), 120, Color.FromArgb(107, 114, 128));
            btnClear.Click += BtnClear_Click;
            filterPanel.Controls.Add(btnClear);

            btnShowRecommended = CreateStyledButton("⭐ Recommendations", new Point(1160, 82), 170, Color.FromArgb(245, 158, 11));
            btnShowRecommended.Click += BtnShowRecommended_Click;
            filterPanel.Controls.Add(btnShowRecommended);

            this.Controls.Add(filterPanel);

            // Recommendation Panel
            recommendationPanel = new Panel
            {
                Location = new Point(30, 420),
                Size = new Size(1340, 0),
                BackColor = Color.FromArgb(254, 252, 232),
                Visible = false
            };
            recommendationPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var path = GetRoundedRect(new Rectangle(0, 0, recommendationPanel.Width, recommendationPanel.Height), 12))
                {
                    using (var brush = new SolidBrush(Color.FromArgb(254, 252, 232)))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                    using (var pen = new Pen(Color.FromArgb(245, 158, 11), 2))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            Label lblStarIcon = new Label
            {
                Text = "⭐",
                Font = new Font("Segoe UI", 32),
                ForeColor = Color.FromArgb(245, 158, 11),
                Location = new Point(25, 20),
                AutoSize = true
            };
            recommendationPanel.Controls.Add(lblStarIcon);

            lblRecommendations = new Label
            {
                Location = new Point(85, 25),
                Size = new Size(1230, 60),
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(120, 53, 15)
            };
            recommendationPanel.Controls.Add(lblRecommendations);

            this.Controls.Add(recommendationPanel);

            // Events ListView
            lvEvents = new ListView
            {
                Location = new Point(30, 420),
                Size = new Size(1340, 380),
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                Font = new Font("Segoe UI", 9),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            lvEvents.Columns.Add("", 50);
            lvEvents.Columns.Add("Event Title", 320);
            lvEvents.Columns.Add("Date", 130);
            lvEvents.Columns.Add("Category", 140);
            lvEvents.Columns.Add("Location", 220);
            lvEvents.Columns.Add("Organizer", 200);
            lvEvents.Columns.Add("Description", 270);
            lvEvents.DoubleClick += LvEvents_DoubleClick;
            this.Controls.Add(lvEvents);

            // Footer
            Panel footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.White
            };
            footerPanel.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                {
                    e.Graphics.DrawLine(pen, 0, 0, footerPanel.Width, 0);
                }
            };

            btnBack = CreateStyledButton("← Back to Menu", new Point(30, 15), 180, Color.FromArgb(107, 114, 128));
            btnBack.Click += (s, e) => this.Close();
            footerPanel.Controls.Add(btnBack);

            Label lblTip = new Label
            {
                Text = "💡 Data Structures: SortedDictionary (dates), Queue (recent views), HashSet (categories), Dictionary (search patterns)",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(240, 25),
                AutoSize = true
            };
            footerPanel.Controls.Add(lblTip);

            this.Controls.Add(footerPanel);

            LoadCategories();
        }

        private void CreateStatCard(string icon, string label, string value, Color accentColor, Point location)
        {
            Panel card = new Panel
            {
                Location = location,
                Size = new Size(300, 60),
                BackColor = Color.FromArgb(249, 250, 251)
            };
            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var path = GetRoundedRect(new Rectangle(0, 0, card.Width, card.Height), 8))
                {
                    using (var brush = new SolidBrush(Color.FromArgb(249, 250, 251)))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }
                using (var brush = new SolidBrush(accentColor))
                {
                    e.Graphics.FillRectangle(brush, 0, 0, 4, card.Height);
                }
            };

            Label lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 24),
                ForeColor = accentColor,
                Location = new Point(15, 12),
                AutoSize = false,
                Size = new Size(52, 40), // fixed width so the icon cannot overlap the label text at x=70
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(lblIcon);

            Label lblLabel = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(70, 15),
                AutoSize = true
            };
            card.Controls.Add(lblLabel);

            if (label == "Total Events")
            {
                lblEventCount = new Label
                {
                    Text = value,
                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                    ForeColor = Color.FromArgb(31, 41, 55),
                    Location = new Point(70, 32),
                    AutoSize = true
                };
                card.Controls.Add(lblEventCount);
            }
            else if (label == "This Week")
            {
                lblUpcomingCount = new Label
                {
                    Text = value,
                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                    ForeColor = Color.FromArgb(31, 41, 55),
                    Location = new Point(70, 32),
                    AutoSize = true
                };
                card.Controls.Add(lblUpcomingCount);
            }
            else
            {
                Label lblValue = new Label
                {
                    Text = value,
                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                    ForeColor = Color.FromArgb(31, 41, 55),
                    Location = new Point(70, 32),
                    AutoSize = true,
                    Name = "stat_" + label
                };
                card.Controls.Add(lblValue);
            }

            statsPanel.Controls.Add(card);
        }

        private Button CreateStyledButton(string text, Point location, int width, Color backgroundColor)
        {
            Button btn = new Button
            {
                Text = text,
                Location = location,
                Size = new Size(width, 38),
                BackColor = backgroundColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backgroundColor, 0.1f);
            btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backgroundColor, 0.05f);
            return btn;
        }

        private System.Drawing.Drawing2D.GraphicsPath GetRoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void LoadCategories()
        {
            // HashSet ensures unique categories - O(1) lookup
            var categories = eventManager.GetAllCategories();
            foreach (var category in categories)
            {
                cmbCategory.Items.Add(category);
            }

            foreach (Control ctrl in statsPanel.Controls)
            {
                if (ctrl is Panel panel)
                {
                    foreach (Control inner in panel.Controls)
                    {
                        if (inner is Label lbl && lbl.Name == "stat_Categories")
                        {
                            lbl.Text = categories.Count.ToString();
                        }
                    }
                }
            }
        }

        private void LoadAllEvents()
        {
            var events = eventManager.GetAllEvents();
            DisplayEvents(events);
            UpdateStats(events);
        }

        private void UpdateStats(List<Event> events)
        {
            lblEventCount.Text = events.Count.ToString();

            int announcements = 0;
            int thisWeek = 0;

            foreach (var evt in events)
            {
                if (evt.IsAnnouncement)
                    announcements++;

                if (evt.EventDate >= DateTime.Now && evt.EventDate < DateTime.Now.AddDays(7))
                    thisWeek++;
            }

            lblUpcomingCount.Text = thisWeek.ToString();

            foreach (Control ctrl in statsPanel.Controls)
            {
                if (ctrl is Panel panel)
                {
                    foreach (Control inner in panel.Controls)
                    {
                        if (inner is Label lbl && lbl.Name == "stat_Announcements")
                        {
                            lbl.Text = announcements.ToString();
                        }
                    }
                }
            }
        }

        private void DisplayEvents(List<Event> events)
        {
            lvEvents.Items.Clear();

            events = eventManager.SortEvents(events, cmbSortBy.SelectedItem.ToString());

            foreach (var evt in events)
            {
                string icon = evt.IsAnnouncement ? "📢" : "🎉";

                ListViewItem item = new ListViewItem(icon);
                item.SubItems.Add(evt.Title);
                item.SubItems.Add(evt.EventDate.ToString("ddd, dd MMM yyyy"));
                item.SubItems.Add(evt.Category);
                item.SubItems.Add(evt.Location);
                item.SubItems.Add(evt.Organizer);
                item.SubItems.Add(evt.Description.Length > 65 ? evt.Description.Substring(0, 62) + "..." : evt.Description);

                item.Tag = evt;

                // Color-code by type and category
                if (evt.IsAnnouncement)
                {
                    item.BackColor = Color.FromArgb(254, 242, 242);
                    item.ForeColor = Color.FromArgb(185, 28, 28);
                    item.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                }
                else if (evt.EventDate < DateTime.Now.AddDays(7))
                {
                    item.BackColor = GetCategoryColor(evt.Category, true);
                    item.ForeColor = GetCategoryColor(evt.Category, false);
                }
                else
                {
                    item.BackColor = Color.FromArgb(252, 252, 252);
                    item.ForeColor = GetCategoryColor(evt.Category, false);
                }

                lvEvents.Items.Add(item);
            }

            UpdateStats(events);
        }

        private Color GetCategoryColor(string category, bool isLight)
        {
            switch (category.ToLower())
            {
                case "community":
                    return isLight ? Color.FromArgb(239, 246, 255) : Color.FromArgb(29, 78, 216);
                case "government":
                    return isLight ? Color.FromArgb(243, 232, 255) : Color.FromArgb(126, 34, 206);
                case "sports":
                    return isLight ? Color.FromArgb(236, 253, 245) : Color.FromArgb(5, 150, 105);
                case "health":
                    return isLight ? Color.FromArgb(254, 242, 242) : Color.FromArgb(220, 38, 38);
                case "market":
                    return isLight ? Color.FromArgb(255, 251, 235) : Color.FromArgb(217, 119, 6);
                case "culture":
                    return isLight ? Color.FromArgb(252, 231, 243) : Color.FromArgb(190, 24, 93);
                case "environment":
                    return isLight ? Color.FromArgb(240, 253, 244) : Color.FromArgb(22, 163, 74);
                case "entertainment":
                    return isLight ? Color.FromArgb(254, 249, 195) : Color.FromArgb(161, 98, 7);
                case "business":
                    return isLight ? Color.FromArgb(241, 245, 249) : Color.FromArgb(71, 85, 105);
                case "safety":
                    return isLight ? Color.FromArgb(255, 237, 213) : Color.FromArgb(234, 88, 12);
                case "education":
                    return isLight ? Color.FromArgb(224, 242, 254) : Color.FromArgb(3, 105, 161);
                case "infrastructure":
                    return isLight ? Color.FromArgb(250, 245, 255) : Color.FromArgb(109, 40, 217);
                case "utilities":
                    return isLight ? Color.FromArgb(236, 254, 255) : Color.FromArgb(8, 145, 178);
                default:
                    return isLight ? Color.FromArgb(243, 244, 246) : Color.FromArgb(75, 85, 99);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string category = cmbCategory.SelectedItem.ToString();
            DateTime? date = chkFilterByDate.Checked ? dtpEventDate.Value.Date : (DateTime?)null;

            // SortedDictionary used internally for efficient date-based retrieval - O(log n)
            var results = eventManager.SearchEvents(
                category == "All Categories" ? null : category,
                date
            );

            DisplayEvents(results);
            HideRecommendations();

            if (results.Count == 0)
            {
                MessageBox.Show(
                    "No events found matching your search criteria.\n\n" +
                    "Try adjusting your filters or selecting a different category.",
                    "No Results",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            else if (date.HasValue)
            {
                MessageBox.Show(
                    $"✓ Search completed using SortedDictionary!\n\n" +
                    $"Data Structure: SortedDictionary<DateTime, List<Event>>\n" +
                    $"Complexity: O(log n) for date-based retrieval\n" +
                    $"Found: {results.Count} event(s) on {date.Value:dd MMM yyyy}",
                    "SortedDictionary Search",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            cmbCategory.SelectedIndex = 0;
            chkFilterByDate.Checked = false;
            dtpEventDate.Value = DateTime.Now;
            LoadAllEvents();
            HideRecommendations();
        }

        private void BtnShowRecommended_Click(object sender, EventArgs e)
        {
            // Uses Dictionary to track search patterns and recommend based on user behavior
            var recommendations = eventManager.GetRecommendedEvents();

            if (recommendations.Count == 0)
            {
                MessageBox.Show(
                    "🔍 Build Your Personalized Recommendations\n\n" +
                    "Search for events by category to help us understand your interests.\n" +
                    "The more you explore, the better our recommendations become!\n\n" +
                    "Data Structure: Dictionary<string, int> tracks your search patterns\n" +
                    "Start by using the search filters above.",
                    "No Recommendations Yet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            DisplayEvents(recommendations);
            ShowRecommendations(recommendations);

            MessageBox.Show(
                $"✓ Recommendations generated using Dictionary!\n\n" +
                $"Data Structure: Dictionary<string, int> for pattern tracking\n" +
                $"Queue: Stores last 10 viewed events\n" +
                $"Algorithm: Analyzes search frequency and suggests relevant events\n\n" +
                $"Found: {recommendations.Count} personalized recommendations",
                "Smart Recommendations",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void ShowRecommendations(List<Event> recommendations)
        {
            string message = $"PERSONALIZED FOR YOU: Based on your search history, we've curated " +
                            $"{recommendations.Count} events that match your interests. " +
                            "These recommendations are tailored to the categories you explore most often.";

            lblRecommendations.Text = message;
            recommendationPanel.Size = new Size(1340, 110);
            recommendationPanel.Visible = true;

            lvEvents.Location = new Point(30, 540);
            lvEvents.Size = new Size(1340, 260);
        }

        private void HideRecommendations()
        {
            recommendationPanel.Visible = false;
            lvEvents.Location = new Point(30, 420);
            lvEvents.Size = new Size(1340, 380);
        }

        private void CmbSortBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            var currentEvents = new List<Event>();
            foreach (ListViewItem item in lvEvents.Items)
            {
                currentEvents.Add((Event)item.Tag);
            }
            DisplayEvents(currentEvents);
        }

        private void LvEvents_DoubleClick(object sender, EventArgs e)
        {
            if (lvEvents.SelectedItems.Count > 0)
            {
                Event selectedEvent = (Event)lvEvents.SelectedItems[0].Tag;
                
                // Queue tracks recently viewed events - FIFO structure
                eventManager.RecordEventView(selectedEvent);
                
                ShowEventDetails(selectedEvent);
            }
        }

        private void ShowEventDetails(Event evt)
        {
            Form detailForm = new Form
            {
                Text = "Event Details",
                Size = new Size(700, 600),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(249, 250, 251)
            };

            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = evt.IsAnnouncement ? Color.FromArgb(239, 68, 68) : Color.FromArgb(59, 130, 246)
            };

            Label lblType = new Label
            {
                Text = evt.IsAnnouncement ? "📢 IMPORTANT ANNOUNCEMENT" : "🎉 COMMUNITY EVENT",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 25),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(lblType);

            Label lblEventTitle = new Label
            {
                Text = evt.Title,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(255, 255, 255, 200),
                Location = new Point(30, 58),
                Size = new Size(640, 30),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(lblEventTitle);

            detailForm.Controls.Add(headerPanel);

            Panel contentPanel = new Panel
            {
                Location = new Point(30, 120),
                Size = new Size(640, 380),
                BackColor = Color.White,
                AutoScroll = true
            };
            contentPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var path = GetRoundedRect(new Rectangle(0, 0, contentPanel.Width, contentPanel.Height), 12))
                {
                    using (var brush = new SolidBrush(Color.White))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                    using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            int yPos = 30;
            CreateDetailRow(contentPanel, "📅 Date", evt.EventDate.ToString("dddd, dd MMMM yyyy"), ref yPos);
            CreateDetailRow(contentPanel, "🏷 Category", evt.Category, ref yPos);
            CreateDetailRow(contentPanel, "📍 Location", evt.Location, ref yPos);
            CreateDetailRow(contentPanel, "👤 Organizer", evt.Organizer, ref yPos);

            Label lblDescTitle = new Label
            {
                Text = "📋 Description",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(30, yPos),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblDescTitle);
            yPos += 30;

            Label lblDesc = new Label
            {
                Text = evt.Description,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(75, 85, 99),
                Location = new Point(30, yPos),
                Size = new Size(580, 0),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblDesc);

            detailForm.Controls.Add(contentPanel);

            Button btnClose = CreateStyledButton("Close", new Point(530, 520), 140, Color.FromArgb(107, 114, 128));
            btnClose.Click += (s, e) => detailForm.Close();
            detailForm.Controls.Add(btnClose);

            detailForm.ShowDialog();
        }

        private void CreateDetailRow(Panel parent, string label, string value, ref int yPos)
        {
            Label lblLabel = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(30, yPos),
                AutoSize = true
            };
            parent.Controls.Add(lblLabel);

            Label lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(75, 85, 99),
                Location = new Point(200, yPos),
                Size = new Size(410, 30),
                AutoSize = false
            };
            parent.Controls.Add(lblValue);

            yPos += 50;
        }
    }
}