using System.Text;
using static Naninovel.Spreadsheet.CompositeSheet;

namespace Naninovel.Spreadsheet
{
    public class TemplateBuilder
    {
        public int Length => Builder.Length;

        protected virtual CompositeSheet Composite { get; }
        protected virtual StringBuilder Builder { get; }

        public TemplateBuilder (CompositeSheet composite)
        {
            Composite = composite;
            Builder = new StringBuilder();
        }

        public virtual void Append (Composite composite, bool appendLine = true)
        {
            Builder.Append(composite.Template);
            if (appendLine) Builder.AppendLine();
            if (composite.Arguments.Count == 0) return;
            foreach (var arg in composite.Arguments)
                AppendArgument(arg);
        }

        public virtual string Build () => Builder.ToString();

        protected virtual void AppendArgument (string arg)
        {
            if (Builder.Length > 0)
            {
                Composite.GetColumnValues(TemplateHeader).Add(Builder.ToString());
                Builder.Clear();
            }
            else Composite.GetColumnValues(TemplateHeader).Add(string.Empty);
            Composite.GetColumnValues(ArgumentHeader).Add(arg);
        }
    }
}
