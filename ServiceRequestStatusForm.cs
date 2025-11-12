using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MunicipalServicesReporting
{
    public partial class ServiceRequestStatusForm : Form
    {
        private ServiceRequestManager requestManager;

        // UI Components
        private Panel headerPanel;
        private Panel filterPanel;
        private Panel statsPanel;
        private ListView lvRequests;
        private ComboBox cmbCategory;
        private ComboBox cmbStatus;
        private ComboBox cmbPriority;
        private TextBox txtSearchId;
        private Button btnSearch;
        private Button btnClear;
        private Button btnShowDependencies;
        private Button btnBack;
        private Label lblTotalRequests;
        private Label lblInProgress;
        private Label lblCompleted;
        private Label lblCritical;

        public ServiceRequestStatusForm()
        {
            InitializeComponent();
            requestManager = new ServiceRequestManager();
            SetupUI();
            LoadAllRequests();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Service Request Status Tracking";
            this.Size = new Size(1400, 900);
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
            // Header Panel
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
                Text = "🔍",
                Font = new Font("Segoe UI", 38),
                ForeColor = Color.FromArgb(168, 85, 247),
                Location = new Point(40, 40),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblIcon);

            Label lblTitle = new Label
            {
                Text = "Service Request Status",
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(115, 30),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Track your service requests with advanced data structure analytics",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(125, 75),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblSubtitle);
            this.Controls.Add(headerPanel);

            // Statistics Panel
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

            CreateStatCard("📊", "Total Requests", "0", Color.FromArgb(59, 130, 246), new Point(40, 20), out lblTotalRequests);
            CreateStatCard("⚙", "In Progress", "0", Color.FromArgb(245, 158, 11), new Point(380, 20), out lblInProgress);
            CreateStatCard("✓", "Completed", "0", Color.FromArgb(34, 197, 94), new Point(720, 20), out lblCompleted);
            CreateStatCard("🔥", "Critical", "0", Color.FromArgb(239, 68, 68), new Point(1060, 20), out lblCritical);

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
                Text = "🔍 Search & Filter (Using BST, AVL Tree, Red-Black Tree)",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(25, 20),
                AutoSize = true
            };
            filterPanel.Controls.Add(lblFilterTitle);

            // Search by ID
            Label lblSearchId = new Label
            {
                Text = "Request ID",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(30, 60),
                AutoSize = true
            };
            filterPanel.Controls.Add(lblSearchId);

            txtSearchId = new TextBox
            {
                Location = new Point(30, 82),
                Size = new Size(180, 32),
                Font = new Font("Segoe UI", 10)
            };
            filterPanel.Controls.Add(txtSearchId);

            // Category filter
            Label lblCat = new Label
            {
                Text = "Category",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(240, 60),
                AutoSize = true
            };
            filterPanel.Controls.Add(lblCat);

            cmbCategory = new ComboBox
            {
                Location = new Point(240, 82),
                Size = new Size(220, 32),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat
            };
            cmbCategory.Items.Add("All Categories");
            cmbCategory.SelectedIndex = 0;
            filterPanel.Controls.Add(cmbCategory);

            // Status filter
            Label lblStatus = new Label
            {
                Text = "Status",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(490, 60),
                AutoSize = true
            };
            filterPanel.Controls.Add(lblStatus);

            cmbStatus = new ComboBox
            {
                Location = new Point(490, 82),
                Size = new Size(200, 32),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat
            };
            cmbStatus.Items.AddRange(new string[] { "All Statuses", "Submitted", "UnderReview", "InProgress", "OnHold", "Completed", "Rejected" });
            cmbStatus.SelectedIndex = 0;
            filterPanel.Controls.Add(cmbStatus);

            // Priority filter
            Label lblPriority = new Label
            {
                Text = "Priority",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(720, 60),
                AutoSize = true
            };
            filterPanel.Controls.Add(lblPriority);

            cmbPriority = new ComboBox
            {
                Location = new Point(720, 82),
                Size = new Size(180, 32),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat
            };
            cmbPriority.Items.AddRange(new string[] { "All Priorities", "Low", "Medium", "High", "Critical" });
            cmbPriority.SelectedIndex = 0;
            filterPanel.Controls.Add(cmbPriority);

            // Buttons
            btnSearch = CreateStyledButton("🔍 Search", new Point(930, 82), 140, Color.FromArgb(59, 130, 246));
            btnSearch.Click += BtnSearch_Click;
            filterPanel.Controls.Add(btnSearch);

            btnClear = CreateStyledButton("Clear", new Point(1090, 82), 110, Color.FromArgb(107, 114, 128));
            btnClear.Click += BtnClear_Click;
            filterPanel.Controls.Add(btnClear);

            btnShowDependencies = CreateStyledButton("📊 Graph", new Point(1220, 82), 100, Color.FromArgb(168, 85, 247));
            btnShowDependencies.Click += BtnShowDependencies_Click;
            filterPanel.Controls.Add(btnShowDependencies);

            this.Controls.Add(filterPanel);

            // Requests ListView
            lvRequests = new ListView
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
            lvRequests.Columns.Add("ID", 80);
            lvRequests.Columns.Add("Title", 250);
            lvRequests.Columns.Add("Category", 160);
            lvRequests.Columns.Add("Status", 120);
            lvRequests.Columns.Add("Priority", 90);
            lvRequests.Columns.Add("Submitted", 110);
            lvRequests.Columns.Add("Department", 160);
            lvRequests.Columns.Add("Est. Completion", 120);
            lvRequests.Columns.Add("Impact", 70);
            lvRequests.Columns.Add("Location", 170);
            lvRequests.DoubleClick += LvRequests_DoubleClick;
            this.Controls.Add(lvRequests);

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
                Text = "💡 Powered by BST, AVL Tree, Red-Black Tree, Min Heap, and Graph • Double-click for details",
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(240, 25),
                AutoSize = true
            };
            footerPanel.Controls.Add(lblTip);

            this.Controls.Add(footerPanel);

            LoadCategories();
        }

        private void CreateStatCard(string icon, string label, string value, Color accentColor, Point location, out Label valueLabel)
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
                AutoSize = true
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

            valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(70, 32),
                AutoSize = true
            };
            card.Controls.Add(valueLabel);

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
            var categories = requestManager.GetAllCategories();
            foreach (var category in categories)
            {
                cmbCategory.Items.Add(category);
            }
        }

        private void LoadAllRequests()
        {
            var requests = requestManager.GetAllRequests();
            DisplayRequests(requests);
            UpdateStatistics();
        }

        private void DisplayRequests(List<ServiceRequest> requests)
        {
            lvRequests.Items.Clear();
            foreach (var request in requests)
            {
                ListViewItem item = new ListViewItem(request.RequestId);
                item.SubItems.Add(request.Title);
                item.SubItems.Add(request.Category);
                item.SubItems.Add(GetStatusText(request.Status));
                item.SubItems.Add(request.PriorityLevel.ToString());
                item.SubItems.Add(request.SubmittedDate.ToString("dd MMM yyyy"));
                item.SubItems.Add(request.AssignedDepartment);

                string completion = request.EstimatedCompletionDate.HasValue
                    ? request.EstimatedCompletionDate.Value.ToString("dd MMM yyyy")
                    : "TBD";
                if (request.ActualCompletionDate.HasValue)
                {
                    completion = request.ActualCompletionDate.Value.ToString("dd MMM yyyy");
                }
                item.SubItems.Add(completion);
                item.SubItems.Add(request.CitizenImpactScore.ToString());
                item.SubItems.Add(request.Location);
                item.Tag = request;

                item.BackColor = GetStatusColor(request.Status);
                item.ForeColor = GetPriorityTextColor(request.PriorityLevel);

                if (request.PriorityLevel == Priority.Critical)
                {
                    item.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                }

                lvRequests.Items.Add(item);
            }
            UpdateStatistics();
        }

        private Color GetStatusColor(RequestStatus status)
        {
            switch (status)
            {
                case RequestStatus.Completed:
                    return Color.FromArgb(236, 253, 245);
                case RequestStatus.InProgress:
                    return Color.FromArgb(254, 249, 195);
                case RequestStatus.Submitted:
                    return Color.FromArgb(239, 246, 255);
                case RequestStatus.OnHold:
                    return Color.FromArgb(254, 242, 242);
                case RequestStatus.UnderReview:
                    return Color.FromArgb(243, 232, 255);
                case RequestStatus.Rejected:
                    return Color.FromArgb(249, 250, 251);
                default:
                    return Color.White;
            }
        }

        private Color GetPriorityTextColor(Priority priority)
        {
            switch (priority)
            {
                case Priority.Critical:
                    return Color.FromArgb(185, 28, 28);
                case Priority.High:
                    return Color.FromArgb(234, 88, 12);
                case Priority.Medium:
                    return Color.FromArgb(3, 105, 161);
                case Priority.Low:
                    return Color.FromArgb(75, 85, 99);
                default:
                    return Color.Black;
            }
        }

        private string GetStatusText(RequestStatus status)
        {
            switch (status)
            {
                case RequestStatus.UnderReview:
                    return "Under Review";
                case RequestStatus.InProgress:
                    return "In Progress";
                case RequestStatus.OnHold:
                    return "On Hold";
                default:
                    return status.ToString();
            }
        }

        private void UpdateStatistics()
        {
            var stats = requestManager.GetStatistics();
            lblTotalRequests.Text = stats["Total"].ToString();
            lblInProgress.Text = stats["InProgress"].ToString();
            lblCompleted.Text = stats["Completed"].ToString();
            lblCritical.Text = stats["Critical"].ToString();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSearchId.Text))
            {
                var request = requestManager.SearchById(txtSearchId.Text.Trim());
                if (request != null)
                {
                    DisplayRequests(new List<ServiceRequest> { request });
                    MessageBox.Show(
                        $"Found using Binary Search Tree!\n\n" +
                        $"Request: {request.Title}\n" +
                        $"Status: {GetStatusText(request.Status)}\n" +
                        $"Priority: {request.PriorityLevel}",
                        "BST Search Result",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    MessageBox.Show("No request found with that ID.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }

            string category = cmbCategory.SelectedItem?.ToString();
            RequestStatus? status = null;
            Priority? priority = null;

            if (cmbStatus.SelectedIndex > 0)
            {
                string statusText = cmbStatus.SelectedItem.ToString();
                status = (RequestStatus)Enum.Parse(typeof(RequestStatus), statusText);
            }

            if (cmbPriority.SelectedIndex > 0)
            {
                priority = (Priority)Enum.Parse(typeof(Priority), cmbPriority.SelectedItem.ToString());
            }

            var results = requestManager.SearchRequests(category, status, priority);
            DisplayRequests(results);

            if (results.Count == 0)
            {
                MessageBox.Show("No requests match your search criteria.", "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtSearchId.Clear();
            cmbCategory.SelectedIndex = 0;
            cmbStatus.SelectedIndex = 0;
            cmbPriority.SelectedIndex = 0;
            LoadAllRequests();
        }

        private void BtnShowDependencies_Click(object sender, EventArgs e)
        {
            if (lvRequests.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a request to view its dependencies.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ServiceRequest selected = (ServiceRequest)lvRequests.SelectedItems[0].Tag;
            var related = requestManager.GetRelatedRequests(selected.RequestId);

            if (related.Count <= 1)
            {
                MessageBox.Show($"Request '{selected.RequestId}' has no dependencies.", "Dependencies (Graph Traversal)", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string message = $"Related Requests (Using Graph BFS Traversal):\n\n";
                foreach (var req in related)
                {
                    if (req.RequestId != selected.RequestId)
                    {
                        message += $"• {req.RequestId}: {req.Title} [{req.Status}]\n";
                    }
                }
                MessageBox.Show(message, "Request Dependencies", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LvRequests_DoubleClick(object sender, EventArgs e)
        {
            if (lvRequests.SelectedItems.Count > 0)
            {
                ServiceRequest selected = (ServiceRequest)lvRequests.SelectedItems[0].Tag;
                ShowRequestDetails(selected);
            }
        }

        private void ShowRequestDetails(ServiceRequest request)
        {
            Form detailForm = new Form
            {
                Text = "Request Details",
                Size = new Size(750, 680),
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
                BackColor = GetPriorityHeaderColor(request.PriorityLevel)
            };

            Label lblType = new Label
            {
                Text = $"{GetPriorityIcon(request.PriorityLevel)} {request.PriorityLevel.ToString().ToUpper()} PRIORITY",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 25),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(lblType);

            Label lblRequestId = new Label
            {
                Text = $"Request ID: {request.RequestId}",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(255, 255, 255, 200),
                Location = new Point(30, 58),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(lblRequestId);
            detailForm.Controls.Add(headerPanel);

            Panel contentPanel = new Panel
            {
                Location = new Point(30, 120),
                Size = new Size(690, 480),
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
            CreateDetailRow(contentPanel, "📋 Title", request.Title, ref yPos);
            CreateDetailRow(contentPanel, "🏷 Category", request.Category, ref yPos);
            CreateDetailRow(contentPanel, "📍 Location", request.Location, ref yPos);
            CreateDetailRow(contentPanel, "🚦 Status", GetStatusText(request.Status), ref yPos);
            CreateDetailRow(contentPanel, "🏢 Department", request.AssignedDepartment, ref yPos);
            CreateDetailRow(contentPanel, "📅 Submitted", request.SubmittedDate.ToString("dddd, dd MMMM yyyy"), ref yPos);

            if (request.EstimatedCompletionDate.HasValue)
            {
                CreateDetailRow(contentPanel, "⏰ Est. Completion", request.EstimatedCompletionDate.Value.ToString("dddd, dd MMMM yyyy"), ref yPos);
            }

            if (request.ActualCompletionDate.HasValue)
            {
                CreateDetailRow(contentPanel, "✓ Completed", request.ActualCompletionDate.Value.ToString("dddd, dd MMMM yyyy"), ref yPos);
            }

            CreateDetailRow(contentPanel, "👥 Citizen Impact", $"{request.CitizenImpactScore} people affected", ref yPos);

            Label lblDescTitle = new Label
            {
                Text = "📝 Description",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(30, yPos),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblDescTitle);
            yPos += 30;

            Label lblDesc = new Label
            {
                Text = request.Description,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(75, 85, 99),
                Location = new Point(30, yPos),
                Size = new Size(630, 0),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblDesc);

            detailForm.Controls.Add(contentPanel);

            Button btnClose = CreateStyledButton("Close", new Point(590, 620), 140, Color.FromArgb(107, 114, 128));
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
                Size = new Size(460, 30),
                AutoSize = false
            };
            parent.Controls.Add(lblValue);

            yPos += 50;
        }

        private Color GetPriorityHeaderColor(Priority priority)
        {
            switch (priority)
            {
                case Priority.Critical:
                    return Color.FromArgb(239, 68, 68);
                case Priority.High:
                    return Color.FromArgb(245, 158, 11);
                case Priority.Medium:
                    return Color.FromArgb(59, 130, 246);
                case Priority.Low:
                    return Color.FromArgb(107, 114, 128);
                default:
                    return Color.FromArgb(107, 114, 128);
            }
        }

        private string GetPriorityIcon(Priority priority)
        {
            switch (priority)
            {
                case Priority.Critical:
                    return "🔥";
                case Priority.High:
                    return "⚠";
                case Priority.Medium:
                    return "📌";
                case Priority.Low:
                    return "ℹ";
                default:
                    return "📋";
            }
        }
    }
}