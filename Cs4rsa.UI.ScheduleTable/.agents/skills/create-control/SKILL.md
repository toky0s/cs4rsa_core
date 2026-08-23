# WPF Flexible UserControl — Cursor Agent Skill

## Goal

Create reusable, flexible, MVVM-friendly WPF UserControls that:

* Support binding from parent
* Avoid code-behind logic
* Use DependencyProperty for customization
* Allow styling and templating
* Work inside any layout container
* Avoid fixed sizes unless explicitly required
* Don't use any 3rd party UI systems like MaterialDesign, use native WPF XAML and Style.
---

# 1. Always Use MVVM-Friendly Structure

```
MyControl/
 ├── MyControl.xaml
 ├── MyControl.xaml.cs
 └── MyControlViewModel.cs (optional)
```

UserControl must NOT assume DataContext internally unless explicitly requested.

❌ Wrong

```xml
<UserControl.DataContext>
    <local:MyViewModel/>
</UserControl.DataContext>
```

✅ Correct

```xml
<UserControl>
```

Parent controls DataContext.

---

# 2. Use DependencyProperty For All Public Inputs

All customizable values must be DependencyProperty.

```csharp
public string Title
{
    get => (string)GetValue(TitleProperty);
    set => SetValue(TitleProperty, value);
}

public static readonly DependencyProperty TitleProperty =
    DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(MyControl),
        new PropertyMetadata(default(string)));
```

Never use public fields or normal properties for UI binding.

---

# 3. Always Bind Using RelativeSource Self

Correct

```xml
Text="{Binding Title, RelativeSource={RelativeSource AncestorType=UserControl}}"
```

or

```xml
x:Name="root"
Text="{Binding Title, ElementName=root}"
```

Never rely on DataContext inside UserControl.

---

# 4. Do Not Hardcode Size

❌ Forbidden

```xml
Width="300"
Height="200"
```

✅ Allowed

```xml
MinWidth="120"
MinHeight="40"
```

Prefer

```xml
HorizontalAlignment="Stretch"
VerticalAlignment="Stretch"
```

UserControl must adapt to parent layout.

---

# 5. Expose ICommand Instead of Events

❌ Wrong

```xml
<Button Click="OnClick"/>
```

✅ Correct

```xml
<Button Command="{Binding ClickCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"/>
```

```csharp
public ICommand ClickCommand
{
    get => (ICommand)GetValue(ClickCommandProperty);
    set => SetValue(ClickCommandProperty, value);
}

public static readonly DependencyProperty ClickCommandProperty =
    DependencyProperty.Register(
        nameof(ClickCommand),
        typeof(ICommand),
        typeof(MyControl));
```

---

# 6. Use ContentPresenter For Extensibility

```xml
<ContentPresenter
    Content="{Binding Content, RelativeSource={RelativeSource AncestorType=UserControl}}"/>
```

Multiple slots

```xml
Header
Body
Footer
```

Expose:

* Header
* Body
* Footer
* HeaderTemplate
* BodyTemplate
* FooterTemplate

---

# 7. Provide Default Style Support

```xml
<UserControl.Resources>
    <Style TargetType="Button">
        ...
    </Style>
</UserControl.Resources>
```

Never override global styles unintentionally.

---

# 8. Support Two-Way Binding When Needed

```xml
<TextBox
Text="{Binding Value,
RelativeSource={RelativeSource AncestorType=UserControl},
Mode=TwoWay,
UpdateSourceTrigger=PropertyChanged}"/>
```

---

# 9. Use Grid Not StackPanel

Prefer Grid

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="*"/>
    </Grid.RowDefinitions>
</Grid>
```

Avoid deeply nested StackPanel.

---

# 10. Support Design-Time Preview

Always include

```xml
xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
mc:Ignorable="d"
```

Optional

```xml
d:DataContext="{d:DesignInstance Type=local:MyViewModel}"
```

---

# 11. Example Flexible Control

## FlexCard.xaml

```xml
<UserControl
    x:Class="Controls.FlexCard"
    x:Name="root"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <Border
        Padding="12"
        Background="{Binding Background, ElementName=root}"
        CornerRadius="6">

        <Grid>
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="*"/>
                <RowDefinition Height="Auto"/>
            </Grid.RowDefinitions>

            <ContentPresenter
                Grid.Row="0"
                Content="{Binding Header, ElementName=root}"/>

            <ContentPresenter
                Grid.Row="1"
                Content="{Binding Body, ElementName=root}"/>

            <ContentPresenter
                Grid.Row="2"
                Content="{Binding Footer, ElementName=root}"/>

        </Grid>

    </Border>
</UserControl>
```

---

## FlexCard.xaml.cs

```csharp
public partial class FlexCard : UserControl
{
    public FlexCard()
    {
        InitializeComponent();
    }

    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(
            nameof(Header),
            typeof(object),
            typeof(FlexCard));

    public object Body
    {
        get => GetValue(BodyProperty);
        set => SetValue(BodyProperty, value);
    }

    public static readonly DependencyProperty BodyProperty =
        DependencyProperty.Register(
            nameof(Body),
            typeof(object),
            typeof(FlexCard));

    public object Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    public static readonly DependencyProperty FooterProperty =
        DependencyProperty.Register(
            nameof(Footer),
            typeof(object),
            typeof(FlexCard));
}
```

---

# 12. Usage

```xml
<controls:FlexCard>

    <controls:FlexCard.Header>
        <TextBlock Text="Header"/>
    </controls:FlexCard.Header>

    <controls:FlexCard.Body>
        <ListView/>
    </controls:FlexCard.Body>

    <controls:FlexCard.Footer>
        <Button Content="OK"/>
    </controls:FlexCard.Footer>

</controls:FlexCard>
```

---

# Cursor Agent Rules

When user asks:

* create usercontrol
* create reusable control
* create WPF component
* create MVVM control

ALWAYS:

1. Use DependencyProperty
2. No DataContext override
3. Use ContentPresenter
4. No fixed size
5. ICommand instead of events
6. MVVM safe binding
7. Flexible layout
8. Design-time safe

Generate full working code.

