using Godot;

[GlobalClass]
public partial class SliderComponent : Control
{
	[Signal] public delegate void ValueChangedEventHandler(float value);

    [Export] public string Title { get; set; } = "Placeholder";

    private Label _valueLabel { get; set; } = null!;
    private HSlider _slider { get; set; } = null!;

    public override void _Ready()
    {
        GetNode<Label>("Title").Text = Title;
        _valueLabel = GetNode<Label>("ValueLabel");
        _slider = GetNode<HSlider>("HSlider");
    }

    public void SetValue(float value)
    {
        _slider.Value = value;
        _valueLabel.Text = value.ToString();
    }

    private void OnValueChanged(float value)
	{
		EmitSignalValueChanged(value);
        _valueLabel.Text = value.ToString();
    }
}
