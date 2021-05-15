using System.Windows.Forms;
using RichText.Enums;

namespace RichText.EventArgs
{
    public class KeyEventArgs
    {
        public KeyEventArgs(Keys key, ModifierKeys modifiers)
        {
            Key = key;
            Modifiers = modifiers;
        }

        public Keys Key { get; set; }
        public ModifierKeys Modifiers { get; }
    }
}
