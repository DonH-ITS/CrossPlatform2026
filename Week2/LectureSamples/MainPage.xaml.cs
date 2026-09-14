namespace MauiApp10
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage() {
            InitializeComponent();

            GreetingLabel.Text = "...but this text was set in the code-behind!";

            // 2. Dynamic UI
            for (int i = 1; i <= 5; i++) {
                Label label = new Label
                {
                    Text = $"Dynamically created item {i}"
                };

                DynamicItemsLayout.Add(label);

            }
        }
    }
}
