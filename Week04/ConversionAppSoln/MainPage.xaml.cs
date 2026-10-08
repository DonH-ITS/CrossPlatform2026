using Microsoft.Maui.Controls.Shapes;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            ConversionTypePicker.SelectedIndex = 0;

        }

        private void btnCalculate_Clicked(object sender, EventArgs e)
        {
            DoCalculation();
        }


        // All the event handlers are going to call the same method to actually do the calculation
        // You should, however, not have an EventHandler calling another EventHandler
        private void DoCalculation()
        {
            double quantity;
            if (string.IsNullOrWhiteSpace(QuantityEntry.Text))
            {
                lblAnswer.Text = "Conversion will go here";
                return;
            }
            if (!double.TryParse(QuantityEntry.Text, out quantity))
            {
                lblAnswer.Text = "Enter a Number Only";
                return;
            }
            int conversion = ConversionTypePicker.SelectedIndex;
            string result;

            // Do this whatever what you want
            // Use ifs
            // Store result as double 
            // update lblAnswer.Text directly in the switch
            // Have DoCalculation return a string would be a good way to do it
            // Or Do it the way I've done it
            switch (conversion)
            {
                case 0:
                    result = $"{quantity} kgs is {quantity * 2.20462:F2} lbs";
                    break;
                case 1:
                    result = $"{quantity} lbs is {quantity / 2.20462:F2} kgs";
                    break;
                case 2:
                    result = $"{quantity} km is {quantity / 1.60934:F2} miles";
                    break;
                case 3:
                    result = $"{quantity} miles is {quantity * 1.60934:F2} km";
                    break;
                case 4:
                    result = $"{quantity} pints is {quantity / 1.75975:F2} l";
                    break;
                case 5:
                    result = $"{quantity} l is {quantity * 1.75975:F2} pints";
                    break;

                default:
                    result = "Conversion will go here";
                    break;

            }
            lblAnswer.Text = result;
        }

        // The separate eventhandlers are kind of redundant, the ConversionTypePicker_SelectedIndexChanged event could be subscribed to 
        // by btnCalculate_Clicked anyway , but I'm leaving the 2 Event Handlers so you can see.
        private void ConversionTypePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Not really needed, but we can update the placeholder
            int conversion = ConversionTypePicker.SelectedIndex;
            switch (conversion)
            {
                case 0:
                    QuantityEntry.Placeholder = "kgs";
                    break;
                case 1:
                    QuantityEntry.Placeholder = "lbs";
                    break;
                case 2:
                    QuantityEntry.Placeholder = "km";
                    break;
                case 3:
                    QuantityEntry.Placeholder = "miles";
                    break;
                case 4:
                    QuantityEntry.Placeholder = "pints";
                    break;
                case 5:
                    QuantityEntry.Placeholder = "litres";
                    break;

            }
            DoCalculation();
        }


        private void QuantityEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            // This is not really necessary, let's prevent the user from typing a non digit
            // This shows how we could use different event handlers similarly and just call a method after additional checks
            if (e.NewTextValue.Length == 0)
            {
                lblAnswer.Text = "Enter a number to convert";
                return;
            }
            if (!char.IsDigit(e.NewTextValue[e.NewTextValue.Length - 1]) && e.NewTextValue[e.NewTextValue.Length - 1] != '.')
            {
                QuantityEntry.Text = e.OldTextValue;
            }
            DoCalculation();
        }

        // Creating the grid via c#
        private void CreateGridButton_Clicked(object sender, EventArgs e)
        {
            CreateGrid(4);
        }

        private void CreateGrid(int n)
        {
            Grid grid = new Grid();
            // Let's make it a square
            grid.HeightRequest = 300;
            grid.WidthRequest = 300;
            grid.BackgroundColor = Colors.Red;
            // This will be n rows, n columns all with * size
            for (int i = 0; i < n; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Border border = new Border
                    {
                        Stroke = Colors.DarkBlue,
                        StrokeThickness = 2,
                        BackgroundColor = Colors.LightBlue,
                        StrokeShape = new RoundRectangle
                        {
                            CornerRadius = 10
                        },
                        Margin = new Thickness(3)
                    };
                    grid.Add(border, j, i);
                }
            }
            MainLayout.Add(grid);
        }
    }
}
