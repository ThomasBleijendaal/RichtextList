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
    {
        public ListElement(T data)
        {
            Entity = data;
        }

        public ListElement(T entity, bool focus)
        {
            Entity = entity;
            Focus = focus;
        }

        public T Entity { get; set; }
    }
}
