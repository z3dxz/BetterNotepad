using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace BetterNotepad
{
    internal class Notebox : TextBox
    {

        public Notebox()
        {
            TabSize = 4;
        }


        public int TabSize
        {
            get;
            set;
            
        }

        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                String tab = new(' ', TabSize);
                int caretPosition = base.CaretIndex;
                base.Text = base.Text.Insert(caretPosition, tab);
                base.CaretIndex = caretPosition + TabSize;
                e.Handled = true;
            }
        }
    }
}
