using System;
using System.Drawing;
using System.Windows.Forms;

namespace GeoJsonImporter.Work.UI
{
    public partial class RevitFileValidationForm : Form
    {
        private CheckBox projectNorthCheckBox;
        private CheckBox basePointsCheckBox;
        private CheckBox unitsCheckBox;
        private Button correctButton;
        private Button cancelButton;
        private Label titleLabel;
        private Label descriptionLabel;
        
        public RevitFileValidationForm()
        {
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            // Form Properties
            this.Text = "Revit File Validation";
            this.Size = new Size(600, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            
            // Title Label
            titleLabel = new Label
            {
                Text = "🔍 Revit File Validation",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 51, 51),
                Location = new Point(20, 20),
                Size = new Size(550, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            // Description Label
            descriptionLabel = new Label
            {
                Text = "Für den korrekten Geodaten-Import müssen folgende Einstellungen angepasst werden:",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(102, 102, 102),
                Location = new Point(20, 60),
                Size = new Size(550, 50),
                TextAlign = ContentAlignment.TopLeft
            };
            
            // Project North CheckBox
            projectNorthCheckBox = new CheckBox
            {
                Text = "✓ Project North = Geographic North",
                Font = new Font("Segoe UI", 10),
                Location = new Point(40, 110),
                Size = new Size(500, 25),
                Checked = false,
                Enabled = false,
                ForeColor = Color.FromArgb(204, 82, 76) // Red for unchecked
            };
            
            // Base Points CheckBox
            basePointsCheckBox = new CheckBox
            {
                Text = "✓ Project & Survey Base Points auf (0,0,0)",
                Font = new Font("Segoe UI", 10),
                Location = new Point(40, 140),
                Size = new Size(500, 25),
                Checked = false,
                Enabled = false,
                ForeColor = Color.FromArgb(204, 82, 76) // Red for unchecked
            };
            
            // Units CheckBox
            unitsCheckBox = new CheckBox
            {
                Text = "✓ Einheiten: Meter mit 0.0001m Präzision",
                Font = new Font("Segoe UI", 10),
                Location = new Point(40, 170),
                Size = new Size(500, 25),
                Checked = false,
                Enabled = false,
                ForeColor = Color.FromArgb(204, 82, 76) // Red for unchecked
            };
            
            // Correct Button
            correctButton = new Button
            {
                Text = "Korrigieren",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(240, 220),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(46, 125, 50), // Green
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };
            correctButton.FlatAppearance.BorderSize = 0;
            correctButton.Click += CorrectButton_Click;
            
            // Cancel Button
            cancelButton = new Button
            {
                Text = "Abbrechen",
                Font = new Font("Segoe UI", 10),
                Location = new Point(350, 220),
                Size = new Size(80, 35),
                BackColor = Color.FromArgb(158, 158, 158), // Gray
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += CancelButton_Click;
            
            // Add Controls to Form
            this.Controls.Add(titleLabel);
            this.Controls.Add(descriptionLabel);
            this.Controls.Add(projectNorthCheckBox);
            this.Controls.Add(basePointsCheckBox);
            this.Controls.Add(unitsCheckBox);
            this.Controls.Add(correctButton);
            this.Controls.Add(cancelButton);
        }
        
        private void CorrectButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        
        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
