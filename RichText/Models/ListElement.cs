using RichText.Abstractions;
using RichText.Enums;
using RichText.State;

namespace RichText.Models
{
    public class ListElement
    {
        public bool Focus { get; set; }
        public ElementState State { get; set; } = ElementState.New;
        public ListState? NestedList { get; set; }
    }

    public class ListElement<T> : ListElement
        where T : IEntity
    {
        public ListElement(T entity)
        {
            Entity = entity;

            State = entity.IsNew ? ElementState.New : ElementState.Existing;
        }

        public ListElement(T entity, bool focus)
        {
            Entity = entity;
            Focus = focus;

            State = entity.IsNew ? ElementState.New : ElementState.Existing;
        }

        public T Entity { get; set; }
    }
}
