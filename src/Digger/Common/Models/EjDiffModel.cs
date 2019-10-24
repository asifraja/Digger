using DiffPlex.DiffBuilder.Model;
using System.Linq;

namespace Digger.Common.Models
{
    public class EjDiffModel
    {
        public DiffPaneModel DiffPaneModel;
        public SideBySideDiffModel SideBySideDiffModel;

        public string ThisFilename { get; }
        public string WithFilename { get; }

        private EjDiffModel()
        {

        }

        public bool HasDifference
        {
            get
            {
                if (DiffPaneModel != null)
                {
                    return DiffPaneModel.Lines.Any(l => l.Type != ChangeType.Unchanged);
                }
                if (SideBySideDiffModel != null)
                {
                    return SideBySideDiffModel.NewText.Lines.Any(l => l.Type != ChangeType.Unchanged) || SideBySideDiffModel.OldText.Lines.Any(l => l.Type != ChangeType.Unchanged);
                }
                return false;
            }
        }

        public EjDiffModel(DiffPaneModel diffPaneModel, string thisFilename, string withFilename)
        {
            DiffPaneModel = diffPaneModel;
            ThisFilename = thisFilename;
            WithFilename = withFilename;
        }

        public EjDiffModel(SideBySideDiffModel sideBySideDiffModel, string thisFilename, string withFilename)
        {
            SideBySideDiffModel = sideBySideDiffModel;
            ThisFilename = thisFilename;
            WithFilename = withFilename;
        }
    }
}

