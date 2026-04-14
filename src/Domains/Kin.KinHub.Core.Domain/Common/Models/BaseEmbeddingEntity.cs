namespace Kin.KinHub.Core.Domain.Common;

public abstract class BaseEmbeddingEntity<T> : BaseDeletableEntity<T>
{
    public float[]? Embedding { get; set; }
}
