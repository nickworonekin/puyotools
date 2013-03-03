// Source: http://snipplr.com/view.php?codeview&id=33427

using System.Drawing;
using System.Windows.Forms;

namespace System.Windows.Forms // May need to set to something else
{
    /// <summary>
    /// A ListView with DragDrop reordering.
    /// <see cref="http://support.microsoft.com/kb/822483/en-us"/>
    /// </summary>
    public class ListViewWithReordering : ListView
    {
        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            //Begins a drag-and-drop operation in the ListView control.
            this.DoDragDrop(this.SelectedItems, DragDropEffects.Move);

            base.OnItemDrag(e);
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            int len = drgevent.Data.GetFormats().Length - 1;
            int i;
            for (i = 0; i <= len; i++)
            {
                if (drgevent.Data.GetFormats()[i].Equals("System.Windows.Forms.ListView+SelectedListViewItemCollection"))
                {
                    //The data from the drag source is moved to the target.	
                    drgevent.Effect = DragDropEffects.Move;
                }
            }

            base.OnDragEnter(drgevent);
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            //Return if the items are not selected in the ListView control.
            if (this.SelectedItems.Count == 0)
            {
                return;
            }
            //Returns the location of the mouse pointer in the ListView control.
            Point cp = this.PointToClient(new Point(drgevent.X, drgevent.Y));
            //Obtain the item that is located at the specified location of the mouse pointer.
            ListViewItem dragToItem = this.GetItemAt(cp.X, cp.Y);
            if (dragToItem == null)
            {
                return;
            }
            //Obtain the index of the item at the mouse pointer.
            int dragIndex = dragToItem.Index;
            ListViewItem[] sel = new ListViewItem[this.SelectedItems.Count];
            for (int i = 0; i <= this.SelectedItems.Count - 1; i++)
            {
                sel[i] = this.SelectedItems[i];
            }
            for (int i = 0; i < sel.GetLength(0); i++)
            {
                //Obtain the ListViewItem to be dragged to the target location.
                ListViewItem dragItem = sel[i];
                int itemIndex = dragIndex;
                if (itemIndex == dragItem.Index)
                {
                    return;
                }
                if (dragItem.Index < itemIndex)
                    itemIndex++;
                else
                    itemIndex = dragIndex + i;
                //Insert the item at the mouse pointer.
                ListViewItem insertItem = (ListViewItem)dragItem.Clone();
                this.Items.Insert(itemIndex, insertItem);
                //Removes the item from the initial location while 
                //the item is moved to the new location.
                this.Items.Remove(dragItem);
            }

            base.OnDragDrop(drgevent);
        }
    }
}