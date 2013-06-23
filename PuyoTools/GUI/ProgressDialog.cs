using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PuyoTools.GUI
{
    public partial class ProgressDialog : Form
    {
        private class ProgressData
        {
            public string WindowTitle;
            public string Title;
            public string Description;
        }

        /// <summary>
        /// Event raised when the dialog is displayed.
        /// </summary>
        /// <remarks>
        /// Use this event to perform the operation that the dialog is showing the progress for.
        /// This event will be raised on a different thread than the UI thread.
        /// </remarks>
        public event DoWorkEventHandler DoWork;

        /// <summary>
        /// Event raised when the operation completes.
        /// </summary>
        public event RunWorkerCompletedEventHandler RunWorkerCompleted;

        /// <summary>
        /// Event raised when <see cref="ReportProgress(int,string,string,object)"/> is called.
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged;

        public string WindowTitle
        {
            get { return this.Text; }
            set
            {
                if (backgroundWorker.IsBusy)
                {
                    // Need to pass this via ReportProgress
                    backgroundWorker.ReportProgress(-1, new ProgressData() { WindowTitle = (value ?? String.Empty) });
                }
                else
                {
                    Text = (value ?? String.Empty);
                }
            }
        }

        public string Title
        {
            get { return this.titleLabel.Text; }
            set
            {
                if (backgroundWorker.IsBusy)
                {
                    // Need to pass this via ReportProgress
                    backgroundWorker.ReportProgress(-1, new ProgressData() { Title = (value ?? String.Empty) });
                }
                else
                {
                    titleLabel.Text = (value ?? String.Empty);
                }
            }
        }

        public string Description
        {
            get { return this.descriptionLabel.Text; }
            set
            {
                if (backgroundWorker.IsBusy)
                {
                    // Need to pass this via ReportProgress
                    backgroundWorker.ReportProgress(-1, new ProgressData() { Description = (value ?? String.Empty) });
                }
                else
                {
                    descriptionLabel.Text = (value ?? String.Empty);
                }
            }
        }

        public ProgressDialog()
        {
            InitializeComponent();
        }

        public void RunWorkerAsync()
        {
            this.Show();

            backgroundWorker.RunWorkerAsync();
        }

        public void ReportProgress(int progress)
        {
            backgroundWorker.ReportProgress(progress);
        }

        public void ReportProgress(int progress, string description)
        {
            backgroundWorker.ReportProgress(progress, new ProgressData() { Description = description });
        }

        /// <summary>
        /// Raises the <see cref="DoWork"/> event.
        /// </summary>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> containing data for the event.</param>
        protected virtual void OnDoWork(DoWorkEventArgs e)
        {
            DoWorkEventHandler handler = DoWork;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="RunWorkerCompleted"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> containing data for the event.</param>
        protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            RunWorkerCompletedEventHandler handler = RunWorkerCompleted;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="ProgressChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="ProgressChangedEventArgs"/> containing data for the event.</param>
        protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
        {
            ProgressChangedEventHandler handler = ProgressChanged;
            if (handler != null)
                handler(this, e);
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            OnDoWork(e);
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage != -1)
            {
                progressBar.Value = e.ProgressPercentage;
            }

            ProgressData progressData = (e.UserState as ProgressData);
            if (progressData != null)
            {
                if (progressData.WindowTitle != null)
                {
                    Text = progressData.WindowTitle;
                }
                if (progressData.Title != null)
                {
                    titleLabel.Text = progressData.Title;
                }
                if (progressData.Description != null)
                {
                    descriptionLabel.Text = progressData.Description;
                }
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Close();

            OnRunWorkerCompleted(e);
        }
    }
}
