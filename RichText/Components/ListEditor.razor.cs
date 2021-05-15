using System;
using System.Windows.Forms;
using Microsoft.AspNetCore.Components;
using RichText.Enums;
using RichText.EventArgs;
using RichText.Interactions;

namespace RichText.Components
{
    /*
     TODO: more flexible editor inside editor + validation 
    + require interface for data object
     */
    public partial class ListEditor : IDisposable
    {
        [Parameter] public ListInteraction Interaction { get; set; } = default!;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            Interaction.StateHasChanged += StateHasChangedAsync;
        }

        private async void StateHasChangedAsync(object? sender, System.EventArgs e)
        {
            await InvokeAsync(() => StateHasChanged());
        }

        private void HandleKeyPress(KeyEventArgs args, ListElement<Ticket> source)
        {
            switch (args.Key)
            {
                case Keys.Enter:
                    if (!string.IsNullOrWhiteSpace(source.Data.Description))
                    {
                        Interaction.AddElement(new Ticket(), source.Data);
                    }
                    break;

                case Keys.Up:
                    Interaction.SelectElementPreviousOf(source.Data, args.Modifiers == ModifierKeys.Control);
                    break;

                case Keys.Down:
                    Interaction.SelectElementNextOf(source.Data, args.Modifiers == ModifierKeys.Control);
                    break;

                case Keys.Tab:
                    if (args.Modifiers == ModifierKeys.Shift)
                    {
                        Interaction.Demote(source.Data);
                    }
                    else
                    {
                        Interaction.Promote(source.Data);
                    }
                    break;

                default: return;
            };
        }
        public void Dispose()
        {
            Interaction.StateHasChanged -= StateHasChangedAsync;
        }

    }
}
