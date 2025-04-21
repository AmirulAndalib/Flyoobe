﻿using Flyby11.Properties;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Views;

namespace Flyby11
{
    public partial class MainForm : Form
    {
        private readonly IsoHandler _isoHandler;
        private Logger logger;
        private ClippyUI clippyUI;

        public FAQHandler _faqHandler { get; private set; }

        [DllImport("CpuCheckNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool HasPopcnt();

        public MainForm()
        {
            InitializeComponent();
            logger = new Logger(this);
            // Uncomment lower line and add lang code to run localization test
            // Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
            _isoHandler = new IsoHandler(UpdateStatusLabel);
            _faqHandler = new FAQHandler(panelFAQ, UpdateStatusLabel);
            _faqHandler.InitializeFAQ();

            InitializeLocalizedStrings();
            InitializeClippyUI();

            // Drag and drop the Windows 11 ISO to patch it and install on unsupported hardware (Inplace Upgrade).
            UpdateStatusLabel(Locales.Strings.ctl_statusLabel);

            // Perform system compatibility check and show the appropriate message
            bool? isCompatible = CheckSystemCompatibility();

            if (isCompatible.HasValue)
            {
                ShowCompatibilityMessage(isCompatible.Value);
            }
            else
            {
                
                MessageBox.Show(
           "A preliminary check of the likelihood for a successful upgrade can be performed if you download the 'CpuCheckNative.dll' from the GitHub repository and place it in the app directory.\n" +
           "Once the DLL is in place, the detailed compatibility check will be enabled, allowing us to evaluate CPU features like SSE4.2 and POPCNT. ",
           "Flyby11 - Clippy has something to say!",
           MessageBoxButtons.OK,
           MessageBoxIcon.Information
       );

            }
        }

        private bool? CheckSystemCompatibility()
        {
            try
            {
                // Attempt to check for POPCNT support
                return HasPopcnt();
            }
            catch (DllNotFoundException)
            {
                // DLL is missing, return null for compatibility
               // UpdateStatusLabel("Required DLL 'CpuCheckNative.dll' not found. Skipping detailed compatibility check.");
                return null; // Indicating that compatibility could not be checked
            }
            catch (Exception ex)
            {
                // Log false for compatibility
                UpdateStatusLabel($"An error occurred while checking system compatibility: {ex.Message}");
                return false; // Defaulting to incompatible if another error occurs
            }
        }

        private void ShowCompatibilityMessage(bool isCompatible)
        {
            if (isCompatible)
            {
                string message = "You should be able to upgrade your system to Windows 11, as long as there are no incompatible device drivers or other issues.\n" +
                                 "Everything looks good for the upgrade, so you're ready to move forward!";
                ShowMessage("Upgrade Ready", message);
            }
            else
            {
                string message = "Unfortunately, your system doesn't support a key requirement for upgrading to Windows 11.\n" +
                                 "The missing feature is POPCNT, which is necessary for the upgrade. Please check your system's hardware to ensure it meets the requirements.";
                ShowMessage("Upgrade Blocked", message);
            }
        }



        private void ShowMessage(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void InitializeLocalizedStrings()
        {
            // Set localized strings for the UI elements
            Text = Locales.Strings.ctl_FormTitle;

            dropdownOptions.Items.Add("More options...");
            dropdownOptions.Items.Add(Locales.Strings.ctl_linkSelectComputer);
            dropdownOptions.Items.Add(Locales.Strings.ctl_inkCompPatch);
            dropdownOptions.Items.Add(Locales.Strings.ctl_linkCiuv);
            dropdownOptions.Items.Add("(Vote) " + Locales.Strings.ctl_linkVote); //  "Did the upgrade work?"
            dropdownOptions.Items.Add("(App) Trim down Windows 11 with TidyOS"); // "TidyOS Repository"
            dropdownOptions.SelectedIndex = 0;
        }

        private void InitializeClippyUI()
        {
            clippyUI = new ClippyUI(this, logger);
            clippyUI.SetClippyPosition(this);
            clippyUI.Show();
        }

        private void panelDragDrop_Paint(object sender, PaintEventArgs e)
        {
            var borderThickness = 8;
            var cornerRadius = 24;
            var glowThickness = 2; // Thin glow layer
            var control = (Control)sender;
            var rect = new Rectangle(
                borderThickness / 2,
                borderThickness / 2,
                control.Width - borderThickness,
                control.Height - borderThickness
            );

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            // subtle shadow
            using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
            {
                var shadowRect = new Rectangle(6, 6, control.Width - 12, control.Height - 12);
                using (var shadowPath = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    shadowPath.AddArc(shadowRect.X, shadowRect.Y, cornerRadius, cornerRadius, 180, 90);
                    shadowPath.AddArc(shadowRect.Right - cornerRadius, shadowRect.Y, cornerRadius, cornerRadius, 270, 90);
                    shadowPath.AddArc(shadowRect.Right - cornerRadius, shadowRect.Bottom - cornerRadius, cornerRadius, cornerRadius, 0, 90);
                    shadowPath.AddArc(shadowRect.X, shadowRect.Bottom - cornerRadius, cornerRadius, cornerRadius, 90, 90);
                    shadowPath.CloseFigure();
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }
            }

            //frosted glass background (simulated with semi-transparent gradient)
            using (var frostedBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new Rectangle(0, 0, control.Width, control.Height),
                Color.FromArgb(220, 240, 245, 255), // Very light blue (mica-like)
                Color.FromArgb(220, 235, 230, 255), // Very light lavender
                System.Drawing.Drawing2D.LinearGradientMode.Vertical))
            {
                using (var frostedPath = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    frostedPath.AddArc(rect.X, rect.Y, cornerRadius, cornerRadius, 180, 90);
                    frostedPath.AddArc(rect.Right - cornerRadius, rect.Y, cornerRadius, cornerRadius, 270, 90);
                    frostedPath.AddArc(rect.Right - cornerRadius, rect.Bottom - cornerRadius, cornerRadius, cornerRadius, 0, 90);
                    frostedPath.AddArc(rect.X, rect.Bottom - cornerRadius, cornerRadius, cornerRadius, 90, 90);
                    frostedPath.CloseFigure();
                    e.Graphics.FillPath(frostedBrush, frostedPath);
                }
            }

            // Glowing border with outer layer, subtle glow
            using (var glowBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                rect,
                Color.FromArgb(100, 150, 200, 255), // Soft blue glow
                Color.FromArgb(100, 200, 150, 255), // Soft lavender glow
                System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal))
            using (var glowPen = new Pen(glowBrush, borderThickness + glowThickness))
            {
                using (var glowPath = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    glowPath.AddArc(rect.X, rect.Y, cornerRadius, cornerRadius, 180, 90);
                    glowPath.AddArc(rect.Right - cornerRadius, rect.Y, cornerRadius, cornerRadius, 270, 90);
                    glowPath.AddArc(rect.Right - cornerRadius, rect.Bottom - cornerRadius, cornerRadius, cornerRadius, 0, 90);
                    glowPath.AddArc(rect.X, rect.Bottom - cornerRadius, cornerRadius, cornerRadius, 90, 90);
                    glowPath.CloseFigure();
                    e.Graphics.DrawPath(glowPen, glowPath);
                }
            }

            // Main border (inner layer, solid but soft)
            using (var borderBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                rect,
                Color.FromArgb(255, 160, 196, 255), // Soft Azure Blue
                Color.FromArgb(255, 199, 135, 255), // Vibrant Lavender
                System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal))
            using (var borderPen = new Pen(borderBrush, borderThickness))
            {
                using (var borderPath = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    borderPath.AddArc(rect.X, rect.Y, cornerRadius, cornerRadius, 180, 90);
                    borderPath.AddArc(rect.Right - cornerRadius, rect.Y, cornerRadius, cornerRadius, 270, 90);
                    borderPath.AddArc(rect.Right - cornerRadius, rect.Bottom - cornerRadius, cornerRadius, cornerRadius, 0, 90);
                    borderPath.AddArc(rect.X, rect.Bottom - cornerRadius, cornerRadius, cornerRadius, 90, 90);
                    borderPath.CloseFigure();
                    e.Graphics.DrawPath(borderPen, borderPath);
                }
            }
        }

        private async Task HandleIsoInput(object sender, EventArgs e)
        {
            string isoPath = null;

            if (e is DragEventArgs dragEvent && dragEvent.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])dragEvent.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1 && Path.GetExtension(files[0]).Equals(".iso", StringComparison.OrdinalIgnoreCase))
                {
                    isoPath = files[0];
                }
            }

            // Always show dialog if no ISO found yet (e.g. from dropdownOptions)
            if (isoPath == null)
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "ISO Files (*.iso)|*.iso";
                    openFileDialog.Title = "Select an ISO File";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        isoPath = openFileDialog.FileName;
                    }
                }
            }

            if (!string.IsNullOrEmpty(isoPath) && File.Exists(isoPath))
            {
                await _isoHandler.HandleIso(isoPath);
            }
        }

        private void panelDragDrop_DragEnter(object sender, DragEventArgs e)
        {
            // Check if the file is an ISO
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                e.Effect = (files.Length == 1 && Path.GetExtension(files[0]).Equals(".iso", StringComparison.OrdinalIgnoreCase))
                    ? DragDropEffects.Copy
                    : DragDropEffects.None;
            }
        }

        private async void panelDragDrop_DragDrop(object sender, DragEventArgs e)
        {
            await HandleIsoInput(sender, e);
        }

        // Update the status label
        private void UpdateStatusLabel(string message)
        {
            statusLabel.Text = message;
            statusLabel.Refresh();
        }

        private async void dropdownOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            {
                // Ignore the default "More options..." item
                if (dropdownOptions.SelectedIndex <= 0)
                    return;

                switch (dropdownOptions.SelectedIndex)
                {
                    case 1: // Select Computer
                        await HandleIsoInput(sender, null);
                        break;

                    case 2: // Compatibility Patch
                        HandleCompatibilityPatch();
                        break;

                    case 3: // Can I Upgrade View
                        HandleCanIUpgradeView();
                        break;

                    case 4:
                        // Open the GitHub discussion for feedback
                        Process.Start("https://github.com/builtbybel/Flyby11/discussions/72");
                        break;

                    case 5:
                        // OPen TidyOS Repository
                        Process.Start("https://github.com/builtbybel/TidyOS/");
                        break;
                }
                // Reset selection back to default prompt after action
                dropdownOptions.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Handles the event for the compatibility patch.
        /// </summary>
        private void HandleCompatibilityPatch()
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                /* Select the USB drive containing your Windows 11 installation files. " +
                "This feature adds a compatibility patch to bypass certain system requirements. " +
                "Compatible with drives prepared by any tool, including Rufus. Ensure the drive is ready! */

                folderDialog.Description = Locales.Strings.compPatch_msgSelectDrive.Replace(@"\n", Environment.NewLine);

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath;
                    var driveInfo = new DriveInfo(Path.GetPathRoot(selectedPath));

                    if (driveInfo.DriveType == DriveType.Removable && driveInfo.IsReady)
                    {
                        if (MessageBox.Show(Locales.Strings.compPatch_msgSelectDriveConfirm, //This will apply compatibility bypass settings on the selected USB drive.Continue ?
                            "Apply Bypass Patch", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            try
                            {
                                _isoHandler.CreateUnattendXml(selectedPath);
                                // Bypass patch applied successfully!
                                MessageBox.Show(Locales.Strings.compPatch_msgSuccess, "Apply Bypass Patch", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                UpdateStatusLabel(Locales.Strings.compPatch_msgSuccess);
                            }
                            catch (Exception ex)
                            {
                                // Failed to apply bypass patch:
                                UpdateStatusLabel($"{Locales.Strings.compPatch_msgFailedEx} {ex.Message}");
                            }
                        }
                        else
                        {
                            UpdateStatusLabel(Locales.Strings.compPatch_debugCancel); // Bypass patch canceled by user.
                        }
                    }
                    else
                    {
                        // The selected path is not a removable drive. Please select a USB drive.
                        MessageBox.Show(Locales.Strings.compPatch_msgNotRemovableDrive, "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        // User attempted to select a non-removable drive.
                        UpdateStatusLabel(Locales.Strings.compPatch_debugNotRemovableDrive);
                    }
                }
                else
                {
                    UpdateStatusLabel(Locales.Strings.compPatch_debugNoUSBDrive); // No USB drive selected for patching.
                }
            }
        }

        /// <summary>
        /// Opens the "Can I Upgrade" view.
        /// </summary>
        private void HandleCanIUpgradeView()
        {
            CanIUpgradeView canIUpgradeView = new CanIUpgradeView();
            SwitchView.SetView(canIUpgradeView, panelContainer);
        }
    }
}