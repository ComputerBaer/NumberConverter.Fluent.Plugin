using Avalonia.Input;
using Blast.Core.Results;

namespace NumberConverter.Fluent.Plugin
{
    public class ResultCopyOperation : SearchOperationBase
    {
        public ResultCopyOperation() : base("Copy Result", "Copy the conversion result to clipboard", "\uE8C8")
        {
            KeyGesture = new KeyGesture(Key.C, KeyModifiers.Control);
        }
    }
}
