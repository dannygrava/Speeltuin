using System.Collections.Generic;
using System.Linq;

namespace PdnDatabase
{
    public class PdnGame
    {
        public IList<PdnTag> Tags { get; private set; }
        public IList<object> Body { get; private set; }

        public PdnGame()
        {
            Tags = new List<PdnTag>();
            Body = new List<object>();
        }
        public string Event { get { return getTagValueByName("Event"); }}
        public string Date { get { return getTagValueByName("Date"); }}
        public string Black { get { return getTagValueByName("Black");}}
        public string White { get { return getTagValueByName("White");}}
        public string Result { get { return getTagValueByName("Result"); } }

        private string getTagValueByName(string name)
        {
            PdnTag tag = Tags.SingleOrDefault(t => t.Name == name);
            return tag == null ? "" : tag.Value;
        }


    }

    public class PdnTag
    {
        public PdnTag(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }
        public string Value { get; private set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", Name, Value);
        }
    }

    public class PdnMove
    {
        public PdnMove(string moveNumber, string move, string moveStrength)
        {
            MoveNumber = moveNumber;
            Move = move;
            MoveStrength = moveStrength;
        }

        protected string MoveStrength { get; private set; }

        public string MoveNumber { get; private set; }
        public string Move { get; private set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", MoveNumber, Move, MoveStrength);
        }
    }

    public class PdnComment
    {
        public PdnComment(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
        public override string ToString()
        {
            return string.Format("Comment: {0}", Value);
        }
    }

    // todo NAG
    // todo RAV

}
