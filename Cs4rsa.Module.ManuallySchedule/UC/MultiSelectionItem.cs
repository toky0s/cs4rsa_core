using System;

namespace Cs4rsa.Module.ManuallySchedule.UC
{
    public class MultiSelectionItem : IEquatable<MultiSelectionItem>
    {
        public string ID { get; set; }
        public string Label { get; set; }

        public bool Equals(MultiSelectionItem other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return ID == other.ID;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MultiSelectionItem);
        }

        public override int GetHashCode()
        {
            return ID != null ? ID.GetHashCode() : 0;
        }
    }
}