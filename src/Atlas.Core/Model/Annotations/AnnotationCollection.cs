using System.Collections;
using System.Collections.Immutable;

namespace Atlas.Core.Model.Annotations;

public record AnnotationCollection : IEnumerable<Annotation>
{
    private IDictionary<int, IEnumerable<Annotation>> annotations;

    public static AnnotationCollection Empty => new AnnotationCollection();

    public AnnotationCollection()
    {
        annotations = new Dictionary<int, IEnumerable<Annotation>>();
    }

    public AnnotationCollection(IEnumerable<Annotation> val)
    {
        annotations = new Dictionary<int, IEnumerable<Annotation>>();
        foreach (var annotation in val)
        {
            if (!annotations.ContainsKey(annotation.StartIndex))
            {
                annotations.Add(annotation.StartIndex, new List<Annotation>());
            }
            annotations[annotation.StartIndex] = annotations[annotation.StartIndex].Append(annotation);
        }
    }

    public IEnumerable<Annotation> GetAnnotations(int startIndex)
    {
        return annotations[startIndex];
    }

    public IEnumerator<Annotation> GetEnumerator() => annotations.SelectMany(kvPair => kvPair.Value).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => annotations.SelectMany(kvPair => kvPair.Value).GetEnumerator();
}
