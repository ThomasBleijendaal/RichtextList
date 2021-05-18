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
        private readonly IReadOnlyList<IEntity> _parentEntities;
        private readonly ListState? _parent;

        public event EventHandler<System.EventArgs>? StateHasChanged;

        public ListState(
            IEntityService entityService,
            IEntity? parentEntity,
            IEnumerable<IEntity> parentEntities,
            ListState? parent)
        {
            _elements = new List<ListElement<IEntity>>();
            _entityService = entityService;
            _parentEntity = parentEntity;
            _parentEntities = parentEntities.ToList();
            _parent = parent;
        }

        public ListState(
            IEntityService entityService,
            IEnumerable<IEntity> entities,
            IEntity? parentEntity,
            IEnumerable<IEntity> parentEntities,
            ListState? parent)
            : this(entityService, parentEntity, parentEntities, parent)
        {
            FillEntities(entities, this, parentEntities);
        }

        private void FillEntities(IEnumerable<IEntity> entities, ListState listState, IEnumerable<IEntity> parentEntities)
        {

            foreach (var entity in entities)
            {
                var parents = parentEntities.Append(entity);

                var element = new ListElement<IEntity>(entity);

                listState._elements.Add(element);

                if (entity.SubEntities != null)
                {
                    element.NestedList = new ListState(_entityService, entity, parents, listState);
                    FillEntities(entity.SubEntities, element.NestedList, parents);
                }
            }
        }

        public IReadOnlyList<ListElement<IEntity>> Elements => _elements;

        public string Id => _parentEntity?.Key ?? "::root";

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

            var replacementEntity = await _entityService.UpsertEntityAsync(entity, _parentEntities);

            _elements[index].Entity = replacementEntity;
            _elements[index].State = ElementState.Existing;

            StateHasChanged?.Invoke(this, new System.EventArgs());
        }

        public bool AddElement(IEntity? after)
        {
            var newEntity = _entityService.CreateNewEntity(_parentEntity);
            return AddElement(newEntity, after);
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

        public async Task<bool> PromoteAsync(IEntity entity)
        {
            if (_parent == null)
            {
                return false;
            }

            var index = GetIndex(entity);
            if (index < 0)
            {
                return false;
            }

            var promotedEntity = await _entityService.ConvertEntityUpAsync(entity, _parentEntities);
            if (promotedEntity == null)
            {
                return false;
            }

            _elements.RemoveAt(index);

            var parentIndex = _parent.GetIndex(this);

            _parent.AddElement(promotedEntity, _parent.Elements[parentIndex].Entity);
            _parent.StateHasChanged?.Invoke(this, new System.EventArgs());

            return true;
        }

        public async Task<bool> DemoteAsync(IEntity entity)
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

            var demotedEntity = await _entityService.ConvertEntityDownAsync(entity, newRoot.Entity);
            if (demotedEntity == null)
            {
                return false;
            }

            _elements.RemoveAt(index);

            newRoot.NestedList ??= new ListState(_entityService, newRoot.Entity, _parentEntities.Append(newRoot.Entity), this);
            newRoot.NestedList.AddElement(demotedEntity, newRoot.NestedList.Elements.Any() ? newRoot.NestedList.Elements[^1].Entity : default);

            return true;
        }

        private bool AddElement(IEntity entity, IEntity? after)
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

        private int GetIndex(IEntity? entity)
            => entity == null ? -1 : _elements.FindIndex(x => x.Entity.Id == entity.Id);

        private int GetIndex(ListState? list)
            => list == null ? -1 : _elements.FindIndex(x => x.NestedList?.Id == list.Id);
    }
}
