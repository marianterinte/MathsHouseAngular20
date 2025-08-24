namespace GCMS.MathHouse.UI.Common;

public partial class NumericKeyboard : ContentView
{
    public event EventHandler<string> NumberClicked;
    public event EventHandler DeleteClicked;
    public event EventHandler OkClicked;

    public NumericKeyboard()
    {
        InitializeComponent();
    }

    private void OnNumberClicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            NumberClicked?.Invoke(this, button.Text);
        }
    }

    private void OnDeleteClicked(object sender, EventArgs e)
    {
        DeleteClicked?.Invoke(this, EventArgs.Empty);
    }

    private void OnOkClicked(object sender, EventArgs e)
    {
        OkClicked?.Invoke(this, EventArgs.Empty);
    }
}