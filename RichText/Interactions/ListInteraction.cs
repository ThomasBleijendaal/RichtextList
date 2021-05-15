using System;
using System.Collections.Generic;
using System.Linq;

namespace RichText.Interactions
{
    public class ListInteraction
    {
        private readonly List<ListElement<Ticket>> _elements;
        private readonly ListInteraction? _parent;

        public event EventHandler<System.EventArgs>? StateHasChanged;

        public ListInteraction(ListInteraction? parent)
        {
            _elements = new List<ListElement<Ticket>>();
            _parent = parent;
        }

        public ListInteraction(IEnumerable<Ticket> tickets, ListInteraction? parent) : this(parent)
        {
            foreach (var ticket in tickets)
            {
                _elements.Add(new ListElement<Ticket>(ticket));
            }
        }

        public IReadOnlyList<ListElement<Ticket>> Elements => _elements;

        public void AddElement(Ticket ticket, Ticket? after)
        {
            var index = GetIndex(after) + 1;

            var newItem = new ListElement<Ticket>(ticket, true);

            if (index < _elements.Count)
            {
                _elements.Insert(index, newItem);
            }
            else
            {
                _elements.Add(newItem);
            }
        }

        public void SelectElementPreviousOf(Ticket ticket, bool skipNestedList)
        {
            var index = GetIndex(ticket) - 1;
            if (index < 0)
            {
                if (_parent != null)
                {
                    _parent.SelectElementPreviousOf(this);
                }

                return;
            }

            if (!skipNestedList && _elements[index].NestedList?.Elements.Count > 0)
            {
                _elements[index].NestedList!.SelectLastItem();
            }
            else
            {
                _elements[index].Focus = true;
            }
        }

        public void SelectElementPreviousOf(ListInteraction list)
        {
            var index = GetIndex(list);
            if (index < 0)
            {
                return;
            }

            _elements[index].Focus = true;
            StateHasChanged?.Invoke(this, new System.EventArgs());
        }

        public void SelectLastItem()
        {
            _elements[^1].Focus = true;
        }

        public void SelectElementNextOf(Ticket ticket, bool skipNestedList)
        {
            var index = GetIndex(ticket);
            if (!skipNestedList && _elements[index].NestedList?.Elements.Count > 0)
            {
                _elements[index].NestedList!.SelectFirstItem();
                return;
            }

            index++;
            
            if (index >= _elements.Count)
            {
                if (_parent != null)
                {
                    _parent.SelectElementNextOf(this);
                }

                return;
            }

            _elements[index].Focus = true;
        }

        public void SelectElementNextOf(ListInteraction list)
        {
            var index = GetIndex(list) + 1;
            if (index >= _elements.Count)
            {
                return;
            }

            _elements[index].Focus = true;
            StateHasChanged?.Invoke(this, new System.EventArgs());
        }

        public void SelectFirstItem()
        {
            _elements[0].Focus = true;
        }

        public bool IsFirst(Ticket ticket)
        {
            return GetIndex(ticket) == 0;
        }

        public void Demote(Ticket ticket)
        {
            if (_parent == null)
            {
                return;
            }

            var index = GetIndex(ticket);
            if (index < _elements.Count - 1)
            {
                return;
            }

            _elements.RemoveAt(index);

            var parentIndex = _parent.GetIndex(this);

            _parent.AddElement(ticket, _parent.Elements[parentIndex].Data);
            _parent.StateHasChanged?.Invoke(this, new System.EventArgs());
        }

        public void Promote(Ticket ticket)
        {
            var index = GetIndex(ticket);
            if (index < 1)
            {
                return;
            }

            var newRoot = _elements[index - 1];

            _elements.RemoveAt(index);

            newRoot.NestedList ??= new ListInteraction(this);
            newRoot.NestedList.AddElement(ticket, default);
        }

        private int GetIndex(Ticket? findTicket)
            => findTicket == null ? -1 : _elements.FindIndex(x => x.Data == findTicket);

        private int GetIndex(ListInteraction? list)
            => list == null ? -1 : _elements.FindIndex(x => x.NestedList == list);
    }

    public class ListElement
    {
        public bool Focus { get; set; }
        public ListInteraction? NestedList { get; set; }
    }

    public class ListElement<T> : ListElement
    {
        public ListElement(T data)
        {
            Data = data;
        }

        public ListElement(T data, bool focus)
        {
            Data = data;
            Focus = focus;
        }

        public T Data { get; set; }
    }

    public class Ticket
    {
        public Ticket(string id, string? description)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Description = description;
        }

        public Ticket()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string? Description { get; set; }
    }
}
