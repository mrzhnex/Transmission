using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerFontFamilyChanged : IEventHandler
    {
        void OnFontFamilyChanged(FontFamilyChangedEvent fontFamilyChangedEvent);
    }
}