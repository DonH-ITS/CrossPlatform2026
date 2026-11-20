

namespace Week9Demo
{
       public class AppSettings
        {
            public bool SoundEnabled { get; set; } = true;
            public double FontSize { get; set; } = 30;
            public Color BackgroundColour { get; set; } = Colors.Red;

        public AppSettings Clone() {
            return new AppSettings
            {
                SoundEnabled = SoundEnabled,
                FontSize = FontSize,
                BackgroundColour = BackgroundColour
            };
        }

        public void CopyFrom(AppSettings other) {
            SoundEnabled = other.SoundEnabled;
            FontSize = other.FontSize;
            BackgroundColour = other.BackgroundColour;
        }
    }
    
}
