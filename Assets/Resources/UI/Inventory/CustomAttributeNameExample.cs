using UnityEngine.UIElements;

[UxmlElement]
public partial class CustomAttributeNameExample : VisualElement
{
    public Image icon;
    public Label quantity;
    
    [UxmlAttribute("character-name")]
    public string myStringValue { get; set; }

    [UxmlAttribute("credits")]
    public float myFloatValue { get; set; }

    [UxmlAttribute("level")]
    public int myIntValue { get; set; }

    [UxmlAttribute("usage")]
    public UsageHints myEnumValue { get; set; }
}