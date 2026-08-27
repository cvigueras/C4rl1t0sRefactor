using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Microsoft.VisualStudio.PlatformUI;

namespace C4rl1t0sRefactor
{
    public partial class RefactorMenuWindow : DialogWindow
    {
        private readonly IReadOnlyList<RefactorDefinition> _items;

        /// <summary>The entry the user picked, or <c>null</c> if the dialog was cancelled.</summary>
        public RefactorDefinition Selected { get; private set; }

        public RefactorMenuWindow(IReadOnlyList<RefactorDefinition> items)
        {
            _items = items;
            InitializeComponent();

            RefactorList.ItemsSource = items;

            Loaded += (s, e) =>
            {
                var first = _items.FirstOrDefault(i => i.IsAvailable) ?? _items.FirstOrDefault();
                if (first != null)
                {
                    RefactorList.SelectedItem = first;
                    RefactorList.ScrollIntoView(first);
                }
                RefactorList.Focus();
            };
        }

        private void Accept()
        {
            var selection = RefactorList.SelectedItem as RefactorDefinition;
            if (selection == null) return;

            Selected = selection;
            DialogResult = true; // closes the modal dialog
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Handled) return;

            switch (e.Key)
            {
                case Key.Enter:
                    Accept();
                    e.Handled = true;
                    return;
                case Key.Escape:
                    DialogResult = false;
                    e.Handled = true;
                    return;
            }

            int digit = -1;
            if (e.Key >= Key.D1 && e.Key <= Key.D9) digit = e.Key - Key.D1 + 1;
            else if (e.Key >= Key.NumPad1 && e.Key <= Key.NumPad9) digit = e.Key - Key.NumPad1 + 1;

            if (digit > 0)
            {
                var match = _items.FirstOrDefault(i => i.Number == digit);
                if (match != null)
                {
                    RefactorList.SelectedItem = match;
                    Accept();
                    e.Handled = true;
                }
            }
        }

        private void RefactorList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Accept();
        }
    }
}
