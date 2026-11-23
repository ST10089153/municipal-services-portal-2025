using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Drawing;

namespace MunicipalServicesReporting
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            IssueRepository.EnsureStorage();
            Application.Run(new MainMenuForm());
        }
    }

    // Data Model & Repository
    public class Issue
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public List<string> AttachmentFileNames { get; set; } = new List<string>();
        public int ParticipationPointsAwarded { get; set; } = 0;
    }

    public static class IssueRepository
    {
        static readonly string BaseFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "MunicipalServicesReports");
        static readonly string AttachmentsFolder = Path.Combine(BaseFolder, "attachments");
        static readonly string DataFile = Path.Combine(BaseFolder, "issues.xml");
        static readonly object locker = new object();

        public static string StorageFolder => BaseFolder;
        public static string AttachmentsFolderPath => AttachmentsFolder;
        public static string DataFilePath => DataFile;

        public static void EnsureStorage()
        {
            if (!Directory.Exists(BaseFolder)) Directory.CreateDirectory(BaseFolder);
            if (!Directory.Exists(AttachmentsFolder)) Directory.CreateDirectory(AttachmentsFolder);
            if (!File.Exists(DataFile))
            {
                SaveIssues(new List<Issue>());
            }
        }

        public static List<Issue> LoadIssues()
        {
            lock (locker)
            {
                if (!File.Exists(DataFile)) return new List<Issue>();
                try
                {
                    using (var fs = new FileStream(DataFile, FileMode.Open))
                    {
                        var serializer = new XmlSerializer(typeof(List<Issue>));
                        return (List<Issue>)serializer.Deserialize(fs);
                    }
                }
                catch
                {
                    return new List<Issue>();
                }
            }
        }

        public static void SaveIssues(List<Issue> issues)
        {
            lock (locker)
            {
                using (var fs = new FileStream(DataFile, FileMode.Create))
                {
                    var serializer = new XmlSerializer(typeof(List<Issue>));
                    serializer.Serialize(fs, issues);
                }
            }
        }

        public static void AddIssue(Issue issue)
        {
            var issues = LoadIssues();
            issues.Add(issue);
            SaveIssues(issues);
        }

        public static string CopyAttachmentToStorage(string sourceFilePath)
        {
            if (!File.Exists(sourceFilePath)) return null;
            var fileName = $"{Guid.NewGuid():N}_{Path.GetFileName(sourceFilePath)}";
            var dest = Path.Combine(AttachmentsFolder, fileName);
            File.Copy(sourceFilePath, dest, overwrite: true);
            return fileName;
        }
    }

    // Main Menu Form
    public class MainMenuForm : Form
    {
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel sidebarPanel;
        private Panel footerPanel;

        public MainMenuForm()
        {
            Text = "Municipal Services Portal";
            Width = 1000;
            Height = 720;
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9);
            BackColor = Color.FromArgb(236, 240, 245);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            AutoScaleMode = AutoScaleMode.Dpi;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Premium Sidebar
            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 280,
                BackColor = Color.FromArgb(31, 41, 55)
            };

            Panel logoPanel = new Panel
            {
                Location = new Point(0, 20),
                Size = new Size(280, 130),
                BackColor = Color.Transparent
            };

            Label lblLogoIcon = new Label
            {
                Text = "🏛",
                Font = new Font("Segoe UI", 44),
                ForeColor = Color.FromArgb(96, 165, 250),
                AutoSize = true,
                Location = new Point(110, 5)
            };
            logoPanel.Controls.Add(lblLogoIcon);

            Label lblBrand = new Label
            {
                Text = "MUNICIPAL\nSERVICES",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 75),
                Size = new Size(240, 55)
            };
            logoPanel.Controls.Add(lblBrand);
            sidebarPanel.Controls.Add(logoPanel);

            Panel divider = new Panel
            {
                Location = new Point(30, 160),
                Size = new Size(220, 2),
                BackColor = Color.FromArgb(55, 65, 81)
            };
            sidebarPanel.Controls.Add(divider);

            CreateSidebarStat("📊 Active Reports", "247", new Point(30, 185));
            CreateSidebarStat("📅 Upcoming Events", "19", new Point(30, 245));
            CreateSidebarStat("👥 Community Members", "12.5K", new Point(30, 305));

            Panel helpPanel = new Panel
            {
                Location = new Point(20, 520),
                Size = new Size(240, 120),
                BackColor = Color.FromArgb(45, 55, 72)
            };
            helpPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var path = GetRoundedRect(new Rectangle(0, 0, helpPanel.Width, helpPanel.Height), 8))
                {
                    using (var brush = new SolidBrush(Color.FromArgb(45, 55, 72)))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }
            };

            Label lblNeedHelp = new Label
            {
                Text = "Need Help?",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 15),
                AutoSize = true
            };
            helpPanel.Controls.Add(lblNeedHelp);

            Label lblHelpText = new Label
            {
                Text = "Our support team is available\nMonday - Friday\n8:00 AM - 5:00 PM",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(209, 213, 219),
                Location = new Point(15, 45),
                Size = new Size(210, 60)
            };
            helpPanel.Controls.Add(lblHelpText);
            sidebarPanel.Controls.Add(helpPanel);
            Controls.Add(sidebarPanel);

            // Main Header
            headerPanel = new Panel
            {
                Location = new Point(280, 0),
                Size = new Size(720, 120),
                BackColor = Color.White
            };
            headerPanel.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                {
                    e.Graphics.DrawLine(pen, 0, headerPanel.Height - 1, headerPanel.Width, headerPanel.Height - 1);
                }
            };

            Label lblWelcome = new Label
            {
                Text = "Welcome Back!",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(40, 25),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblWelcome);

            Label lblSubtitle = new Label
            {
                Text = "Select a service to get started with community engagement",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(40, 70),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblSubtitle);
            Controls.Add(headerPanel);

            // Content Area
            contentPanel = new Panel
            {
                Location = new Point(280, 120),
                Size = new Size(720, 510),
                BackColor = Color.FromArgb(236, 240, 245),
                AutoScroll = true
            };

            // Service Cards
            CreatePremiumServiceCard(
                "Report Issues",
                "Submit service requests and track their progress",
                "Report problems like potholes, broken streetlights, water leaks, and more",
                "📋",
                Color.FromArgb(239, 68, 68),
                Color.FromArgb(252, 165, 165),
                40,
                true,
                BtnReportIssues_Click
            );

            CreatePremiumServiceCard(
                "Local Events & Announcements",
                "Discover community activities and stay informed",
                "Browse upcoming events, workshops, and important municipal announcements",
                "📅",
                Color.FromArgb(59, 130, 246),
                Color.FromArgb(147, 197, 253),
                220,
                true,
                BtnEvents_Click
            );

            CreatePremiumServiceCard(
                "Service Request Status",
                "Track and monitor your submitted requests",
                "View real-time updates on issue resolution and estimated completion times",
                "🔍",
                Color.FromArgb(168, 85, 247),
                Color.FromArgb(216, 180, 254),
                400,
                true,
                BtnServiceStatus_Click
            );

            Controls.Add(contentPanel);

            // Footer
            footerPanel = new Panel
            {
                Location = new Point(280, 640),
                Size = new Size(720, 80),
                BackColor = Color.White
            };
            footerPanel.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                {
                    e.Graphics.DrawLine(pen, 0, 0, footerPanel.Width, 0);
                }
            };

            Label lblFooter = new Label
            {
                Text = "© 2025 Municipal Services • Emergency: 0800-MUNICIPALITY • support@municipal.gov.za",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(40, 25),
                AutoSize = true
            };
            footerPanel.Controls.Add(lblFooter);
            Controls.Add(footerPanel);
        }

        private void CreateSidebarStat(string label, string value, Point location)
        {
            Label lblStat = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(156, 163, 175),
                Location = location,
                AutoSize = true
            };
            sidebarPanel.Controls.Add(lblStat);

            Label lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(96, 165, 250),
                Location = new Point(location.X, location.Y + 22),
                AutoSize = true
            };
            sidebarPanel.Controls.Add(lblValue);
        }

        private void CreatePremiumServiceCard(string title, string subtitle, string description, string icon,
            Color primaryColor, Color lightColor, int yPosition, bool enabled, EventHandler clickHandler)
        {
            Panel card = new Panel
            {
                Location = new Point(30, yPosition),
                Size = new Size(660, 160),
                BackColor = Color.White,
                Cursor = enabled ? Cursors.Hand : Cursors.Default
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var shadowPath = GetRoundedRect(new Rectangle(4, 4, card.Width - 4, card.Height - 4), 12))
                {
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                    {
                        e.Graphics.FillPath(shadowBrush, shadowPath);
                    }
                }
                using (var cardPath = GetRoundedRect(new Rectangle(0, 0, card.Width - 8, card.Height - 8), 12))
                {
                    using (var cardBrush = new SolidBrush(enabled ? Color.White : Color.FromArgb(249, 250, 251)))
                    {
                        e.Graphics.FillPath(cardBrush, cardPath);
                    }
                    using (var borderPen = new Pen(Color.FromArgb(229, 231, 235), 1))
                    {
                        e.Graphics.DrawPath(borderPen, cardPath);
                    }
                }
            };

            Panel iconCircle = new Panel
            {
                Location = new Point(25, 25),
                Size = new Size(80, 80),
                BackColor = lightColor
            };
            iconCircle.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(lightColor))
                {
                    e.Graphics.FillEllipse(brush, 0, 0, 80, 80);
                }
            };

            Label lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 32),
                Location = new Point(16, 16),
                Size = new Size(48, 48),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            iconCircle.Controls.Add(lblIcon);
            card.Controls.Add(iconCircle);

            Label lblTitle = new Label
            {
                Text = title,
                UseMnemonic = false, // titles contain "&"
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = enabled ? Color.FromArgb(31, 41, 55) : Color.FromArgb(156, 163, 175),
                Location = new Point(125, 28),
                AutoSize = true
            };
            card.Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = subtitle,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = primaryColor,
                Location = new Point(125, 60),
                Size = new Size(510, 20)
            };
            card.Controls.Add(lblSubtitle);

            Label lblDescription = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(125, 85),
                Size = new Size(510, 40)
            };
            card.Controls.Add(lblDescription);

            if (enabled)
            {
                Label lblArrow = new Label
                {
                    Text = "→",
                    Font = new Font("Segoe UI", 24, FontStyle.Bold),
                    ForeColor = primaryColor,
                    Location = new Point(610, 60),
                    AutoSize = true,
                    BackColor = Color.Transparent
                };
                card.Controls.Add(lblArrow);

                card.MouseEnter += (s, e) =>
                {
                    card.BackColor = Color.FromArgb(249, 250, 251);
                    lblTitle.ForeColor = primaryColor;
                };
                card.MouseLeave += (s, e) =>
                {
                    card.BackColor = Color.White;
                    lblTitle.ForeColor = Color.FromArgb(31, 41, 55);
                };

                if (clickHandler != null)
                {
                    card.Click += clickHandler;
                    iconCircle.Click += clickHandler;
                    lblIcon.Click += clickHandler;
                    lblTitle.Click += clickHandler;
                    lblSubtitle.Click += clickHandler;
                    lblDescription.Click += clickHandler;
                }
            }

            contentPanel.Controls.Add(card);
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

        private void BtnReportIssues_Click(object sender, EventArgs e)
        {
            using (var reportForm = new ReportIssuesForm())
            {
                reportForm.ShowDialog(this);
            }
        }

        private void BtnEvents_Click(object sender, EventArgs e)
        {
            using (var eventsForm = new LocalEventsForm())
            {
                eventsForm.ShowDialog(this);
            }
        }

        private void BtnServiceStatus_Click(object sender, EventArgs e)
        {
            using (var statusForm = new ServiceRequestStatusForm())
            {
                statusForm.ShowDialog(this);
            }
        }
    }

    // Report Issues Form - COMPLETE IMPLEMENTATION
    public class ReportIssuesForm : Form
    {
        private Panel sidebarPanel;
        private Panel headerPanel;
        private Panel mainContentPanel;
        private Panel actionPanel;
        private TextBox txtLocation;
        private ComboBox cmbCategory;
        private RichTextBox rtbDescription;
        private Button btnAttach;
        private ListBox lstAttachments;
        private ProgressBar progressEngagement;
        private Label lblEncouragement;
        private Label lblPoints;
        private Button btnSubmit;
        private Button btnBack;
        private NotifyIcon notifier;
        private int participationPoints = 0;
        // Full paths of files the user picked; they are only copied into storage when the report is submitted
        private readonly List<string> pendingAttachments = new List<string>();

        public ReportIssuesForm()
        {
            Text = "Report Issues - Municipal Services";
            Width = 1100;
            Height = 770;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9);
            BackColor = Color.FromArgb(236, 240, 245);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            AutoScaleMode = AutoScaleMode.Dpi;
            InitializeComponents();
            WireUpEvents();
            UpdateEngagement();
        }

        private void InitializeComponents()
        {
            // Sidebar
            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 280,
                BackColor = Color.FromArgb(31, 41, 55)
            };

            Label lblSidebarTitle = new Label
            {
                Text = "Report Progress",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 30),
                AutoSize = true
            };
            sidebarPanel.Controls.Add(lblSidebarTitle);

            Panel engagementCard = new Panel
            {
                Location = new Point(20, 480),
                Size = new Size(240, 180),
                BackColor = Color.FromArgb(45, 55, 72)
            };

            Label lblEngagementTitle = new Label
            {
                Text = "🎯 Your Progress",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 15),
                AutoSize = true
            };
            engagementCard.Controls.Add(lblEngagementTitle);

            progressEngagement = new ProgressBar
            {
                Location = new Point(15, 50),
                Width = 210,
                Height = 20,
                Style = ProgressBarStyle.Continuous,
                Minimum = 0,
                Maximum = 100,
                Value = 0
            };
            engagementCard.Controls.Add(progressEngagement);

            lblEncouragement = new Label
            {
                Location = new Point(15, 80),
                Width = 210,
                Height = 50,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(209, 213, 219),
                Text = "Let's get started!"
            };
            engagementCard.Controls.Add(lblEncouragement);

            lblPoints = new Label
            {
                Location = new Point(15, 140),
                Width = 210,
                Height = 30,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 211, 153),
                Text = "⭐ Points: 0"
            };
            engagementCard.Controls.Add(lblPoints);

            sidebarPanel.Controls.Add(engagementCard);
            Controls.Add(sidebarPanel);

            // Header
            headerPanel = new Panel
            {
                Location = new Point(280, 0),
                Size = new Size(820, 100),
                BackColor = Color.White
            };

            Label lblTitle = new Label
            {
                Text = "Report an Issue",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(30, 20),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Help us improve your community by reporting service issues",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(30, 60),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblSubtitle);
            Controls.Add(headerPanel);

            // Main Content
            mainContentPanel = new Panel
            {
                Location = new Point(280, 100),
                Size = new Size(820, 570),
                AutoScroll = true,
                BackColor = Color.FromArgb(236, 240, 245)
            };

            int yPos = 30;

            Label lblLocationTitle = new Label
            {
                Text = "📍 Location",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(40, yPos),
                AutoSize = true
            };
            mainContentPanel.Controls.Add(lblLocationTitle);
            yPos += 40;

            txtLocation = new TextBox
            {
                Location = new Point(40, yPos),
                Width = 740,
                Height = 35,
                Font = new Font("Segoe UI", 11)
            };
            mainContentPanel.Controls.Add(txtLocation);
            yPos += 60;

            Label lblCategoryTitle = new Label
            {
                Text = "🏷 Category",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(40, yPos),
                AutoSize = true
            };
            mainContentPanel.Controls.Add(lblCategoryTitle);
            yPos += 40;

            cmbCategory = new ComboBox
            {
                Location = new Point(40, yPos),
                Width = 400,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategory.Items.AddRange(new string[] {
                "Sanitation",
                "Roads & Infrastructure",
                "Water & Electricity",
                "Public Safety",
                "Parks & Recreation",
                "Other"
            });
            if (cmbCategory.Items.Count > 0) cmbCategory.SelectedIndex = 0;
            mainContentPanel.Controls.Add(cmbCategory);
            yPos += 60;

            Label lblDescTitle = new Label
            {
                Text = "📝 Description",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(40, yPos),
                AutoSize = true
            };
            mainContentPanel.Controls.Add(lblDescTitle);
            yPos += 40;

            rtbDescription = new RichTextBox
            {
                Location = new Point(40, yPos),
                Width = 740,
                Height = 160,
                Font = new Font("Segoe UI", 10)
            };
            mainContentPanel.Controls.Add(rtbDescription);
            yPos += 180;

            Label lblAttachTitle = new Label
            {
                Text = "📎 Attachments (Optional)",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(40, yPos),
                AutoSize = true
            };
            mainContentPanel.Controls.Add(lblAttachTitle);
            yPos += 40;

            btnAttach = new Button
            {
                Text = "📤 Choose Files",
                Location = new Point(40, yPos),
                Size = new Size(180, 40),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAttach.FlatAppearance.BorderSize = 0;
            mainContentPanel.Controls.Add(btnAttach);

            lstAttachments = new ListBox
            {
                Location = new Point(240, yPos),
                Width = 540,
                Height = 100,
                Font = new Font("Segoe UI", 9)
            };
            mainContentPanel.Controls.Add(lstAttachments);

            Controls.Add(mainContentPanel);

            // Action Panel
            actionPanel = new Panel
            {
                Location = new Point(280, 670),
                Size = new Size(820, 80),
                BackColor = Color.White
            };

            btnBack = new Button
            {
                Text = "← Back to Menu",
                Location = new Point(30, 20),
                Size = new Size(160, 40),
                BackColor = Color.FromArgb(107, 114, 128),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderSize = 0;
            actionPanel.Controls.Add(btnBack);

            btnSubmit = new Button
            {
                Text = "✓ Submit Report",
                Location = new Point(630, 20),
                Size = new Size(160, 40),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSubmit.FlatAppearance.BorderSize = 0;
            actionPanel.Controls.Add(btnSubmit);

            Controls.Add(actionPanel);

            // NotifyIcon
            notifier = new NotifyIcon
            {
                Icon = SystemIcons.Information,
                Visible = true,
                Text = "Municipal Services"
            };
        }

        private void WireUpEvents()
        {
            txtLocation.TextChanged += (s, e) => UpdateEngagement();
            cmbCategory.SelectedIndexChanged += (s, e) => UpdateEngagement();
            rtbDescription.TextChanged += (s, e) => UpdateEngagement();
            btnAttach.Click += BtnAttach_Click;
            btnSubmit.Click += BtnSubmit_Click;
            btnBack.Click += (s, e) => Close();
        }

        private void BtnAttach_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Multiselect = true;
                ofd.Filter = "Images and Documents|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.pdf;*.doc;*.docx;*.txt|All Files|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (var file in ofd.FileNames)
                    {
                        if (pendingAttachments.Contains(file)) continue;
                        pendingAttachments.Add(file);
                        lstAttachments.Items.Add("📎 " + Path.GetFileName(file));
                    }
                    UpdateEngagement();
                }
            }
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                MessageBox.Show("Please enter the location of the issue.",
                    "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLocation.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(rtbDescription.Text))
            {
                MessageBox.Show("Please provide a description of the issue.",
                    "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rtbDescription.Focus();
                return;
            }

            var issue = new Issue
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                Location = txtLocation.Text.Trim(),
                Category = cmbCategory.SelectedItem?.ToString() ?? "Unspecified",
                Description = rtbDescription.Text.Trim(),
                ParticipationPointsAwarded = 5
            };

            try
            {
                // Copy attachments now and record the stored (unique) file names so the
                // saved report points at the files in the attachments folder
                foreach (var file in pendingAttachments)
                {
                    var storedName = IssueRepository.CopyAttachmentToStorage(file);
                    if (storedName != null) issue.AttachmentFileNames.Add(storedName);
                }

                IssueRepository.AddIssue(issue);
                participationPoints += issue.ParticipationPointsAwarded;
                lblPoints.Text = $"⭐ Points: {participationPoints}";

                MessageBox.Show(
                    "✓ Success!\n\n" +
                    "Your report has been submitted successfully.\n" +
                    "Our team will review it shortly.\n\n" +
                    "Thank you for helping improve our community!",
                    "Report Submitted",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                notifier.BalloonTipTitle = "Report Submitted";
                notifier.BalloonTipText = $"Your {issue.Category} report has been recorded.";
                notifier.ShowBalloonTip(3000);

                // Reset form
                txtLocation.Clear();
                rtbDescription.Clear();
                lstAttachments.Items.Clear();
                pendingAttachments.Clear();
                cmbCategory.SelectedIndex = 0;
                UpdateEngagement();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save report:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateEngagement()
        {
            int score = 0;
            if (!string.IsNullOrWhiteSpace(txtLocation.Text)) score += 30;
            if (!string.IsNullOrWhiteSpace(rtbDescription.Text)) score += 50;
            if (lstAttachments.Items.Count > 0) score += 20;
            if (score > 100) score = 100;

            progressEngagement.Value = score;

            if (score < 30)
            {
                lblEncouragement.Text = "Start by entering the location";
                lblEncouragement.ForeColor = Color.FromArgb(248, 113, 113);
            }
            else if (score < 60)
            {
                lblEncouragement.Text = "Great! Now add details";
                lblEncouragement.ForeColor = Color.FromArgb(251, 191, 36);
            }
            else if (score < 90)
            {
                lblEncouragement.Text = "Almost there! Add photos?";
                lblEncouragement.ForeColor = Color.FromArgb(96, 165, 250);
            }
            else
            {
                lblEncouragement.Text = "Perfect! Ready to submit";
                lblEncouragement.ForeColor = Color.FromArgb(52, 211, 153);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            notifier.Visible = false;
            notifier.Dispose();
        }
    }
}