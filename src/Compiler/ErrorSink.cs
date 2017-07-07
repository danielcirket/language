using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Compiler
{
    internal class ErrorSink : IEnumerable<Error>
    {
        private List<Error> _errors;

        public IEnumerable<Error> Errors => _errors.AsReadOnly();
        public bool HasErrors => _errors.Any(error => error.Severity == Severity.Error);
        public bool HasWarnings => _errors.Any(error => error.Severity == Severity.Warning);
        public bool HasMessage => _errors.Any(error => error.Severity == Severity.Message);

        public void AddError(string message, SourceFilePart filePart, Severity severity)
        {
            _errors.Add(new Error(message, filePart.Lines, severity, filePart));
        }
        public void Clear()
        {
            _errors.Clear();
        }

        public IEnumerator<Error> GetEnumerator()
        {
            return _errors.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _errors.GetEnumerator();
        }

        public ErrorSink()
        {
            _errors = new List<Error>();
        }
    }
}
