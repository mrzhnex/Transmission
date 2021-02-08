using Core.Handlers;
using Core.Main;
using System.Linq;

namespace Core.Events
{
    public class FontFamilyChangedEvent : Event
    {
        public string FontFamilyName { get; set; } = Manage.DefaultInformation.DefaultFontFamily;
        public FontFamilyChangedEvent(string FontFamilyName)
        {
            this.FontFamilyName = FontFamilyName;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            var installedFontCollection = new System.Drawing.Text.InstalledFontCollection();
            if (installedFontCollection.Families.FirstOrDefault(x => x.Name == FontFamilyName) == default)
                FontFamilyName = Manage.DefaultInformation.DefaultFontFamily;
            ((IEventHandlerFontFamilyChanged)handler).OnFontFamilyChanged(this);
        }
    }
}