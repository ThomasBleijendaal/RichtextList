using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNetCore.Components;
using RichText.Abstractions;
using RichText.Entities;
using RichText.Enums;
using RichText.EventArgs;
using RichText.State;

namespace RichText.Components
{
    /*
     TODO: more flexible editor inside editor + validation 
    + require interface for data object
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
                Keys.Enter when source.IsSaveable => State.AddElement(new Ticket(), source),
                Keys.Up => State.SelectElementPreviousOf(source, args.Modifiers == ModifierKeys.Control),
                Keys.Down => State.SelectElementNextOf(source, args.Modifiers == ModifierKeys.Control),
                Keys.Tab when args.Modifiers == ModifierKeys.Shift => State.Demote(source),
                Keys.Tab => State.Promote(source),

                _ => default(bool?)
            };

            StateHasChanged();

            if (keyPressSuccessful == true)
            {
                await HandleDefocusAsync(source);
            }
        }

        private async Task HandleDefocusAsync(IEntity source)
        {
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
