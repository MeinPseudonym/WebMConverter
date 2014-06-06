﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WebMConverter
{
    public partial class SubtitleForm : Form
    {
        public SubtitleFilter GeneratedFilter;

        public SubtitleForm()
        {
            InitializeComponent();

            if (Program.SubtitleTracks.Count == 0)
            {
                checkBoxInternalSubs.Checked = false;
                checkBoxInternalSubs.Enabled = false;
            }
            else
            {
                Dictionary<int, string> subtitleTracks = new Dictionary<int, string>();
                foreach (KeyValuePair<int, string> Track in Program.SubtitleTracks)
                {
                    subtitleTracks.Add(Track.Key, string.Format("#{0}: {1}", Track.Key, Track.Value));
                }
                comboBoxVideoTracks.DataSource = new BindingSource(subtitleTracks, null);
                comboBoxVideoTracks.ValueMember = "Key";
                comboBoxVideoTracks.DisplayMember = "Value";
            }
        }

        public SubtitleForm(SubtitleFilter SubtitleFilter) : this()
        {
            if (SubtitleFilter.FileName == Program.InputFile)
            {
                comboBoxVideoTracks.SelectedValue = SubtitleFilter.Track;
            }
            else
            {
                checkBoxInternalSubs.Checked = false;
                textBoxSubtitleFile.Text = SubtitleFilter.FileName;
                if (Program.SubtitleTracks.Count == 0)
                    checkBoxInternalSubs.Enabled = false;
            }
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            if (checkBoxInternalSubs.Checked)
            {
                GeneratedFilter = new SubtitleFilter(Path.Combine(Program.AttachmentDirectory, string.Format("sub{0}.ass", (int)comboBoxVideoTracks.SelectedValue)), (int)comboBoxVideoTracks.SelectedValue);
            }
            else
            {
                GeneratedFilter = new SubtitleFilter(textBoxSubtitleFile.Text);
            }
        }

        private void checkBoxInternalSubs_CheckedChanged(object sender, EventArgs e)
        {
            tableLayoutPanelSubtitleFileSelector.Visible = !checkBoxInternalSubs.Checked;
            comboBoxVideoTracks.Visible = checkBoxInternalSubs.Checked;
            label2.Text = checkBoxInternalSubs.Checked ? "Subtitle track:" : "Subtitle file:";
            checkBoxInternalSubs.Text = checkBoxInternalSubs.Checked ? "Yes" : "No";
        }

        private void buttonSelectSubtitleFile_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.InitialDirectory = Path.GetDirectoryName(Program.InputFile);
                dialog.Filter = "Subtitle files (*.ass, *.srt, *.ssa)|*.ass;*.srt;*.ssa|All files|*.*";
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxSubtitleFile.Text = dialog.FileName;
                }
            }
        }
    }

    public class SubtitleFilter
    {
        public readonly string FileName;
        public readonly int Track;

        public SubtitleFilter(string fileName, int track = -1)
        {
            FileName = fileName;
            Track = track;
        }

        public override string ToString()
        {
            return string.Format("assrender(\"{0}\", fontdir=\"{1}\")", FileName, Program.AttachmentDirectory);
        }
    }
}
