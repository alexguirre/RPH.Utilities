namespace RPH.Utilities.AI.Serialization
{
    // System
    using System.Xml.Serialization;

    [XmlRoot(ElementName = "BehaviorTree")]
    public class SerializableBehaviorTree
    {
        [XmlElement(ElementName = "Root")]
        public SerializableBehaviorTask Root { get; set; }

        public BehaviorTree CreateTree()
        {
            return new BehaviorTree(Root.CreateBehaviorTask());
        }
    }
}
