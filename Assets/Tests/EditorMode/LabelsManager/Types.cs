using Data;
using Models.Game;

namespace Tests.EditorMode.LabelsManager
{
    internal struct UpdatePrestigeTestCase
    {
        public string name;
        public LabelInfo label;
        public int dice;
        public ExpValue want;
    };
}