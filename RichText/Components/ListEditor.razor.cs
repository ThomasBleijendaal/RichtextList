using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNetCore.Components;
using RichText.Abstractions;
using RichText.Enums;
using RichText.EventArgs;
using RichText.State;

namespace RichText.Components
{
    /*
     TODO: more flexible editor inside editor + validation 
    + require interface for data object
    
    + add support for:
    - reordering stuff
    - select items in bulk
    - 
     */
    public partial class ListEditor : IDisposable
    {
        [Parameter] public ListState State { get; set; } = default!;

        protected override void OnInitialized()
        {
            State.StateHasChanged += StateHasChangedAsync;
        }

        private async void StateHasChangedAsync(object? sender, System.EventArgs e)
        {
            await InvokeAsync(() => StateHasChanged());
        }

        private async Task HandleKeyPressAsync(KeyEventArgs args, IEntity source)
        {
            if (State == null)
            {
                return;
            }

            var keyPressSuccessful = args.Key switch
            {
                Keys.Enter when source.IsSaveable => State.AddElement(source),
                Keys.Up => State.SelectElementPreviousOf(source, args.Modifiers == ModifierKeys.Control),
                Keys.Down => State.SelectElementNextOf(source, args.Modifiers == ModifierKeys.Control),
                Keys.Tab when args.Modifiers == ModifierKeys.Shift => State.Promote(source),
                Keys.Tab => State.Demote(source),

                _ => default(bool?)
            };

            if (keyPressSuccessful != null)
            {
                StateHasChanged();
            }

            if (keyPressSuccessful == true)
            {
                await HandleDefocusAsync(source);
            }
        }

        private async Task HandleDefocusAsync(IEntity source)
        {
            // TODO: when making edits while saving leads to chaos
            await State.SaveElementAsync(source);
        }

        public void Dispose()
        {
            if (State == null)
            {
                return;
            }

            State.StateHasChanged -= StateHasChangedAsync;
        }

    }
}
