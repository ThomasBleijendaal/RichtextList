using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RichText.Abstractions;
using RichText.Enums;
using RichText.Models;

namespace RichText.State
{
    public class ListState
    {
        private readonly List<ListElement<IEntity>> _elements;
        private readonly IEntityService _entityService;
        private readonly IEntity? _parentEntity;
        private readonly ListState? _parent;

        public event EventHandler<System.EventArgs>? StateHasChanged;

        public ListState(
            IEntityService entityService, 
            IEntity? parentEntity, 
            ListState? parent)
        {
            _elements = new List<ListElement<IEntity>>();
            _entityService = entityService;
            _parentEntity = parentEntity;
            _parent = parent;
        }

        public ListState(
            IEntityService entityService, 
            IEnumerable<IEntity> entities, 
            IEntity? parentEntity, 
            ListState? parent) 
            : this(entityService, parentEntity, parent)
        {
            FillEntities(entities, this);
        }

        private void FillEntities(IEnumerable<IEntity> entities, ListState listState)
        {
            foreach (var entity in entities)
            {
                var element = new ListElement<IEntity>(entity)
                {
                    State = ElementState.Existing
                };

                listState._elements.Add(element);

                if (entity.SubEntities != null)
                {
                    element.NestedList = new ListState(_entityService, entity, listState);
                    FillEntities(entity.SubEntities, element.NestedList);
                }
            }
        }

        public IReadOnlyList<ListElement<IEntity>> Elements => _elements;

        public string Id => _parentEntity?.Id ?? "::root";

        public async Task SaveElementAsync(IEntity entity)
        {
            if (!entity.IsSaveable)
            {
                return;
            }

            // TODO: check for modification state

            var index = GetIndex(entity);
            if (index < 0)
            {
                return;
            }

            _elements[index].State = ElementState.Saving;
            StateHasChanged?.Invoke(this, new System.EventArgs());

            var replacementEntity = await _entityService.UpsertEntityAsync(entity);

            _elements[index].Entity = replacementEntity;
            _elements[index].State = ElementState.Existing;

            StateHasChanged?.Invoke(this, new System.EventArgs());
        }

        public bool AddElement(IEntity entity, IEntity? after)
        {
            var index = GetIndex(after) + 1;

            var newItem = new ListElement<IEntity>(entity, true);

            if (index < _elements.Count)
            {
                _elements.Insert(index, newItem);
            }
            else
            {
                _elements.Add(newItem);
            }

            return true;
        }

        public bool SelectElementPreviousOf(IEntity entity, bool skipNestedList)
        {
            var index = GetIndex(entity) - 1;
            if (index < 0)
            {
                if (_parent != null)
                {
                    return _parent.SelectElementPreviousOf(this);
                }

                return false;
            }

            if (!skipNestedList && _elements[index].NestedList?.Elements.Count > 0)
            {
                _elements[index].NestedList!.SelectLastItem();
            }
            else
            {
                _elements[index].Focus = true;
            }

            return true;
        }

        public bool SelectElementPreviousOf(ListState list)
        {
            var index = GetIndex(list);
            if (index < 0)
            {
                return false;
            }

            _elements[index].Focus = true;
            StateHasChanged?.Invoke(this, new System.EventArgs());

            return true;
        }

        public void SelectLastItem()
        {
            var lastElement = _elements[^1];

            if (lastElement.NestedList == null || !lastElement.NestedList.Elements.Any())
            {
                lastElement.Focus = true;
            }
            else
            {
                lastElement.NestedList.SelectLastItem();
            }
        }

        public bool SelectElementNextOf(IEntity entity, bool skipNestedList)
        {
            var index = GetIndex(entity);
            if (!skipNestedList && _elements[index].NestedList?.Elements.Count > 0)
            {
                _elements[index].NestedList!.SelectFirstItem();
                _elements[index].NestedList!.StateHasChanged?.Invoke(this, new System.EventArgs());
                return true;
            }

            index++;

            if (index >= _elements.Count)
            {
                if (_parent != null)
                {
                    return _parent.SelectElementNextOf(this);
                }

                return false;
            }

            _elements[index].Focus = true;
            return true;
        }

        public bool SelectElementNextOf(ListState list)
        {
            var index = GetIndex(list) + 1;
            if (index >= _elements.Count)
            {
                if (_parent != null)
                {
                    return _parent.SelectElementNextOf(this);
                }

                return false;
            }

            _elements[index].Focus = true;
            StateHasChanged?.Invoke(this, new System.EventArgs());
            return true;
        }

        public void SelectFirstItem() 
            => _elements[0].Focus = true;

        public bool IsFirst(IEntity entity)
            => GetIndex(entity) == 0;

        public bool Demote(IEntity entity)
        {
            if (_parent == null)
            {
                return false;
            }

            var index = GetIndex(entity);
            if (index < _elements.Count - 1)
            {
                return false;
            }

            _elements.RemoveAt(index);

            var parentIndex = _parent.GetIndex(this);

            _parent.AddElement(entity, _parent.Elements[parentIndex].Entity);
            _parent.StateHasChanged?.Invoke(this, new System.EventArgs());

            return true;
        }

        public bool Promote(IEntity entity)
        {
            var index = GetIndex(entity);
            if (index < 1)
            {
                return false;
            }

            if (_elements[index].NestedList?.Elements.Any() == true)
            {
                return false;
            }

            var newRoot = _elements[index - 1];

            _elements.RemoveAt(index);

            newRoot.NestedList ??= new ListState(_entityService, newRoot.Entity, this);
            newRoot.NestedList.AddElement(entity, newRoot.NestedList.Elements.Any() ? newRoot.NestedList.Elements[^1].Entity : default);

            return true;
        }

        private int GetIndex(IEntity? entity)
            => entity == null ? -1 : _elements.FindIndex(x => x.Entity.Id == entity.Id);

        private int GetIndex(ListState? list)
            => list == null ? -1 : _elements.FindIndex(x => x.NestedList?.Id == list.Id);
    }
}
