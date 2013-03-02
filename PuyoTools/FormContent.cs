using System;
using System.Drawing;
using System.Windows.Forms;

namespace PuyoTools
{
    public class FormContent
    {
        // Create the Form
        public static void Create(Form form, string title, Size size)
        {
            form.Text            = title;
            form.ClientSize      = size;
            form.MaximizeBox     = false;
            form.StartPosition   = FormStartPosition.CenterScreen;
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.Icon            = IconResources.ProgramIcon;
        }

        public static void Create(Form form, string title, Size size, bool controls)
        {
            form.Text            = title;
            form.ClientSize      = size;
            form.ControlBox      = controls;
            form.MaximizeBox     = false;
            form.StartPosition   = FormStartPosition.CenterScreen;
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.Icon            = IconResources.ProgramIcon;
        }

        // Button
        public static void Add(Control parent, Button content, string text, Point position, Size size, EventHandler onClick)
        {
            content.Text     = text;
            content.Location = position;
            content.Size     = size;
            content.Click += new EventHandler(onClick);

            parent.Controls.Add(content);
        }

        // Checkboxes
        public static void Add(Control parent, CheckBox content, string text, Point position, Size size)
        {
            content.Text     = text;
            content.Location = position;
            content.Size     = size;

            parent.Controls.Add(content);
        }

        public static void Add(Control parent, CheckBox content, bool selected, string text, Point position, Size size)
        {
            content.Text     = text;
            content.Location = position;
            content.Size     = size;
            content.Checked  = selected;

            parent.Controls.Add(content);
        }

        // Label
        public static void Add(Control parent, Label content, string text, Point position, Size size)
        {
            content.Text     = text;
            content.Location = position;
            content.Size     = size;

            parent.Controls.Add(content);
        }

        public static void Add(Control parent, Label content, string text, Point position, Size size, ContentAlignment alignment)
        {
            content.Text      = text;
            content.Location  = position;
            content.Size      = size;
            content.TextAlign = alignment;

            parent.Controls.Add(content);
        }

        public static void Add(Control parent, Label content, string text, Point position, Size size, ContentAlignment alignment, Font font)
        {
            content.Text      = text;
            content.Location  = position;
            content.Size      = size;
            content.TextAlign = alignment;
            content.Font      = font;

            parent.Controls.Add(content);
        }

        // Progrss Bar
        public static void Add(Control parent, ProgressBar content, Point position, Size size, int total)
        {
            content.Location = position;
            content.Size     = size;
            content.Style    = ProgressBarStyle.Blocks;
            content.Minimum  = 0;
            content.Maximum  = total;

            parent.Controls.Add(content);
        }

        // Combo Box
        public static void Add(Control parent, ComboBox content, string[] choices, Point position, Size size)
        {
            content.Items.AddRange(choices);
            content.Location         = position;
            content.Size             = size;
            content.DropDownStyle    = ComboBoxStyle.DropDownList;
            content.MaxDropDownItems = choices.Length;
            content.SelectedIndex    = 0;

            parent.Controls.Add(content);
        }

        public static void Add(Control parent, ComboBox content, string[] choices, Point position, Size size, EventHandler onChange)
        {
            content.Items.AddRange(choices);
            content.Location              = position;
            content.Size                  = size;
            content.DropDownStyle         = ComboBoxStyle.DropDownList;
            content.MaxDropDownItems      = choices.Length;
            content.SelectedIndex         = 0;
            content.SelectedIndexChanged += new EventHandler(onChange);

            parent.Controls.Add(content);
        }

        // List View
        public static void Add(Control parent, ListView content, string[] columnHeader, int[] columnWidth, Point position, Size size)
        {
            content.Location      = position;
            content.Size          = size;
            content.View          = View.Details;
            content.GridLines     = true;
            content.FullRowSelect = true;

            for (int i = 0; i < columnHeader.Length; i++)
            {
                ColumnHeader head = new ColumnHeader();
                head.Text         = columnHeader[i];
                head.Width        = columnWidth[i];

                content.Columns.Add(head);
            }

            parent.Controls.Add(content);
        }

        // Input Box
        public static void Add(Control parent, TextBox content, string defaultText, Point position, Size size)
        {
            content.Location  = position;
            content.Size      = size;
            content.Text      = defaultText;

            parent.Controls.Add(content);
        }

        // Group Box
        public static void Add(Control parent, GroupBox content, string title, Point position, Size size)
        {
            content.Location = position;
            content.Size     = size;
            content.Text     = title;

            parent.Controls.Add(content);
        }
    }
}